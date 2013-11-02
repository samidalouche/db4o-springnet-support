
using com.db4o;
using Spring.Objects.Factory;
using System;
using com.db4o.ext;
using log4net;
namespace Spring.Data
{
	/// <summary>
	/// A Simple non-pooling Factory that allows to create
	/// DB4o Datasources depending on a
	/// <see cref="T:Spring.Data.AbstractDb4oConnectionParameters>Db4oConnectionParameters</see>
	/// </summary>
	public class BasicDb4oDataSource : IDb4oDataSource,IInitializingObject, IDisposable
	{
		//private Db4oConnectionType _Db4oConnectionType;
		private IDb4oConnectionParameters _Db4oConnectionParameters;
		#region Logging
        private readonly ILog logger = LogManager.GetLogger(this.GetType());
        #endregion
		
		/// <summary>
		/// The Connection Parameters
		/// </summary>
		public IDb4oConnectionParameters Db4oConnectionParameters
		{
			set {
				_Db4oConnectionParameters = value;
			}
			
			get {
				return _Db4oConnectionParameters;
			}
		}
		
		
		
		/// <summary>
		/// Method GetConnection
		/// </summary>
		/// <returns>An ObjectContainer</returns>
		public ObjectContainer GetConnection()
		{
			ObjectContainer container;
			
			logger.Debug("GetConnection");
			container = _Db4oConnectionParameters.CreateObjectContainer();
			logger.Debug("Connection Created with Db4o: " + Db4o.version());
			
			return container;
		}
	
		
		
		/// <summary>
		/// Invoked by an <see cref="T:Spring.Objects.Factory.IObjectFactory"></see>
		/// after it has injected all of an object's dependencies.
		/// </summary>
		/// <remarks>
		/// <p>
		/// This method allows the object instance to perform the kind of
		/// initialization only possible when all of it's dependencies have
		/// been injected (set), and to throw an appropriate exception in the
		/// event of misconfiguration.
		/// </p>
		/// <p>
		/// Please do consult the class level documentation for the
		/// <see cref="T:Spring.Objects.Factory.IObjectFactory"></see> interface for a
		/// description of exactly <i>when</i> this method is invoked. In
		/// particular, it is worth noting that the
		/// <see cref="T:Spring.Objects.Factory.IObjectFactoryAware"></see>
		/// and <see cref="T:Spring.Context.IApplicationContextAware"></see>
		/// callbacks will have been invoked <i>prior</i> to this method being
		/// called.
		/// </p>
		/// </remarks>
		/// <exception cref="T:System.Exception">
		/// In the event of misconfiguration (such as the failure to set a
		/// required property) or if initialization fails.
		/// </exception>
		public void AfterPropertiesSet()
		{
			// check nullity
			if(_Db4oConnectionParameters == null){
				throw new Exception("Db4oConnectionParameters cannot be null");
			}
		}
		
		/// <summary>
		/// Method Dispose
		/// </summary>
		public void Dispose()
		{
			// TODO: Implement this method
			// Should cleanup connections that were created ?
		}
		
	}
}
