
using Spring.Dao;
using System;
namespace Spring.Data
{
	/// <summary>
	/// Generic Db4o Data Access Exception
	/// </summary>
	public class Db4oDataAccessException : DataAccessException
	{
		public Db4oDataAccessException(string message, Exception ex)  : base(message,ex){
		}
	}
}
