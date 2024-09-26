namespace LinqToDB.Repository
{
    public interface IDbRepository<T> where T : class
    {
        ITable<T> Table { get; }
        IDbContextRepository DbContext { get; }
    }
}