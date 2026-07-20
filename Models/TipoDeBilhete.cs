using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ProjetoFinal.Models
{
    public class TipoDeBilhete
    {
        public int Id { get; set; }

        [Required]
        public string? Tipo { get; set; }

        [Required]
        [Precision(4,2)]
        [Display(Name = "Preço")]
        public decimal Preco { get; set; }

        [Display(Name = "Ativo?")]
        public bool Ativo { get; set; } = true;

        public ICollection<Bilhete> Bilhetes { get; set; } = new List<Bilhete>();
    }
}
