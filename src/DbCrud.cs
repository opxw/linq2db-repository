
using LinqToDB.Data;
using LinqToDB.Mapping;
using LinqToDB.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace LinqToDB.Repository
{
    public static class DbCrud
    {
        #region CREATE
        public static async Task<object> InsertAsync<T>(this IDbRepository<T> repository, T entity,
            bool ignoreNullValue = false, CancellationToken cancellation = default) where T : class
        {
            if (ignoreNullValue)
            {
                var validCols = GetNotNullColumns(entity);

                if (HasIdentity(entity))
                    return await repository.Table.DataContext.InsertWithIdentityAsync(entity, (a, b) => validCols.Contains(b.ColumnName),
                        null, null, null, null, TableOptions.NotSet, cancellation);
                else
                    return await repository.Table.DataContext.InsertAsync(entity, (a, b) => validCols.Contains(b.ColumnName),
                        null, null, null, null, TableOptions.NotSet, cancellation);
            }
            else
            {
                if (HasIdentity(entity))
                    return await repository.Table.DataContext.InsertWithIdentityAsync(entity,
                        null, null, null, null, null, TableOptions.NotSet, cancellation);
                else
                    return await repository.Table.DataContext.InsertAsync(entity,
                        null, null, null, null, null, TableOptions.NotSet, cancellation);
            }
        }

        public static object? Insert<T>(this IDbRepository<T> repository, T entity,
            bool ignoreNullValue = false) where T : class
        {
            if (ignoreNullValue)
            {
                var validCols = GetNotNullColumns(entity);

                if (HasIdentity(entity))
                    return repository.Table.DataContext.InsertWithIdentity(entity, (a, b) => validCols.Contains(b.ColumnName),
                        null, null, null, null, TableOptions.NotSet);
                else
                    return repository.Table.DataContext.Insert(entity, (a, b) => validCols.Contains(b.ColumnName),
                        null, null, null, null, TableOptions.NotSet);
            }
            else
            {
                if (HasIdentity(entity))
                    return repository.Table.DataContext.InsertWithIdentity(entity,
                        null, null, null, null, null, TableOptions.NotSet);
                else
                    return repository.Table.DataContext.Insert(entity,
                        null, null, null, null, null, TableOptions.NotSet);
            }
        }
        #endregion

        #region READ
        public static async Task<IEnumerable<T>> FindAsync<T>(this IDbRepository<T> repository,
            Expression<Func<T, bool>>? criteria = null, CancellationToken cancellation = default) where T : class
        {
            return await repository.BuildQuery(criteria).ToListAsync(cancellation);
        }

        public static async Task<IEnumerable<T>> FindAsync<T>(this IDbRepository<T> repository,
            Func<IQueryable<T>, IQueryable<T>>? criteria, CancellationToken cancellation = default) where T : class
        {
            return await repository.BuildQuery(criteria).ToListAsync(cancellation);
        }

        public static IEnumerable<T> Find<T>(this IDbRepository<T> repository,
            Expression<Func<T, bool>>? criteria = null) where T : class
        {
            return repository.BuildQuery(criteria).ToList();
        }

        public static IEnumerable<T> Find<T>(this IDbRepository<T> repository,
            Func<IQueryable<T>, IQueryable<T>>? criteria) where T : class
        {
            return repository.BuildQuery(criteria).ToList();
        }

        public static async Task<IEnumerable<T>> PageFindAsync<T>(this IDbRepository<T> repository,
            int page, int recordPerPage, Func<IQueryable<T>, IQueryable<T>>? criteria = null,
            CancellationToken cancellation = default) where T : class
        {
            return await repository.BuildQuery(criteria).Paging(page, recordPerPage).ToListAsync();
        }

        public static IEnumerable<T> PageFind<T>(this IDbRepository<T> repository,
            int page, int recordPerPage, Func<IQueryable<T>, IQueryable<T>>? criteria = null) where T : class
        {
            return repository.BuildQuery(criteria).Paging(page, recordPerPage).ToList();
        }

        public static async Task<T?> FindFirstAsync<T>(this IDbRepository<T> repository,
            Expression<Func<T, bool>> criteria, CancellationToken cancellation = default) where T : class
        {
            return await repository.BuildQuery(criteria).FirstOrDefaultAsync(cancellation);
        }

        public static async Task<T?> FindFirstAsync<T>(this IDbRepository<T> repository,
            Func<IQueryable<T>, IQueryable<T>> criteria, CancellationToken cancellation = default) where T : class
        {
            return await repository.BuildQuery(criteria).FirstOrDefaultAsync(cancellation);
        }

        public static T? FindFirst<T>(this IDbRepository<T> repository,
            Expression<Func<T, bool>> criteria) where T : class
        {
            return repository.BuildQuery(criteria).FirstOrDefault();
        }

        public static T? FindFirst<T>(this IDbRepository<T> repository,
            Func<IQueryable<T>, IQueryable<T>> criteria) where T : class
        {
            return repository.BuildQuery(criteria).FirstOrDefault();
        }

        public static async Task<object?> MaxAsync<T, TResult>(this IDbRepository<T> repository,
            Expression<Func<T, TResult>> selector, Func<IQueryable<T>, IQueryable<T>>? criteria,
            CancellationToken cancellation = default) where T : class
        {
            return await repository.BuildQuery(criteria).MaxAsync(selector, cancellation);
        }

        public static async Task<object?> MaxAsync<T, TResult>(this IDbRepository<T> repository,
            Expression<Func<T, TResult>> selector, Expression<Func<T, bool>>? criteria = null,
            CancellationToken cancellation = default) where T : class
        {
            return await repository.BuildQuery(criteria).MaxAsync(selector, cancellation);
        }

        public static object? Max<T, TResult>(this IDbRepository<T> repository,
            Expression<Func<T, TResult>> selector, Func<IQueryable<T>, IQueryable<T>>? criteria) where T : class
        {
            return repository.BuildQuery(criteria).Max(selector);
        }

        public static object? Max<T, TResult>(this IDbRepository<T> repository,
            Expression<Func<T, TResult>> selector, Expression<Func<T, bool>>? criteria = null) where T : class
        {
            return repository.BuildQuery(criteria).Max(selector);
        }

        public static async Task<int> RowCountAsync<T>(this IDbRepository<T> repository,
            Func<IQueryable<T>, IQueryable<T>>? criteria,
            CancellationToken cancellation = default) where T : class
        {
            return await repository.BuildQuery(criteria).CountAsync(cancellation);
        }

        public static async Task<int> RowCountAsync<T>(this IDbRepository<T> repository,
            Expression<Func<T, bool>>? criteria = null,
            CancellationToken cancellation = default) where T : class
        {
            return await repository.BuildQuery(criteria).CountAsync(cancellation);
        }

        public static int RowCount<T>(this IDbRepository<T> repository,
            Func<IQueryable<T>, IQueryable<T>>? criteria) where T : class
        {
            return repository.BuildQuery(criteria).Count();
        }

        public static int RowCount<T>(this IDbRepository<T> repository,
            Expression<Func<T, bool>>? criteria = null) where T : class
        {
            return repository.BuildQuery(criteria).Count();
        }
        #endregion

        #region UPDATE
        public static async Task<int> UpdateAsync<T>(this IDbRepository<T> repository, T entity,
            bool ignoreNullValue = false, CancellationToken cancellation = default) where T : class
        {
            if (ignoreNullValue)
            {
                var validCols = GetNotNullColumns(entity);
                return await repository.Table.DataContext.UpdateAsync(entity, (a, b) => validCols.Contains(b.ColumnName),
                    null, null, null, null, TableOptions.NotSet, cancellation);
            }
            else
                return await repository.Table.DataContext.UpdateAsync(entity,
                    null, null, null, null, null, TableOptions.NotSet, cancellation);
        }

        public static int Update<T>(this IDbRepository<T> repository, T entity,
           bool ignoreNullValue = false) where T : class
        {
            if (ignoreNullValue)
            {
                var validCols = GetNotNullColumns(entity);
                return repository.Table.DataContext.Update(entity, (a, b) => validCols.Contains(b.ColumnName),
                    null, null, null, null, TableOptions.NotSet);
            }
            else
                return repository.Table.DataContext.Update(entity,
                    null, null, null, null, null, TableOptions.NotSet);
        }
        #endregion

        #region DELETE
        public static async Task<int> DeleteAsync<T>(this IDbRepository<T> repository, Expression<Func<T, bool>>? criteria,
            CancellationToken cancellation = default) where T : class
        {
            if (criteria == null)
                return 0;

            return await repository.Table.DeleteAsync(criteria);
        }

        public static int Delete<T>(this IDbRepository<T> repository, Expression<Func<T, bool>>? criteria) where T : class
        {
            if (criteria == null)
                return 0;

            return repository.Table.Delete(criteria);
        }
        #endregion

        #region MISC
        private static string GetNewId(object? value, string prefix, int padCount)
        {
            var intValue = 0;

            if (value != null)
            {
                var temp = value.ToString();
                intValue = Convert.ToInt32(temp.Substring(prefix.Length, temp.Length - prefix.Length));
            }

            return $"{prefix}{(intValue + 1).ToString().PadLeft(padCount, '0')}";
        }

        public static async Task<string> GenerateIdAsync<T>(this IDbRepository<T> repository,
            Expression<Func<T, string>> selector, string prefix, int padCount,
            CancellationToken cancellation = default) where T : class
        {
            var filter = Utils.ModifyStringExpression(selector, LinqFilter.StartsWith, prefix);
            var maxValue = await repository.MaxAsync(selector, filter, cancellation);

            return GetNewId(maxValue, prefix, padCount);
        }

        public static string GenerateId<T>(this IDbRepository<T> repository,
            Expression<Func<T, string>> selector, string prefix, int padCount) where T : class
        {
            var filter = Utils.ModifyStringExpression(selector, LinqFilter.StartsWith, prefix);
            var maxValue = repository.Max(selector, filter);

            return GetNewId(maxValue, prefix, padCount);
        }
        #endregion

        #region QUERY HELPER
        public static IQueryable<T> BuildQuery<T>(this IDbRepository<T> repository,
            Expression<Func<T, bool>>? criteria = null) where T : class
        {
            var q = repository.Table.AsQueryable();

            if (criteria != null)
                q = q.Where(criteria);

            return q;
        }

        public static IQueryable<T> BuildQuery<T>(this IDbRepository<T> repository,
            Func<IQueryable<T>, IQueryable<T>>? criteria = null) where T : class
        {
            var q = repository.Table.AsQueryable();

            q = criteria != null ? criteria(q) : q;

            return q;
        }

        public static IQueryable<T> Paging<T>(this IQueryable<T> query,
            int page, int recordToShow) where T : class
        {
            query = query.Skip((page - 1) * recordToShow).Take(recordToShow);

            return query;
        }

        private static HashSet<string> GetNotNullColumns<T>(T entity) where T : class
        {
            var result = new HashSet<string>();
            var accessor = TypeAccessor.GetAccessor<T>();

            foreach (var member in accessor.Members)
            {
                if (!member.IsAttributeDefined(typeof(NotColumnAttribute)))
                {
                    var value = member.GetValue(entity);
                    if (value != null)
                    {
                        var propertyInfo = ((System.Reflection.PropertyInfo)member.MemberInfo).PropertyType;
                        if (propertyInfo != null && propertyInfo is DateTime)
                        {
                            if ((DateTime)value == DateTime.MinValue)
                                break;
                        }

                        var columnAttribute = member.GetAttribute(typeof(NotColumnAttribute));
                        var columnName = columnAttribute == null ? member.Name : columnAttribute.ConstructorArguments.First().Value;

                        result.Add(columnName.ToString());
                    }
                }
            }

            return result;
        }

        private static bool HasIdentity<T>(T entity) where T : class
        {
            var result = false;
            var accessor = TypeAccessor.GetAccessor<T>();

            foreach (var member in accessor.Members)
            {
                if (member.IsAttributeDefined(typeof(IdentityAttribute)))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }
        #endregion

        #region CONNECTION
        public static void UseRepositoryPattern(this IServiceCollection services,
           string providerName, string connectionString)
        {
            services.AddScoped<IDbContextRepository, DbContextRepository>(connection =>
                new DbContextRepository(providerName, connectionString));
            services.AddScoped(typeof(IDbRepository<>), typeof(DbRepository<>));
        }

        public static void UseRepositoryPattern(this IServiceCollection services,
            DataConnection dataConnection)
        {
            services.AddScoped<IDbContextRepository, DbContextRepository>(connection =>
                new DbContextRepository(dataConnection));
            services.AddScoped(typeof(IDbRepository<>), typeof(DbRepository<>));
        }

        public static IDbRepository<T> Repository<T>(this IDbContextRepository context) where T : class
        {
            return new DbRepository<T>(context);
        }
        #endregion

    }
}