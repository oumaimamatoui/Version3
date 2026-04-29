using NeoEvaluation.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NeoEvaluation.API.Data;
using NeoEvaluation.API.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using SendGrid.Extensions.DependencyInjection; // Assurez-vous d'avoir installé le package

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURATION DES SERVICES ---

builder.Services.AddControllers()
    .AddJsonOptions(options => {
        // CRITIQUE : Force le format camelCase pour correspondre au JavaScript
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//  MODIFICATION : Configuration de SendGrid
builder.Services.AddSendGrid(options => {
    options.ApiKey = builder.Configuration["SendGridSettings:ApiKey"];
});

//  MODIFICATION : Utilisation de GmailApiService (Senior Architecture)
builder.Services.AddScoped<IEmailService, GmailApiService>();

builder.Services.AddSingleton<IAuditLogService, AuditLogService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// OPTIMISATION : Compression des réponses pour réduire la taille des transferts
builder.Services.AddResponseCompression(options => {
    options.EnableForHttps = true;
});

builder.Services.AddCors(options => {
    options.AddPolicy("VueCorsPolicy", policy => {
        policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173") 
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var jwtSecret = builder.Configuration["JwtSettings:SecretKey"] ?? "Cle_Tres_Longue_Pour_Securite_JWT_32_Chars_Minimum";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });

var app = builder.Build();

// --- 2. MIDDLEWARE ---

// S'assurer que le dossier d'upload existe
var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles");
if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("VueCorsPolicy");
app.UseResponseCompression();

// Fix for Google Auth COOP warnings
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Cross-Origin-Opener-Policy", "same-origin-allow-popups");
    await next();
});

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// --- 3. SEED DATA ---
using (var scope = app.Services.CreateScope()) {
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try {
        // Applique les migrations automatiquement
        context.Database.Migrate();

        // 1. Seed Roles
        var rolesToSeed = new List<Role> {
            new Role { 
                Nom = "SuperAdmin", 
                Description = "Administrateur Global",
                Permissions = new List<string> { "ALL" }
            },
            new Role { 
                Nom = "AdminEntreprise", 
                Description = "Administrateur d'organisation",
                Permissions = new List<string> { "MANAGE_USERS", "MANAGE_CAMPAIGNS", "VIEW_RESULTS", "MANAGE_BRANDING" }
            }
        };

        foreach (var r in rolesToSeed) {
            var existingRole = context.Roles.IgnoreQueryFilters().FirstOrDefault(x => x.Nom == r.Nom && x.EntrepriseId == null);
            if (existingRole == null) {
                context.Roles.Add(r);
            } else {
                existingRole.Permissions = r.Permissions; // Sync permissions
            }
        }
        
        // Remove old static roles Evaluateur and Candidat if they exist
        var oldEvaluateur = context.Roles.IgnoreQueryFilters().FirstOrDefault(x => x.Nom == "Evaluateur" && x.EntrepriseId == null);
        if (oldEvaluateur != null) context.Roles.Remove(oldEvaluateur);
        
        var oldCandidat = context.Roles.IgnoreQueryFilters().FirstOrDefault(x => x.Nom == "Candidat" && x.EntrepriseId == null);
        if (oldCandidat != null) context.Roles.Remove(oldCandidat);

        context.SaveChanges();

        // 2. Admin Seed
        if (!context.Utilisateurs.IgnoreQueryFilters().Any(u => u.Email == "admin@evaluatech.tn")) {
            context.Utilisateurs.Add(new Utilisateur {
                Id = Guid.NewGuid(),
                Email = "admin@evaluatech.tn", Prenom = "Admin", Nom = "Evaluatech",
                RoleNom = "SuperAdmin", 
                EstActif = true, CreeLe = DateTime.UtcNow,
                MotDePasseHash = BCrypt.Net.BCrypt.HashPassword("Admin123"),
                Privileges = new List<string> { "ALL" }
            });
        }


        // 3. Questionnaire Seed
        if (!context.Questionnaires.Any()) {
            context.Questionnaires.Add(new Questionnaire {
                Id = Guid.NewGuid(),
                Titre = "Examen de Certification Fullstack",
                Description = "Test de compétences globales pour développeurs web."
            });
        }

        context.SaveChanges();
    } catch (Exception ex) { Console.WriteLine($"--> [SEED ERROR] {ex.Message}"); }
}

app.Run();