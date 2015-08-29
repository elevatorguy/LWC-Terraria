using OTA.Command;
using LWC.Util;
using Terraria;

namespace LWC
{
    public partial class LWCPlugin
	{
		void PublicCommand (ISender sender, ArgumentList argz)
		{
				Player player = sender as Player;
				Protection temp = new Protection ();
				Pair<Action, Protection> pair = new Pair<Action, Protection> (Action.NOTHING, null);
			
				temp.Owner = player.Name;
				temp.Type = Protection.PUBLIC_PROTECTION;

				pair.First = Action.CREATE;
				pair.Second = temp;
					
				player.SendMessage ("Open the chest to protect it!", 255, 0, 255, 0);
			
			// cache the action if it's not null!
			if (pair.First != Action.NOTHING) {
				ResetActions (player);
				Cache.Actions.Add (player.Name, pair);
			}
		}

		void PrivateCommand (ISender sender, ArgumentList argz)
		{
				Player player = sender as Player;
				Protection temp = new Protection ();
				Pair<Action, Protection> pair = new Pair<Action, Protection> (Action.NOTHING, null);
			
				temp.Owner = player.Name;
				temp.Type = Protection.PRIVATE_PROTECTION;
					
				pair.First = Action.CREATE;
				pair.Second = temp;
					
				player.SendMessage ("Open the chest to protect it!", 255, 0, 255, 0);
		
			// cache the action if it's not null!
			if (pair.First != Action.NOTHING) {
				ResetActions (player);
				Cache.Actions.Add (player.Name, pair);
			}
		}

		void PasswordCommand (ISender sender, ArgumentList args)
		{
				Player player = sender as Player;
				Protection temp = new Protection ();
				Pair<Action, Protection> pair = new Pair<Action, Protection> (Action.NOTHING, null);
			
				if (args.Count != 1) {
					player.SendMessage ("Usage: /cpassword <password>", 255, 255, 0, 0);
					return;
				}
				
				string Extra = args[0];
			
				temp = new Protection ();
				temp.Owner = player.Name;
				temp.Type = Protection.PASSWORD_PROTECTION;
				temp.Data = SHA1.Hash (Extra);
					
				char[] pass = Extra.ToCharArray ();
				for (int index = 0; index < pass.Length; index++) {
					pass [index] = '*';
				}
					
				pair.First = Action.CREATE;
				pair.Second = temp;
					
				player.SendMessage ("Password: " + new string (pass), 255, 255, 0, 0);
				player.SendMessage ("Open the chest to protect it!", 255, 0, 255, 0);
		
			// cache the action if it's not null!
			if (pair.First != Action.NOTHING) {
				ResetActions (player);
				Cache.Actions.Add (player.Name, pair);
			}
		}

		void UnlockCommand (ISender sender, ArgumentList args)
		{
				Player player = sender as Player;
				Protection temp = new Protection ();
				Pair<Action, Protection> pair = new Pair<Action, Protection> (Action.NOTHING, null);
			
				if (args.Count != 1) {
					player.SendMessage ("Usage: /cunlock <password>", 255, 255, 0, 0);
					return;
				}
					
				string Extra = args[0];
			
					// see if they have an action for unlock already ??
				Pair<Action, Protection> Password = Cache.Actions.Get (player.Name);
					
				if (Password.First == Action.UNLOCK) {
					string hash = SHA1.Hash (Extra);
					Protection PasswordProtection = Password.Second;
						
					// compare the passwords
					if (PasswordProtection.Data.Equals (hash)) {
						PasswordProtection.Access.Add (player.Name);
						ResetActions (player);
							
						// TODO: remove them from access when they log out
						player.SendMessage ("Password accepted!", 255, 0, 255, 0);
					} else {
						player.SendMessage ("Invalid password!", 255, 255, 0, 0);
					}
				} else {
					player.SendMessage ("You need to open a password-protected chest to do that!", 255, 255, 0, 0);
				}
		
			// cache the action if it's not null!
			if (pair.First != Action.NOTHING) {
				ResetActions (player);
				Cache.Actions.Add (player.Name, pair);
			}
		}

		void InfoCommand (ISender sender, ArgumentList argz)
		{
			Player player = sender as Player;
			Pair<Action, Protection> pair = new Pair<Action, Protection> (Action.NOTHING, null);
			
			pair.First = Action.INFO;
			player.SendMessage ("Open a chest to view information about it.", 255, 0, 255, 0);
			
			// cache the action if it's not null!
			if (pair.First != Action.NOTHING) {
				ResetActions (player);
				Cache.Actions.Add (player.Name, pair);
			}
		}

		void RemoveCommand (ISender sender, ArgumentList argz)
		{
			Player player = sender as Player;
			Pair<Action, Protection> pair = new Pair<Action, Protection> (Action.NOTHING, null);
			
			pair.First = Action.REMOVE;
			player.SendMessage ("Open a chest to remove a protection you own.", 255, 0, 255, 0);
		
			// cache the action if it's not null!
			if (pair.First != Action.NOTHING) {
				ResetActions (player);
				Cache.Actions.Add (player.Name, pair);
			}
		}

	}
}
