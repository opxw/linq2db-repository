# linq2db-repository

Generic repository pattern using [linq2db](https://github.com/linq2db/linq2db) with CRUD functionality.

## Defining Database Connection

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

## Defining Entity
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
}
```
For more information about defining attribute, you can visit [linq2db](https://github.com/linq2db/linq2db) documentation page.

## Usage

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

## INSERT

`object? Insert<T>(T entity, bool ignoreNullValue = false)`

> It will returning `affected rows`, but if `[Identity]` attribute defined in your entity, it will returning inserted record's identity.

```c#
// define entity with auto increment field
public class Artist
{
    [PrimaryKey, Identity]
    public int? Id { get; set; }
    public string Name { get; set; }
}

// you can leave Id property
_trackRepo.Insert(new Artist()
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

// you must fill Id with desired value
_trackRepo.Insert(new Artist()
{
    Id = 123
    Name = "Edane"
});
```
If you set `ignoreNullValue = true`, it will ignore `null` value on property.
```c#
public class Artist
{
    [PrimaryKey, Identity]
    public int Id? { get; set; }
    public string Name { get; set; }
    public string? Notes { get; set; }
}

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

## GET RECORDS

Available methods

- ```c#
  /// returning all customers with no condition
  await _customerRepo.FindAsync();
  ```

/// returning customers with conditions
await _customerRepo.FindAsync(x => x.Company == "Microsoft");

/// if you need more customization
await _customerRepo.FindAsync(q =>
{
    if (activeOnly)
      q = q.Where(c => c.Active == true);

    if (useCurrentCity)
        q = q.Where(c => c.City == "PARIS");
    
    q = q.OrderBy(c => c.FirstName);
    
    return q;

});

/// single row
await _customerRepo.FindFirstAsync(c => c.CustomerId == id);

```
### INSERT

```c#
InsertAsync<T>(T entity, bool ignoreNullValue = false);
```

```c#
var customer = new Customer()
{
    FirstName = "John",
    LastName = "Doe",
    Email = "john@doe.com",
};
await _customerRepo.Insert(customer, true);
```

Asynchronous & synchronous operation supported.

- `ignoreNullValues = true`, NULL value in column will not be inserted.
- For `AutoIncrement` column, it will check `Identity` attribute on your POCO class configuration, make sure you have setup it correctly.

### UPDATE

```c#
UpdateAsync<T>(T entity, bool ignoreNullValue = false, CancellationToken);
```

```c#
customer.CustomerId = 1;
customer.City = "Sydney";

await _customerRepo.Update(customer, true);
```

Asynchronous & synchronous operation supported.

- `ignoreNullValues = true`, NULL value in column will not be updated.
- It will check automatically the key of entity based on `PrimaryKey & Key` attribute

### DELETE

```c#
Delete<T>(Expression<Func<T, bool>>? criteria);
```

```c#
await _customerRepo.DeleteAsync(c => c.CustomerId == 1);
```

Asynchronous & synchronous operation supported.

- It will check automatically the key of entity based on `PrimaryKey & Key` attribute

### MISC

```c#
GenerateIdAsync(Expression<Func<T, string>> selector, prefix, padCount;
```

It will generate auto number based on Max value on column, for example "INV-0001010"

```c#
await _invoiceRepo.GenerateIdAsync(x => x.InvoiceId, "INV-", 7);
```
