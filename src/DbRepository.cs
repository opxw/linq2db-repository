namespace LinqToDB.Repository
{
    public class DbRepository<T> : IDbRepository<T> where T : class
    {
        private readonly IDbContextRepository _dbContext;
        private ITable<T> _table;

        public DbRepository(IDbContextRepository dbContext)
        {
            _dbContext = dbContext;
            _table = _dbContext.Connection.GetTable<T>();
        }

        public ITable<T> Table => _table;
    }
}