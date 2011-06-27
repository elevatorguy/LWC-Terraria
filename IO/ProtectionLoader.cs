using System;

namespace LWC.IO
{
	public interface ProtectionLoader
	{
		
		/**
		 * @return the array of protections that were loaded
		 */
		Protection[] LoadProtections();
		
	}
}
