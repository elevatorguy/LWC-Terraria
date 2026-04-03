using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;
using Terraria;

namespace LWC
{
	public class Protection
	{
		public const int PUBLIC_PROTECTION = 0;
		
		public const int PASSWORD_PROTECTION = 1;
		
		public const int PRIVATE_PROTECTION = 2;
		
		public int ChestId { get; set; }

		public string Owner { get; set; }
		
		public string Data { get; set; }
		
		public int Type { get; set; }
		
		public int X { get; set; }
		
		public int Y { get; set; }
		
		public List<string> Access { get; private set; }
		
		public bool Valid { get; set; }
		
		public Protection()
		{
			Access = new List<string>();
		}
		
		public void Remove()
		{
			if(!Valid)
			{
				return;
			}
			
			LWCPlugin.Get().Cache.Protections.Remove(new LocationKey(X, Y));
		}
		
		public string AccessToString()
		{
			string access = "";
			
			foreach(string temp in Access)
			{
				access += temp + ",";
			}
			
			if(Access.Count > 0)
			{
				access = access.Substring(0, access.Length - 1);
			}
			
			return access;
		}
		
		public bool CanAccess(TSPlayer player)
		{
			string playerName = player.Name;
			
			if(IsOwner(player))
			{
				return true;
			}
			
			switch(Type)
			{
				case PUBLIC_PROTECTION:
					return true;
				
				case PRIVATE_PROTECTION:
				case PASSWORD_PROTECTION:
					if(Access.Contains(playerName))
					{
						return true;
					}
					break;
					
			}
			
			return false;
		}
		
		public bool IsOwner(TSPlayer player)
		{
			if(Owner.Equals(player.Name))
			{
				return true;
			}
			
			if(player.Group.HasPermission("admin"))
			{
				return true;
			}
			
			return false;
		}
		
		public string TypeToString()
		{
			switch(Type)
			{
				case PUBLIC_PROTECTION:
					return "Public";
					
				case PASSWORD_PROTECTION:
					return "Password";
					
				case PRIVATE_PROTECTION:
					return "Private";
					
				default:
					return "Unknown type (" + Type + ")";
			}
		}
	}
}
