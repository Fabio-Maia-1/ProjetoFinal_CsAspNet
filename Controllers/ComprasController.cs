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
    public class ComprasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ComprasController(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> ConfirmarCompra(string lugaresSelecionados, int sessaoId, int tipoBilheteId)
        {
            if (lugaresSelecionados == null || lugaresSelecionados.Length == 0) NotFound();

            // Buscar a sessão, filme e sala
            var sessao = await _context.Sessoes
                .Include(s => s.Filme)
                .Include(s => s.Sala)
                .FirstOrDefaultAsync(s => s.Id == sessaoId);

            if (sessao == null) return NotFound();

            // Buscar o tipo de bilhete escolhido
            var tipoBilhete = await _context.TiposDeBilhetes.FindAsync(tipoBilheteId);
            if (tipoBilhete == null) return NotFound();

            // Converter a string de lugares selecionados em uma lista de inteiros
            

            // Buscar os detalhes dos lugares selecionados (Fila e Número)
            

            // Passar dados auxiliares via ViewBag (sem ViewModel)
           
            ViewBag.xxx = lugaresSelecionados;

            // Retorna a view de confirmação (NADA É GRAVADO AINDA NA BD)
            return View(sessao);
        }


        // GET: Compras
        public async Task<IActionResult> Index()
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null && HttpContext.Session.GetString("TIPO") == "admin")
            {
                var compras = _context.Compras
                    .Include(c => c.Cliente)
                    .Include(c => c.Bilhetes)
                    .OrderByDescending(c => c.DataCompra)
                    .AsQueryable();

                return View(await compras.ToListAsync());
            }

            return RedirectToAction("Index", "Login");
        }

        // GET: Compras/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null && HttpContext.Session.GetString("TIPO") == "admin")
            {
                if (id == null) return NotFound();

                var compra = await _context.Compras
                    .Include(c => c.Cliente)
                    .Include(c => c.Bilhetes).ThenInclude(b => b.Sessao).ThenInclude(s => s.Filme)
                    .Include(c => c.Bilhetes).ThenInclude(b => b.Lugar)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (compra == null) return NotFound();

                return View(compra);
            }

            return RedirectToAction("Index", "Login");
        }

        // GET: Compras/Create
        public IActionResult Create()
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null && HttpContext.Session.GetString("TIPO") == "admin")
            {
                ViewBag.ClienteId = new SelectList(_context.Utilizadors.Where(u => u.Ativo).OrderBy(u => u.Nome), "Id", "Nome");
                return View();
            }

            return RedirectToAction("Index", "Login");
        }

        // POST: Compras/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DataCompra,ClienteId")] Compra compra)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user == null || HttpContext.Session.GetString("TIPO") != "admin")
            {
                return RedirectToAction("Index", "Login");
            }

            if (ModelState.IsValid)
            {
                _context.Add(compra);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ClienteId = new SelectList(_context.Utilizadors.Where(u => u.Ativo).OrderBy(u => u.Nome), "Id", "Nome", compra.ClienteId);
            return View(compra);
        }

        // GET: Compras/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null && HttpContext.Session.GetString("TIPO") == "admin")
            {
                if (id == null) return NotFound();

                var compra = await _context.Compras.FindAsync(id);
                if (compra == null) return NotFound();

                ViewBag.ClienteId = new SelectList(_context.Utilizadors.Where(u => u.Ativo).OrderBy(u => u.Nome), "Id", "Nome", compra.ClienteId);
                return View(compra);
            }

            return RedirectToAction("Index", "Login");
        }

        // POST: Compras/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DataCompra,ClienteId")] Compra compra)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user == null || HttpContext.Session.GetString("TIPO") != "admin")
            {
                return RedirectToAction("Index", "Login");
            }

            if (id != compra.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(compra);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompraExists(compra.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ClienteId = new SelectList(_context.Utilizadors.Where(u => u.Ativo).OrderBy(u => u.Nome), "Id", "Nome", compra.ClienteId);
            return View(compra);
        }

        // GET: Compras/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null && HttpContext.Session.GetString("TIPO") == "admin")
            {
                if (id == null) return NotFound();

                var compra = await _context.Compras
                    .Include(c => c.Cliente)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (compra == null) return NotFound();

                return View(compra);
            }

            return RedirectToAction("Index", "Login");
        }

        // POST: Compras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user == null || HttpContext.Session.GetString("TIPO") != "admin")
            {
                return RedirectToAction("Index", "Login");
            }

            var compra = await _context.Compras.FindAsync(id);
            if (compra != null)
            {
                _context.Compras.Remove(compra);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CompraExists(int id)
        {
            return _context.Compras.Any(e => e.Id == id);
        }
    }
}
