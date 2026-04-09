using Microsoft.EntityFrameworkCore;
using NeoEvaluation.API.Models;
using System.Text.Json;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace NeoEvaluation.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // --- TABLES ---
        public DbSet<Utilisateur> Utilisateurs { get; set; } = null!;
        public DbSet<Entreprise> Entreprises { get; set; } = null!;
        public DbSet<InscriptionsEntreprise> InscriptionsEntreprises { get; set; } = null!;
        public DbSet<TokensActivation> TokensActivation { get; set; } = null!;
        public DbSet<Campagne> Campagnes { get; set; } = null!;
        public DbSet<Candidature> Candidatures { get; set; } = null!;
        public DbSet<Evaluation> Evaluations { get; set; } = null!;
        public DbSet<Questionnaire> Questionnaires { get; set; } = null!;
        public DbSet<Question> Questions { get; set; } = null!;
        public DbSet<Reponse> Reponses { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Personnel> Personnels { get; set; } = null!;
        public DbSet<Candidat> Candidats { get; set; } = null!;
        public DbSet<Planning> Plannings { get; set; } = null!;
        public DbSet<EntrepriseParSA> EntrepriseParSA { get; set; } = null!;
       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. CONFIGURATION DE L'HÉRITAGE (Table-per-Hierarchy)
            modelBuilder.Entity<Utilisateur>()
                .HasDiscriminator<string>("UserType")
                .HasValue<Personnel>("Personnel")
                .HasValue<Candidat>("Candidat")
                .HasValue<SuperAdmin>("SuperAdmin");

            // 2. RELATION 1:1 (Candidature <-> Evaluation)
            modelBuilder.Entity<Candidature>()
                .HasOne(c => c.Evaluation)
                .WithOne(e => e.Candidature)
                .HasForeignKey<Evaluation>(e => e.CandidatureId);

            // 3. CONFIGURATION DES LISTES (List<string> vers PostgreSQL Text/JSON)
            
            // Convertisseur : Transforme List<string> en string JSON pour la DB et vice-versa
            var listConverter = new ValueConverter<List<string>, string>(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null) ?? "[]",
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
            );

            // Comparateur : Permet à EF Core de détecter si un élément a été ajouté/supprimé dans la liste
            var listComparer = new ValueComparer<List<string>>(
                (c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()
            );

            // Application au champ Permissions de la table Roles
            modelBuilder.Entity<Role>()
                .Property(e => e.Permissions)
                .HasConversion(listConverter)
                .Metadata.SetValueComparer(listComparer);

            // Application au champ Privileges de la table Utilisateurs
            // IMPORTANT : Assurez-vous d'avoir ajouté "public List<string> Privileges { get; set; } = new();" 
            // dans votre classe Utilisateur.cs comme indiqué précédemment.
            modelBuilder.Entity<Utilisateur>()
                .Property(e => e.Privileges)
                .HasConversion(listConverter)
                .Metadata.SetValueComparer(listComparer);
        }
    }
}