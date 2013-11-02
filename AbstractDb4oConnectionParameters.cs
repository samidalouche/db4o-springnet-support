
using com.db4o;namespace Spring.Data
{
	/// <summary>
	/// <para>
	/// A connection Parameter, must be subclassed.
	/// </para>
	/// <para> Db4o currently supports Several ways to connect to a database :
	/// - Direct File Access
	/// - Memory File
	/// - Embedded Client : Connects to an Embedded Server
	/// - Remote Client : Connects to a Remote Server
	/// </para>
	/// </summary>
	public abstract class AbstractDb4oConnectionParameters : IDb4oConnectionParameters
	{
		
		/// <summary>
		/// Method CreateObjectContainer
		/// </summary>
		/// <returns>An ObjectContainer</returns>
		public abstract ObjectContainer CreateObjectContainer();
	}
}
