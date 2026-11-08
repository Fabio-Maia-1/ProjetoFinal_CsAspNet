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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarCompra(string lugaresSelecionados, int sessaoId, int tipoBilheteId)
        {
            if (string.IsNullOrEmpty(lugaresSelecionados)) return NotFound();


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


            // Desserializar a lista de lugares
            var listaLugaresStr = System.Text.Json.JsonSerializer.Deserialize<List<string>>(lugaresSelecionados) ?? new List<string>();


            // Buscar os detalhes dos lugares selecionados (Fila e Número)
            var detalhesLugares = new List<object>();
            foreach (var l in listaLugaresStr)
            {
                // Exemplo: se o lugar for "A1", a fila é 'A' e o número é 1
                if (l.Length > 1)
                {
                    char fila = l[0];
                    if (int.TryParse(l.Substring(1), out int numLugar))
                    {
                        detalhesLugares.Add(new { Fila = fila, Numero = numLugar, Formatado = l });
                    }
                }
            }

            //Calcular preço
            decimal precoTotal = detalhesLugares.Count * tipoBilhete.Preco;


            // Passar dados auxiliares via ViewBag (sem ViewModel)
            ViewBag.TipoBilhete = tipoBilhete;
            ViewBag.Lugares = detalhesLugares;
            ViewBag.LugaresSelecionadosJson = lugaresSelecionados; // Para reutilizar no POST final
            ViewBag.PrecoTotal = precoTotal;


            // Retorna a view de confirmação (bilhetes ainda não são gravados)
            return View(sessao);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EfetuarCompra(string lugaresSelecionados, int sessaoId, int tipoBilheteId)
        {
            // Identificar o cliente que faz a compra
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user == null || HttpContext.Session.GetString("TIPO") != "cliente")
            {
                return RedirectToAction("Index", "Login");
            }

            if (string.IsNullOrEmpty(lugaresSelecionados)) return NotFound();


            // Buscar a sessão e o tipo de bilhete (calcular preços e verificar existência)
            var sessao = await _context.Sessoes.FindAsync(sessaoId);
            var tipoBilhete = await _context.TiposDeBilhetes.FindAsync(tipoBilheteId);

            if (sessao == null || tipoBilhete == null) return NotFound();


            // Desserializar a lista de lugares
            var listaLugaresStr = System.Text.Json.JsonSerializer.Deserialize<List<string>>(lugaresSelecionados) ?? new List<string>();

            if (!listaLugaresStr.Any()) return RedirectToAction("Comprar", "Sessaos", new { id = sessaoId });


            // Iniciar transação na base de dados para garantir consistência (se houver um erro a meio do processo, será tudo cancelado)
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Criar e registar a Compra principal
                var compra = new Compra
                {
                    ClienteId = user.Id,
                    DataCompra = DateTime.Now
                };

                _context.Compras.Add(compra);
                await _context.SaveChangesAsync(); // Guarda para gerar o ID da compra

                // Criar e associar cada bilhete à compra
                foreach (var l in listaLugaresStr)
                {
                    if (l.Length > 1)
                    {
                        string fila = l[0].ToString();
                        if (int.TryParse(l.Substring(1), out int numLugar))
                        {
                            // Decifrar lugar escolhido
                            var lugarEscolhido = await _context.Lugares.Where(l => l.Fila == fila && l.Numero == numLugar && l.SalaId == sessao.SalaId).FirstOrDefaultAsync();

                            // Verificar se bilhete para o mesmo lugar foi criado por outro cliente entretanto
                            bool lugarOcupado = await _context.Bilhetes.AnyAsync(b => b.SessaoId == sessaoId && b.Lugar.Fila == fila && b.Lugar.Numero == numLugar && b.Estado != EstadosBilhete.Reembolsado);

                            if (lugarOcupado)
                            {
                                // Cancela a transação e avisa o utilizador
                                await transaction.RollbackAsync();
                                TempData["Erro"] = "Pelo menos um dos lugares selecionados já foram ocupados por outro cliente. Por favor, escolha outros lugares."; // Guarda a mensagem no TempData para sobreviver ao redirecionamento
                                return RedirectToAction("Comprar", "Sessaos", new { id = sessaoId });
                            }

                            var bilhete = new Bilhete
                            {
                                SessaoId = sessaoId,
                                LugarId = lugarEscolhido.Id,
                                CompraId = compra.Id,
                                TipoId = tipoBilheteId,
                                Estado = EstadosBilhete.Pendente
                            };

                            _context.Bilhetes.Add(bilhete);
                        }
                    }
                }

                await _context.SaveChangesAsync();

                // Confirmar a transação com sucesso
                await transaction.CommitAsync();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                // Num erro, reverter todas as operações na BD
                await transaction.RollbackAsync();
                throw;
            }
        }


        //Lista de compras do cliente
        public async Task<IActionResult> ListaCompras()
        {
            var user = _context.Utilizadors.Where(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo).FirstOrDefault();

            if (user != null)
            {
                var compras = await _context.Compras
                .Include(c => c.Cliente)
                .Include(c => c.Bilhetes).ThenInclude(b => b.Sessao).ThenInclude(s => s.Filme)
                .Include(c => c.Bilhetes).ThenInclude(b => b.Lugar)
                .Include(c => c.Bilhetes).ThenInclude(b => b.TipoDeBilhete)
                .Where(c => c.ClienteId == user.Id)
                .OrderByDescending(c => c.DataCompra)
                .ToListAsync();

                return View(compras);
            }

            return RedirectToAction("Index", "Login");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReembolsarCompra(int id)
        {
            var user = _context.Utilizadors.FirstOrDefault(u => u.Email == HttpContext.Session.GetString("EMAIL") && u.Ativo);

            if (user == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Procurar a compra com os bilhetes
            var compra = await _context.Compras
                .Include(c => c.Bilhetes).ThenInclude(b => b.Sessao)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (compra == null) return NotFound();

            // Verificar se a compra pertence ao utilizador ou se é admin
            if (HttpContext.Session.GetString("TIPO") != "admin" && compra.ClienteId != user.Id)
            {
                return RedirectToAction("Index", "Home");
            }

            // Validar se os bilhetes elegíveis
            bool todosValidos = compra.Bilhetes.Any() && compra.Bilhetes.All(b =>
                b.Estado == EstadosBilhete.Pendente && b.Sessao.DataHora > DateTime.Now.AddHours(48));

            if (!todosValidos)
            {
                return RedirectToAction("ListaCompras");
            }

            // Atualizar estado de todos os bilhetes
            foreach (var bilhete in compra.Bilhetes)
            {
                bilhete.Estado = EstadosBilhete.Reembolsado;
                _context.Update(bilhete);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("ListaCompras");
        }
    }
}