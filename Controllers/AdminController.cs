using Microsoft.AspNetCore.Mvc;
using ProjetoFinal.Data;

namespace ProjetoFinal.Controllers
{
    public class AdminController : Controller
    {
        ApplicationDbContext db;

        public AdminController(ApplicationDbContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            //Retorna o primeiro utilizador com este nome
            var user = db.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null)
            {
                if (HttpContext.Session.GetString("TIPO") == "admin")
                {
                    return View();
                }
            }

            return RedirectToAction("Index", "Login"); //Faz reload da página de login.
        }
    }
}
