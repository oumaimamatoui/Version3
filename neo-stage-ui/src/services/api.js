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
  
  // NOUVEAUX : Gestion des Utilisateurs Plateforme
  getPlatformUsers: () => api.get('/SuperAdmin/users'),
  deleteUser: (id) => api.delete(`/SuperAdmin/users/${id}`),
  inviteAdmin: (data) => api.post('/SuperAdmin/invite-admin', data),
  toggleUserStatus: (id) => api.post(`/SuperAdmin/users/${id}/toggle-status`)
};

export default api;