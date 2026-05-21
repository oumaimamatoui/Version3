import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5172/api',
 
  headers: { 'Content-Type': 'application/json' }
});

// Intercepteur pour porter le Token JWT
api.interceptors.request.use(config => {
  const token = localStorage.getItem('token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

// --- SERVICES SUPERADMIN ---
export const superAdminApi = {
  getStats: () => api.get('/SuperAdmin/stats'),
  getAuditLogs: () => api.get('/SuperAdmin/audit-logs'),
  clearAuditLogs: () => api.delete('/SuperAdmin/audit-logs'),
  getPendingRequests: () => api.get('/SuperAdmin/pending'),
  approveRequest: (id) => api.post(`/SuperAdmin/approve/${id}`),
  rejectRequest: (id) => api.post(`/SuperAdmin/reject/${id}`),
  createOrg: (data) => api.post('/SuperAdmin/create-org', data),
  getOrganizations: (params) => api.get('/SuperAdmin/organizations', { params }),
  deleteOrg: (id) => api.delete(`/SuperAdmin/organizations/${id}`),
  updateOrg: (id, data) => api.put(`/SuperAdmin/organizations/${id}`, data),

  // Gestion des Utilisateurs Plateforme
  getPlatformUsers: () => api.get('/SuperAdmin/users'),
  deleteUser: (id) => api.delete(`/SuperAdmin/users/${id}`),
  inviteAdmin: (data) => api.post('/SuperAdmin/invite-admin', data),
  toggleUserStatus: (id) => api.post(`/SuperAdmin/users/${id}/toggle-status`),

  // Gestion dynamique du Mailer System, Abonnements et Sécurité
  getMailerDiagnostics: () => api.get('/SuperAdmin/mailer-diagnostics'),
  retriggerMailer: () => api.post('/SuperAdmin/mailer-retrigger'),
  getExpiringSubscriptions: () => api.get('/SuperAdmin/expiring-subscriptions'),
  notifyRenewal: (id) => api.post(`/SuperAdmin/notify-renewal/${id}`),
  getSecurityStatus: () => api.get('/SuperAdmin/security-status'),
  runSecurityAudit: () => api.post('/SuperAdmin/run-security-audit')
};

// --- SERVICES ENTERPRISE ---
export const enterpriseApi = {
  getRecommendations: () => api.get('/Dashboard/enterprise-recommendations'),
  resolveAnomaly: (id) => api.post(`/Dashboard/resolve-anomaly/${id}`),
  publishDraft: (id) => api.post(`/Dashboard/publish-draft/${id}`),
  exportWeeklyReport: () => api.get('/Dashboard/export-weekly-report', { responseType: 'blob' })
};

export default api;