using LinqToDB;
using LinqToDB.Repository;

namespace LinqToDb.Repository
{
	public class DbContext : DbContextRepository
	{
		public DbContext(string providerName, string connectionString) : base(providerName, connectionString)
		{
		}

		public ITable<T> GetTable<T>() where T : class
		{
			return Connection.GetTable<T>();
		}
	}
}
