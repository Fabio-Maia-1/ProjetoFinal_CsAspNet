using System.ComponentModel.DataAnnotations;

namespace ProjetoFinal.Models
{
    public class Bilhete
    {
        public int Id { get; set; }

        public EstadosBilhete Estado { get; set; } = EstadosBilhete.Pendente;

        public int SessaoId { get; set; }

        public int LugarId { get; set; }

        public int CompraId { get; set; }

        public int TipoId { get; set; }

        // Navigation Properties
        public Sessao? Sessao { get; set; }

        public Lugar? Lugar { get; set; }

        public Compra? Compra { get; set; }

        public TipoDeBilhete? TipoDeBilhete { get; set; }
    }

    public enum EstadosBilhete
    {
        Pendente = 0,
        Concluido = 1,
        Reembolsado = 2
    }
}
