using System.ComponentModel.DataAnnotations;

namespace ProjetoFinal.Models
{
    public class Sala
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da sala é obrigatório.")]
        [StringLength(50, ErrorMessage = "O nome da sala não pode exceder 50 caracteres.")]
        public string? Nome { get; set; }

        [Required(ErrorMessage = "O número de filas da sala é obrigatório.")]
        [Range(3, 26, ErrorMessage = "O número de filas deve estar entre 3 e 26.")]
        [Display(Name = "Nº de Filas")]
        public int NumeroFilas { get; set; }

        [Required(ErrorMessage = "O número de lugares por fila é obrigatório.")]
        [Range(3, 30, ErrorMessage = "O número de lugares por fila deve estar entre 3 e 30.")]
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
