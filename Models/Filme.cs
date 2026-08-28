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
        [Required(ErrorMessage = "O título do filme é obrigatório.")]
        [StringLength(150, ErrorMessage = "O título do filme não pode exceder 150 caracteres.")]
        public string? Titulo { get; set; }


        [Display(Name = "Descrição")]
        public string? Descricao { get; set; }


        //Medido em minutos
        [Display(Name = "Duração")]
        [Range(1, 500, ErrorMessage = "A duração deve estar entre 1 e 500 minutos.")]
        [Required(ErrorMessage = "A duração do filme é obrigatória.")]
        public int Duracao { get; set; }


        [Required(ErrorMessage = "O género do filme é obrigatório.")]
        [Display(Name = "Género")]
        public GenerosFilme Genero { get; set; }


        [Display(Name = "Classificação Etária")]
        [Required(ErrorMessage = "A classificação etária do filme é obrigatória.")]
        public classificacoesEtariasFilme? ClassificacaoEtaria { get; set; }


        [Display(Name = "Poster Path")]
        [Required(ErrorMessage = "O poster do filme é obrigatório.")]
        public string? PosterPath { get; set; }


        [Display(Name = "Disponível?")]
        public bool Disponivel { get; set; } = true;


        // Navigation Property
        public ICollection<Sessao> Sessoes { get; set; } = new List<Sessao>();
    }

    public enum GenerosFilme
    {
        [Display(Name = "Ação")]
        Acao = 0,
        [Display(Name = "Comédia")]
        Comedia = 1,
        Drama = 2,
        Terror = 3,
        [Display(Name = "Ficção Científica")]
        FiccaoCientifica = 4,
        [Display(Name = "Animação")]
        Animacao = 5,
        Romance = 6,
        [Display(Name = "Aventura")]
        Aventura = 7,
        Thriller = 8,
        Musical = 9,
        Fantasia = 10,
        Biografia = 11,
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
