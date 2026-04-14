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
        // Nouvelles tables
        public DbSet<Rapport> Rapports { get; set; } = null!;
        public DbSet<QuestionnaireQuestion> QuestionnaireQuestions { get; set; } = null!;
        public DbSet<CampagneQuestionnaire> CampagneQuestionnaires { get; set; } = null!;

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

            // 3. RELATION 1:1 (Evaluation <-> Rapport)
            modelBuilder.Entity<Evaluation>()
                .HasOne(e => e.Rapport)
                .WithOne(r => r.Evaluation)
                .HasForeignKey<Rapport>(r => r.EvaluationId);

            // 4. MANY-TO-MANY : Questionnaire <-> Question (via QuestionnaireQuestion)
            modelBuilder.Entity<QuestionnaireQuestion>()
                .HasKey(qq => new { qq.QuestionnaireId, qq.QuestionId });

            modelBuilder.Entity<QuestionnaireQuestion>()
                .HasOne(qq => qq.Questionnaire)
                .WithMany(q => q.QuestionnaireQuestions)
                .HasForeignKey(qq => qq.QuestionnaireId);

            modelBuilder.Entity<QuestionnaireQuestion>()
                .HasOne(qq => qq.Question)
                .WithMany(q => q.QuestionnaireQuestions)
                .HasForeignKey(qq => qq.QuestionId);

            // 5. MANY-TO-MANY : Campagne <-> Questionnaire (via CampagneQuestionnaire)
            modelBuilder.Entity<CampagneQuestionnaire>()
                .HasKey(cq => new { cq.CampagneId, cq.QuestionnaireId });

            modelBuilder.Entity<CampagneQuestionnaire>()
                .HasOne(cq => cq.Campagne)
                .WithMany(c => c.CampagneQuestionnaires)
                .HasForeignKey(cq => cq.CampagneId);

            modelBuilder.Entity<CampagneQuestionnaire>()
                .HasOne(cq => cq.Questionnaire)
                .WithMany(q => q.CampagneQuestionnaires)
                .HasForeignKey(cq => cq.QuestionnaireId);

            // 6. CONFIGURATION DES LISTES (List<string> vers JSON)
            var listConverter = new ValueConverter<List<string>, string>(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null) ?? "[]",
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
            );

            var listComparer = new ValueComparer<List<string>>(
                (c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()
            );

            // 7. CONFIGURATION DES DICTIONNAIRES (Dictionary<string,float> vers JSON)
            var dictConverter = new ValueConverter<Dictionary<string, float>, string>(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null) ?? "{}",
                v => JsonSerializer.Deserialize<Dictionary<string, float>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, float>()
            );

            var dictComparer = new ValueComparer<Dictionary<string, float>>(
                (c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.Count == c2.Count && !c1.Except(c2).Any()),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => new Dictionary<string, float>(c)
            );

            // Application aux champs List<string>
            modelBuilder.Entity<Role>()
                .Property(e => e.Permissions)
                .HasConversion(listConverter)
                .Metadata.SetValueComparer(listComparer);

            modelBuilder.Entity<Utilisateur>()
                .Property(e => e.Privileges)
                .HasConversion(listConverter)
                .Metadata.SetValueComparer(listComparer);

            modelBuilder.Entity<Question>()
                .Property(e => e.Choix)
                .HasConversion(listConverter)
                .Metadata.SetValueComparer(listComparer);

            modelBuilder.Entity<Question>()
                .Property(e => e.Prerequis)
                .HasConversion(listConverter)
                .Metadata.SetValueComparer(listComparer);

            // Application aux champs Dictionary<string, float>
            modelBuilder.Entity<Evaluation>()
                .Property(e => e.ScoresParTheme)
                .HasConversion(dictConverter)
                .Metadata.SetValueComparer(dictComparer);

            modelBuilder.Entity<Rapport>()
                .Property(r => r.ScoresParTheme)
                .HasConversion(dictConverter)
                .Metadata.SetValueComparer(dictComparer);

            modelBuilder.Entity<Rapport>()
                .Property(r => r.ScoresParNiveau)
                .HasConversion(dictConverter)
                .Metadata.SetValueComparer(dictComparer);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}