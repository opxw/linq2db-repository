using LinqToDB.Data;

namespace LinqToDB.Repository
{
    public class DbContextRepository : IDbContextRepository
    {
        private readonly DataConnection _connection;
        private bool disposedValue;

        public DbContextRepository(string providerName, string connectionString)
        {
            _connection = new DataConnection(providerName, connectionString);
        }

        public DbContextRepository(DataConnection dataConnection)
        {
            _connection = dataConnection;
        }

        public DataConnection Connection => _connection;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _connection.Close();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}