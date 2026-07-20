using System.ComponentModel.DataAnnotations;

namespace ProjetoFinal.Models
{
    public class Compra
    {
        public int Id { get; set; }

        [Display(Name = "Data da Compra")]
        public DateTime DataCompra { get; set; } = DateTime.Now;

        public int ClienteId { get; set; }

        // Navigation Property
        public Utilizador? Cliente { get; set; }

        public ICollection<Bilhete> Bilhetes { get; set; } = new List<Bilhete>();
    }
}
