using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoFinal.Data;
using ProjetoFinal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjetoFinal.Controllers
{
    public class BilhetesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BilhetesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bilhetes
        public async Task<IActionResult> Index(int? estado)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null && HttpContext.Session.GetString("TIPO") == "admin")
            {
                var bilhetes = _context.Bilhetes
                    .Include(b => b.Sessao).ThenInclude(s => s.Filme)
                    .Include(b => b.Lugar)
                    .Include(b => b.Compra)
                    .Include(b => b.TipoDeBilhete)
                    .OrderByDescending(b => b.Id).AsQueryable();

                if (estado.HasValue)
                {
                    bilhetes = bilhetes.Where(b => (int)b.Estado == estado.Value);
                }

                return View(await bilhetes.ToListAsync());
            }

            return RedirectToAction("Index", "Login");
        }


        // Ver todos os bilhetes do cliente autenticado
        // GET: Bilhetes/MeusBilhetes
        public async Task<IActionResult> MeusBilhetes()
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null && HttpContext.Session.GetString("TIPO") == "cliente")
            {
                var bilhetes = await _context.Bilhetes
                    .Include(b => b.Sessao).ThenInclude(s => s.Filme)
                    .Include(b => b.Lugar)
                    .Include(b => b.Compra)
                    .Include(b => b.TipoDeBilhete)
                    .Where(b => b.Compra.ClienteId == user.Id)
                    .OrderByDescending(b => b.Compra.DataCompra)
                    .ToListAsync();

                return View(bilhetes);
            }

            return RedirectToAction("Index", "Login");
        }



        // GET: Bilhetes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null && HttpContext.Session.GetString("TIPO") == "admin")
            {
                if (id == null) return NotFound();

                var bilhete = await _context.Bilhetes
                    .Include(b => b.Sessao).ThenInclude(s => s.Filme)
                    .Include(b => b.Lugar)
                    .Include(b => b.Compra)
                    .Include(b => b.TipoDeBilhete)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (bilhete == null) return NotFound();

                return View(bilhete);
            }

            return RedirectToAction("Index", "Login");
        }

        // GET: Bilhetes/Create
        public IActionResult Create()
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null && HttpContext.Session.GetString("TIPO") == "admin")
            {
                ViewBag.SessaoId = new SelectList(_context.Sessoes.OrderByDescending(s => s.DataHora), "Id", "DataHora");
                ViewBag.LugarId = new SelectList(_context.Lugares.OrderBy(l => l.SalaId).ThenBy(l => l.Numero), "Id", "Numero");
                ViewBag.CompraId = new SelectList(_context.Compras.OrderByDescending(c => c.DataCompra), "Id", "Id");
                ViewBag.TipoId = new SelectList(_context.TiposDeBilhetes.Where(t => t.Ativo), "Id", "Tipo");
                return View();
            }

            return RedirectToAction("Index", "Login");
        }

        // POST: Bilhetes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Estado,SessaoId,LugarId,CompraId,TipoId")] Bilhete bilhete)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user == null || HttpContext.Session.GetString("TIPO") != "admin")
            {
                return RedirectToAction("Index", "Login");
            }

            if (ModelState.IsValid)
            {
                _context.Add(bilhete);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SessaoId = new SelectList(_context.Sessoes.OrderByDescending(s => s.DataHora), "Id", "DataHora", bilhete.SessaoId);
            ViewBag.LugarId = new SelectList(_context.Lugares.OrderBy(l => l.SalaId).ThenBy(l => l.Numero), "Id", "Numero", bilhete.LugarId);
            ViewBag.CompraId = new SelectList(_context.Compras.OrderByDescending(c => c.DataCompra), "Id", "Id", bilhete.CompraId);
            ViewBag.TipoId = new SelectList(_context.TiposDeBilhetes.Where(t => t.Ativo), "Id", "Tipo", bilhete.TipoId);
            return View(bilhete);
        }

        // GET: Bilhetes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null && HttpContext.Session.GetString("TIPO") == "admin")
            {
                if (id == null) return NotFound();

                var bilhete = await _context.Bilhetes.FindAsync(id);
                if (bilhete == null) return NotFound();

                ViewBag.SessaoId = new SelectList(_context.Sessoes.OrderByDescending(s => s.DataHora), "Id", "DataHora", bilhete.SessaoId);
                ViewBag.LugarId = new SelectList(_context.Lugares.OrderBy(l => l.SalaId).ThenBy(l => l.Numero), "Id", "Numero", bilhete.LugarId);
                ViewBag.CompraId = new SelectList(_context.Compras.OrderByDescending(c => c.DataCompra), "Id", "Id", bilhete.CompraId);
                ViewBag.TipoId = new SelectList(_context.TiposDeBilhetes.Where(t => t.Ativo), "Id", "Tipo", bilhete.TipoId);
                return View(bilhete);
            }

            return RedirectToAction("Index", "Login");
        }

        // POST: Bilhetes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Estado,SessaoId,LugarId,CompraId,TipoId")] Bilhete bilhete)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user == null || HttpContext.Session.GetString("TIPO") != "admin")
            {
                return RedirectToAction("Index", "Login");
            }

            if (id != bilhete.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bilhete);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BilheteExists(bilhete.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SessaoId = new SelectList(_context.Sessoes.OrderByDescending(s => s.DataHora), "Id", "DataHora", bilhete.SessaoId);
            ViewBag.LugarId = new SelectList(_context.Lugares.OrderBy(l => l.SalaId).ThenBy(l => l.Numero), "Id", "Numero", bilhete.LugarId);
            ViewBag.CompraId = new SelectList(_context.Compras.OrderByDescending(c => c.DataCompra), "Id", "Id", bilhete.CompraId);
            ViewBag.TipoId = new SelectList(_context.TiposDeBilhetes.Where(t => t.Ativo), "Id", "Tipo", bilhete.TipoId);
            return View(bilhete);
        }

        // GET: Bilhetes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null && HttpContext.Session.GetString("TIPO") == "admin")
            {
                if (id == null) return NotFound();

                var bilhete = await _context.Bilhetes
                    .Include(b => b.Sessao).ThenInclude(s => s.Filme)
                    .Include(b => b.Lugar)
                    .Include(b => b.Compra)
                    .Include(b => b.TipoDeBilhete)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (bilhete == null) return NotFound();

                return View(bilhete);
            }

            return RedirectToAction("Index", "Login");
        }

        // POST: Bilhetes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user == null || HttpContext.Session.GetString("TIPO") != "admin")
            {
                return RedirectToAction("Index", "Login");
            }

            var bilhete = await _context.Bilhetes.FindAsync(id);
            if (bilhete != null)
            {
                _context.Bilhetes.Remove(bilhete);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BilheteExists(int id)
        {
            return _context.Bilhetes.Any(e => e.Id == id);
        }
    }
}
