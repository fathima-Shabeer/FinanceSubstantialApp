using FinanceManagementApp.Data;
using FinanceSubstantialApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagementApp.Areas.Financials.Controllers
{
    [Area("Financials")]
    public class JournalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JournalController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Display all journal entries
        public async Task<IActionResult> Index()
        {
            // The .Include(j => j.Account) is essential to get the Account Name.
            var entries = await _context.JournalEntries
                .Include(j => j.Account)
                .OrderByDescending(j => j.TransactionDate)
                .ToListAsync();

            return View(entries);
        }

        // GET: Show form to create a new journal entry
        public IActionResult Create()
        {
            ViewData["AccountId"] = new SelectList(_context.ChartOfAccounts.Where(a => a.IsActive), "Id", "AccountName");
            return View();
        }

        // POST: Create a new journal entry
        // ALGORITHM: This is the core transaction posting algorithm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DateTime transactionDate, string description, int debitAccountId, decimal debitAmount, int creditAccountId, decimal creditAmount)
        {
            // 1. Basic Validation
            if (debitAmount != creditAmount || debitAmount <= 0)
            {
                ModelState.AddModelError("", "Debit and Credit amounts must be equal and greater than zero.");
            }
            if (debitAccountId == creditAccountId)
            {
                ModelState.AddModelError("", "Debit and Credit accounts cannot be the same.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["AccountId"] = new SelectList(_context.ChartOfAccounts.Where(a => a.IsActive), "Id", "AccountName");
                return View();
            }

            // 2. Use a Database Transaction for Atomicity
            // This ensures that either both entries are created and balances updated, or nothing is.
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 3. Find the accounts to be updated
                    var debitAccount = await _context.ChartOfAccounts.FindAsync(debitAccountId);
                    var creditAccount = await _context.ChartOfAccounts.FindAsync(creditAccountId);

                    if (debitAccount == null || creditAccount == null)
                    {
                        ModelState.AddModelError("", "Invalid account specified.");
                        ViewData["AccountId"] = new SelectList(_context.ChartOfAccounts.Where(a => a.IsActive), "Id", "AccountName");
                        await transaction.RollbackAsync();
                        return View();
                    }

                    // 4. Create the Debit Entry
                    var debitEntry = new JournalEntry
                    {
                        AccountId = debitAccountId,
                        TransactionDate = transactionDate,
                        Description = description,
                        Debit = debitAmount,
                        Credit = 0
                    };
                    _context.JournalEntries.Add(debitEntry);

                    // 5. Create the Credit Entry
                    var creditEntry = new JournalEntry
                    {
                        AccountId = creditAccountId,
                        TransactionDate = transactionDate,
                        Description = description,
                        Debit = 0,
                        Credit = creditAmount
                    };
                    _context.JournalEntries.Add(creditEntry);

                    // 6. Update Account Balances based on their type
                    // For Assets & Expenses: Balance increases with Debits
                    if (debitAccount.AccountType == AccountType.Asset || debitAccount.AccountType == AccountType.Expense)
                        debitAccount.CurrentBalance += debitAmount;
                    else // For Liability, Equity, Revenue: Balance decreases with Debits
                        debitAccount.CurrentBalance -= debitAmount;

                    // For Liability, Equity, Revenue: Balance increases with Credits
                    if (creditAccount.AccountType == AccountType.Liability || creditAccount.AccountType == AccountType.Equity || creditAccount.AccountType == AccountType.Revenue)
                        creditAccount.CurrentBalance += creditAmount;
                    else // For Assets & Expenses: Balance decreases with Credits
                        creditAccount.CurrentBalance -= creditAmount;

                    // 7. Save all changes and commit the transaction
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // If anything fails, roll back the entire operation
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                    ViewData["AccountId"] = new SelectList(_context.ChartOfAccounts.Where(a => a.IsActive), "Id", "AccountName");
                    return View();
                }
            }
        }
    }
}