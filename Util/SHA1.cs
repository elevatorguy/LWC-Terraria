using System;
using System.Security.Cryptography;
using System.Text;

namespace LWC.Util
{
	public class SHA1
	{
		
		public static string Hash(string input)
		{
			byte[] buffer = Encoding.ASCII.GetBytes(input);
			
			SHA1CryptoServiceProvider cryptoTransformSHA1 =
				new SHA1CryptoServiceProvider();
			
			string hash = BitConverter.ToString(
				cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "");

			return hash;
		}
		
	}
}
