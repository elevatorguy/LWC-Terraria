using System;

using Terraria_Server;
using Terraria_Server.Plugin;

using LWC.IO;

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
				LocationKey key = new LocationKey(protection.X, protection.Y);
				
				Cache.Protections.Add(key, protection);
			}
			
			Log("Loaded " + Cache.Protections.Count + " protections.");
		}

		public override void Enable()
		{
			/* Register our events */
			registerHook(Hooks.PLAYER_COMMAND);
			registerHook(Hooks.PLAYER_CHEST);
			registerHook(Hooks.TILE_BREAK);

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
			string command = args[0].Trim();

			if(command.Equals("/cpublic"))
			{
				// player.sendMessage("Hello!");
			}
		}

		public override void onPlayerOpenChest(Terraria_Server.Events.ChestOpenEvent Event)
		{
			base.onPlayerOpenChest(Event);
		}

		public override void onTileBreak(Terraria_Server.Events.TileBreakEvent Event)
		{
			Tile tile = Event.getTile();
			
			base.onTileBreak(Event);
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
