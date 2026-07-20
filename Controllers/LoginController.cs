using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProjetoFinal.Data;
using ProjetoFinal.Models;

namespace ProjetoFinal.Controllers
{
    public class LoginController : Controller
    {
        ApplicationDbContext db;

        public LoginController(ApplicationDbContext context)
        {
            db = context;
        }

        public IActionResult Index(string email, string password)
        {
            var user = db.Utilizadors.Where(u => u.Email == email && u.PasswordHash == password).FirstOrDefault();

            //Tenta criar session para "USER"
            if (user != null && user.Ativo)
            {
                HttpContext.Session.SetString("NOME", user.Nome);
                HttpContext.Session.SetString("EMAIL", user.Email);
                HttpContext.Session.SetString("TIPO", user.TipoUtilizador);

                if (HttpContext.Session.GetString("TIPO") == "admin")
                {
                    return RedirectToAction("Index", "Admin");
                } else if (HttpContext.Session.GetString("TIPO") == "cliente")
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            //Caso a página esteja a ser aberta pela primeira vez
            if (string.IsNullOrEmpty(password) && string.IsNullOrEmpty(email))
            {
                ViewBag.User = "Por favor, introduza as suas credênciais.";
            }
            else
            {
                ViewBag.User = "Ocorreu um erro. Verifique o seu email e palavra-passe e tente outra vez.";
            }

            return View();
        }

        public IActionResult Registar(string nome, string email, string password, string password2)
        {
            ViewBag.Aviso1 = null;
            ViewBag.Aviso2 = null;

            //Nenhum campo pode estar vazio
            if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(password2) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(nome))
            {
                //Verifica se já existe um utilizador com o mesmo email e que esteja ativo
                var verificar = db.Utilizadors.Where(u => u.Email == email && u.Ativo == true).FirstOrDefault();
                if (verificar == null)
                {
                    //Verifica se as palavras-passe são iguais
                    if (password == password2)
                    {

                        //Criar novo utilizador
                        Utilizador user = new Utilizador();
                        user.Nome = char.ToUpper(nome[0]) + nome.Substring(1); //Capitaliza a primeira letra do nome
                        user.Email = email;
                        user.PasswordHash = password;
                        user.TipoUtilizador = "cliente";
                        user.Ativo = true;

                        db.Utilizadors.Add(user);
                        db.SaveChanges();

                        return RedirectToAction("Index", "Login");
                    }
                    else
                    {
                        ViewBag.Aviso2 = "As palavras-passe não são iguais.";
                    }
                }
                else
                {
                    ViewBag.Aviso1 = "Já existe uma conta ativa associada a esse email.";
                    if (password != password2) { ViewBag.Aviso2 = "As palavras-passe não são iguais."; }
                }
            }
            else
            {
                //Mostra erros simultaneamente caso se apliquem
                if (password != password2) { ViewBag.Aviso2 = "As palavras-passe não são iguais."; }
                var verificar = db.Utilizadors.Where(u => u.Email == email && u.Ativo == true).FirstOrDefault();
                if (verificar != null) { ViewBag.Aviso1 = "Já existe uma conta ativa associada a esse email."; }
            }
            return View();
        }
    }
}