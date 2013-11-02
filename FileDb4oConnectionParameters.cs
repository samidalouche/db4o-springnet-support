
using Spring.Globalization;
using com.db4o;
using log4net;

namespace Spring.Data
{
	public class FileDb4oConnectionParameters: AbstractDb4oConnectionParameters
	{
		#region Logging
        private readonly ILog logger = LogManager.GetLogger(this.GetType());
        #endregion
		private Resource _DatabaseFile;
		
		/// <summary>
		/// The Database File
		/// </summary>
		public Resource DatabaseFile
		{
			set {
				_DatabaseFile = value;
			}
			
			get {
				return _DatabaseFile;
			}
		}

		///<summary>
		/// Constructor
		/// </summary>
		/// <param name="DatabaseFile">A  Resource</param>
		public FileDb4oConnectionParameters(Resource databaseFile)
		{
			_DatabaseFile = databaseFile;
		}
		
		
		/// <summary>
		/// Method CreateObjectContainer
		/// </summary>
		/// <returns>An ObjectContainer</returns>
		public override ObjectContainer CreateObjectContainer()
		{
			logger.Debug("Connection Type: File");
			ObjectContainer container;
			
			//FileDb4oConnectionParameters parameters = (FileDb4oConnectionParameters) _Db4oConnectionParameters;
			container = Db4o.openFile(_DatabaseFile.ToString());
			return container;
		}
		
	}
}
