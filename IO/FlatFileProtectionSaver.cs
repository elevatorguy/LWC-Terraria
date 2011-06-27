using System;
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
			
			FileInfo fileInfo = new FileInfo(@database);
			
			// move the old lwc db if it exists
			if(File.Exists(@database))
			{
				fileInfo.MoveTo(@backup);
			}
			
			// now save the protections
			StreamWriter stream = fileInfo.CreateText();
			
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
			if(File.Exists(@backup))
			{
				FileInfo backupInfo = new FileInfo(@backup);
				backupInfo.Delete();
			}
		}
		
		/**
		 * Move a file to a different name
		 */
		public static void MoveFile(string originalName, string newName)
        {
            FileInfo fileInfo = new FileInfo(@originalName);
            DirectoryInfo dirInfo = null;

            if (!Directory.Exists(@newName))
            {
                dirInfo = new DirectoryInfo(@newName);
                dirInfo.Create();
            }

            if (File.Exists(@originalName))
            {
                fileInfo.MoveTo(newName);
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
