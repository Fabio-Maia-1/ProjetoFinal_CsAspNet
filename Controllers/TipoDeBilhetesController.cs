using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoFinal.Data;
using ProjetoFinal.Models;
using Microsoft.AspNetCore.Http;

namespace ProjetoFinal.Controllers
{
    public class TipoDeBilhetesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TipoDeBilhetesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TipoDeBilhetes
        public async Task<IActionResult> Index(bool? ativo)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null && HttpContext.Session.GetString("TIPO") == "admin")
            {
                var tipos = _context.TiposDeBilhetes.OrderByDescending(t => t.Id).AsQueryable();

                if (ativo.HasValue)
                {
                    tipos = tipos.Where(t => t.Ativo == ativo.Value);
                }

                return View(await tipos.ToListAsync());
            }

            return RedirectToAction("Index", "Login");
        }

        // GET: TipoDeBilhetes/Create
        public IActionResult Create()
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null && HttpContext.Session.GetString("TIPO") == "admin")
            {
                return View();
            }

            return RedirectToAction("Index", "Login");
        }

        // POST: TipoDeBilhetes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Tipo,Preco,Ativo")] TipoDeBilhete tipoDeBilhete)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user == null || HttpContext.Session.GetString("TIPO") != "admin")
            {
                return RedirectToAction("Index", "Login");
            }

            if (ModelState.IsValid)
            {
                _context.Add(tipoDeBilhete);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoDeBilhete);
        }

        // GET: TipoDeBilhetes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null && HttpContext.Session.GetString("TIPO") == "admin")
            {
                if (id == null) return NotFound();

                var tipo = await _context.TiposDeBilhetes.FindAsync(id);
                if (tipo == null) return NotFound();
                return View(tipo);
            }

            return RedirectToAction("Index", "Login");
        }

        // POST: TipoDeBilhetes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tipo,Preco,Ativo")] TipoDeBilhete tipoDeBilhete)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user == null || HttpContext.Session.GetString("TIPO") != "admin")
            {
                return RedirectToAction("Index", "Login");
            }

            if (id != tipoDeBilhete.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoDeBilhete);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoDeBilheteExists(tipoDeBilhete.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tipoDeBilhete);
        }

        // GET: TipoDeBilhetes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null && HttpContext.Session.GetString("TIPO") == "admin")
            {
                if (id == null) return NotFound();

                var tipo = await _context.TiposDeBilhetes.FirstOrDefaultAsync(m => m.Id == id);
                if (tipo == null) return NotFound();

                return View(tipo);
            }

            return RedirectToAction("Index", "Login");
        }

        // POST: TipoDeBilhetes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user == null || HttpContext.Session.GetString("TIPO") != "admin")
            {
                return RedirectToAction("Index", "Login");
            }

            var tipo = await _context.TiposDeBilhetes.FindAsync(id);
            if (tipo != null)
            {
                // Mark as inactive instead of deleting
                tipo.Ativo = false;
                _context.Update(tipo);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // ------------ REATIVAR
        public async Task<IActionResult> Ativar(int? id)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null && HttpContext.Session.GetString("TIPO") == "admin")
            {
                if (id == null) return NotFound();

                var tipo = await _context.TiposDeBilhetes.FirstOrDefaultAsync(m => m.Id == id);
                if (tipo == null) return NotFound();

                return View(tipo);
            }

            return RedirectToAction("Index", "Login");
        }

        [HttpPost, ActionName("Ativar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtivarConfirmed(int id)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user == null || HttpContext.Session.GetString("TIPO") != "admin")
            {
                return RedirectToAction("Index", "Login");
            }

            var tipo = await _context.TiposDeBilhetes.FindAsync(id);
            if (tipo != null)
            {
                tipo.Ativo = true;
                _context.Update(tipo);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        private bool TipoDeBilheteExists(int id)
        {
            return _context.TiposDeBilhetes.Any(e => e.Id == id);
        }
    }
}
