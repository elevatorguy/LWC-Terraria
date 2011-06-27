using System;
using System.Collections.Generic;

namespace LWC
{
	/**
	 * Stores various loaded objects (e.g protections, etc.)
	 */
	public class Store
	{
		
		/**
		 * The cache of the currently loaded protections
		 */
		private Dictionary<LocationKey, Protection> protections = new Dictionary<LocationKey, Protection>();
		
		
	}
}
