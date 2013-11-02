
using com.db4o;namespace Spring.Data
{
	/// <summary>
	/// Factory that allows to get Connections
	/// to DB4o Database (ObjectContainer)
	/// Currently, the only implementation is
	/// <see cref="T:Spring.Data.BasicDb4oDataSource>BasicDb4oDataSource</see>
	/// </summary>
	///
	public interface IDb4oDataSource
	{
		/// <summary>
		/// Get an active connection to the Database
		/// </summary>
		/// <returns> a Db4o ObjectContainer that
		/// can be used to query the database</returns>
		ObjectContainer GetConnection();
	}
}
