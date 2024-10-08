# linq2db-repository

[![NuGet version (SAPB1.DIAPI.Helper)](https://img.shields.io/nuget/v/DbRepository.LinqToDb.svg?style=flat-square)](https://www.nuget.org/packages/DbRepository.LinqToDb/)

Generic repository pattern using [linq2db](https://github.com/linq2db/linq2db).

## 1. Defining Database Connection

```c#
builder.Services.UseRepositoryPattern(ProviderName.PostgreSQL15,
    @"User ID=postgres;Password=postgres;Host=localhost;Port=5432;Database=chinook");
```

or you can register it manually

```c#
services.AddScoped<IDbContextRepository, DbContextRepository>(connection =>
    new DbContextRepository(ProviderName.PostgreSQL15, 
    @"User ID=postgres;Password=postgres;Host=localhost;Port=5432;Database=chinook"));
services.AddScoped(typeof(IDbRepository<>), typeof(DbRepository<>));
```

## 2. Defining Entity

```c#
public class Artist
{
    [PrimaryKey, Identity]
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Notes { get; set; }
}

public class Album
{
    [PrimaryKey, Identity]
    public int Id { get; set; }
    public string Name { get; set; }
    public int? ArtistId { get; set; }
}

public class Customer
{
    [Identity, PrimaryKey]
    public int? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Company { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public int? Active { get; set; }
}
```

For more information about defining attribute, you can visit [linq2db](https://github.com/linq2db/linq2db) documentation page.

## 3. Usage

Put on constructor of your class

```c#
public class CatalogService
{
    private readonly IDbContextRepository _dbContext;
    private readonly IDbRepository<Artist> _artistRepo;
    private readonly IDbRepository<Album> _albumRepo;

    public CatalogService(IDbContextRepository dbContext,
        IDbRepository<Artist> artistRepo,
        IDbRepository<Album> albumRepo)
    {
        _dbContext = dbContext;
        _artistRepo = artistRepo;
        _albumRepo = albumRepo;
    }
}
```

or

```c#
public class CatalogService
{
    private readonly IDbContextRepository _dbContext;

    public CatalogService(IDbContextRepository dbContext)
    {
        _dbContext = dbContext;
    }

    private void SomeMethod()
    {
        var artistRepo = _dbContext.Repository<Artist>();
    }
}
```

## 4. CREATE (INSERT)

`object? Insert<T>(T entity, bool ignoreNullValue = false)`

> It will returning `affected rows`, but if `[Identity]` attribute defined in your entity, it will returning inserted record's identity.

### 4.A With / without Identity

```c#
// define entity with auto-increment field
public class Artist
{
    [PrimaryKey, Identity]
    public int? Id { get; set; }
    public string Name { get; set; }
}

// you can leave "Id" property empty
_artistRepo.Insert(new Artist()
{
    Name = "Edane"
});
```

```c#
public class Artist
{
    [PrimaryKey]
    public int Id { get; set; }
    public string Name { get; set; }
}

// you must fill "Id" with unique value because "Id" is PrimaryKey
_artistRepo.Insert(new Artist()
{
    Id = 123
    Name = "Edane"
});
```

### 4.B Ignoring NULL value

Set `ignoreNullValue = true`, it will ignore `null` value. 
`DateTime` data type can be ignored by set the value to `DateTime.MinValue`.

```c#
var artist = new Artist() { Name = "Edane" }; //=> Id & Notes = NULL

_artistRepo.Insert(artist, false);
/*------------------------------------------
INSERT INTO [Artist]
(
    [Name],
    [Notes]
)
VALUES
(
    @Name,
    @Notes
)
-------------------------------------------*/

_artistRepo.Insert(artist, true);
/*------------------------------------------
INSERT INTO [Artist]
(
    [Name]
)
VALUES
(
    @Name
)
-------------------------------------------*/
```

## 5. READ

Available methods

### 5.A `Find`

`IEnumerable<T> Find<T>(Expression<Func<T, bool>>? criteria = null);`<br>
`IEnumerable<T> Find<T>(Func<IQueryable<T>, IQueryable<T>>? criteria);`

```c#
// all customers
var customers = _customerRepo.Find();

// customers with condition
var customers = _customerRepo.Find(x => x.Company == "Microsoft");

// customers with more complex condition
var customers = _customerRepo.FindAsync(q =>
{
    if (activeOnly)
      q = q.Where(c => c.Active == 1);

    if (useCurrentCity)
        q = q.Where(c => c.City == "PARIS");

    q = q.OrderBy(c => c.FirstName);

    return q;
});
```

### 5.B `FindFirst`

`T? FindFirst<T>(this IDbRepository<T> repository, Expression<Func<T, bool>> criteria);`<br>
`T? FindFirst<T>(this IDbRepository<T> repository, Func<IQueryable<T>, IQueryable<T>>? criteria);`

```c#
var customer =  _customerRepo.FindFirst(x => x.Active == 1 && x.Id == 123);

//for complex condition use this
var customer = _customerRepo.FindFirst(q =>
{
    q = q.Where(x => x.Prop)
    ....
    ....
    return q;
});
```
### 5.C `PageFind`
Paging functionality.<br>
`IEnumerable<T> PageFind<T>(this IDbRepository<T> repository, int page, int recordPerPage, Func<IQueryable<T>, IQueryable<T>>? criteria = null);`

example :
```c#
public IEnumerable<Customer> GetPagedCustomers(int page, int show, string? sortField, string? sortDirection)
{
    return _customerRepo.PageFind(1, 10, q =>
    {
        Expression<Func<Customer, object>> selector = s => s.Id;

        if (!string.IsNullOrWhiteSpace(sortField))
        {
            switch (sortField)
            {
                case "lastName":
                    selector = s => s.LastName;
                    break;
                case "address":
                    selector = s => s.Address;
                    break;
                default:
                    selector = s => s.FirstName;
                    break;
            }
        }

        if (sortDirection == "desc")
            q = q.OrderByDescending(selector);
        else
            q = q.OrderBy(selector);

        return q;
    });
}
```

## 6. UPDATE

`int Update<T>(T entity, bool ignoreNullValue = false)`

> It will returning `affected rows`.

```c#
var artist = _artistRepo.FindFirst(x => x.Id == 1);
artist.Notes = "brown fox jumps";

 _artistRepo.Update(artist);

/*------------------------------------------
UPDATE
	[t1]
SET
	[t1].[Name] = @Name,
	[t1].[Notes] = @Notes
FROM
	[Artist] [t1]
WHERE
	[t1].[Id] = @Id
-------------------------------------------*/
```

But sometimes we need to update the column partially, just set `ignoreNullValue = true`:

```c#
var artist = new Artist() { Id = 1, Notes = "brown fox jumps" };

_artistRepo.Update(artist, true);

/*------------------------------------------
UPDATE
	[t1]
SET
	[t1].[Notes] = @Notes
FROM
	[Artist] [t1]
WHERE
	[t1].[Id] = @Id
-------------------------------------------*/
```

## 7. DELETE
`int Delete<T>(this IDbRepository<T> repository, Expression<Func<T, bool>>? criteria);`

```c#
_artistRepo.Delete(x => x.Id == 123);
```

## 8. MISC

### Generate Auto Number

Sometimes we need to generate auto number with pattern to invoice & another transaction. We can achieve the purpose using this :<br>
`string GenerateId<T>(Expression<Func<T, string>> selector, string prefix, int padCount);`           

| InvoiceNum |
| ---------- |
| INV-000001 |
| INV-000002 |
| INV-000003 |

Next number must be : INV-000004, 
```c#
_invoiceRepo.GenerateId(x => x.InvoiceNum, "INV-", 6);
```
