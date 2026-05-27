import { defineStore } from 'pinia';

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: JSON.parse(localStorage.getItem('user')) || null,
    role: localStorage.getItem('role') || null,
    token: localStorage.getItem('token') || null,
    privileges: JSON.parse(localStorage.getItem('privileges')) || [],
  }),
  getters: {
    isAuthenticated: (state) => !!state.token,
    isAdmin: (state) => state.role === 'SuperAdmin',
    hasPermission: (state) => (permission) => {
      // SuperAdmin et AdminEntreprise ont toujours tous les droits
      if (state.role === 'SuperAdmin' || state.role === 'AdminEntreprise') return true;
      return state.privileges.includes(permission);
    }
  },
  actions: {
    setUser(user, role, token, privileges = []) {
      this.user = user;
      this.role = role;
      this.token = token;
      this.privileges = privileges;
      
      // Sauvegarde locale
      localStorage.setItem('user', JSON.stringify(user));
      localStorage.setItem('role', role);
      localStorage.setItem('token', token);
      localStorage.setItem('privileges', JSON.stringify(privileges));
    },
    logout() {
      this.user = null;
      this.role = null;
      this.token = null;
      this.privileges = [];
      localStorage.removeItem('user');
      localStorage.removeItem('role');
      localStorage.removeItem('token');
      localStorage.removeItem('privileges');
    }
  }
});