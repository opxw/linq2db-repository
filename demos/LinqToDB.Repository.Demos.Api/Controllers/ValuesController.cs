using LinqToDB.Repository.Demos.Api.Domain;
using LinqToDB.Repository.Demos.Api.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace LinqToDB.Repository.Demos.Api.Controllers
{
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IDbContextRepository _dbContext;
        private readonly IDbRepository<Customer> _customerRepo;
        private readonly IDbRepository<Invoice> _invoiceRepo;
        private readonly IDbRepository<InvoiceLine> _invoiceLineRepo;
        private readonly IDbRepository<Track> _trackRepo;
        
        public ValuesController(
            IDbContextRepository dbContext,
            IDbRepository<Customer> customerRepo,
            IDbRepository<Invoice> invoiceRepo,
            IDbRepository<InvoiceLine> invoiceLineRepo,
            IDbRepository<Track> trackRepo)
        {
            _dbContext = dbContext;
            _customerRepo = customerRepo;
            _invoiceRepo = invoiceRepo;
            _invoiceLineRepo = invoiceLineRepo;
            _trackRepo = trackRepo;
        }

        [HttpGet, Route("customers")]
        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _customerRepo.FindAsync(q =>
            {
                q = q.OrderBy(c => c.FirstName);

                return q;
            });
        }

        [HttpGet, Route("customers/{id}")]
        public async Task<Customer?> GetCustomerByIdAsync([FromRoute] int id)
        {
            return await _customerRepo.FindFirstAsync(c => c.CustomerId == id);
        }

        [HttpGet, Route("customers/page/{page}")]
        public async Task<IEnumerable<Customer>> GetPagedCustomersAsync([FromRoute] int page, [FromQuery] int show)
        {
            return await _customerRepo.PageFindAsync(page, show, q =>
            {
                q = q.OrderBy(c => c.FirstName);

                return q;
            });
        }

        [HttpPost, Route("customers/create")]
        public async Task<int> CreateCustomerAsync([FromBody] Customer customer, [FromQuery] bool skipNull = true)
        {
            return (int)await _customerRepo.InsertAsync(customer, skipNull);
        }

        [HttpPost, Route("customers/update")]
        public async Task<int> UpdateCustomersAsync([FromBody] Customer customer)
        {
            return (int)await _customerRepo.UpdateAsync(customer);
        }

        [HttpPost, Route("customers/delete/{customerId}")]
        public async Task<int> DeleteCustomerAsync([FromRoute] int customerId)
        {
            return await _customerRepo.DeleteAsync(c => c.CustomerId == customerId);
        }

        [HttpPost, Route("invoices/create")]
        public async Task<int> CreateInvoiceAsync([FromBody] InvoiceCreateDto request)
        {
            var result = 0;

            var lines = new List<InvoiceLine>();
            foreach (var line in request.Lines)
            {
                var track = await _trackRepo.FindFirstAsync(x => x.TrackId == line.TrackId);
                lines.Add(new InvoiceLine()
                {
                    Quantity = line.Quantity,
                    TrackId = line.TrackId,
                    UnitPrice = track.UnitPrice
                });
            }

            var invoice = new Invoice()
            {
                BillingAddress = request.BillingAddress,
                BillingCity = request.BillingCity,
                BillingCountry = request.BillingCountry,
                CustomerId = request.CustomerId,
                InvoiceDate = DateTime.Now,
                Total = lines.Sum(x => x.UnitPrice * x.Quantity)
            };

            // if billing address empty then take address from customer's address
            if (string.IsNullOrWhiteSpace(request.BillingAddress))
            {
                var customer = await _customerRepo.FindFirstAsync(x => x.CustomerId == request.CustomerId);
                invoice.BillingAddress = customer.Address;
                invoice.BillingCity = customer.City;
                invoice.BillingCountry = customer.Country;
            }

            var transaction = _dbContext.Connection.BeginTransaction();
            try
            {
                var invoiceId = await _invoiceRepo.InsertAsync(invoice);

                foreach (var line in lines)
                {
                    line.InvoiceId = Convert.ToInt32(invoiceId);
                    await _invoiceLineRepo.InsertAsync(line);
                }

                transaction.Commit();

                result = Convert.ToInt32(invoiceId);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
            }

            return result;
        }

        [HttpGet, Route("invoices")]
        public async Task<IEnumerable<InvoiceReportDto>> GetInvoiceReportAsync()
        {
            var q =
                from i in _invoiceRepo.Table
                join c in _customerRepo.Table on i.CustomerId equals c.CustomerId
                orderby i.InvoiceDate descending
                select new InvoiceReportDto()
                {
                    BillingAddress = i.BillingAddress,
                    BillingCity = i.BillingCity,
                    BillingCountry = i.BillingCountry,
                    CustomerId = i.CustomerId,
                    CustomerName = $"{c.FirstName} {c.LastName}",
                    InvoiceDate = i.InvoiceDate,
                    InvoiceId = i.InvoiceId,
                    Total = i.Total
                };

            return await q.ToListAsync();
        }
    }
}
