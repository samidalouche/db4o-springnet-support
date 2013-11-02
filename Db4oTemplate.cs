using log4net;
using com.db4o;
using System;
using Spring.Transaction.Support;

namespace Spring.Data
{
	/// <summary>
	/// A Template that allows to run code against Db4o ObjectContainer
	/// </summary>
	public class Db4oTemplate : AbstractDb4oAccessor,IDb4oOperations
	{
		
		#region Logging
        private readonly ILog logger = LogManager.GetLogger(this.GetType());
        #endregion
		
		public Db4oTemplate(IDb4oDataSource dataSource)
		{
			base.DataSource= dataSource;
			AfterPropertiesSet();
		}
		
		/// <summary>
		/// Execute a ADO.NET operation on a command object using a delegate callback.
		/// </summary>
		/// <remarks>This allows for implementing arbitrary data access operations
		/// on a single command within Spring's managed DB4o environment.</remarks>
		/// <param name="del">The delegate called with a command object.</param>
		/// <returns>A result object returned by the action or null</returns>
		public Object Execute(Db4oDelegate del)
		{
			logger.Debug("Execute delegate");
			IDb4oCallback execObject = new ExecuteDb4oCallbackUsingDelegate(del);
			return Execute(execObject);
		}
		
		/// <summary>
		/// Execute a ADO.NET operation on a command object using an Object Callback.
		/// </summary>
		/// <remarks>This allows for implementing arbitrary data access operations
		/// on a single command within Spring's managed DB4o environment.</remarks>
		/// <param name="callback">The delegate called with a command object.</param>
		/// <returns>A result object returned by the action or null</returns>
		public Object Execute(IDb4oCallback callback)
		{
			logger.Debug("Execute Callback");
			// get a possibly existing connection
			ObjectContainer container = Db4oUtils.GetConnection(DataSource);
			object result;
			try{
				logger.Debug("DoInDb4o");
				result = callback.DoInDb4o(container);
			} catch(Exception e){
				logger.Error("Exception while executing in Db4o", e);
				throw ConvertDb4oAccessException(e);
			}
			
			
			// dispose from the connection
			Db4oUtils.DisposeConnection(container, DataSource);
			
			
			return result;
		}
		
		
	
	
	private class ExecuteDb4oCallbackUsingDelegate : IDb4oCallback
	{
		private Db4oDelegate del;
		
		public ExecuteDb4oCallbackUsingDelegate(Db4oDelegate d)
		{
			del = d;
		}
		
		public object DoInDb4o(ObjectContainer container)
		{
			return del(container);
		}
	}
}
}

