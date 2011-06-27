using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server;

namespace LWC
{
	public class Protection
	{
		// Public protection constant
		public const int PUBLIC_PROTECTION = 0;
		
		// Password protection constant
		public const int PASSWORD_PROTECTION = 1;
		
		// Private protection constant
		public const int PRIVATE_PROTECTION = 2;
		
		/**
		 * The internal chest id
		 */
		public int ChestId { get; set; }

		/**
		 * The player that owners the protection
		 */
		public string Owner { get; set; }
		
		/**
		 * Protection data such as a password
		 */
		public string Data { get; set; }
		
		/**
		 * The protection type
		 */
		public int Type { get; set; }
		
		/**
		 * The protection's x coordinate
		 */
		public int X { get; set; }
		
		/**
		 * The protection's y coordinate
		 */
		public int Y { get; set; }
		
		/**
		 * The players who can access the protection (besides the owner)
		 */
		public List<string> Access { get; private set; }
		
		/**
		 * If the protection is valid and correctly created in the world
		 */
		public bool Valid { get; set; }
		
		public Protection()
		{
			Access = new List<string>();
		}
		
		/**
		 * Remove the protection
		 */
		public void Remove()
		{
			if(!Valid)
			{
				return;
			}
			
			LWCPlugin.Get().Cache.Protections.Remove(new LocationKey(X, Y));
		}
		
		/**
		 * Convert the access list to a comma-delimited list
		 */
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
		
		/**
		 * Check if a player can access the protection
		 * 
		 * @param player
		 * @return true if the player can access the protection
		 */
		public bool CanAccess(Player player)
		{
			string playerName = player.getName();
			
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
		
		/**
		 * If the player is an Op, they are considered an owner!
		 * 
		 * @param player
		 * @return true if the player is considered an owner
		 */
		public bool IsOwner(Player player)
		{
			if(Owner.Equals(player.getName()))
			{
				return true;
			}
			
			if(player.isInOpList())
			{
				return true;
			}
			
			return false;
		}
		
		/**
		 * @return a textual representation of the protection type
		 */
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
		
		/**
		 * Find a protection at a given tile.
		 * 
		 * @param tile
		 * @return Protection object if found, otherwise null
		 */
		public static Protection findProtection(Tile tile)
		{
			if(!IsProtectable(tile.type))
			{
				return null;
			}
			
			return LWCPlugin.Get().Cache.Protections.Get(new LocationKey(tile.tileX, tile.tileY));
		}
		
		/**
		 * @param id
		 * @return true if the block is protectable
		 */
		public static bool IsProtectable(int id)
		{
			
			switch(id)
			{
					case 48: /* Regular chest */
					case 306: /* Gold chest */
					return true;

				default:
					return false;
			}

		}
		
		/**
		 * @param tile
		 * @return true if the tile is protectable
		 */
		public static bool IsProtectable(Tile tile)
		{
			return tile != null ? IsProtectable(tile.type) : false;
		}

	}
}
