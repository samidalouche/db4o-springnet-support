
using com.db4o;
using log4net;
using Spring.Objects.Factory;
using System;
using Spring.Dao;

namespace Spring.Data
{
	///<summary>
	/// <p>
	/// Base class for Db4oTemplate and a future Db4oInterceptor.
	/// Defines common properties such as a Db4o DataSource.
	/// </p>
	/// <p>
	/// See <see cref="T:Spring.Data.IDb4oDataSource">IDb4oDataSource</see>
	/// and
	/// <see cref="T:Spring.Data.BasicDb4oDataSource>BasicDb4oDataSource</see>
	/// </p>
	/// </summary>
	public abstract class AbstractDb4oAccessor : IInitializingObject
	{
		//private ObjectContainer _ObjectContainer;
		#region Logging
        private readonly ILog logger = LogManager.GetLogger(this.GetType());
        #endregion

		private IDb4oDataSource _DataSource;
		
		/// <summary> The datasource used to create connections
		/// </summary>
		public IDb4oDataSource DataSource
		{
			set {
				_DataSource = value;
			}
			
			get {
				return _DataSource;
			}
		}
		
		/// <summary>
		/// Convert the Db4o exception to an equivalent one from the
		/// the Spring Exception Hierarchy
		/// </summary>
		/// <returns> An exception from the
		/// <see cref="T:Spring.Dao.DataAccessException"> Spring DataAccessException Hierarchy </see>
		/// </returns>
		public DataAccessException ConvertDb4oAccessException(Exception ex) {
			logger.Debug("Converting Db4o to Spring Exception", ex);
			return Db4oUtils.TranslateException(ex);
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
			if (_DataSource== null)
				throw new Exception("DataSource is required");
		}
	}
}
