using System;
using System.Collections.Generic;
using System.IO;

namespace LWC.IO
{
	public class FlatFileProtectionLoader : ProtectionLoader
	{
		
		/**
		 * @return array of protections loaded
		 */
		public Protection[] LoadProtections()
		{
			string database = LWCPlugin.Folder + "lwc.db";
			
			if(!File.Exists(database))
			{
				return new Protection[0];
			}
			
			List<Protection> protections = new List<Protection>();
			
			// open the file & read it
			using(StreamReader reader = File.OpenText(database))
			{
				string line = "";
				
				while((line = reader.ReadLine()) != null)
				{
					string[] split = line.Split(':');
					
					// check for consistency
					if(split.Length != 6)
					{
						continue;
					}
					
					Protection protection = new Protection();
					
					// ChestId:Owner:Data:Type:x,y:access1,access2,access3.....
					int ChestId = int.Parse(split[0]);
					string Owner = split[1];
					string Data = split[2];
					int Type = int.Parse(split[3]);
					string Location = split[4];
					string Access = split[5];
					
					// parse x/y
					string[] Location2 = Location.Split(',');
					
					int X = int.Parse(Location2[0]);
					int Y = int.Parse(Location2[1]);
					
					// parse Access
					foreach(string Temp in Access.Split(','))
					{
						protection.Access.Add(Temp);
					}
					
					// set everything else
					protection.ChestId = ChestId;
					protection.Owner = Owner;
					protection.Data = Data;
					protection.Type = Type;
					protection.X = X;
					protection.Y = Y;
					protection.Valid = true;
					
					// good!
					LWCPlugin.Get().Cache.Protections.Add(new LocationKey(X, Y), protection);				}
			}
			
			return protections.ToArray();
		}
		
	}
}
