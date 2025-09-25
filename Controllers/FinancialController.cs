using Microsoft.AspNetCore.Mvc;
using Budgetly.Models;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Budgetly.Controllers

{
    public class FinancialControllerController : Controller
    {
        // Actions aqui...
    }
}

namespace Budgetly.Controllers
{
    public class FinancialController : Controller
    {
        [HttpGet]
        public IActionResult Register()
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

            return View();
        }
    }
    
}
