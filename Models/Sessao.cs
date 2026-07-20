using System.ComponentModel.DataAnnotations;

namespace ProjetoFinal.Models
{
    public class Sessao
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Data e Hora")]
        public DateTime DataHora { get; set; }

        public EstadosSessao Estado { get; set; } = EstadosSessao.Agendada;

        [Required]
        public int FilmeId { get; set; }

        [Required]
        public int SalaId { get; set; }

        // Navigation Properties
        public Filme? Filme { get; set; }

        public Sala? Sala { get; set; }

        public ICollection<Bilhete> Bilhetes { get; set; } = new List<Bilhete>();
    }

    public enum EstadosSessao
    {
        Agendada = 0,
        Concluida = 1,
        Cancelada = 2
    }
}
