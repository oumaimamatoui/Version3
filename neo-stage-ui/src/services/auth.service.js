import api from './api';

export default {
  async login(credentials) {
    const res = await api.post('/Auth/login', credentials);
    if (res.data.token) {
      localStorage.setItem('token', res.data.token);
      localStorage.setItem('role', res.data.role); // Contient "SuperAdmin", "AdminEntreprise"
      localStorage.setItem('nom', res.data.nom);
    }
    return res.data;
  },

  // Utilisé dans LandingPage.vue
  async sInscrire(data) {
    const payload = {
        nomEntreprise: data.nomEntreprise,
        nomResponsable: data.nomResponsable,
        emailResponsable: data.emailResponsable,
        matriculeFiscale: data.matriculeFiscale
    };
    return await api.post('/Registration', payload);
  },

  // Utilisé dans Activation.vue
  async activer(token, password) {
    return await api.post('/Activation/completer', { token, password });
  }
};