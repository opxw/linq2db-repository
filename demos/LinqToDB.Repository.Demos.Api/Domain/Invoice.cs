namespace LinqToDB.Repository.Demos.Api.Domain
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public int CustomerId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string BillingAddress { get; set; }
        public string BillingCountry { get; set; }
        public double Total { get; set; }
    }
}