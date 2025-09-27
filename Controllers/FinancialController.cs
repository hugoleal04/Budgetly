using Microsoft.AspNetCore.Mvc;
using Budgetly.Models;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;
using System.Linq;


namespace Budgetly.Controllers
{
    public class FinancialController : Controller
    {


        public IActionResult Index()
        {
            string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Budgetly");
            string filePath = Path.Combine(appDataFolder, "accounts.json");
            List<Account> accounts = new();

            if (System.IO.File.Exists(filePath))
            {
                string json = System.IO.File.ReadAllText(filePath);
                accounts = JsonSerializer.Deserialize<List<Account>>(json) ?? new List<Account>();
            }
            return View(accounts);
        }
        [HttpGet]
        public IActionResult Hub(int id)
        {
            ProcessRecurringExpenses(id);
            string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Budgetly");

            // Ler despesas
            string filePathExpenses = Path.Combine(appDataFolder, "Expenses.json");
            List<Expenses> expenses = new();
            if (System.IO.File.Exists(filePathExpenses))
            {
                string json = System.IO.File.ReadAllText(filePathExpenses);
                expenses = JsonSerializer.Deserialize<List<Expenses>>(json) ?? new List<Expenses>();
            }

            // Filtrar apenas as despesas da conta atual
            var accountExpenses = expenses.Where(e => e.id_user == id).OrderByDescending(e => e.Date).ToList();

            // Ler conta
            string filePathAccounts = Path.Combine(appDataFolder, "accounts.json");
            Account? account = null;
            if (System.IO.File.Exists(filePathAccounts))
            {
                var json = System.IO.File.ReadAllText(filePathAccounts);
                var accounts = JsonSerializer.Deserialize<List<Account>>(json) ?? new List<Account>();
                account = accounts.FirstOrDefault(a => a.id == id);
            }

            if (account == null)
                return NotFound();

            // Montar ViewModel
            var viewModel = new HubViewModel
            {
                Account = account,
                Expenses = accountExpenses
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Success()
        {
            return View();
        }
        public IActionResult InsufficientMoney()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Register(Account model)
        {
            string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Budgetly");
            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }
            string filePath = Path.Combine(appDataFolder, "accounts.json");
            // Read existing JSON or create new list
            List<Account> accounts;
            if (System.IO.File.Exists(filePath))
            {
                string existingJson = System.IO.File.ReadAllText(filePath);
                accounts = JsonSerializer.Deserialize<List<Account>>(existingJson) ?? new List<Account>();
            }
            else
            {
                accounts = new List<Account>();
            }

            model.id = accounts.Count > 0 ? accounts.Max(a => a.id) + 1 : 1;
            accounts.Add(model);

            // Save JSON
            string jsonString = JsonSerializer.Serialize(accounts, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(filePath, jsonString);

            return RedirectToAction("Success");
        }

        [HttpPost]
        public IActionResult AddMoney(int id, decimal amount)
        {
            string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Budgetly");
            string filePath = Path.Combine(appDataFolder, "accounts.json");

            List<Account> accounts = new();
            if (System.IO.File.Exists(filePath))
            {
                var json = System.IO.File.ReadAllText(filePath);
                accounts = JsonSerializer.Deserialize<List<Account>>(json) ?? new();
            }

            var account = accounts.FirstOrDefault(a => a.id == id);
            if (account != null)
            {
                account.money += amount;

                var jsonString = JsonSerializer.Serialize(accounts, new JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText(filePath, jsonString);
            }

            return RedirectToAction("Hub", new { id = id });
        }

        [HttpGet]
        public IActionResult ExpenseMenu()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ExpenseMenu(Expenses model, int id)
        {
            ViewBag.ExpenseId = id;

            string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Budgetly");
            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }
            string filePath = Path.Combine(appDataFolder, "Expenses.json");
            string filePathAccounts = Path.Combine(appDataFolder, "accounts.json");

            // Read existing JSON or create new list
            List<Expenses> expenses;
            if (System.IO.File.Exists(filePath))
            {
                string existingJson = System.IO.File.ReadAllText(filePath);
                expenses = JsonSerializer.Deserialize<List<Expenses>>(existingJson) ?? new List<Expenses>();
            }
            else
            {
                expenses = new List<Expenses>();
            }

            if (model.Recurring)
            {
                model.NextDueDate = model.Date.AddMonths(1);
            }
            model.id_user = id;
            expenses.Add(model);




            List<Account> accounts;
            if (System.IO.File.Exists(filePathAccounts))
            {
                string existingJson = System.IO.File.ReadAllText(filePathAccounts);
                accounts = JsonSerializer.Deserialize<List<Account>>(existingJson) ?? new List<Account>();
            }
            else
            {
                accounts = new List<Account>();
            }
            var account = accounts.FirstOrDefault(a => a.id == id);
            if (account != null)
            {
                account.money -= model.Price;
            }

            if (account.money < 0)
            {
                return RedirectToAction("InsufficientMoney");
            }

            // Save JSON Account
            string jsonAccountsUpdated = JsonSerializer.Serialize(accounts, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(filePathAccounts, jsonAccountsUpdated);
            // Save JSON Expense
            string jsonString = JsonSerializer.Serialize(expenses, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(filePath, jsonString);

            return RedirectToAction("Hub", new { id = id });
        }

        private void ProcessRecurringExpenses(int accountId)
        {
            string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Budgetly");
            string filePath = Path.Combine(appDataFolder, "Expenses.json");

            List<Expenses> expenses = new();

            if (System.IO.File.Exists(filePath))
            {
                var json = System.IO.File.ReadAllText(filePath);
                expenses = JsonSerializer.Deserialize<List<Expenses>>(json) ?? new();
            }
            bool updated = false;
            foreach (var expense in expenses.Where(e => e.Recurring && e.id_user == accountId).ToList())
            {
                while (expense.NextDueDate.HasValue && expense.NextDueDate.Value <= DateTime.Today)
                {
                    var newExpense = new Expenses
                    {
                        Type = expense.Type,
                        Description = expense.Description,
                        Price = expense.Price,
                        Date = expense.NextDueDate.Value,
                        Recurring = false,
                        id_user = expense.id_user
                    };

                    expenses.Add(newExpense);

                    expense.NextDueDate = expense.NextDueDate.Value.AddMonths(1);

                    updated = true;
                }
            }

            if (updated)
            {
                var jsonString = JsonSerializer.Serialize(expenses, new JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText(filePath, jsonString);
            }
        }   

    }
}
