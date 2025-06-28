using FinanceSubstantialApp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceSubstantialApp.Models
{
    public class JournalEntry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AccountId { get; set; } // Foreign Key to ChartOfAccount

        [ForeignKey("AccountId")]
        public virtual ChartOfAccount Account { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        public string Description { get; set; }

        // In any transaction, an entry is either a Debit or a Credit, not both.
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Debit { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Credit { get; set; } = 0;
    }
}