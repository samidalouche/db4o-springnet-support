
using Spring.Globalization;
using com.db4o.ext;
using log4net;
using com.db4o;

namespace Spring.Data
{
	public class MemoryDb4oConnectionParameters: AbstractDb4oConnectionParameters
	{
		
		/// <summary>
		/// Method CreateObjectContainer
		/// </summary>
		/// <returns>An ObjectContainer</returns>
		public override ObjectContainer CreateObjectContainer()
		{
			logger.Debug("Connection Type: Memory");
			ObjectContainer container;
			
			
			container = ExtDb4o.openMemoryFile(_MemoryFile);
			
			return container;
		}
		
		private MemoryFile _MemoryFile;
		
		#region Logging
        private readonly ILog logger = LogManager.GetLogger(this.GetType());
        #endregion
		
		/// <summary> The Memory File
		/// </summary>
		public MemoryFile MemoryFile
		{
			set {
				_MemoryFile = value;
			}
			
			get {
				return _MemoryFile;
			}
		}
		
				
	}
}
