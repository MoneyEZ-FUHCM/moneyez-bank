using System.ComponentModel.DataAnnotations;

namespace MoneyEzBank.Repositories.Entities
{
    public class User : BaseEntity
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        public string PasswordHash { get; set; } = default!;

        [Required]
        public string FullName { get; set; } = default!;

        public string? PhoneNumber { get; set; }

        // Navigation properties
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
