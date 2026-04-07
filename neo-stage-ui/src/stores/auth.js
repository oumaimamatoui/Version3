import { defineStore } from 'pinia';

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: JSON.parse(localStorage.getItem('user')) || null,
    role: localStorage.getItem('role') || null,
    token: localStorage.getItem('token') || null,
  }),
  getters: {
    isAuthenticated: (state) => !!state.token,
    isAdmin: (state) => state.role === 'SuperAdmin',
  },
  actions: {
    setUser(user, role, token) {
      this.user = user;
      this.role = role;
      this.token = token;
      
      // Sauvegarde locale
      localStorage.setItem('user', JSON.stringify(user));
      localStorage.setItem('role', role);
      localStorage.setItem('token', token);
    },
    logout() {
      this.user = null;
      this.role = null;
      this.token = null;
      localStorage.removeItem('user');
      localStorage.removeItem('role');
      localStorage.removeItem('token');
    }
  }
});