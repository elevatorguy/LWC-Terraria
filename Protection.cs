using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server;

namespace LWC
{
    class Protection
    {
    	/**
    	 * Public protection constant
    	 */
    	public const int PUBLIC_PROTECTION = 0;
    	
    	/**
    	 * Password protection constant
    	 */
    	public const int PASSWORD_PROTECTION = 1;
    	
    	/**
    	 * Private protection constant
    	 */
    	private const int PRIVATE_PROTECTION = 2;

    	/**
    	 * The player that owners the protection
    	 */
    	private string Owner { get; set; }
    	
    	/**
    	 * The protection type
    	 */
    	private int Type { get; set; }
    	
    	/**
    	 * The protection's x coordinate
    	 */
    	private int X { get; set; }
    	
    	/**
    	 * The protection's y coordinate
    	 */
    	private int Y { get; set; }
    	
    	/**
    	 * The players who can access the protection (besides the owner)
    	 */
    	private List<string> Access { get; private set; }
    	
    	public Protection()
    	{
    		Access = new List<string>();
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
			
			return null;
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
