<template>
  <div class="notif-wrapper" ref="wrapper">
    <!-- Cloche -->
    <button class="bell-btn" @click="toggle" :class="{ active: open }">
      <i class="ti ti-bell" aria-hidden="true"></i>
      <span v-if="store.unreadCount > 0" class="badge">
        {{ store.unreadCount > 99 ? '99+' : store.unreadCount }}
      </span>
    </button>

    <!-- Indicateur connexion -->
    <span class="conn-dot" :class="store.connected ? 'online' : 'offline'"
          :title="store.connected ? 'Connecté' : 'Déconnecté'"></span>

    <!-- Panneau -->
    <Transition name="panel">
      <div v-if="open" class="notif-panel">
        <div class="panel-header">
          <span class="panel-title">Notifications</span>
          <div class="panel-actions">
            <button v-if="store.unreadCount" @click="store.markAllAsRead()" class="btn-text">
              Tout lire
            </button>
            <button v-if="store.notifications.length" @click="store.clearAll()" class="btn-text danger">
              Effacer
            </button>
          </div>
        </div>

        <div class="notif-list" v-if="store.sorted.length">
          <TransitionGroup name="item">
            <div
              v-for="n in store.sorted"
              :key="n.id"
              class="notif-item"
              :class="[`type-${n.type}`, { unread: !n.read }]"
              @click="handleClick(n)"
            >
              <span class="notif-icon" aria-hidden="true">
                <i :class="iconFor(n.type)"></i>
              </span>
              <div class="notif-body">
                <p class="notif-title">{{ n.title }}</p>
                <p class="notif-msg">{{ n.message }}</p>
                <time class="notif-time">{{ timeAgo(n.createdAt) }}</time>
              </div>
              <button class="notif-close" @click.stop="store.remove(n.id)" aria-label="Fermer">
                <i class="ti ti-x"></i>
              </button>
            </div>
          </TransitionGroup>
        </div>

        <div v-else class="notif-empty">
          <i class="ti ti-bell-off" aria-hidden="true"></i>
          <p>Aucune notification</p>
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useNotificationStore } from '@/stores/notification'

const store = useNotificationStore()
const router = useRouter()
const open = ref(false)
const wrapper = ref(null)

function toggle() { open.value = !open.value }

function handleClick(n) {
  store.markAsRead(n.id)
  if (n.link) router.push(n.link)
  open.value = false
}

function iconFor(type) {
  const icons = {
    success: 'ti ti-circle-check',
    warning: 'ti ti-alert-triangle',
    alert:   'ti ti-alert-circle',
    info:    'ti ti-info-circle',
  }
  return icons[type] ?? icons.info
}

function timeAgo(date) {
  const diff = Math.floor((Date.now() - new Date(date)) / 1000)
  if (diff < 60) return "À l'instant"
  if (diff < 3600) return `Il y a ${Math.floor(diff / 60)} min`
  if (diff < 86400) return `Il y a ${Math.floor(diff / 3600)} h`
  return `Il y a ${Math.floor(diff / 86400)} j`
}

// Fermer si clic extérieur
function onClickOutside(e) {
  if (wrapper.value && !wrapper.value.contains(e.target)) open.value = false
}
onMounted(() => document.addEventListener('mousedown', onClickOutside))
onUnmounted(() => document.removeEventListener('mousedown', onClickOutside))
</script>

<style scoped>
.notif-wrapper { position: relative; display: inline-flex; align-items: center; }

.bell-btn {
  position: relative; background: none; border: none; cursor: pointer;
  color: var(--color-text-secondary); font-size: 20px; padding: 6px; border-radius: 8px;
  transition: color .2s, background .2s;
}
.bell-btn:hover, .bell-btn.active { color: var(--color-text-primary); background: var(--color-background-secondary); }

.badge {
  position: absolute; top: 2px; right: 2px;
  background: #e24b4a; color: #fff; border-radius: 10px;
  font-size: 10px; font-weight: 500; min-width: 16px; height: 16px;
  padding: 0 4px; display: flex; align-items: center; justify-content: center;
  border: 2px solid var(--color-background-primary);
}

