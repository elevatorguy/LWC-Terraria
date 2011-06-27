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
		 * The cache of the currently loaded protections.
		 */
		public Dictionary<LocationKey, Protection> Protections { get; private set; }
		
		/**
		 * The "database" of actions players are using
		 */
		public Dictionary<string, Pair<Action, Protection>> Actions { get; private set; }
		
		public Store()
		{
			Protections = new Dictionary<LocationKey, Protection>();
			Actions = new Dictionary<string, Pair<Action, Protection>>();
		}
		
	}
}
