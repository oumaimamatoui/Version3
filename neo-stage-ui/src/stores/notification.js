import { defineStore } from 'pinia';

export const useNotificationStore = defineStore('notification', {
  state: () => ({
    notifications: [
      { id: 1, text: "Nouveau candidat inscrit", time: "10 min", read: false, type: 'info' },
      { id: 2, text: "Tentative de triche détectée !", time: "1h", read: false, type: 'alert' }
    ]
  }),
  getters: {
    unreadCount: (state) => state.notifications.filter(n => !n.read).length
  },
  actions: {
    add(msg) {
      this.notifications.unshift({ id: Date.now(), text: msg, time: "À l'instant", read: false });
    },
    markAllAsRead() {
      this.notifications.forEach(n => n.read = true);
    }
  }
});