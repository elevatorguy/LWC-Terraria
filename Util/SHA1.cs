using System;
using System.Security.Cryptography;
using System.Text;

namespace LWC.Util
{
	public class SHA1Hash
	{
		
		public static string Hash(string input)
		{
			byte[] buffer = Encoding.ASCII.GetBytes(input);
			
			using var sha1 = System.Security.Cryptography.SHA1.Create();
			string hash = BitConverter.ToString(
				sha1.ComputeHash(buffer)).Replace("-", "");

			return hash;
		}
		
	}
}
