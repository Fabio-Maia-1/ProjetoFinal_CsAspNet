using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoFinal.Data;
using ProjetoFinal.Models;
using System.Diagnostics;

namespace ProjetoFinal.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext db;

        public HomeController(ApplicationDbContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("NOME") == null || HttpContext.Session.GetString("NOME") == "")
            {
                ViewBag.Cliente = "Bem-Vindo";
            }
            else
            {
                ViewBag.Cliente = "Bem-Vindo, " + HttpContext.Session.GetString("NOME");
            }

            //Coleção de filmes disponíveis para exibir na página inicial
            ViewBag.Filmes = db.Filmes.Where(f => f.Disponivel == true).ToList();

            return View();
        }

        public async Task<IActionResult> Detalhes(int? id)
        {
            if (id == null) return NotFound();

            var filme = await db.Filmes.Include(s => s.Sessoes).FirstOrDefaultAsync(m => m.Id == id);

            if (filme == null) return NotFound();

            return View(filme);
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
