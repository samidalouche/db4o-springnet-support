
using Spring.Transaction.Support;
using com.db4o;
namespace Spring.Data
{
	public class ObjectContainerHolder : ResourceHolderSupport
	{
		private ObjectContainer _ObjectContainer;
		
		
		/// <summary>
		/// The contained DB4o Object Container
		/// </summary>
		public ObjectContainer ObjectContainer
		{
			set {
				_ObjectContainer = value;
			}
			
			get {
				return _ObjectContainer;
			}
		}
		
		///<summary>
		/// Constructor
		/// </summary>
		/// <param name="ObjectContainer">An ObjectContainer</param>
		public ObjectContainerHolder(ObjectContainer objectContainer)
		{
			_ObjectContainer = objectContainer;
		}
		
	}
}
