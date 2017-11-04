
namespace TY.SPIMS.POCOs
{
    public class InventoryUserDisplayModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsApprover { get; set; }
        public bool IsVisitor { get; set; }
        public bool IsDeleted { get; set; }
    }
}
