namespace TY.SPIMS.POCOs
{
    public class CustomerDisplayModel
    {
        public int Id { get; set; }
        public string CustomerCode { get; set; }
        public string CompanyName { get; set; }
        public string ContactPerson { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string TIN { get; set; }
        public string Agent { get; set; }
        public int PaymentTerms { get; set; }
        public bool IsDeleted { get; set; }
    }
}
