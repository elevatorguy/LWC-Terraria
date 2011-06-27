using System;

namespace LWC
{
	public class LocationKey
	{
		
		/**
		 * The x location for the key
		 */
		public int X { get; set; }
		
		/**
		 * The y location for the key
		 */
		public int Y { get; set; }
		
		/**
		 * @return true if the two keys are equal
		 */
		public override bool Equals(object obj)
		{
			if(!(obj is LocationKey))
			{
				return false;
			}
			
			LocationKey other = (LocationKey) obj;
			
			return X == other.X && Y == other.Y;
		}
		
		/**
		 * @return the hash code of the location (x and y)
		 */
		public override int GetHashCode()
		{
			int hash = 23;
			
			hash = (hash * 31) + X.GetHashCode();
			hash = (hash * 31) + X.GetHashCode();
			
			return hash;
		}
		
	}
}
