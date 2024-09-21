using LinqToDB.Repository.Demos.Api.Domain;

namespace LinqToDB.Repository.Demos.Api.Dtos
{
    public class InvoiceReportDto : Invoice
    {
        public string CustomerName { get; set; }
    }

    public class InvoiceCreateDto
    { 
        public int CustomerId { get; set; }
        public string? BillingAddress { get; set; }
        public string? BillingCity { get; set; }
        public string? BillingCountry { get; set; }
        public List<InvoiceCreateLineDto> Lines { get; set; }
    }

    public class InvoiceCreateLineDto
    {
        public int TrackId { get; set; }
        public int Quantity { get; set; }
    }
}