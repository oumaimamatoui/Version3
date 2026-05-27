public interface IQuotaService
{
    // Vérifie si l'entreprise peut effectuer l'action et incrémente le compteur
    Task<(bool Success, string Message, int Remaining)> ConsumeQuotaAsync(Guid entrepriseId);
}