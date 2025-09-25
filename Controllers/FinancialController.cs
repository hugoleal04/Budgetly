using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Mvc;
using Budgetly.Models;

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
        public IActionResult Register()
        {
            return View();
        }
    }
}
