namespace NeoEvaluation.API.Models
{
    public enum TypeQuestion { QCU, QCM, VRAI_FAUX, LIBRE, CODE }
    public enum StatutCampagne { BROUILLON, PLANIFIEE, EN_COURS, TERMINEE, ARCHIVEE }
    public enum ApplicationStatus { POSTULE, EN_COURS_DE_REVISION, PRESELECTIONNE, REJETE, EMBAUCHE }
    public enum StatutPassage { NON_COMMENCE, EN_COURS, SUSPENDU, TERMINE, EXPIRE }
    public enum CandidateDocType { CV, LETTRE_DE_MOTIVATION, PORTFOLIO, CERTIFICAT, AUTRE }
    public enum NiveauComplexite { DEBUTANT, INTERMEDIAIRE, AVANCE, EXPERT }
    public enum TypeRapport { PAR_CANDIDAT, PAR_CAMPAGNE, PAR_THEME }
    public enum RoleValues { SUPER_ADMIN, ADMIN_ENTREPRISE, FORMATEUR, RECRUTEUR, CANDIDAT }
}