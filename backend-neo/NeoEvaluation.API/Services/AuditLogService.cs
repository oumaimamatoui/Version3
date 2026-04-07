using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using NeoEvaluation.API.DTOs;

namespace NeoEvaluation.API.Services
{
    public interface IAuditLogService
    {
        Task LogActionAsync(string action, string user, string details, string status = "Succès");
        Task<List<AuditLogEntry>> GetLogsAsync();
        Task ClearLogsAsync();
    }

    public class AuditLogService : IAuditLogService
    {
        private readonly string _logFilePath;

        public AuditLogService()
        {
            _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "audit_logs.json");
        }

        public async Task LogActionAsync(string action, string user, string details, string status = "Succès")
        {
            var logs = await GetLogsAsync();
            logs.Insert(0, new AuditLogEntry 
            { 
                Action = action, 
                Utilisateur = user, 
                Details = details, 
                Statut = status 
            });

            // Garder seulement les 500 derniers logs
            if (logs.Count > 500) logs = logs.GetRange(0, 500);

            var json = JsonSerializer.Serialize(logs, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_logFilePath, json);
        }

        public async Task<List<AuditLogEntry>> GetLogsAsync()
        {
            if (!File.Exists(_logFilePath)) return new List<AuditLogEntry>();

            try
            {
                var json = await File.ReadAllTextAsync(_logFilePath);
                return JsonSerializer.Deserialize<List<AuditLogEntry>>(json) ?? new List<AuditLogEntry>();
            }
            catch
            {
                return new List<AuditLogEntry>();
            }
        }

        public async Task ClearLogsAsync()
        {
            if (File.Exists(_logFilePath))
            {
                await File.WriteAllTextAsync(_logFilePath, "[]");
            }
        }
    }
}
