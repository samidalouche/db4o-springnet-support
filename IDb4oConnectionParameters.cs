
using com.db4o;namespace Spring.Data
{
	public interface IDb4oConnectionParameters
	{
		/// <summary>
		/// Creates an Object Container
		/// </summary>
		/// <returns>
		/// A Db4o ObjectContainer that can be used
		/// to query the database
		/// </returns>
		ObjectContainer CreateObjectContainer();
	}
}
