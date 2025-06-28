using FinanceSubstantialApp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceSubstantialApp.Models
{
    public class ChartOfAccount
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

        [Required]
        [Display(Name = "Account Name")]
        public string AccountName { get; set; }

        [Required]
        [Display(Name = "Account Type")]
        public AccountType AccountType { get; set; }

        public string? Description { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        // The running balance of the account. This is a critical field.
        [Column(TypeName = "decimal(18, 2)")]
        public decimal CurrentBalance { get; set; } = 0;
    }
}