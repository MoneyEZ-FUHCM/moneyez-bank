using System.ComponentModel.DataAnnotations;

namespace MoneyEzBank.Repositories.Entities
{
    public class Account : BaseEntity
    {
        [Required]
        [StringLength(20)]
        public string AccountNumber { get; set; } = default!;

        [Required]
        public string AccountHolder { get; set; } = default!;

        public decimal Balance { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; } = default!;

        public ICollection<Transaction> SourceTransactions { get; set; } = new List<Transaction>();

        public ICollection<Transaction> DestinationTransactions { get; set; } = new List<Transaction>();
    }
}
