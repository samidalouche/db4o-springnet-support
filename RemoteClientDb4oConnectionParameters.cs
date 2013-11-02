
using log4net;
using com.db4o;namespace Spring.Data
{
	public class RemoteClientDb4oConnectionParameters : AbstractDb4oConnectionParameters
	{
		
		/// <summary>
		/// Method CreateObjectContainer
		/// </summary>
		/// <returns>An ObjectContainer</returns>
		public override ObjectContainer CreateObjectContainer()
		{
			logger.Debug("Connection Type: Remote Client");
			ObjectContainer container;
			
			
			//RemoteClientDb4oConnectionParameters parameters = (RemoteClientDb4oConnectionParameters) _Db4oConnectionParameters;
			container = Db4o.openClient(_Hostname, _Port, _UserName, _Password);
			return container;
		}
		
		private string _Hostname;
		private string _UserName;
		private string _Password;
		private int _Port;
	
		#region Logging
        private readonly ILog logger = LogManager.GetLogger(this.GetType());
        #endregion
		
		/// <summary>
		/// The hostname
		/// </summary>
		public string Hostname
		{
			set {
				_Hostname = value;
			}
			
			get {
				return _Hostname;
			}
		}
		
		/// <summary>
		/// The user name used to connect to the server
		/// </summary>
		public string UserName
		{
			set {
				_UserName = value;
			}
			
			get {
				return _UserName;
			}
		}
		
		/// <summary>
		/// The Password used to connect to the server
		/// </summary>
		public string Password
		{
			set {
				_Password = value;
			}
			
			get {
				return _Password;
			}
		}
		
		/// <summary>
		/// The port on which the server is running
		/// </summary>
		public int Port
		{
			set {
				_Port = value;
			}
			
			get {
				return _Port;
			}
		}
		
		
	}
}
