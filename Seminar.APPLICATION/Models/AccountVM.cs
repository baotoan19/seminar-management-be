namespace Seminar.APPLICATION.Models
{
    public class AccountVM
    {
        public required int Id { get; set; }
        public required string Email { get; set; }
        public required int RoleId { get; set; }
        public required string RoleName { get; set; }
        public required bool Status { get; set; }
    }
}