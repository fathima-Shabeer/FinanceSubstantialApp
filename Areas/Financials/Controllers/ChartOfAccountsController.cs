using FinanceManagementApp.Data;
using FinanceSubstantialApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceSubstantialApp.Areas.Financials.Controllers
{
    // This attribute tells ASP.NET that this controller belongs to the "Financials" area.
    [Area("Financials")]
    public class ChartOfAccountsController : Controller
    {
        // _context is our connection to the database. It's provided by dependency injection.
        private readonly ApplicationDbContext _context;

        public ChartOfAccountsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Financials/ChartOfAccounts
        // This action gets all accounts from the database and passes them to the Index view.
        public async Task<IActionResult> Index()
        {
            // The ?. is a null-conditional operator, a safe way to check if ChartOfAccounts exists.
            return View(await _context.ChartOfAccounts.ToListAsync());
        }

        // GET: Financials/ChartOfAccounts/Details/5
        // This action gets a single account by its ID to show its details.
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chartOfAccount = await _context.ChartOfAccounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chartOfAccount == null)
            {
                return NotFound();
            }

            return View(chartOfAccount);
        }

        // GET: Financials/ChartOfAccounts/Create
        // This action simply displays the form for creating a new account.
        public IActionResult Create()
        {
            return View();
        }

        // POST: Financials/ChartOfAccounts/Create
        // This action handles the form submission when a user creates a new account.
        [HttpPost]
        [ValidateAntiForgeryToken] // Security feature to prevent cross-site request forgery.
        public async Task<IActionResult> Create([Bind("Id,AccountNumber,AccountName,AccountType,Description,IsActive,CurrentBalance")] ChartOfAccount chartOfAccount)
        {
            if (ModelState.IsValid) // Checks if the submitted data is valid (e.g., required fields are filled).
            {
                _context.Add(chartOfAccount);
                await _context.SaveChangesAsync(); // Saves the new account to the database.
                return RedirectToAction(nameof(Index)); // Sends the user back to the list of accounts.
            }
            return View(chartOfAccount); // If data is not valid, show the form again with error messages.
        }

        // GET: Financials/ChartOfAccounts/Edit/5
        // This action gets an account by ID and displays it in an editable form.
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chartOfAccount = await _context.ChartOfAccounts.FindAsync(id);
            if (chartOfAccount == null)
            {
                return NotFound();
            }
            return View(chartOfAccount);
        }

        // POST: Financials/ChartOfAccounts/Edit/5
        // This action handles the form submission when a user updates an existing account.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AccountNumber,AccountName,AccountType,Description,IsActive,CurrentBalance")] ChartOfAccount chartOfAccount)
        {
            if (id != chartOfAccount.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chartOfAccount);
                    await _context.SaveChangesAsync(); // Saves the changes to the database.
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChartOfAccountExists(chartOfAccount.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(chartOfAccount);
        }

        // GET: Financials/ChartOfAccounts/Delete/5
        // This action shows a confirmation page before deleting an account.
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chartOfAccount = await _context.ChartOfAccounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chartOfAccount == null)
            {
                return NotFound();
            }

            return View(chartOfAccount);
        }

        // POST: Financials/ChartOfAccounts/Delete/5
        // This action performs the actual deletion after the user confirms.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chartOfAccount = await _context.ChartOfAccounts.FindAsync(id);
            if (chartOfAccount != null)
            {
                _context.ChartOfAccounts.Remove(chartOfAccount);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChartOfAccountExists(int id)
        {
            return _context.ChartOfAccounts.Any(e => e.Id == id);
        }
    }
}