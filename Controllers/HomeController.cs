using Microsoft.AspNetCore.Mvc;
using ProjetoFinal.Models;
using System.Diagnostics;

namespace ProjetoFinal.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("NOME") == null)
            {
                ViewBag.Cliente = "Cliente";
            }
            else
            {
                ViewBag.Cliente = HttpContext.Session.GetString("NOME");
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
