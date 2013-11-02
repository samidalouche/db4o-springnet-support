
using Spring.Transaction.Support;
using Spring.Transaction;
using System;
using System.Data;
using com.db4o;
using log4net;

namespace Spring.Data
{
	public class Db4oPlatformTransactionManager : AbstractPlatformTransactionManager
	{
		private IDb4oDataSource _DataSource;
		#region Logging
        private readonly ILog logger = LogManager.GetLogger(this.GetType());
        #endregion
		
		
		/// <summary>
		/// The DataSource used to query the TransactionSynchronizationManager
		/// Should be the same as the one specified in the DataUtils calls, since
		/// it is used as the key in the TransactionSynchronizationManager calls
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
		
		public Db4oPlatformTransactionManager(){
			NestedTransactionsAllowed = false;
		}
		
		public Db4oPlatformTransactionManager(IDb4oDataSource dataSource) : this() {
			_DataSource = DataSource;
		}
		
		/// <summary>
		/// Return the current transaction object.
		/// </summary>
		/// <returns>The current transaction object.</returns>
		/// <exception cref="Spring.Transaction.CannotCreateTransactionException">
		/// If transaction support is not available.
		/// </exception>
		/// <exception cref="Spring.Transaction.TransactionException">
		/// In the case of lookup or system errors.
		/// </exception>
		protected override Object DoGetTransaction()
		{
			logger.Debug("Do Get Transaction");
			Db4oTransactionObject txObject = new Db4oTransactionObject();
			ObjectContainerHolder contHolder = (ObjectContainerHolder) TransactionSynchronizationManager.GetResource(_DataSource);
			txObject.ObjectContainerHolder = contHolder;
			return txObject;
		}
		
		/// <summary>
		/// Check if the given transaction object indicates an existing,
		/// i.e. already begun, transaction.
		/// </summary>
		/// <param name="transaction">
		/// Transaction object returned by
		/// <see cref="Spring.Transaction.Support.AbstractPlatformTransactionManager.DoGetTransaction"/>.
		/// </param>
		/// <returns>True if there is an existing transaction.</returns>
		/// <exception cref="Spring.Transaction.TransactionException">
		/// In the case of system errors.
		/// </exception>
		protected override bool IsExistingTransaction(Object transaction)
		{
			logger.Debug("Transaction Already exists ?" + ((Db4oTransactionObject) transaction).HasTransaction);
			return ((Db4oTransactionObject) transaction).HasTransaction;
		}
		
		/// <summary>
		/// Begin a new transaction with the given transaction definition.
		/// </summary>
		/// <param name="transaction">
		/// Transaction object returned by
		/// <see cref="Spring.Transaction.Support.AbstractPlatformTransactionManager.DoGetTransaction"/>.
		/// </param>
		/// <param name="definition">
		/// <see cref="Spring.Transaction.ITransactionDefinition"/> instance, describing
		/// propagation behavior, isolation level, timeout etc.
		/// </param>
		/// <remarks>
		/// Does not have to care about applying the propagation behavior,
		/// as this has already been handled by this abstract manager.
		/// </remarks>
		/// <exception cref="Spring.Transaction.TransactionException">
		/// In the case of creation or system errors.
		/// </exception>
		protected override void DoBegin(Object transaction, ITransactionDefinition definition)
		{
			logger.Debug("Do Begin Transaction");
			
			if(definition.TransactionIsolationLevel != IsolationLevel.Unspecified){
				logger.Error("Db4o Does not support Isolation Level");
				throw new CannotCreateTransactionException("Db4o does not support an isolation level concept");
			}
			
			try{
			
				Db4oTransactionObject txObject = (Db4oTransactionObject) transaction;
				if (txObject.ObjectContainerHolder == null) {
					// use the given container
					logger.Warn ("Are we supposed to  be in this case ??");
					ObjectContainer container = Db4oUtils.GetConnection(_DataSource);
					logger.Debug("Using given objectContainer ["
								 + container
								 + "] for the current thread transaction");
					
					txObject.ObjectContainerHolder = new ObjectContainerHolder(container);
				}
				
				ObjectContainerHolder holder = txObject.ObjectContainerHolder;
				
				//holder. set readonly ??
				if(definition.TransactionTimeout != -1){
					logger.Debug("Setting Transaction Timeout : " + definition.TransactionTimeout);
					holder.TimeoutInSeconds = definition.TransactionTimeout;
				}
				
			} catch(Exception e){
				logger.Error("Cannot create transaction");
				throw new CannotCreateTransactionException("Cannot create transaction", e);
			}
			
		}
		
		/// <summary>
		/// Suspend the resources of the current transaction.
		/// </summary>
		/// <param name="transaction">
		/// Transaction object returned by
		/// <see cref="Spring.Transaction.Support.AbstractPlatformTransactionManager.DoGetTransaction"/>.
		/// </param>
		/// <returns>
		/// An object that holds suspended resources (will be kept unexamined for passing it into
		/// <see cref="Spring.Transaction.Support.AbstractPlatformTransactionManager.DoResume"/>.)
		/// </returns>
		/// <remarks>
		/// Transaction synchronization will already have been suspended.
		/// </remarks>
		/// <exception cref="Spring.Transaction.IllegalTransactionStateException">
		/// If suspending is not supported by the transaction manager implementation.
		/// </exception>
		/// <exception cref="Spring.Transaction.TransactionException">
		/// in case of system errors.
		/// </exception>
		protected override Object DoSuspend(Object transaction)
		{
			logger.Debug("Do Suspend");
			Db4oTransactionObject txObject = (Db4oTransactionObject) transaction;
			txObject.ObjectContainerHolder = null;
			ObjectContainerHolder containerHolder = (ObjectContainerHolder) TransactionSynchronizationManager.GetResource(_DataSource);
			return new SuspendedResourcesHolder(containerHolder);
		}
		
