using System.ComponentModel.DataAnnotations;

namespace ProjetoFinal.Models
{
    public class Sala
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nº de Filas")]
        public int NumeroFilas { get; set; }

        [Required]
        [Display(Name = "Nº de Lugares por Fila")]
        public int LugaresPorFila { get; set; }

        public EstadosSala Estado { get; set; } = EstadosSala.Ativa;

        public int Capacidade => NumeroFilas * LugaresPorFila;

        // Navigation Properties
        public ICollection<Lugar> Lugares { get; set; } = new List<Lugar>();

        public ICollection<Sessao> Sessoes { get; set; } = new List<Sessao>();
    }

    public enum EstadosSala
    {
        Ativa = 0,
        Manutencao = 1,
        Fechada = 2
    }
}
