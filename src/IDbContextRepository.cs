using LinqToDB.Data;

namespace LinqToDB.Repository
{
    public interface IDbContextRepository : IDisposable
    {
        DataConnection Connection { get; }
    }
}