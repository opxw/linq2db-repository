using LinqToDB.Repository.Demos.Api.Domain;

namespace LinqToDB.Repository.Demos.Api.Dtos
{
    public class InvoiceReportDto : Invoice
    {
        public string CustomerName { get; set; }
    }
}