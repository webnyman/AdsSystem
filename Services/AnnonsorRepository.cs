using Microsoft.AspNetCore.Mvc;

namespace Annonssystem.Services
{
    public class AnnonsorRepository : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
