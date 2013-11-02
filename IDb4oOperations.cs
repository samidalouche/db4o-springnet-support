
namespace Spring.Data
{
	/// <summary>
	/// Operations supported by the db4o template
	/// </summary>
	public interface IDb4oOperations
	{
		
		/// <summary>
        /// Execute a ADO.NET operation on a command object using a delegate callback.
        /// </summary>
        /// <remarks>This allows for implementing arbitrary data access operations
        /// on a single command within Spring's managed DB4o environment.</remarks>
        /// <param name="del">The delegate called with a command object.</param>
        /// <returns>A result object returned by the action or null</returns>
		object Execute(Db4oDelegate del);
		
		/// <summary>
        /// Execute a ADO.NET operation on a command object using an Object Callback.
        /// </summary>
        /// <remarks>This allows for implementing arbitrary data access operations
        /// on a single command within Spring's managed DB4o environment.</remarks>
        /// <param name="callback">The delegate called with a command object.</param>
        /// <returns>A result object returned by the action or null</returns>
		object Execute(IDb4oCallback callback);
		
		
		
	}
}
