using System;
using System.IO;

using TerrariaApi.Server;
using TShockAPI;

using LWC.IO;
using Terraria;
using Terraria.ID;

namespace LWC
{
    [ApiVersion(2, 1)]
    public partial class LWCPlugin : TerrariaPlugin
    {
        public override string Name => "LWC";
        public override string Author => "Hidendra";
        public override string Description => "Chest protection mod";
        public override Version Version => new Version(1, 11, 6);

        public Store Cache { get; private set; }
        
        private ProtectionLoader Loader;
        
        private ProtectionSaver Saver;
        
        private static LWCPlugin instance = null;
        
        public static string Folder = "";

        public void ResetActions(TSPlayer player)
        {
            if (Cache != null)
                Cache.Actions.Remove(player.Name);
        }
        
        public LWCPlugin(Main game) : base(game)
        {
            instance = this;
        }
        
        public override void Initialize()
        {
            Log("Initializing v" + Version);
            
            Folder = Path.Combine(TShock.SavePath, "LWC");
            
            if(!Directory.Exists(Folder))
            {
                Directory.CreateDirectory(Folder);
            }
            
            Loader = new FlatFileProtectionLoader();
            Saver = new FlatFileProtectionSaver();
            
            Cache = new Store();
        
            Log("Syncing protections");
            
            foreach(Protection protection in Loader.LoadProtections())
            {
                Cache.Protections.Add(new LocationKey(protection.X, protection.Y), protection);
            }
            
            Log("Loaded " + Cache.Protections.Count + " protections!");
            
            Commands.ChatCommands.Add(new Command("lwc.public", PublicCommand, "cpublic"));
            Commands.ChatCommands.Add(new Command("lwc.private", PrivateCommand, "cprivate"));
            Commands.ChatCommands.Add(new Command("lwc.password", PasswordCommand, "cpassword"));
            Commands.ChatCommands.Add(new Command("lwc.unlock", UnlockCommand, "cunlock"));
            Commands.ChatCommands.Add(new Command("lwc.info", InfoCommand, "cinfo"));
            Commands.ChatCommands.Add(new Command("lwc.remove", RemoveCommand, "cremove"));

            GetDataHandlers.ChestOpen += OnChestOpen;
            GetDataHandlers.ChestItemChange += OnChestItemChange;
        }
        
        private void OnChestOpen(object sender, GetDataHandlers.ChestOpenEventArgs e)
        {
            if (e.Handled)
                return;

            TSPlayer player = e.Player;
            int x = e.X;
            int y = e.Y;

            LocationKey key = new LocationKey(x, y);
            Protection protection = Cache.Protections.Get(key);

            Pair<Action, Protection> pair = Cache.Actions.Get(player.Name);

            if (protection != null)
            {
                bool canAccess = protection.CanAccess(player);

                if (!canAccess)
                {
                    if (protection.Type == Protection.PASSWORD_PROTECTION)
                    {
                        Pair<Action, Protection> PassTemp = new Pair<Action, Protection>(Action.UNLOCK, protection);
                        ResetActions(player);
                        Cache.Actions.Add(player.Name, PassTemp);
                        
                        player.SendMessage("This chest is locked with a password!", 255, 255, 0);
                        player.SendMessage("Type /cunlock <password> to unlock it.", 150, 255, 0);
                    }
                    
                    player.SendErrorMessage("That chest is locked with a magical spell.");
                    e.Handled = true;
                    return;
                }

                if (pair != null)
                {
                    switch (pair.First)
                    {
                        case Action.INFO:
                            player.SendMessage("Owner: " + protection.Owner, 255, 255, 0);
                            player.SendMessage("Type: " + protection.TypeToString(), 255, 255, 0);
                            ResetActions(player);
                            break;

                        case Action.REMOVE:
                            if (!protection.IsOwner(player))
                            {
                                player.SendErrorMessage("You are not the owner of that chest!");
                            }
                            else
                            {
                                protection.Remove();
                                Cache.Actions.Remove(player.Name);
                                player.SendSuccessMessage("Protection removed!");
                            }
                            break;
                    }
                }
            }
            else if (pair != null)
            {
                switch (pair.First)
                {
                    case Action.CREATE:
                        int chestIndex = -1;
                        for (int i = 0; i < Main.chest.Length; i++)
                        {
                            if (Main.chest[i] != null && Main.chest[i].x == x && Main.chest[i].y == y)
                            {
                                chestIndex = i;
                                break;
                            }
                        }

                        if (chestIndex >= 0)
                        {
                            var temp = pair.Second;
                            temp.ChestId = (short)chestIndex;
                            temp.X = x;
                            temp.Y = y;
                            temp.Valid = true;

                            Cache.Protections.Add(key, temp);
                            Cache.Actions.Remove(player.Name);

                            player.SendSuccessMessage("Registered a " + temp.TypeToString() + " Chest successfully!");
                        }
                        break;
                }
            }
        }

        private void OnChestItemChange(object sender, GetDataHandlers.ChestItemEventArgs e)
        {
            if (e.Handled)
                return;

            TSPlayer player = e.Player;
            short chestId = e.ID;

            if (chestId < 0 || chestId >= Main.chest.Length || Main.chest[chestId] == null)
                return;

            Chest chest = Main.chest[chestId];
            LocationKey key = new LocationKey(chest.x, chest.y);
            Protection protection = Cache.Protections.Get(key);

            if (protection != null && !protection.CanAccess(player))
            {
                player.SendErrorMessage("That chest is locked!");
                e.Handled = true;
            }
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Log("Disposing v" + Version);
                
                GetDataHandlers.ChestOpen -= OnChestOpen;
                GetDataHandlers.ChestItemChange -= OnChestItemChange;

                if (Cache != null)
                {
                    Log("Desyncing protections");
                    
                    try
                    {
                        Protection[] protections = new Protection[Cache.Protections.Count];
                        
                        if(protections.Length > 0)
                        {
                            Cache.Protections.Values.CopyTo(protections, 0);
                            Saver.SaveProtections(protections);
                        }
                    }
                    catch(Exception exception)
                    {
                        Log("Exception occured! " + exception.Message);
                        Log(exception.ToString());
                    }
                    
                    Log("LWC has been disabled!");
                }
            }
            
            base.Dispose(disposing);
        }

        public static LWCPlugin Get()
        {
            return instance;
        }

        public static void Log(string message)
        {
            if (TShock.Log != null)
                TShock.Log.Info("[LWC] " + message);
            else
                Console.WriteLine("[LWC] " + message);
        }
    }
}
