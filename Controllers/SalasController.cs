using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoFinal.Data;
using ProjetoFinal.Models;
using Microsoft.AspNetCore.Http;

namespace ProjetoFinal.Controllers
{
    public class SalasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Salas
        public async Task<IActionResult> Index()
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null && HttpContext.Session.GetString("TIPO") == "admin")
            {
                return View(await _context.Salas.OrderByDescending(s => s.Id).ToListAsync());
            }

            return RedirectToAction("Index", "Login");
        }

        // GET: Salas/Create
        public IActionResult Create()
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null && HttpContext.Session.GetString("TIPO") == "admin")
            {
                return View();
            }

            return RedirectToAction("Index", "Login");
        }

        // POST: Salas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,NumeroFilas,LugaresPorFila,Estado")] Sala sala)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user == null || HttpContext.Session.GetString("TIPO") != "admin")
            {
                return RedirectToAction("Index", "Login");
            }

            if (ModelState.IsValid)
            {
                _context.Add(sala);

                for(int i = 0; i < sala.NumeroFilas; i++)
                {
                    for(int j = 1; j <= sala.LugaresPorFila; j++)
                    {
                        Lugar lugar = new Lugar();
                        lugar.Fila = ((char)('A' + i)).ToString(); //Conertar número para letra (1 -> A, 2 -> B, etc.)
                        lugar.Numero = j;
                        lugar.SalaId = sala.Id;
                        lugar.Sala = sala;

                        _context.Lugares.Add(lugar);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sala);
        }

        // GET: Salas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null && HttpContext.Session.GetString("TIPO") == "admin")
            {
                if (id == null) return NotFound();

                var sala = await _context.Salas.FindAsync(id);
                if (sala == null) return NotFound();
                return View(sala);
            }

            return RedirectToAction("Index", "Login");
        }

        // POST: Salas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,NumeroFilas,LugaresPorFila,Estado")] Sala sala)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user == null || HttpContext.Session.GetString("TIPO") != "admin")
            {
                return RedirectToAction("Index", "Login");
            }

            if (id != sala.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sala);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalaExists(sala.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(sala);
        }

        // GET: Salas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null && HttpContext.Session.GetString("TIPO") == "admin")
            {
                if (id == null) return NotFound();

                var sala = await _context.Salas.FirstOrDefaultAsync(m => m.Id == id);
                if (sala == null) return NotFound();

                return View(sala);
            }

            return RedirectToAction("Index", "Login");
        }

        // POST: Salas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user == null || HttpContext.Session.GetString("TIPO") != "admin")
            {
                return RedirectToAction("Index", "Login");
            }

            var sala = await _context.Salas.FindAsync(id);
            if (sala != null)
            {
                var lugares = _context.Lugares.Where(l => l.SalaId == sala.Id);
                _context.Lugares.RemoveRange(lugares);

                _context.Salas.Remove(sala);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SalaExists(int id)
        {
            return _context.Salas.Any(e => e.Id == id);
        }
    }
}
