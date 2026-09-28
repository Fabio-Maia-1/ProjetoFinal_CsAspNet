using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IActionResult> Details()
        {
            //Retorna o primeiro utilizador com este nome
            var user = db.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null)
            {
                return View(user); 
            }
            return RedirectToAction("Index", "Login"); //Faz reload da página de login.
        }


        // ------- Novo Nome
        public async Task<IActionResult> NovoNome(int? id)
        {
            if (id == null) return NotFound();

            // Usa FindAsync ou FirstOrDefaultAsync para ser assíncrono
            var user = await db.Utilizadors.FindAsync(id);

            if (user != null)
            {
                return View(user);
            }

            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NovoNome(int id, string Nome)
        {
            // Procura o utilizador real na base de dados
            var user = await db.Utilizadors.FindAsync(id);

            if (user == null) return NotFound();

            // Valida se o novo nome foi preenchido
            if (string.IsNullOrWhiteSpace(Nome))
            {
                ModelState.AddModelError("Nome", "Introduza o novo nome.");
                return View(user);
            }

            if (Nome != null && Nome.Length > 0)
            {
                user.Nome = Nome;
                try
                {
                    await db.SaveChangesAsync();
                    return RedirectToAction(nameof(Details));
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
            return RedirectToAction("Index", "Login");
        }


        // ------- Nova Pass
        public async Task<IActionResult> NovaPass(int? id)
        {
            if (id == null) return NotFound();

            // Usa FindAsync ou FirstOrDefaultAsync para ser assíncrono
            var user = await db.Utilizadors.FindAsync(id);

            if (user != null)
            {
                return View(user);
            }

            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NovaPass(int id, string passVelha, string passNova, string passNova2)
        {
            ViewBag.PassVelha = "";
            ViewBag.PassNova = "";
            ViewBag.PassNova2 = "";

            if (!string.IsNullOrEmpty(passVelha))
            {
                ViewBag.PassVelha = passVelha;
            }
            if (!string.IsNullOrEmpty(passNova))
            {
                ViewBag.PassNova = passNova;
            }
            if (!string.IsNullOrEmpty(passNova2))
            {
                ViewBag.PassNova2 = passNova2;
            }

            // Procura o utilizador na base de dados
            var user = await db.Utilizadors.FindAsync(id);

            if (user == null) return NotFound();

            // Verifica a pass atual
            if (user.PasswordHash != passVelha)
            {
                ModelState.AddModelError("passVelha", "A Palavra-Passe atual foi introduzida incorretamente.");
                return View(user);
            }

            // Verifica se são iguais
            if (passNova != passNova2)
            {
                ModelState.AddModelError("passNova", "A confirmação da Palavra-Passe não foi feita corretamente.");
                return View(user);
            }

            // Valida se a nova é diferente
            if (passVelha == passNova)
            {
                ModelState.AddModelError("passNova", "A nova Palavra-Passe não pode ser igual à anterior.");
                return View(user);
            }

            // Se houver algum erro registrado no ModelState, devolve a view com os erros
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            try
            {
                user.PasswordHash = passNova;
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Details));
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }


        public IActionResult Logout()
        {
            //Limpar o session
            HttpContext.Session.SetString("NOME", "");
            HttpContext.Session.SetString("EMAIL", "");
            HttpContext.Session.SetString("TIPO", "");

            return RedirectToAction("Index", "Home");
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

        // ---------- Desativar
        public async Task<IActionResult> Desativar(int? id)
        {
            if (id == null) return NotFound();

            // Usa FindAsync ou FirstOrDefaultAsync para ser assíncrono
            var user = await db.Utilizadors.FindAsync(id);

            if (user != null)
            {
                return View(user);
            }

            return RedirectToAction("Index", "Login");
        }

        [HttpPost, ActionName("Desativar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DesativarConfirmed(int id)
        {
            var user = await db.Utilizadors.FindAsync(id);

            if (user != null)
            {
                user.Ativo = false;
                await db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}