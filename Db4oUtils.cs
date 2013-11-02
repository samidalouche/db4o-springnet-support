
using log4net;
using Spring.Data;
using Spring.Dao;
using System;
using com.db4o.ext;
using System.IO;
using com.db4o;
using Spring.Transaction.Support;
///<summary>
/// Utilities related to DB4o connections
/// </summary>
namespace Spring.Data
{
	public class Db4oUtils
	{
		#region Logging
        private static readonly ILog logger = LogManager.GetLogger(typeof(Db4oUtils));
		#endregion
		
		/// <summary>
		/// <p>
		/// Gets a connection from the given Db4oDataSource. Additionnally, it changes
		/// any DB4o Exception into the Spring.NET Hierarchy of Data Access Exceptions.
		/// </p>
		/// <p> Is Aware of corresponding connections bound to the current thread when using
		/// a TransactionManager. It will bind the connection to the current thread
		/// if Transaction Synchronization is active
		/// <p>
		/// </summary>
		public static ObjectContainer GetConnection(IDb4oDataSource dataSource){
			try{
				logger.Debug("GetConnection");
				return DoGetConnection(dataSource);
			} catch(Exception ex){
				logger.Error("Error Creating DB4o Connection", ex);
				// and translate exception
				throw TranslateException(ex);
			}
		}
		
		/// <summary>
		/// Method DoGetConnection does the actual Connection creation/recuperation
		/// It throws the orgiginal DB4o Exceptions
		/// </summary>
		/// <param name="dataSource">An IDb4oDataSource</param>
		/// <returns>An ObjectContainer</retutns>
		private static ObjectContainer DoGetConnection(IDb4oDataSource dataSource)
		{
			logger.Debug("Do Get Connection");
			logger.Debug("GetResource from TransactionSynchronizationManager");
			ObjectContainerHolder holder = (ObjectContainerHolder ) TransactionSynchronizationManager.GetResource(dataSource);
			
			ObjectContainer container;
			if(holder != null){
				logger.Debug("ObjectContainerHolder exists");
				holder.Requested();
				if(holder.ObjectContainer == null){
					logger.Debug("No connection inside the ObjectContainerHolder");
					logger.Debug("Creating One");
					holder.ObjectContainer = dataSource.GetConnection();
				}
				container = holder.ObjectContainer;
			} else{
				// the connection should be created
				logger.Debug("The Holder does not exist. It will be created");
				container = dataSource.GetConnection();
				if(TransactionSynchronizationManager.SynchronizationActive){
					logger.Debug("Registerbe increaseing transaction synchronization");
					logger.Debug("Will use the same connection for further DB4o actions within the transaction");
					logger.Debug("Thread-bound object will get removed by synchronization at transaction completion");
					holder = new ObjectContainerHolder(container);
					holder.SynchronizedWithTransaction = true;
					holder.Requested();
					TransactionSynchronizationManager.RegisterSynchronization(
						new ObjectContainerSynchronization(holder, dataSource));
					TransactionSynchronizationManager.BindResource(dataSource,holder);
				}
			}
			
			return container;
		}
		
		/// <summary>
		/// Close the given container, created via the given dataSource, if
		/// it is not managed externally (i.e. not bound to the thread)
		/// </summary>
		public static void DisposeConnection(ObjectContainer container, IDb4oDataSource dataSource)
		{
			logger.Debug("Dispose Connection");
			try{
				DoDisposeConnection(container, dataSource);
			} catch(Exception ex){
				logger.Error("Cannot dispose connection", ex);
				throw TranslateException(ex);
			}
		}
		
		/// <summary>
		/// Method DoReleaseConnection
		/// </summary>
		/// <param name="container">An ObjectContainer</param>
		/// <param name="dataSource">An IDb4oDataSource</param>
		private static void DoDisposeConnection(ObjectContainer container, IDb4oDataSource dataSource)
		{
			logger.Debug("DoDisposeConnection");
			
			if(container == null){
				logger.Debug("Container is null, doing nothing");
				return ;
			}
			
			if(dataSource != null){
				logger.Debug("Data Source is not null, trying to release");
				ObjectContainerHolder holder = (ObjectContainerHolder) TransactionSynchronizationManager.GetResource(dataSource);
				
				if(holder != null){
					logger.Debug("holder is not null, releasing");
					// should we make sure the connection is the right one ?
					holder.Released();
					
					// should not go further since we are in a transactional context
					// => No connection closing directly
					return;
				}
				
				logger.Debug("No transaction, closing the container");
				// no transactional context so
				// close the container
				container.close();
			}
			
			
		}
		
		/// <summary>
		/// Method translateException
		/// </summary>
		/// <param name="ex">An Exception</param>
		/// <returns>A  DataAccessException</retutns>
		public static DataAccessException TranslateException(Exception exception)
		{
		
			logger.Debug("Converting Db4o to Spring Exception", exception);
			if(exception is DatabaseFileLockedException){
				return new DataAccessResourceFailureException("Database is already locked", exception);
			}
			
			if(exception is ObjectNotStorableException){
				return new InvalidDataAccessApiUsageException("object is not storable " , exception);
			}
			
			if (exception is IOException){
				return new DataAccessResourceFailureException("can not do backup ", exception);
			}
			
			return new Db4oDataAccessException("Unknown DB4o Exception", exception);
		}
	
	
	
