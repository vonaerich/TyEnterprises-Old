
namespace TY.SPIMS.POCOs
{
    public class CustomerDebitColumnModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Reference { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public bool Debited { get; set; }
    }
}
