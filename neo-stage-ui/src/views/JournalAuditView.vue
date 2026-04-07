<template>
  <div class="admin-dashboard-container">
    <!-- COUCHES D'ARRIÈRE-PLAN (Style Signature) -->
    <div class="background-overlay"></div>
    <div class="glow-orb orb-amber"></div>
    <div class="glow-orb orb-indigo"></div>
    <div class="tech-grid-subtle"></div>

    <div class="d-flex position-relative z-index-10">
      <AppSidebar />

      <div class="content flex-grow-1">
        <AppNavbar />

        <div class="p-4 pt-0 main-viewport animate__animated animate__fadeIn">
          
          <!-- HEADER HUD -->
          <div class="d-flex justify-content-between align-items-end mb-5 mt-4">
            <div class="header-content">
              <div class="badge-tech mb-2">
                <span class="pulse-dot-red"></span> PROTOCOLE DE SÉCURITÉ ACTIF
              </div>
              <h1 class="display-title">Journal <span>d'Audit</span></h1>
              <p class="brand-subtitle text-uppercase">Registre des événements et intégrité du système</p>
            </div>
            
              <div class="system-health d-none d-md-flex align-items-center gap-3">
                <div class="health-card">
                  <span class="label">ÉTAT DU NOYAU</span>
                  <span class="val text-success">SÉCURISÉ</span>
                </div>
                <button @click="fetchLogs" class="btn-primary-gradient px-3 py-2 small me-2">
                  <i class="fa-solid fa-rotate me-1"></i> ACTUALISER
                </button>
                <button @click="handleClearLogs" class="btn btn-outline-danger btn-sm px-3 py-2">
                  <i class="fa-solid fa-trash-can me-1"></i> EFFACER
                </button>
              </div>
          </div>

          <!-- LOGS CONTAINER (Style Cyber-Terminal) -->
          <div class="glass-surface overflow-hidden shadow-lg border-red-soft">
            <div class="terminal-header d-flex justify-content-between align-items-center p-4 border-bottom border-light">
              <h6 class="label-heading m-0"><i class="fa-solid fa-shield-halved text-danger me-2"></i>FLUX DE DONNÉES EN TEMPS RÉEL</h6>
              <div class="d-flex gap-2">
                <span class="status-pill-small">Uptime: 99.9%</span>
                <span class="status-pill-small">Filtrage: Actif</span>
              </div>
            </div>

            <div class="log-viewport custom-scrollbar">
              <!-- Scanner line animation inside the log list -->
              <div class="scanner-line"></div>

              <div v-for="(log, index) in logs" :key="log.id" 
                   class="log-entry animate-staggered" 
                   :style="{'--delay': index * 0.1 + 's'}">
                
                <div class="log-indicator">
                  <div :class="['indicator-dot', log.type === 'alert' ? 'bg-danger' : 'bg-indigo']"></div>
                  <div class="indicator-line"></div>
                </div>

                <div class="log-content flex-grow-1">
                  <div class="d-flex justify-content-between align-items-center mb-1">
                    <div class="d-flex align-items-center gap-2">
                      <span class="user-id">@{{ log.utilisateur?.toLowerCase() }}</span>
                      <span :class="['type-tag', log.statut === 'Succès' ? 'tag-info' : 'tag-alert']">
                        {{ log.statut?.toUpperCase() }}
                      </span>
                    </div>
                    <span class="timestamp-tech font-mono">{{ formatDate(log.horodatage) }}</span>
                  </div>
                  
                  <p class="action-text m-0 text-slate-700 fw-bold">{{ log.action }}</p>
                  <p class="tiny text-muted m-0">{{ log.details }}</p>
                  
                  <div class="mt-2 d-flex gap-3 align-items-center">
                    <span class="metadata-badge"><i class="fa-solid fa-network-wired me-1"></i> IPv4: {{ log.ip }}</span>
                    <span class="metadata-badge"><i class="fa-solid fa-fingerprint me-1"></i> ID: {{ log.id?.substring(0,8) }}</span>
                  </div>
                </div>

                <div class="log-action">
                  <button class="btn-inspect" title="Inspecter le paquet">
                    <i class="fa-solid fa-magnifying-glass-plus"></i>
                  </button>
                </div>
              </div>
            </div>

            <!-- TERMINAL FOOTER -->
            <div class="p-3 bg-light-subtle text-center">
              <button class="btn-link-tech">CHARGER LES ARCHIVES PRÉCÉDENTES <i class="fa-solid fa-chevron-down ms-2"></i></button>
            </div>
          </div>

          <!-- SUMMARY CARDS -->
          <div class="row g-4 mt-2 pb-5">
             <div class="col-md-4">
                <div class="glass-surface p-3 d-flex align-items-center gap-3">
                  <div class="mini-icon-box bg-danger-soft text-danger"><i class="fa-solid fa-triangle-exclamation"></i></div>
                  <div><span class="d-block fw-800 text-dark">02</span> <span class="tiny-label">ALERTES CRITIQUES</span></div>
                </div>
             </div>
             <div class="col-md-4">
                <div class="glass-surface p-3 d-flex align-items-center gap-3">
                  <div class="mini-icon-box bg-indigo-soft text-indigo"><i class="fa-solid fa-user-check"></i></div>
                  <div><span class="d-block fw-800 text-dark">145</span> <span class="tiny-label">ACCÈS VALIDÉS</span></div>
                </div>
             </div>
             <div class="col-md-4">
                <div class="glass-surface p-3 d-flex align-items-center gap-3">
                  <div class="mini-icon-box bg-amber-soft text-amber"><i class="fa-solid fa-clock-rotate-left"></i></div>
                  <div><span class="d-block fw-800 text-dark">24h</span> <span class="tiny-label">SANS INCIDENT</span></div>
                </div>
             </div>
          </div>

        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';
