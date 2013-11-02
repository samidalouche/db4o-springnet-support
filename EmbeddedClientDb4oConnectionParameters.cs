
using com.db4o;
using log4net;namespace Spring.Data
{
	/// <summary>
	/// </summary>
	public class EmbeddedClientDb4oConnectionParameters : AbstractDb4oConnectionParameters
	{
		
		/// <summary>
		/// Method CreateObjectContainer
		/// </summary>
		/// <returns>An ObjectContainer</returns>
		public override ObjectContainer CreateObjectContainer()
		{
			logger.Debug("Connection Type: Embeddded Client");
			ObjectContainer container;
		
			// EmbeddedClientDb4oConnectionParameters parameters = (EmbeddedClientDb4oConnectionParameters) _Db4oConnectionParameters;
			container = _Server.openClient();
			return container;
		}
		
		private ObjectServer _Server;
		
		#region Logging
        private readonly ILog logger = LogManager.GetLogger(this.GetType());
        #endregion
		
		/// <summary>The running ObjectServer that is used
		/// to obtain connections
		/// </summary>
		public ObjectServer Server
		{
			set {
				_Server = value;
			}
			
			get {
				return _Server;
			}
		}
	}
}
