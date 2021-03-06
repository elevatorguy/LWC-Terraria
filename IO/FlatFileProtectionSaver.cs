﻿using System;
using System.IO;

namespace LWC.IO
{
	public class FlatFileProtectionSaver : ProtectionSaver
	{
		
		public void SaveProtections(Protection[] protections)
		{
			if(protections == null || protections.Length == 0)
			{
				// nothing to save
				return;
			}
			
			string database = LWCPlugin.Folder + "lwc.db";
			string backup = LWCPlugin.Folder + "lwc.db.bak";
			
			// ensure a dirty backup does not exist
			if(File.Exists(backup))
			{
				File.Delete(backup);
			}
			
			// move the old lwc db if it exists
			if(File.Exists(database))
			{
				File.Move(database, backup);
			}
			
			// now save the protections
			StreamWriter stream = File.CreateText(database);
			
			foreach(Protection protection in protections)
			{
				string converted = ConvertProtection(protection);
				
				if(converted == null)
				{
					continue;
				}
				
				stream.WriteLine(converted);
				stream.Flush();
			}
			
			// close the stream
			stream.Close();
			
			// everything seems ok, let's delete the backup
			if(File.Exists(backup))
			{
				File.Delete(backup);
			}
		}
		
		/**
		 * Convert a protection to the flatfile format
		 */
		public string ConvertProtection(Protection protection)
		{
			if(protection == null || !protection.Valid)
			{
				return null;
			}
			
			string AccessList = "";
			
			// if it's a private protection, we should save the access list
			if(protection.Type == Protection.PRIVATE_PROTECTION)
			{
				AccessList = protection.AccessToString();
			}
			
			return string.Format("{0}:{1}:{2}:{3}:{4}:{5}",
			                     protection.ChestId, protection.Owner, protection.Data, protection.Type,
			                     (protection.X + "," + protection.Y), AccessList);
		}
		
	}
}