import { superAdminApi } from '@/services/api';
import { useToast } from 'vue-toastification';

const toast = useToast();
const logs = ref([]);
const loading = ref(false);

const fetchLogs = async () => {
  loading.ref = true;
  try {
    const res = await superAdminApi.getAuditLogs();
    logs.value = res.data;
  } catch (err) {
    console.error("Erreur logs:", err);
  } finally {
    loading.value = false;
  }
};

const handleClearLogs = async () => {
    if(confirm("Voulez-vous vraiment effacer tout le journal d'audit ?")) {
        try {
            await superAdminApi.clearAuditLogs();
            toast.success("Journal effacé.");
            fetchLogs();
        } catch (e) { toast.error("Erreur lors de la suppression."); }
    }
};

const formatDate = (date) => new Date(date).toLocaleTimeString('fr-FR', { hour: '2-digit', minute: '2-digit', second: '2-digit' });

onMounted(() => {
  fetchLogs();
});
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;600;700;800&family=JetBrains+Mono:wght@500;800&display=swap');

.admin-dashboard-container {
  font-family: 'Plus Jakarta Sans', sans-serif;
  min-height: 100vh;
  background-color: #f8fafc;
  position: relative;
  overflow-x: hidden;
}

/* --- BACKGROUND --- */
.background-overlay { position: absolute; inset: 0; background: radial-gradient(circle at 30% 30%, #ffffff 0%, #f1f5f9 100%); z-index: 0; }
.tech-grid-subtle { position: absolute; inset: 0; background-image: linear-gradient(rgba(148, 163, 184, 0.08) 1px, transparent 1px), linear-gradient(90deg, rgba(148, 163, 184, 0.08) 1px, transparent 1px); background-size: 45px 45px; z-index: 1; }
.glow-orb { position: absolute; border-radius: 50%; filter: blur(120px); z-index: 1; opacity: 0.15; }
.orb-amber { width: 400px; height: 400px; background: #fbbf24; top: -10%; right: -5%; }
.orb-indigo { width: 500px; height: 500px; background: #4f46e5; bottom: -10%; left: -5%; }
.z-index-10 { z-index: 10; }

/* --- TYPOGRAPHY --- */
.display-title { font-weight: 800; color: #0f172a; font-size: 42px; margin: 0; letter-spacing: -2px; }
.display-title span { color: #eab308; }
.brand-subtitle { color: #94a3b8; font-size: 11px; font-weight: 700; letter-spacing: 2px; }
.label-heading { font-size: 12px; font-weight: 800; letter-spacing: 0.5px; text-transform: uppercase; }

/* --- GLASS SURFACES --- */
.glass-surface {
  background: rgba(255, 255, 255, 0.8);
  backdrop-filter: blur(12px);
  border: 1px solid rgba(226, 232, 240, 0.8);
  border-radius: 24px;
}
.border-red-soft { border-top: 4px solid #ef4444; }

/* --- TERMINAL LOGS --- */
.log-viewport { max-height: 500px; overflow-y: auto; position: relative; }
.log-entry {
  display: flex; gap: 20px; padding: 20px 25px;
  border-bottom: 1px solid #f1f5f9;
  transition: all 0.3s ease;
  animation: slideIn 0.5s ease backwards;
  animation-delay: var(--delay);
}
.log-entry:hover { background: rgba(234, 179, 8, 0.03); transform: translateX(5px); }

.log-indicator { display: flex; flex-direction: column; align-items: center; padding-top: 5px; }
.indicator-dot { width: 8px; height: 8px; border-radius: 50%; }
.indicator-line { width: 2px; flex-grow: 1; background: #f1f5f9; margin-top: 5px; }

.user-id { font-weight: 800; color: #0f172a; font-size: 13px; }
.timestamp-tech { font-size: 11px; color: #94a3b8; }
.action-text { font-size: 14px; font-weight: 500; }

.type-tag { font-size: 9px; font-weight: 800; padding: 2px 8px; border-radius: 4px; }
.tag-info { background: #eef2ff; color: #4f46e5; }
.tag-alert { background: #fef2f2; color: #ef4444; }

.metadata-badge { font-size: 10px; font-weight: 700; color: #94a3b8; background: #f8fafc; padding: 3px 10px; border-radius: 100px; border: 1px solid #e2e8f0; }

/* --- ANIMATIONS --- */
.scanner-line {
  position: absolute; left: 0; top: 0; width: 100%; height: 2px;
  background: linear-gradient(90deg, transparent, #ef4444, transparent);
  animation: scan 4s linear infinite;
  z-index: 5; opacity: 0.3;
}
@keyframes scan { 0% { top: 0; } 100% { top: 100%; } }
@keyframes slideIn { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }

/* --- BUTTONS & PILLS --- */
.btn-primary-gradient {
  background: linear-gradient(135deg, #eab308 0%, #facc15 100%);
  color: #fff; border: none; border-radius: 10px;
  font-weight: 800; box-shadow: 0 4px 12px rgba(234, 179, 8, 0.2);
}
.btn-inspect { background: #f8fafc; border: 1px solid #e2e8f0; color: #94a3b8; width: 35px; height: 35px; border-radius: 10px; transition: 0.2s; }
.btn-inspect:hover { background: #0f172a; color: #fff; }

.health-card { background: white; border: 1px solid #e2e8f0; padding: 5px 15px; border-radius: 10px; text-align: center; }
.health-card .label { font-size: 8px; color: #94a3b8; font-weight: 800; display: block; }
.health-card .val { font-size: 12px; font-weight: 800; }

.status-pill-small { background: #f1f5f9; color: #64748b; font-size: 9px; font-weight: 800; padding: 4px 10px; border-radius: 5px; }

.badge-tech { display: inline-flex; align-items: center; gap: 8px; background: white; padding: 4px 12px; border-radius: 100px; font-size: 9px; font-weight: 800; border: 1px solid #e2e8f0; color: #64748b; }
.pulse-dot-red { width: 6px; height: 6px; background: #ef4444; border-radius: 50%; animation: blink 1s infinite; }
@keyframes blink { 0%, 100% { opacity: 1; } 50% { opacity: 0.3; } }

.mini-icon-box { width: 40px; height: 40px; border-radius: 10px; display: flex; align-items: center; justify-content: center; font-size: 18px; }
.bg-danger-soft { background: #fef2f2; }
.bg-indigo-soft { background: #eef2ff; }
.bg-amber-soft { background: #fffbeb; }

.font-mono { font-family: 'JetBrains Mono', monospace; }
.fw-800 { font-weight: 800; }
.tiny-label { font-size: 9px; font-weight: 800; color: #94a3b8; }
.btn-link-tech { background: none; border: none; color: #3b82f6; font-size: 11px; font-weight: 800; }
</style>