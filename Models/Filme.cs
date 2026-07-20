using ProjetoFinal.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoFinal.Models
{
    //Por algum motivo esta classe fica singular nos controllers automaticos.
    //Isto corrige e faz reconhecer como plural (Igual à db)
    //[Table("Filmes")]
    public class Filme
    {
        [Display(Name = "Id Filme")]
        public int Id { get; set; }


        [Display(Name = "Título")]
        [Required]
        [StringLength(150)]
        public string? Titulo { get; set; }


        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }


        //Medido em minutos
        [Display(Name = "Duração")]
        [Range(1, 500)]
        [Required]
        public int Duracao { get; set; }


        [Required]
        [Display(Name = "Género")]
        public GenerosFilme Genero { get; set; }


        [Display(Name = "Classificação Etária")]
        public classificacoesEtariasFilme? ClassificacaoEtaria { get; set; }


        [Display(Name = "Poster Path")]
        public string? PosterPath { get; set; }


        [Display(Name = "Disponível?")]
        public bool Disponivel { get; set; } = true;


        // Navigation Property
        public ICollection<Sessao> Sessoes { get; set; } = new List<Sessao>();
    }

    public enum GenerosFilme
    {
        Acao = 0,
        Comedia = 1,
        Drama = 2,
        Terror = 3,
        FiccaoCientifica = 4,
        Animacao = 5,
        Romance = 6,
        Aventura = 7,
        Thriller = 8,
        Musical = 9
    }

    public enum classificacoesEtariasFilme
    {
        M3 = 0,
        M6 = 1,
        M12 = 2, // M/12
        M14 = 3,
        M16 = 4,
        M18 = 5
    }

}
