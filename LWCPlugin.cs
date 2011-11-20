using System;
using System.IO;
using System.Collections.Generic;

using Terraria_Server;
using Terraria_Server.Commands;
using Terraria_Server.Plugins;

using LWC.IO;
using LWC.Util;
using Terraria_Server.Logging;
using Terraria_Server.Permissions;

/* LWC-Terraria for TDSM (tdsm.org) */
namespace LWC
{
	public partial class LWCPlugin : BasePlugin
	{
		/**
		 * The object that stores most of what we use
		 */
		public Store Cache { get; private set; }
		
		/**
		 * The loader that loads protections
		 */
		private ProtectionLoader Loader;
		
		/**
		 * The saver that saves protections
		 */
		private ProtectionSaver Saver;
		
		private bool enabled = false;
		
		/**
		 * The LWCPlugin instance loaded
		 */
		private static LWCPlugin instance = null;
		
		/**
		 * The folder to save files in
		 */
		public static string Folder = "";

		/**
		 * Remove the actions for the player
		 * 
		 * @param palyer
		 */
		public void ResetActions(Player player)
		{
			Cache.Actions.Remove(player.Name);
		}
		
		public LWCPlugin()
		{
			Name = "LWC";
			Description = "Chest protection mod";
			Author = "Hidendra";
			Version = "1.11";
			TDSMBuild = 36;
			instance = this;
		}
		
		protected override void Initialized(object state)
		{
			// create the LWC dir if needed
			Folder = Statics.PluginPath + Path.DirectorySeparatorChar + "LWC/";
			
			if(!Directory.Exists(Folder))
			{
				Directory.CreateDirectory(Folder);
			}
			
			// default loader/saver for now
			Loader = new FlatFileProtectionLoader();
			Saver = new FlatFileProtectionSaver();
			
			// fire up them cache
			Cache = new Store();
		
			Log("Syncing protections");
			
			// and now load the protections
			foreach(Protection protection in Loader.LoadProtections())
			{
				Cache.Protections.Add(new LocationKey(protection.X, protection.Y), protection);
			}
			
			Log("Loaded " + Cache.Protections.Count + " protections!");
			
			AddCommand ("cpublic")
				.WithDescription ("command")
                .WithAccessLevel(AccessLevel.PLAYER)
				.WithHelpText ("help not written")
                .WithPermissionNode("lwc.public")
				.Calls (this.PublicCommand);
			
			AddCommand ("cprivate")
				.WithDescription ("command")
                .WithAccessLevel(AccessLevel.PLAYER)
				.WithHelpText ("help not written")
                .WithPermissionNode("lwc.private")
				.Calls (this.PrivateCommand);
			
			AddCommand ("cpassword")
				.WithDescription ("command")
                .WithAccessLevel(AccessLevel.PLAYER)
				.WithHelpText ("help not written")
                .WithPermissionNode("lwc.password")
				.Calls (this.PasswordCommand);
			
			AddCommand ("cunlock")
				.WithDescription ("command")
                .WithAccessLevel(AccessLevel.PLAYER)
				.WithHelpText ("help not written")
                .WithPermissionNode("lwc.unlock")
				.Calls (this.UnlockCommand);
			
			AddCommand ("cinfo")
				.WithDescription ("command")
                .WithAccessLevel(AccessLevel.PLAYER)
				.WithHelpText ("help not written")
                .WithPermissionNode("lwc.info")
				.Calls (this.InfoCommand);
			
			AddCommand ("cremove")
				.WithDescription ("command")
                .WithAccessLevel(AccessLevel.PLAYER)
				.WithHelpText ("help not written")
                .WithPermissionNode("lwc.remove")
				.Calls (this.RemoveCommand);
		}
		
		protected override void Disposed (object state)
		{
			
		}
		
		protected override void Enabled()
		{
			enabled = true;
			Log("VERSION: " + Version);
		}

		protected override void Disabled()
		{
			Log("Desyncing protections");
			
			try
			{
				// save the protections
				Protection[] protections = new Protection[Cache.Protections.Count];
				
				if(protections.Length > 0)
				{
					Cache.Protections.Values.CopyTo(protections, 0);
					Saver.SaveProtections(protections);
				}
			} catch(Exception exception)
			{
				Log("Exception occured! " + exception.Message);
				Log(exception.ToString());
			}
			
			enabled = false;
			Log("LWC has been disabled!");
		}

        #region Events

