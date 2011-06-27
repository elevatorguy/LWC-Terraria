using System;

using Terraria_Server;
using Terraria_Server.Commands;
using Terraria_Server.Plugin;

using LWC.IO;
using LWC.Util;

/* LWC-Terraria for TDSM (tdsm.org) */
namespace LWC
{
	public class LWCPlugin : Plugin
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

		public LWCPlugin()
		{
			instance = this;
		}

		/**
		 * Remove the actions for the player
		 * 
		 * @param palyer
		 */
		public void ResetActions(Player player)
		{
			Cache.Actions.Remove(player.getName());
		}
		
		public override void Load()
		{
			Name = "LWC";
			Description = "Chest protection mod";
			Author = "Hidendra";
			Version = "1.00-dev";
			
			// default loader/saver for now
			Loader = new FlatFileProtectionLoader();
			Saver = new FlatFileProtectionSaver();
			
			// fire up them cache
			Cache = new Store();
			
			Log("Synching protections....");
			
			// and now load the protections
			foreach(Protection protection in Loader.LoadProtections())
			{
				Cache.Protections.Add(new LocationKey(protection.X, protection.Y), protection);
			}
			
			Log("Loaded " + Cache.Protections.Count + " protections.");
		}

		public override void Enable()
		{
			/* Register our events */
			registerHook(Hooks.PLAYER_COMMAND);
			registerHook(Hooks.PLAYER_CHEST);

			enabled = true;
			Log("VERSION: " + Version);
		}

		public override void Disable()
		{
			Log("Desynching protections....");
			
			// save the protections
			Protection[] protections = new Protection[Cache.Protections.Count];
			
			if(protections.Length > 0)
			{
				Cache.Protections.Values.CopyTo(protections, 0);
				Saver.SaveProtections(protections);
			}
			
			enabled = false;
			Log("LWC has been disabled!");
		}
		
		public override void onPlayerCommand(Terraria_Server.PlayerCommandEvent Event)
		{
			if(enabled == false)
			{
				return;
			}

			// split the args
			string[] args = Event.getMessage().ToLower().Split(' ');

			if(args.Length == 0)
			{
				return;
			}

			Player player = Event.getPlayer();
			string command = args[0].Trim().Substring(1);
			string Extra = "";
			
			if(args.Length > 1)
			{
				for(int index = 1; index < args.Length; index++)
				{
					Extra += args[index] + " ";
				}
				
				Extra = Extra.Trim();
			}
			
			// The pair to cache
			Pair<Action, Protection> pair = new Pair<Action, Protection>(Action.NOTHING, null);
			
			// no mucking with the local scope, you silly switch!
			Protection temp;
			
			switch(command)
			{
				case "cpublic":
				case "cprivate":
					Event.setCancelled(true);
					
					temp = new Protection();
					temp.Owner = player.getName();
					
					if(command.Equals("cpublic"))
					{
						temp.Type = Protection.PUBLIC_PROTECTION;
					} else {
						temp.Type = Protection.PRIVATE_PROTECTION;
					}
					
					pair.First = Action.CREATE;
					pair.Second = temp;
					
					player.sendMessage("Open the chest to protect it!", 255, 0, 255, 0);
					break;
					
				case "cpassword":
					Event.setCancelled(true);
					
					if(args.Length == 1)
					{
						player.sendMessage("Usage: /cprivate <password>", 255, 255, 0, 0);
						break;
					}
					
					temp = new Protection();
					temp.Owner = player.getName();
					temp.Type = Protection.PASSWORD_PROTECTION;
					temp.Data = SHA1.Hash(Extra);
					
					char[] pass = Extra.ToCharArray();
					for(int index = 0; index < pass.Length; index++)
					{
						pass[index] = '*';
					}
					
					pair.First = Action.CREATE;
					pair.Second = temp;
					
					player.sendMessage("Password: " + new String(pass), 255, 255, 0, 0);
					player.sendMessage("Open the chest to protect it!", 255, 0, 255, 0);
					
					break;
				
				case "cunlock":
					Event.setCancelled(true);
					
					if(args.Length == 1)
					{
						player.sendMessage("Usage: /cunlock <password>", 255, 255, 0, 0);
						break;
					}
					
					// see if they have an action for unlock already ??
					Pair<Action, Protection> Password = Cache.Actions.Get(player.getName());
					
					if(Password.First == Action.UNLOCK)
					{
						string hash = SHA1.Hash(Extra);
						Protection PasswordProtection = Password.Second;
						
						// compare the passwords
						if(PasswordProtection.Data.Equals(hash))
						{
							PasswordProtection.Access.Add(player.getName());
							ResetActions(player);
							
							// TODO: remove them from access when they log out
							player.sendMessage("Password accepted!", 255, 0, 255, 0);
						} else
						{
							player.sendMessage("Invalid password!", 255, 255, 0, 0);
						}
					} else
					{
						player.sendMessage("You need to open a password-protected chest to do that!", 255, 255, 0, 0);
					}
					
					
					break;
					
				case "cinfo":
					Event.setCancelled(true);
					
					pair.First = Action.INFO;
					player.sendMessage("Open a chest to view information about it.", 255, 0, 255, 0);
					break;
					
				case "cremove":
					Event.setCancelled(true);
					
					pair.First = Action.REMOVE;
					player.sendMessage("Open a chest to remove a protection you own.", 255, 0, 255, 0);
					break;
			}
			
			// cache the action if it's not null!
			if(pair.First != Action.NOTHING)
			{
				ResetActions(player);
				Cache.Actions.Add(player.getName(), pair);
			}
		}

		public override void onPlayerOpenChest(Terraria_Server.Events.ChestOpenEvent Event)
		{
			Sender sender = Event.getSender();
			int ChestId = Event.getChestID();
			
			if(!(sender is Player))
			{
				return;
			}
			
			// we only want players ?
			Player player = (Player) sender;
			
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
						Cache.Actions.Add(player.getName(), PassTemp);
						
						player.sendMessage("This chest is locked with a password!", 255, 255, 0, 0);
						player.sendMessage("Type /cunlock <password> to unlock it.", 150, 255, 0, 0);
					}
				}
			}
			
			// if they can't access it, cancel the event !!
			if(!CanAccess)
			{
				Event.setCancelled(true);
			}
			
			// is there an action for this player?
			Pair<Action, Protection> pair = Cache.Actions.Get(sender.getName());
			
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
					Cache.Actions.Remove(player.getName());
					
					player.sendMessage("Registered a " + Temp.TypeToString() + " Chest successfully!", 255, 0, 255, 0);
					player.sendMessage("Note: Currently, empty chests can be destroyed !! Ensure 1 item stays in it so the chest itself cannot be stolen.", 255, 255, 0, 255);
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
					Cache.Actions.Remove(player.getName());
					
					player.sendMessage("Protection removed!", 255, 255, 0, 255);
					break;
			}
		}
		
		public static LWCPlugin Get()
		{
			return instance;
		}

		private void Log(string message)
		{
			Program.tConsole.WriteLine("[LWC] " + message);
		}

	}
}
