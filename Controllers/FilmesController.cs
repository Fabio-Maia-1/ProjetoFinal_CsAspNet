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
        public async Task<IActionResult> Index()
        {
            return View(await _context.Filmes.ToListAsync());
        }

        // GET: Filmes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var filme = await _context.Filmes
                .FirstOrDefaultAsync(m => m.Id == id);

            if (filme == null) return NotFound();

            return View(filme);
        }

        // GET: Filmes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Filmes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,Descricao,Duracao,Genero,ClassificacaoEtaria,PosterPath,Disponivel")] Filme filme, IFormFile Poster)
        {
            if (ModelState.IsValid)
            {
                if (Poster != null && Poster.Length > 0)
                {
                    //Adicionar o poster ao diretório wwwroot/posters
                    var pasta = Path.Combine("wwwroot", "posters");
                    Directory.CreateDirectory(pasta);

                    var caminho = Path.Combine(pasta, Poster.FileName);

                    using (var stream = new FileStream(caminho, FileMode.Create))
                    {
                        Poster.CopyTo(stream);
                    }

                    // Atualizar o caminho do poster ao objeto filme
                    filme.PosterPath = "/posters/" + Poster.FileName;
                    filme.Disponivel = true; // Definir como disponível por padrão

                    _context.Add(filme);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(filme);
        }

        // GET: Filmes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var filme = await _context.Filmes.FindAsync(id);
            if (filme == null) return NotFound();
            return View(filme);
        }

        // POST: Filmes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Descricao,Duracao,Genero,ClassificacaoEtaria,PosterPath,Disponivel")] Filme filme)
        {
            if (id != filme.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
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
            if (id == null) return NotFound();

            var filme = await _context.Filmes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (filme == null) return NotFound();

            return View(filme);
        }

        // POST: Filmes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var filme = await _context.Filmes.FindAsync(id);
            if (filme != null)
            {
                _context.Filmes.Remove(filme);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool FilmeExists(int id)
        {
            return _context.Filmes.Any(e => e.Id == id);
        }
    }
}
