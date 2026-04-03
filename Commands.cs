using TShockAPI;
using LWC.Util;
using Terraria;

namespace LWC
{
    public partial class LWCPlugin
    {
        void PublicCommand(CommandArgs args)
        {
            TSPlayer player = args.Player;
            Protection temp = new Protection();
            Pair<Action, Protection> pair = new Pair<Action, Protection>(Action.NOTHING, null);
        
            temp.Owner = player.Name;
            temp.Type = Protection.PUBLIC_PROTECTION;

            pair.First = Action.CREATE;
            pair.Second = temp;
                
            player.SendSuccessMessage("Open the chest to protect it!");
            
            if (pair.First != Action.NOTHING)
            {
                ResetActions(player);
                Cache.Actions.Add(player.Name, pair);
            }
        }

        void PrivateCommand(CommandArgs args)
        {
            TSPlayer player = args.Player;
            Protection temp = new Protection();
            Pair<Action, Protection> pair = new Pair<Action, Protection>(Action.NOTHING, null);
        
            temp.Owner = player.Name;
            temp.Type = Protection.PRIVATE_PROTECTION;
                
            pair.First = Action.CREATE;
            pair.Second = temp;
                
            player.SendSuccessMessage("Open the chest to protect it!");
        
            if (pair.First != Action.NOTHING)
            {
                ResetActions(player);
                Cache.Actions.Add(player.Name, pair);
            }
        }

        void PasswordCommand(CommandArgs args)
        {
            TSPlayer player = args.Player;
            Protection temp = new Protection();
            Pair<Action, Protection> pair = new Pair<Action, Protection>(Action.NOTHING, null);
        
            if (args.Parameters.Count != 1)
            {
                player.SendErrorMessage("Usage: /cpassword <password>");
                return;
            }
            
            string extra = args.Parameters[0];
        
            temp = new Protection();
            temp.Owner = player.Name;
            temp.Type = Protection.PASSWORD_PROTECTION;
            temp.Data = SHA1Hash.Hash(extra);
                
            char[] pass = extra.ToCharArray();
            for (int index = 0; index < pass.Length; index++)
            {
                pass[index] = '*';
            }
                
            pair.First = Action.CREATE;
            pair.Second = temp;
                
            player.SendInfoMessage("Password: " + new string(pass));
            player.SendSuccessMessage("Open the chest to protect it!");
        
            if (pair.First != Action.NOTHING)
            {
                ResetActions(player);
                Cache.Actions.Add(player.Name, pair);
            }
        }

        void UnlockCommand(CommandArgs args)
        {
            TSPlayer player = args.Player;
            Protection temp = new Protection();
            Pair<Action, Protection> pair = new Pair<Action, Protection>(Action.NOTHING, null);
        
            if (args.Parameters.Count != 1)
            {
                player.SendErrorMessage("Usage: /cunlock <password>");
                return;
            }
                
            string extra = args.Parameters[0];
        
            Pair<Action, Protection> Password = Cache.Actions.Get(player.Name);
                
            if (Password.First == Action.UNLOCK)
            {
                string hash = SHA1Hash.Hash(extra);
                Protection PasswordProtection = Password.Second;
                        
                if (PasswordProtection.Data.Equals(hash))
                {
                    PasswordProtection.Access.Add(player.Name);
                    ResetActions(player);
                            
                    player.SendSuccessMessage("Password accepted!");
                }
                else
                {
                    player.SendErrorMessage("Invalid password!");
                }
            }
            else
            {
                player.SendErrorMessage("You need to open a password-protected chest to do that!");
            }
        
            if (pair.First != Action.NOTHING)
            {
                ResetActions(player);
                Cache.Actions.Add(player.Name, pair);
            }
        }

        void InfoCommand(CommandArgs args)
        {
            TSPlayer player = args.Player;
            Pair<Action, Protection> pair = new Pair<Action, Protection>(Action.NOTHING, null);
            
            pair.First = Action.INFO;
            player.SendSuccessMessage("Open a chest to view information about it.");
            
            if (pair.First != Action.NOTHING)
            {
                ResetActions(player);
                Cache.Actions.Add(player.Name, pair);
            }
        }

        void RemoveCommand(CommandArgs args)
        {
            TSPlayer player = args.Player;
            Pair<Action, Protection> pair = new Pair<Action, Protection>(Action.NOTHING, null);
            
            pair.First = Action.REMOVE;
            player.SendSuccessMessage("Open a chest to remove a protection you own.");
        
            if (pair.First != Action.NOTHING)
            {
                ResetActions(player);
                Cache.Actions.Add(player.Name, pair);
            }
        }
    }
}
