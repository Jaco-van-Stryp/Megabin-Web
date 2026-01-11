namespace Megabin_Web.Entities
{
    public class Users
    {
        public Guid Id{ get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }
        public required int TotalBins { get; set; }
        public ICollection<Addresses> Addresss { get; set; } = new List<Addresses>();
    }
}
