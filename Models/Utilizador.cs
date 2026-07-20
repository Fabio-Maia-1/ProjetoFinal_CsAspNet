using System.ComponentModel.DataAnnotations;

namespace ProjetoFinal.Models
{
    public class Utilizador
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string TipoUtilizador { get; set; } = string.Empty;

        [Display(Name = "Ativo?")]
        public bool Ativo { get; set; } = true;

        // Navigation Property
        public ICollection<Compra> Compras { get; set; } = new List<Compra>();
    }
}
