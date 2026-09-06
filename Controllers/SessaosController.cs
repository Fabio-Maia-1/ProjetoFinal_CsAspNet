using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoFinal.Data;
using ProjetoFinal.Models;
using Microsoft.AspNetCore.Http;

namespace ProjetoFinal.Controllers
{
    public class SessaosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SessaosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Sessaos
        public async Task<IActionResult> Index()
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null && HttpContext.Session.GetString("TIPO") == "admin")
            {
                var sessoes = _context.Sessoes
                    .Include(s => s.Filme)
                    .Include(s => s.Sala)
                    .OrderByDescending(s => s.DataHora);

                return View(await sessoes.ToListAsync());
            }

            return RedirectToAction("Index", "Login");
        }

        // GET: Sessaos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null && HttpContext.Session.GetString("TIPO") == "admin")
            {
                if (id == null) return NotFound();

                var sessao = await _context.Sessoes
                    .Include(s => s.Filme)
                    .Include(s => s.Sala)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (sessao == null) return NotFound();

                return View(sessao);
            }

            return RedirectToAction("Index", "Login");
        }

        // GET: Sessaos/Create
        public IActionResult Create()
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null && HttpContext.Session.GetString("TIPO") == "admin")
            {
                ViewBag.FilmeId = new SelectList(_context.Filmes.Where(f => f.Disponivel), "Id", "Titulo");
                ViewBag.SalaId = new SelectList(_context.Salas, "Id", "Nome");
                return View();
            }

            return RedirectToAction("Index", "Login");
        }

        // POST: Sessaos/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DataHora,Estado,FilmeId,SalaId")] Sessao sessao)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user == null || HttpContext.Session.GetString("TIPO") != "admin")
            {
                return RedirectToAction("Index", "Login");
            }

            // Procurar o filme selecionado (para obter a sua duração)
            var filme = await _context.Filmes.FindAsync(sessao.FilmeId);

            if (filme != null)
            {
                // Hora de início
                DateTime inicioNovaSessao = sessao.DataHora;

                // Hora de fim (Início + Duração do filme + 60 minutos intervalo)
                DateTime fimNovaSessao = inicioNovaSessao.AddMinutes(filme.Duracao + 60);

                // Procurar sessões para a mesma sala à mesma hora
                bool temConflito = await _context.Sessoes.Where(s => s.SalaId == sessao.SalaId)
                    .AnyAsync(s => inicioNovaSessao < s.DataHora.AddMinutes(filme.Duracao + 60) && fimNovaSessao > s.DataHora);

                if (temConflito)
                {
                    ModelState.AddModelError("DataHora", "A sala selecionada está ocupada no horário indicado.");
                }
            }
            else
            {
                ModelState.AddModelError("FilmeId", "O filme selecionado é inválido.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(sessao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Se falhar a validação, recarrega a View com as dropdowns selecionados
            ViewBag.FilmeId = new SelectList(_context.Filmes.Where(f => f.Disponivel), "Id", "Titulo", sessao.FilmeId);
            ViewBag.SalaId = new SelectList(_context.Salas, "Id", "Nome", sessao.SalaId);
            return View(sessao);
        }


        // GET: Sessaos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null && HttpContext.Session.GetString("TIPO") == "admin")
            {
                if (id == null) return NotFound();

                var sessao = await _context.Sessoes.FindAsync(id);
                if (sessao == null) return NotFound();

                ViewBag.FilmeId = new SelectList(_context.Filmes.Where(f => f.Disponivel), "Id", "Titulo", sessao.FilmeId);
                ViewBag.SalaId = new SelectList(_context.Salas, "Id", "Nome", sessao.SalaId);
                return View(sessao);
            }

            return RedirectToAction("Index", "Login");
        }

        // POST: Sessaos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DataHora,Estado,FilmeId,SalaId")] Sessao sessao)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user == null || HttpContext.Session.GetString("TIPO") != "admin")
            {
                return RedirectToAction("Index", "Login");
            }

            if (id != sessao.Id) return NotFound();



            var filme = await _context.Filmes.FindAsync(sessao.FilmeId);

            if (filme != null)
            {
                DateTime inicioNovaSessao = sessao.DataHora;
                DateTime fimNovaSessao = inicioNovaSessao.AddMinutes(filme.Duracao + 60);

                // Verificar sobreposição excluindo a sessão atual (s.Id != sessao.Id)
                bool temConflito = await _context.Sessoes.Where(s => s.SalaId == sessao.SalaId && s.Id != sessao.Id)
                    .AnyAsync(s => inicioNovaSessao < s.DataHora.AddMinutes(filme.Duracao + 60) && fimNovaSessao > s.DataHora);

                if (temConflito)
                {
                    ModelState.AddModelError("DataHora", "A sala selecionada está ocupada no horário indicado.");
                }
            }
            else
            {
                ModelState.AddModelError("FilmeId", "O filme selecionado é inválido.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sessao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SessaoExists(sessao.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["FilmeId"] = new SelectList(_context.Filmes.Where(f => f.Disponivel), "Id", "Titulo", sessao.FilmeId);
            ViewData["SalaId"] = new SelectList(_context.Salas, "Id", "Nome", sessao.SalaId);
            return View(sessao);
        }

        // GET: Sessaos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null && HttpContext.Session.GetString("TIPO") == "admin")
            {
                if (id == null) return NotFound();

                var sessao = await _context.Sessoes
                    .Include(s => s.Filme)
                    .Include(s => s.Sala)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (sessao == null) return NotFound();

                return View(sessao);
            }

            return RedirectToAction("Index", "Login");
        }

        // POST: Sessaos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user == null || HttpContext.Session.GetString("TIPO") != "admin")
            {
                return RedirectToAction("Index", "Login");
            }

            var sessao = await _context.Sessoes.FindAsync(id);
            if (sessao != null)
            {
                _context.Sessoes.Remove(sessao);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SessaoExists(int id)
        {
            return _context.Sessoes.Any(e => e.Id == id);
        }
    }
}
