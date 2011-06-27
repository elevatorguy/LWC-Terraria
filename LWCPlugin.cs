using System;

using Terraria_Server;
using Terraria_Server.Plugin;

/* LWC-Terraria for TDSM (tdsm.org) */
namespace LWC
{
	public class LWCPlugin : Plugin
	{

		/* If LWC is enabled */
		private bool enabled = false;

		private void Log(string message)
		{
			Program.tConsole.WriteLine("[LWC] " + message);
		}

		public override void Load()
		{
			Name = "LWC";
			Description = "Chest protection mod";
			Author = "Hidendra";
			Version = "1.00-dev";
			
		}

		public override void Enable()
		{
			/* Register our events */
			registerHook(Hooks.PLAYER_COMMAND);
			registerHook(Hooks.PLAYER_CHEST);
			registerHook(Hooks.TILE_BREAK);

			enabled = true;
			Log("LWC has been enabled!");
		}

		public override void Disable()
		{
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

	}
}
