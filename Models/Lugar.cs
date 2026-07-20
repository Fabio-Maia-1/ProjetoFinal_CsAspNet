using System.ComponentModel.DataAnnotations;

namespace ProjetoFinal.Models
{
    public class Lugar
    {
        public int Id { get; set; }

        public string? Fila { get; set; }

        public int Numero { get; set; }

        public int SalaId { get; set; }

        // Navigation Property
        public Sala? Sala { get; set; }

        public ICollection<Bilhete> Bilhetes { get; set; } = new List<Bilhete>();
    }
}
