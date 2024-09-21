using LinqToDB.Mapping;

namespace LinqToDB.Repository.Demos.Api.Domain
{
    public class InvoiceLine
    { 
        [PrimaryKey, Identity]
        public int InvoiceLineId { get; set; }
        public int InvoiceId { get; set; }
        public int TrackId { get; set; }
        public double UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}