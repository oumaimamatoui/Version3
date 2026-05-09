import { defineStore } from 'pinia'
import * as signalR from '@microsoft/signalr'
import { useAuthStore } from '@/stores/auth'

export const useNotificationStore = defineStore('notification', {
  state: () => ({
    notifications: [],
    connection: null,
    connected: false,
  }),

  getters: {
    unreadCount: (state) => state.notifications.filter(n => !n.read).length,
    sorted: (state) => [...state.notifications].sort(
      (a, b) => new Date(b.createdAt) - new Date(a.createdAt)
    ),
  },

  actions: {
    // ─── CONNEXION SIGNALR ───────────────────────────────────────
    async connect() {
      if (this.connection) return

      const authStore = useAuthStore()
      const token = authStore.token  // adaptez selon votre store auth

      this.connection = new signalR.HubConnectionBuilder()
        .withUrl('http://localhost:5172/hubs/notifications', {
          accessTokenFactory: () => token,
        })
        .withAutomaticReconnect([0, 2000, 5000, 10000])
        .configureLogging(signalR.LogLevel.Warning)
        .build()

      // Réception d'une notification du serveur
      this.connection.on('ReceiveNotification', (payload) => {
        this.add(payload)
        this.playSound(payload.type)
      })

      this.connection.onreconnecting(() => { this.connected = false })
      this.connection.onreconnected(() => { this.connected = true })
      this.connection.onclose(() => { this.connected = false })

      try {
        await this.connection.start()
        this.connected = true
      } catch (err) {
        console.error('[SignalR] Connexion échouée :', err)
      }
    },

    async disconnect() {
      if (this.connection) {
        await this.connection.stop()
        this.connection = null
        this.connected = false
      }
    },

    // ─── GESTION DES NOTIFICATIONS ───────────────────────────────
    add(payload) {
      this.notifications.unshift({
        id: payload.id ?? crypto.randomUUID(),
        type: payload.type ?? 'info',
        title: payload.title ?? 'Notification',
        message: payload.message ?? '',
        link: payload.link ?? null,
        createdAt: payload.createdAt ? new Date(payload.createdAt) : new Date(),
        read: false,
      })

      // Garder 50 notifications max
      if (this.notifications.length > 50)
        this.notifications = this.notifications.slice(0, 50)
    },

    markAsRead(id) {
      const n = this.notifications.find(n => n.id === id)
      if (n) n.read = true
    },

    markAllAsRead() {
      this.notifications.forEach(n => (n.read = true))
    },

    remove(id) {
      this.notifications = this.notifications.filter(n => n.id !== id)
    },

    clearAll() {
      this.notifications = []
    },

    playSound(type) {
      // Son discret (optionnel)
      try {
        const ctx = new AudioContext()
        const osc = ctx.createOscillator()
        const gain = ctx.createGain()
        osc.connect(gain)
        gain.connect(ctx.destination)
        osc.frequency.value = type === 'alert' ? 440 : 880
        gain.gain.setValueAtTime(0.08, ctx.currentTime)
        gain.gain.exponentialRampToValueAtTime(0.001, ctx.currentTime + 0.3)
        osc.start(ctx.currentTime)
        osc.stop(ctx.currentTime + 0.3)
      } catch {}
    },
  },
})