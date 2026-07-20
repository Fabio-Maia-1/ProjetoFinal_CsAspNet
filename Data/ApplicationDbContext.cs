using ProjetoFinal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace ProjetoFinal.Data

{
    public class ApplicationDbContext :DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {

        }

        public DbSet<Utilizador> Utilizadors { get; set; }

        public DbSet<Filme> Filmes { get; set; }

        public DbSet<Sala> Salas { get; set; }

        public DbSet<Lugar> Lugares { get; set; }

        public DbSet<Sessao> Sessoes { get; set; }

        public DbSet<Compra> Compras { get; set; }

        public DbSet<TipoDeBilhete> TiposDeBilhetes { get; set; }

        public DbSet<Bilhete> Bilhetes { get; set; }

        //Na constroção do modelo da base de dados
        //Desativar o cascade delete numa das relações que chega a Bilhete (neste cado Sessoes)
        //porque apenas pode existir um cascade delete por entidade
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); //executa a configuração da classe base (DbContext)

            // configurar tabela Bilhete
            modelBuilder.Entity<Bilhete>()
                .HasOne(b => b.Sessao) //Cada Bilhete tem uma Sessao
                .WithMany(s => s.Bilhetes) //Uma Sessao pode ter muitos Bilhetes
                .HasForeignKey(b => b.SessaoId) //Indicar a chave estrangeira
                .OnDelete(DeleteBehavior.NoAction); //Desativar eliminação automatica nesta relação

            //A eleminação automatica (Cascade delete) ainda funciona a partir dos lugares, mas se tentar
            //apagar uma sessão que ainda tem bilhetes, a base de dados devolve um erro de integridade
            //referencial em vez de apagar os bilhetes relevantes automaticamente.
        }
    }
}


//Nota: O servidor e base de dados está no "appsetting.json" e são usados em "Program.cs"