.conn-dot {
  width: 7px; height: 7px; border-radius: 50%;
  margin-left: -6px; margin-top: -14px; border: 1.5px solid var(--color-background-primary);
}
.conn-dot.online { background: #1d9e75; }
.conn-dot.offline { background: #888; }

/* Panel */
.notif-panel {
  position: absolute; top: calc(100% + 10px); right: 0;
  width: 340px; max-height: 480px;
  background: var(--color-background-primary);
  border: 1px solid var(--color-border-secondary);
  border-radius: 12px; box-shadow: 0 8px 24px rgba(0,0,0,.12);
  display: flex; flex-direction: column; overflow: hidden; z-index: 1000;
}

.panel-header {
  display: flex; align-items: center; justify-content: space-between;
  padding: 14px 16px 10px; border-bottom: 1px solid var(--color-border-tertiary);
}
.panel-title { font-size: 14px; font-weight: 500; color: var(--color-text-primary); }
.panel-actions { display: flex; gap: 8px; }
.btn-text {
  background: none; border: none; cursor: pointer; font-size: 12px;
  color: var(--color-text-secondary); padding: 2px 6px; border-radius: 4px;
  transition: color .2s, background .2s;
}
.btn-text:hover { background: var(--color-background-secondary); color: var(--color-text-primary); }
.btn-text.danger:hover { color: #e24b4a; }

.notif-list { overflow-y: auto; flex: 1; }

.notif-item {
  display: flex; align-items: flex-start; gap: 10px;
  padding: 12px 14px; cursor: pointer;
  border-bottom: 1px solid var(--color-border-tertiary);
  transition: background .15s; position: relative;
}
.notif-item:hover { background: var(--color-background-secondary); }
.notif-item.unread { background: var(--color-background-info); }
.notif-item.unread:hover { background: var(--color-background-secondary); }

/* Barre colorée gauche */
.notif-item::before {
  content: ''; position: absolute; left: 0; top: 0; bottom: 0;
  width: 3px; border-radius: 0 2px 2px 0;
}
.notif-item.type-success::before { background: #1d9e75; }
.notif-item.type-warning::before { background: #ef9f27; }
.notif-item.type-alert::before   { background: #e24b4a; }
.notif-item.type-info::before    { background: #378add; }

.notif-icon { font-size: 18px; margin-top: 2px; flex-shrink: 0; }
.type-success .notif-icon { color: #1d9e75; }
.type-warning .notif-icon { color: #ef9f27; }
.type-alert   .notif-icon { color: #e24b4a; }
.type-info    .notif-icon { color: #378add; }

.notif-body { flex: 1; min-width: 0; }
.notif-title { font-size: 13px; font-weight: 500; color: var(--color-text-primary); margin: 0 0 2px; }
.notif-msg   { font-size: 12px; color: var(--color-text-secondary); margin: 0 0 4px; line-height: 1.4; }
.notif-time  { font-size: 11px; color: var(--color-text-tertiary); }

.notif-close {
  background: none; border: none; cursor: pointer; color: var(--color-text-tertiary);
  font-size: 14px; padding: 2px; opacity: 0; transition: opacity .15s;
}
.notif-item:hover .notif-close { opacity: 1; }

.notif-empty {
  display: flex; flex-direction: column; align-items: center; justify-content: center;
  padding: 40px 20px; color: var(--color-text-tertiary); gap: 8px;
}
.notif-empty i { font-size: 32px; }
.notif-empty p { font-size: 13px; margin: 0; }

/* Transitions */
.panel-enter-active, .panel-leave-active { transition: opacity .15s, transform .15s; }
.panel-enter-from, .panel-leave-to { opacity: 0; transform: translateY(-6px); }

.item-enter-active { transition: all .25s ease; }
.item-enter-from   { opacity: 0; transform: translateX(-10px); }
.item-leave-active { transition: all .2s ease; position: absolute; width: 100%; }
.item-leave-to     { opacity: 0; transform: translateX(10px); }
</style>