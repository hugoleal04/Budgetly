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
            string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Budgetly");
            string filePath = Path.Combine(appDataFolder, "accounts.json");

            Account? account = null;

            if (System.IO.File.Exists(filePath))
            {
                var json = System.IO.File.ReadAllText(filePath);
                var accounts = JsonSerializer.Deserialize<List<Account>>(json) ?? new();
                account = accounts.FirstOrDefault(a => a.id == id);
            }
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
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

            model.id_user = id;

            expenses.Add(model);

            // Save JSON
            string jsonString = JsonSerializer.Serialize(expenses, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(filePath, jsonString);

            return RedirectToAction("Success");
        }

    }
}
