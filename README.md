# linq2db-repository

Generic repository pattern using [linq2db](https://github.com/linq2db/linq2db) with CRUD functionality.

## HOW TO USE

### Defining Database Connection

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

### Defining Entity

```c#
public class Customer
{
    [Identity, PrimaryKey]
    public int? CustomerId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Company { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public bool? Active { get; set;}
}

public class Invoice
{
    [PrimaryKey]
    public string InvoiceId { get; set; }
    public int CustomerId { get; set; }
    public DateTime InvoiceDate { get; set; }
    public string BillingAddress { get; set; }
    public string BillingCity { get; set; }
    public string BillingCountry { get; set; }
    public double Total { get; set; }
}
```

For more information about defining attribute, you can visit [linq2db](https://github.com/linq2db/linq2db) documentation page.

### Usage

Put on constructor 

```c#
private readonly IDbContextRepository _dbContext;
private readonly IDbRepository<Customer> _customerRepo;

public ValuesController(
    IDbContextRepository dbContext,
    IDbRepository<Customer> customerRepo
{
    _dbContext = dbContext;
    _customerRepo = customerRepo;
}
```

or direct access to class

```c#
private readonly IDbContextRepository _dbContext;

public ValuesController(
    IDbContextRepository dbContext
{
    _dbContext = dbContext;
}

public async Task<Customer> GetCustomer()
{
    var customerRepo = _dbContext.Repository<Customer>();
    .......
}
```

### FIND
synchronous & synchronous operation supported.

```c#
/// returning all customers with no condition
await _customerRepo.FindAsync();

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
