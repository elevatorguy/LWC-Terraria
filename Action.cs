using System;

namespace LWC
{
	/**
	 * An action is defined by a LWC action; this can mean they attempted to open a Password protection
	 * and are waiting for it to open, or that we are waiting for them to open a chest to
	 * finalize a protection.
	 */
	public enum Action
	{
		/**
		 * Create a protection of any type
		 */
		CREATE,
		
		/**
		 * Remove a protection that you have access to
		 */
		REMOVE,
		
		/**
		 * View info on an existing protection
		 */
		INFO,
		
		/**
		 * Unlock a locked password protection
		 */
		UNLOCK
		
	}
}
