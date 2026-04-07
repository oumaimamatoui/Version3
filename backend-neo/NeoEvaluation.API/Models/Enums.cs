namespace NeoEvaluation.API.Models
{
    public enum DocumentType { DESCRIPTION_POSTE, SPECIFICATIONS_EXIGENCES, BRIEF_PROJET, AUTRE_REFERENCE }
    public enum QuestionType { QCM, TEXTE, CODE, TELEVERSEMENT_FICHIER }
    public enum CampaignStatus { BROUILLON, OUVERTE, FERMEE, ARCHIVEE }
    public enum ApplicationStatus { POSTULE, EN_COURS_DE_REVISION, PRESELECTIONNE, REJETE, EMBAUCHE }
    public enum EvaluationStatus { NON_COMMENCE, EN_COURS, TERMINE, EXPIRE }
    public enum CandidateDocType { CV, LETTRE_DE_MOTIVATION, PORTFOLIO, CERTIFICAT, AUTRE }
}