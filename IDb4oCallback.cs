
using com.db4o;

namespace Spring.Data
{
	/// <summary>
    /// Generic callback interface for code that operates on a
    /// ObjectContainer.
	/// </summary>
	/// <remarks>
	/// <p>Allows you to execute any number of operations
	/// on a single ObjectContainer
	/// </p>
	/// <p>Used internally by Db4oTemplate, but also useful for
	/// application code.  Note that the passed in ObjectContainer
	/// has been created by the framework.  </p>
	/// </remarks>
	public interface IDb4oCallback
	{
		/// <summary>
        /// Called by Db4oTemplate.Execute with an active Db4o ObjectContainer.
        /// The calling code does not need to care about closing the
        /// command or the connection, or
        /// about handling transactions:  this will all be handled by
        /// Spring's Db4oTemplate
        /// </summary>
        /// <param name="command">An active ObjectContainer instance</param>
        /// <returns>The result object</returns>
		object DoInDb4o(ObjectContainer container);
	}
}