		/// <summary>
		/// Resume the resources of the current transaction.
		/// </summary>
		/// <param name="transaction">
		/// Transaction object returned by
		/// <see cref="Spring.Transaction.Support.AbstractPlatformTransactionManager.DoGetTransaction"/>.
		/// </param>
		/// <param name="suspendedResources">
		/// The object that holds suspended resources as returned by
		/// <see cref="Spring.Transaction.Support.AbstractPlatformTransactionManager.DoSuspend"/>.
		/// </param>
		/// <remarks>
		/// Transaction synchronization will be resumed afterwards.
		/// </remarks>
		/// <exception cref="Spring.Transaction.IllegalTransactionStateException">
		/// If suspending is not supported by the transaction manager implementation.
		/// </exception>
		/// <exception cref="Spring.Transaction.TransactionException">
		/// In the case of system errors.
		/// </exception>
		protected override void DoResume(Object transaction, Object suspendedResources)
		{
			logger.Debug("Do Resume");
			SuspendedResourcesHolder resourcesHolder = (SuspendedResourcesHolder) suspendedResources;
			Db4oTransactionObject txObject = (Db4oTransactionObject) transaction;
			txObject.ObjectContainerHolder = resourcesHolder.ObjectContainerHolder;
			if (TransactionSynchronizationManager.HasResource(_DataSource)) {
				TransactionSynchronizationManager.UnbindResource(_DataSource);
			}
			TransactionSynchronizationManager.BindResource(_DataSource,
														   resourcesHolder.ObjectContainerHolder);
		}
		
		/// <summary>
		/// Perform an actual commit on the given transaction.
		/// </summary>
		/// <param name="status">The status representation of the transaction.</param>
		/// <remarks>
		/// <p>
		/// An implementation does not need to check the rollback-only flag.
		/// </p>
		/// </remarks>
		/// <exception cref="Spring.Transaction.TransactionException">
		/// In the case of system errors.
		/// </exception>
		protected override void DoCommit(DefaultTransactionStatus status)
		{
			logger.Debug("Do Commit");
			Db4oTransactionObject txObject = (Db4oTransactionObject) status.Transaction;
			try{
				txObject.ObjectContainerHolder.ObjectContainer.commit();
			}catch(Exception e){
				throw new TransactionSystemException("Cannot commit Db4o transaction", e);
			}
		}
		
		/// <summary>
		/// Perform an actual rollback on the given transaction.
		/// </summary>
		/// <param name="status">The status representation of the transaction.</param>
		/// <remarks>
		/// An implementation does not need to check the new transaction flag.
		/// </remarks>
		/// <exception cref="Spring.Transaction.TransactionException">
		/// In the case of system errors.
		/// </exception>
		protected override void DoRollback(DefaultTransactionStatus status)
		{
			logger.Debug("Do Rollback");
			Db4oTransactionObject txObject = (Db4oTransactionObject) status.Transaction;
			try{
				txObject.ObjectContainerHolder.ObjectContainer.rollback();
			}catch(Exception e){
				throw new TransactionSystemException("Cannot rollback Db4o transaction", e);
			}
		}
		
		/// <summary>
		/// Set the given transaction rollback-only. Only called on rollback
		/// if the current transaction takes part in an existing one.
		/// </summary>
		/// <param name="status">The status representation of the transaction.</param>
		/// <exception cref="Spring.Transaction.TransactionException">
		/// In the case of system errors.
		/// </exception>
		protected override void DoSetRollbackOnly(DefaultTransactionStatus status)
		{
			logger.Debug("Do Set Rollback Only");
			Db4oTransactionObject txObject = (Db4oTransactionObject) status.Transaction;
			//txObject.
			// FIXME
		}
		
		
	
	private static class Db4oTransactionObject : ISmartTransactionObject
	{
		private ObjectContainerHolder _ObjectContainerHolder;
		
		/// <summary> The associated ObjectContainerHolder
		/// </summary>
		public ObjectContainerHolder ObjectContainerHolder
		{
			set {
				_ObjectContainerHolder = value;
			}
			
			get {
				return _ObjectContainerHolder;
			}
		}
		
		/// <summary>
		/// Return whether the transaction is internally marked as rollback-only.
		/// </summary>
		/// <returns>True of the transaction is marked as rollback-only.</returns>
		public bool IsRollbackOnly()
		{
			return _ObjectContainerHolder.RollbackOnly;
		}
		
		public bool HasTransaction {
			get{
				// Db4o executes everything in a transaction
				return _ObjectContainerHolder != null;
			}
		}
		
	}
		
		/// <summary>
		/// Holder for suspended resources. Used internally by doSuspend and
		/// doResume.
		/// </summary>
		private static class SuspendedResourcesHolder {
			
			private readonly ObjectContainerHolder _ObjectContainerHolder;
			
			public SuspendedResourcesHolder(ObjectContainerHolder containerHolder) {
				this._ObjectContainerHolder = containerHolder;
			}
			
			public ObjectContainerHolder ObjectContainerHolder
			{
				get {
					return _ObjectContainerHolder;
				}
			}
			
			
		}
}
}
