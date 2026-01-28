namespace Megabin_Web.Shared.DTOs.Users
{
    public class ResetPassword
    {
        public required Guid UserId { get; set; }
        public required string NewPassword { get; set; }
    }
}
