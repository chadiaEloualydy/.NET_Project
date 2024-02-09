using Microsoft.EntityFrameworkCore;


namespace test_API.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Cours> Courses { get; set; }
        public DbSet<Etudiant> Etudiants { get; set; }
        public DbSet<Formateur> Formateurs { get; set; }
        public DbSet<Niveau> Niveaux { get; set; }

    }
}
