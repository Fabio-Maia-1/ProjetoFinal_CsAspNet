using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoFinal.Data;
using ProjetoFinal.Models;

namespace ProjetoFinal.Controllers
{
    public class FilmesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FilmesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Filmes
        public async Task<IActionResult> Index(bool? disponivel)
        {
            //Retorna o primeiro utilizador com este nome
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null)
            {
                if (HttpContext.Session.GetString("TIPO") == "admin")
                {
                    var filmes = _context.Filmes.OrderByDescending(f => f.Id).AsQueryable();

                    if (disponivel.HasValue)
                    {
                        filmes = filmes.Where(f => f.Disponivel == disponivel.Value);
                        
                    }

                    return View(await filmes.ToListAsync());
                }
            }
            return RedirectToAction("Index", "Login"); //Faz reload da página de login.
        }

        // GET: Filmes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //Retorna o primeiro utilizador com este nome
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null)
            {
                if (HttpContext.Session.GetString("TIPO") == "admin")
                {
                    if (id == null) return NotFound();

                    var filme = await _context.Filmes
                        .FirstOrDefaultAsync(m => m.Id == id);

                    if (filme == null) return NotFound();

                    return View(filme);
                }
            }
            return RedirectToAction("Index", "Login"); //Faz reload da página de login.
        }

        // GET: Filmes/Create
        public IActionResult Create()
        {
            //Retorna o primeiro utilizador com este nome
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null)
            {
                if (HttpContext.Session.GetString("TIPO") == "admin")
                {
                    return View();
                }
            }
            return RedirectToAction("Index", "Login"); //Faz reload da página de login.
        }

        // POST: Filmes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,Descricao,Duracao,Genero,ClassificacaoEtaria,Disponivel")] Filme filme, IFormFile Poster)
        {
            // 1. Validar se o ficheiro foi submetido
            if (Poster == null || Poster.Length == 0)
            {
                ModelState.AddModelError("PosterPath", "O poster do filme é obrigatório.");
            }
            else
            {
                // Limpa o erro padrão de [Required] do PosterPath para permitir que o ModelState.IsValid passe
                ModelState.Remove("PosterPath");
            }

            // 2. Agora sim, verificar se o modelo é válido
            if (ModelState.IsValid && Poster != null && Poster.Length > 0)
            {
                // Guardar a imagem na pasta
                var pasta = Path.Combine("wwwroot", "posters");
                Directory.CreateDirectory(pasta);

                var nomeFicheiro = Guid.NewGuid().ToString() + Path.GetExtension(Poster.FileName);
                var caminho = Path.Combine(pasta, nomeFicheiro);

                using (var stream = new FileStream(caminho, FileMode.Create))
                {
                    await Poster.CopyToAsync(stream);
                }

                // Atribuir o caminho gerado ao objeto filme
                filme.PosterPath = "/posters/" + nomeFicheiro;
                filme.Disponivel = true;

                _context.Add(filme);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(filme);
        }


        // GET: Filmes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //Retorna o primeiro utilizador com este nome
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null)
            {
                if (HttpContext.Session.GetString("TIPO") == "admin")
                {
                    if (id == null) return NotFound();

                    var filme = await _context.Filmes.FindAsync(id);
                    if (filme == null) return NotFound();
                    return View(filme);
                }
            }
            return RedirectToAction("Index", "Login"); //Faz reload da página de login.
        }

        // POST: Filmes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Descricao,Duracao,Genero,ClassificacaoEtaria,PosterPath,Disponivel")] Filme filme, IFormFile? Poster)
        {
            if (id != filme.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (Poster != null && Poster.Length > 0)
                    {
                        var pasta = Path.Combine("wwwroot", "posters");
                        Directory.CreateDirectory(pasta);

                        var fileName = Guid.NewGuid() +
                                       Path.GetExtension(Poster.FileName);

                        var caminho = Path.Combine(pasta, fileName);

                        using (var stream = new FileStream(caminho, FileMode.Create))
                        {
                            await Poster.CopyToAsync(stream);
                        }

                        filme.PosterPath = "/posters/" + fileName;
                    }

                    _context.Update(filme);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmeExists(filme.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(filme);
        }

        // GET: Filmes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            //Retorna o primeiro utilizador com este nome
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null)
            {
                if (HttpContext.Session.GetString("TIPO") == "admin")
                {
                    if (id == null) return NotFound();

                    var filme = await _context.Filmes
                        .FirstOrDefaultAsync(m => m.Id == id);
                    if (filme == null) return NotFound();

                    return View(filme);
                }
            }
            return RedirectToAction("Index", "Login"); //Faz reload da página de login.
        }

        // POST: Filmes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //Retorna o primeiro utilizador com este nome
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null)
            {
                if (HttpContext.Session.GetString("TIPO") == "admin")
                {
                    var filme = await _context.Filmes.FindAsync(id);
                    if (filme != null)
                    {
                        filme.Disponivel = false; // Marca o filme como indisponível em vez de removê-lo do banco de dados

                        _context.Update(filme);
                        await _context.SaveChangesAsync();
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
            return RedirectToAction("Index", "Login"); //Faz reload da página de login.
        }

        private bool FilmeExists(int id)
        {
            return _context.Filmes.Any(e => e.Id == id);
        }
    }
}
