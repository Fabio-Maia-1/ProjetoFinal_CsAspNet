using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoFinal.Data;
using ProjetoFinal.Models;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

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
        public async Task<IActionResult> Index(int? estado)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null && HttpContext.Session.GetString("TIPO") == "admin")
            {
                var sessoes = _context.Sessoes.Include(s => s.Filme).Include(s => s.Sala).OrderByDescending(s => s.DataHora).AsQueryable();

                if (estado.HasValue)
                {
                    if (estado.Value == 0) // Agendada
                    {
                        sessoes = sessoes.Where(s => s.Estado == EstadosSessao.Agendada || s.Estado == EstadosSessao.EmPreparacao);
                    }
                    else if (estado.Value == 1) // Concluida
                    {
                        sessoes = sessoes.Where(s => s.Estado == EstadosSessao.Concluida);
                    }
                    else // Cancelada
                    {
                        sessoes = sessoes.Where(s => s.Estado == EstadosSessao.Cancelada);
                    }
                }
                return View(await sessoes.ToListAsync());
            }

            return RedirectToAction("Index", "Login");
        }


        public async Task<IActionResult> Comprar(int? id)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo == true).FirstOrDefault();

            if (user != null)
            {
                if (id == null) return NotFound();

                var sessao = await _context.Sessoes.Include(s => s.Filme).Include(s => s.Sala).FirstOrDefaultAsync(m => m.Id == id);

                if (sessao == null) return NotFound();

                ViewBag.SerAdmin = false;
                if (HttpContext.Session.GetString("TIPO") == "admin")
                {
                    ViewBag.SerAdmin = true;
                }
                return View(sessao);
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
                bool temConflito = await _context.Sessoes.Where(s => s.SalaId == sessao.SalaId && s.Estado != EstadosSessao.Cancelada)
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

            // Se falhar a validação, recarrega a View com os dropdowns preenchidos e os erros de validação
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
                bool temConflito = await _context.Sessoes.Where(s => s.SalaId == sessao.SalaId && s.Estado != EstadosSessao.Cancelada)
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
                sessao.Estado = EstadosSessao.Cancelada; // Marca a sessão como cancelada em vez de removê-la
                _context.Update(sessao);
                //_context.Sessoes.Remove(sessao);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        // ------------ Disponibilizar
        public async Task<IActionResult> Disponibilizar(int? id)
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

        [HttpPost, ActionName("Disponibilizar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisponibilizarConfirmed(int id)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user == null || HttpContext.Session.GetString("TIPO") != "admin")
            {
                return RedirectToAction("Index", "Login");
            }

            var sessao = await _context.Sessoes.FindAsync(id);
            if (sessao != null)
            {
                sessao.Estado = EstadosSessao.Agendada;
                _context.Update(sessao);
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
