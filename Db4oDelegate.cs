using com.db4o;

namespace Spring.Data
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
	public delegate object Db4oDelegate(ObjectContainer container);
}