        [Hook(HookOrder.EARLY)]		
		void onPlayerOpenChest(ref HookContext ctx, ref HookArgs.ChestOpenReceived args)
		{
			ISender sender = ctx.Sender;
			int ChestId = args.ChestIndex;
			
			if(!(sender is Player))
			{
				return;
			}
			
			// we only want players ?
			Player player = sender as Player;
			
			// get the Chest object
			Chest chest = Main.chest[ChestId];
			
			// the location of the chest
			LocationKey key = new LocationKey(chest.x, chest.y);
			
			// see if we have a protection attached to the chest
			Protection protection = Cache.Protections.Get(key);
			bool CanAccess = true;
			
			// if it's a valid protection, ensure they can access it
			if(protection != null)
			{
				CanAccess = protection.CanAccess(player);
				
				// PASSWORD PROTECTION CHECK
				if(protection.Type == Protection.PASSWORD_PROTECTION)
				{
					if(!CanAccess)
					{
						Pair<Action, Protection> PassTemp = new Pair<Action, Protection>(Action.UNLOCK, protection);
						ResetActions(player);
						Cache.Actions.Add(player.Name, PassTemp);
						
						player.sendMessage("This chest is locked with a password!", 255, 255, 0, 0);
						player.sendMessage("Type /cunlock <password> to unlock it.", 150, 255, 0, 0);
					}
				}
				
				// Update the chest id if it changed somehow
				if(protection.ChestId != ChestId)
				{
					protection.ChestId = ChestId;
				}
			}
			
			// if they can't access it, cancel the event !!
			if(!CanAccess)
			{
				ctx.SetResult (HookResult.IGNORE);
			}
			
			// is there an action for this player?
			Pair<Action, Protection> pair = Cache.Actions.Get(sender.Name);
			
			if(pair == null)
			{
				// check again if they dont have access and pester them
				if(!CanAccess)
				{
					sender.sendMessage("That Chest is locked with a magical spell.", 255, 255, 0, 0);
				}
				
				return;
			}
			
			// action data
			Action action = pair.First;
			Protection Temp = pair.Second;
			
			switch(action)
			{
				case Action.INFO:
					
					if(protection == null)
					{
						player.sendMessage("That chest is not protected!", 255, 255, 0, 0);
					} else
					{
						player.sendMessage("Owner: " + protection.Owner, 255, 255, 0, 0);
						player.sendMessage("Type: " + protection.TypeToString(), 255, 255, 0, 0);
						
						if(CanAccess)
						{
							player.sendMessage("Can access: Yes", 255, 0, 255, 0);
						} else
						{
							player.sendMessage("Can access: No", 255, 255, 0, 0);
						}
					}
					
					ResetActions(player);
					break;
				
				case Action.CREATE:
					if(protection != null)
					{
						player.sendMessage("That chest has already been registered!", 255, 255, 0, 0);
						break;
					}
					
					// good good, now set the rest of the meta data
					Temp.ChestId = ChestId;
					Temp.X = chest.x;
					Temp.Y = chest.y;
					Temp.Valid = true;
					
					// register it !!
					Cache.Protections.Add(key, Temp);
					
					// remove the action
					Cache.Actions.Remove(player.Name);
					
					player.sendMessage("Registered a " + Temp.TypeToString() + " Chest successfully!", 255, 0, 255, 0);
					player.sendMessage("Note: Currently, empty chests can be destroyed !!", 255, 255, 0, 255);
					player.sendMessage("Ensure 1 item stays in it so the chest itself cannot be stolen.", 255, 255, 0, 255);
					break;
					
				case Action.REMOVE:
					if(protection == null)
					{
						player.sendMessage("Please open a protected chest!", 255, 255, 0, 0);
						return;
					}
					
					if(!protection.IsOwner(player))
					{
						player.sendMessage("You are not the owner of that chest!", 255, 255, 0, 0);
						return;
					}
					
					//  we're the owner, remove the protection !!
					protection.Remove();
					Cache.Actions.Remove(player.Name);
					
					player.sendMessage("Protection removed!", 255, 255, 0, 255);
					break;
			}
		}


        /** This is for the next release -- it needs to be fixed first
        [Hook(HookOrder.EARLY)]		
        void onPlayerBreakChest(ref HookContext ctx, ref HookArgs.ChestBreakReceived args)
        {
			
            ISender sender = ctx.Sender;
            int ChestX = args.X;
            int ChestY = args.Y;
			
            if(!(sender is Player))
            {
                return;
            }

            Player player = sender as Player;

            // the location of the chest
            LocationKey key = new LocationKey(ChestX, ChestY);
			
            // see if we have a protection attached to the chest
            Protection protection = Cache.Protections.Get(key);

            if(protection == null)
            {
                player.sendMessage("Chest not protected. Allowing breakage.", 255, 0, 255, 0);
                return; // chest is not protected so allow it to be broken		
            }
            else
            {
                //chest has protection, inform user to remove protection first
                player.sendMessage("Please use /cremove first", 255, 0, 255, 0);
                //and ignore the break until they do so
                ctx.SetResult (HookResult.IGNORE);
            }	
        }
         * **/

        #endregion

        public static LWCPlugin Get()
		{
			return instance;
		}

		public static void Log(string message)
		{
			ProgramLog.Plugin.Log("[LWC] " + message);
		}
	}
}
