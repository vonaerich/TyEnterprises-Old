
namespace TY.SPIMS.POCOs
{
    public class CustomerCreditDisplayModel
    {
        public int Id { get; set; }
        public string Customer { get; set; }
        public string Reference { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public bool Credited { get; set; }
    }
}
