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
        private readonly IDbRepository<Babi> _babiRepo;
        
        public ValuesController(
            IDbContextRepository dbContext,
            IDbRepository<Customer> customerRepo,
            IDbRepository<Invoice> invoiceRepo,
            IDbRepository<Babi> babiRepo)
        {
            _dbContext = dbContext;
            _customerRepo = customerRepo;
            _invoiceRepo = invoiceRepo;
            _babiRepo = babiRepo;
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

        [HttpGet, Route("invoices")]
        public async Task<IEnumerable<InvoiceReportDto>> GetInvoiceReportAsync()
        {
            var q =
                from i in _invoiceRepo.Table
                join c in _customerRepo.Table on i.CustomerId equals c.CustomerId
                select new InvoiceReportDto()
                {
                    BillingAddress = i.BillingAddress,
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
