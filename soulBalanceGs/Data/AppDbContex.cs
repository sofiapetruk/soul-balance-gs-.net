using Microsoft.EntityFrameworkCore;
using soulBalanceGs.Models;

namespace soulBalanceGs.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<CheckinManual> CheckinManuais { get; set; }

        public DbSet<Atividade> Atividades { get; set; }

        public DbSet<DadosSensor> DadosSensores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Nome).IsRequired();
                entity.Property(u => u.Email).IsRequired();
                entity.Property(u => u.Senha).IsRequired();
            });

            modelBuilder.Entity<CheckinManual>(entity =>
            {
                entity.HasKey(c => c.ChekinId);

                entity.Property(c => c.Humor).IsRequired();
                entity.Property(c => c.Energia).IsRequired();
                entity.Property(c => c.Foco).IsRequired();
                entity.Property(c => c.Time).IsRequired();

                entity.HasOne(c => c.Usuario)
                      .WithMany() 
                      .HasForeignKey(c => c.FkIdUsuario)
                      .IsRequired();
            });

            modelBuilder.Entity<Atividade>(entity =>
            {
                entity.HasKey(a => a.AtividadeId);

                entity.Property(a => a.TipoAtividade)
                    .IsRequired()
                    .HasConversion<int>();

                entity.Property(a => a.Descricao).IsRequired(false);
                entity.Property(a => a.Inicio).IsRequired();
                entity.Property(a => a.Fim).IsRequired();
                entity.Property(a => a.DuracaoMinutosAtividade).IsRequired();

                entity.HasOne(a => a.Usuario)
                      .WithMany()
                      .HasForeignKey(a => a.FkIdUsuario)
                      .IsRequired();
            });

            modelBuilder.Entity<DadosSensor>(entity => 
            {
                entity.HasKey(d => d.DadosSensorId); 

                entity.Property(d => d.TipoDado).IsRequired().HasConversion<string>(); 

                entity.Property(d => d.Valor).IsRequired();
                entity.Property(d => d.Time).IsRequired();


                entity.HasOne(d => d.Usuario)
                        .WithMany()
                        .HasForeignKey(d => d.FkIdUsuario);
            });

            base.OnModelCreating(modelBuilder);
        }

    }
}
