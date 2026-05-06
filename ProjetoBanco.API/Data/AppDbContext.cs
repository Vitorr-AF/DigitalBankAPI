using Microsoft.EntityFrameworkCore;
using ProjetoBanco.API.Models;

namespace ProjetoBanco.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<PessoaFisica> PessoasFisicas { get; set; }
        public DbSet<PessoaJuridica> PessoasJuridicas { get; set; }
        public DbSet<Agencia> Agencias { get; set; }
        public DbSet<Contratacao> Contratacoes { get; set; }
        public DbSet<Produto> Produtos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente>()
                .HasDiscriminator<string>("TipoCliente")
                .HasValue<PessoaFisica>("PF")
                .HasValue<PessoaJuridica>("PJ");

            modelBuilder.Entity<Produto>()
                .HasDiscriminator<string>("TipoProduto")
                .HasValue<MaquinaDeCartao>("Maquina")
                .HasValue<ReceberSalario>("Salario")
                .HasValue<Emprestimo>("Emprestimo");

            modelBuilder.Entity<Cliente>()
                .HasOne(c => c.Agencia)
                .WithMany(a => a.Clientes)
                .HasForeignKey(c => c.AgenciaId);

            modelBuilder.Entity<Contratacao>()
                .HasOne(c => c.Cliente)
                .WithMany(cl => cl.Contratacoes)
                .HasForeignKey(c => c.ClienteId);
        }
    }
}
