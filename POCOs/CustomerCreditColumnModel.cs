
namespace TY.SPIMS.POCOs
{
    public class CustomerCreditColumnModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Reference { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public bool Credited { get; set; }
    }
}