	/// <summary>
	/// Callback for resources cleanup at the end of a transaction.
	/// These methods will be called by the PlatformTransactionManager.
		/// In fact, only the TransactionManager can determine WHEN
		/// resources should be cleaned up
	/// </summary>
	private static class ObjectContainerSynchronization : TransactionSynchronizationAdapter
	{
		private ObjectContainerHolder _Holder;
		private IDb4oDataSource _DataSource;
			private bool _HolderActive = true;
		#region Logging
		private static readonly ILog logger = LogManager.GetLogger(typeof(ObjectContainerSynchronization));
		#endregion
			
		public ObjectContainerSynchronization(ObjectContainerHolder holder, IDb4oDataSource dataSource){
			_Holder = holder;
			_DataSource = dataSource;
		}
		
		
		
		/// <summary>
		/// Suspend this synchronization.
		/// </summary>
		/// <remarks>
		/// <p>
		/// Supposed to unbind resources from
		/// <see cref="Spring.Transaction.Support.TransactionSynchronizationManager"/>
		/// if managing any.
		/// </p>
		/// </remarks>
		public override void Suspend()
		{
			base.Suspend();
			logger.Debug("Suspend");
			if(this.HolderActive){
				TransactionSynchronizationManager.UnbindResource(_DataSource);
				if(_Holder.ObjectContainer != null){
					//DisposeConnection(_Holder.ObjectContainer, _DataSource);
					//_Holder.ObjectContainer = null;
				}
			}
		}
		
		/// <summary>
		/// Resume this synchronization.
		/// </summary>
		/// <remarks>
		/// <p>
		/// Supposed to unbind resources from
		/// <see cref="Spring.Transaction.Support.TransactionSynchronizationManager"/>
		/// if managing any.
		/// </p>
		/// </remarks>
		public override void Resume()
		{
			base.Resume();
			logger.Debug("Resume");
			if(HolderActive){
				//TransactionSynchronizationManager.BindResource(DataSource,_Holder);
			}
		}
		
		/// <summary>
		/// Invoked after transaction commit/rollback.
		/// </summary>
		/// <param name="status">
		/// Status according to <see cref="Spring.Transaction.Support.TransactionSynchronizationStatus"/>
		/// </param>
		/// <remarks>
		/// Can e.g. perform resource cleanup, in this case after transaction completion.
		/// <p>
		/// Note that exceptions will get propagated to the commit or rollback
		/// caller, although they will not influence the outcome of the transaction.
		/// </p>
		/// </remarks>
		public override void AfterCompletion(TransactionSynchronizationStatus status)
		{
			base.AfterCompletion(status);
			logger.Debug("After Completion");
			// if not closed BeforeCompletion, do it now
			// (and if it hasn't been done by someone else)
			if(TransactionSynchronizationManager.HasResource(_DataSource)){
				TransactionSynchronizationManager.UnbindResource(_DataSource);
				_HolderActive = false;
				if(_Holder.ObjectContainer != null){
					DisposeConnection(_Holder.ObjectContainer, _DataSource);
				}
			}
			
		}
		
		/// <summary>
		/// Invoked before transaction commit/rollback (after
		/// <see cref="Spring.Transaction.Support.ITransactionSynchronization.BeforeCommit"/>,
		/// even if
		/// <see cref="Spring.Transaction.Support.ITransactionSynchronization.BeforeCommit"/>
		/// threw an exception).
		/// </summary>
		/// <remarks>
		/// <p>
		/// Can e.g. perform resource cleanup.
		/// </p>
		/// <p>
		/// Note that exceptions will get propagated to the commit caller
		/// and cause a rollback of the transaction.
		/// </p>
		/// </remarks>
		public override void BeforeCompletion()
		{
			base.BeforeCompletion();
			
			logger.Debug("Before Completion");
			// Release connection early if the Holder is not opened
			// anymore. (ref count = 0)
			if(! _Holder.IsOpen){
				TransactionSynchronizationManager.UnbindResource(_DataSource);
				_HolderActive = false;
				if(_Holder.ObjectContainer != null){
					DisposeConnection(_Holder.ObjectContainer, _DataSource);
				}
			}
		}
		
		/// <summary>
		/// Invoked before transaction commit (before
		/// <see cref="Spring.Transaction.Support.ITransactionSynchronization.BeforeCompletion"/>)
		/// </summary>
		/// <param name="readOnly">
		/// If the transaction is defined as a read-only transaction.
		/// </param>
		/// <remarks>
		/// <p>
		/// Can flush transactional sessions to the database.
		/// </p>
		/// <p>
		/// Note that exceptions will get propagated to the commit caller and
		/// cause a rollback of the transaction.
		/// </p>
		/// </remarks>
		public override void BeforeCommit(bool readOnly)
		{
			base.BeforeCommit(readOnly);
			logger.Warn("BeforeCommit Not Implemented");
			
		}
		
		
		
		public ObjectContainerHolder Holder
		{
			set {
				_Holder = value;
			}
			
			get {
				return _Holder;
			}
		}
		
		public IDb4oDataSource DataSource
		{
			set {
				_DataSource = value;
			}
			
			get {
				return _DataSource;
			}
		}
			
			public bool HolderActive
			{
				set {
					_HolderActive = value;
				}
				
				get {
					return _HolderActive;
				}
			}
	}
}

}
