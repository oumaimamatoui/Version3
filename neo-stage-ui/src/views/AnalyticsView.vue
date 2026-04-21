<template>
  <div class="d-flex admin-body">
    <AppSidebar />
    
    <div class="content flex-grow-1 position-relative">
      <!-- COUCHES DÉCORATIVES PREMIUM -->
      <div class="background-overlay"></div>
      <div class="glow-orb orb-amber"></div>
      <div class="glow-orb orb-indigo"></div>
      <div class="tech-grid-subtle"></div>

      <AppNavbar />

      <main class="p-4 main-viewport animate-fade-in">
        
        <!-- 1. HEADER ANALYTIQUE & MASTER STATUS -->
        <div class="d-flex flex-wrap justify-content-between align-items-end mb-5 gap-3">
          <div class="header-content">
            <nav class="breadcrumb-cyber mb-2">MASTER_CONSOLE / INFRASTRUCTURE / ANALYTICS</nav>
            <h2 class="display-title-pro m-0">Analytique <span>Plateforme</span></h2>
            <p class="text-muted-pro small mt-1">Supervision globale des ressources et de la croissance de l'écosystème EvaluaTech.</p>
          </div>
          <div class="d-flex gap-3 align-items-center">
            <div class="system-status d-none d-lg-flex">
              <span class="status-dot pulse"></span>
              <span class="status-text tiny fw-800 uppercase">Système : <span class="text-success">OPÉRATIONNEL</span></span>
            </div>
            <button class="btn-primary-pro-sm shadow-amber">
              <i class="fa-solid fa-cloud-arrow-down me-2"></i> Exporter le bilan global
            </button>
          </div>
        </div>

        <!-- 2. KPI BENTO GRID (Vision Macro) -->
        <div class="row g-3 mb-5">
          <div class="col-md-3" v-for="stat in masterKpis" :key="stat.label">
            <div class="glass-card p-3 border-0 shadow-sm d-flex align-items-center gap-3">
              <div class="icon-vessel-v2" :style="{ background: stat.bg, color: stat.color }">
                <i :class="stat.icon"></i>
              </div>
              <div>
                <div class="fw-800 fs-4 text-navy">{{ stat.val }}</div>
                <div class="tiny fw-bold text-muted uppercase">{{ stat.label }}</div>
              </div>
            </div>
          </div>
        </div>

        <!-- 3. ANALYSE GRAPHIQUE -->
        <div class="row g-4 mb-4">
          <!-- Croissance (Histogramme Pro) -->
          <div class="col-lg-8">
            <div class="glass-surface p-4 h-100 shadow-sm border-0">
              <div class="d-flex justify-content-between align-items-center mb-5">
                <h6 class="fw-bold m-0 text-navy uppercase tiny ls-1"><i class="fa-solid fa-chart-line me-2 text-primary"></i>Croissance des Organisations (6 mois)</h6>
                <div class="chart-legend d-flex gap-3">
                  <span class="tiny fw-bold text-muted"><span class="dot-sm bg-primary"></span> Clients Actifs</span>
                </div>
              </div>
              
              <div class="chart-container-pro d-flex align-items-end gap-3 px-3">
                <div v-for="(item, i) in growthData" :key="i" class="chart-column-pro flex-grow-1">
                  <div class="bar-value tiny fw-bold text-muted">{{ item.count }}</div>
                  <div class="bar-fill-pro" :style="{ height: (item.count > 0 ? (item.count * 10 + 20) : 5) + '%' }">
                    <div class="bar-glow"></div>
                  </div>
                  <span class="bar-label-pro tiny fw-bold text-muted mt-3">{{ item.mois }}</span>
                </div>
              </div>
            </div>
          </div>
          
          <!-- Moniteur de Ressources -->
          <div class="col-lg-4">
            <div class="glass-surface p-4 h-100 shadow-sm border-0">
              <h6 class="fw-bold mb-4 uppercase tiny ls-1"><i class="fa-solid fa-server me-2 text-warning"></i>Santé Infrastructure</h6>
              
              <div class="resource-stack mt-4">
                <div class="resource-item mb-5" v-for="res in resources" :key="res.name">
                  <div class="d-flex justify-content-between align-items-end mb-2">
                    <div>
                      <span class="d-block fw-800 text-navy fs-5">{{ res.usage }}%</span>
                      <span class="tiny text-muted fw-bold uppercase">{{ res.name }}</span>
                    </div>
                    <i :class="res.icon + ' text-muted opacity-50 fs-5'"></i>
                  </div>
                  <div class="progress-track-pro">
                    <div class="progress-fill-pro" :class="res.class" :style="{ width: res.usage + '%' }"></div>
                  </div>
                  <div class="d-flex justify-content-between mt-2">
                    <span class="tiny text-success fw-bold" v-if="res.usage < 90">SÉCURISÉ</span>
                    <span class="tiny text-danger fw-bold pulse" v-else>CRITIQUE</span>
                    <span class="tiny text-muted">{{ res.limit }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- 4. ALERTES SYSTÈME RÉCENTES -->
        <div class="glass-surface p-4 border-start border-danger border-4 shadow-sm">
           <div class="d-flex align-items-center gap-3">
             <i class="fa-solid fa-shield-virus text-danger fs-4"></i>
             <div>
               <h6 class="m-0 fw-bold text-navy">Registre de Sécurité</h6>
               <p class="tiny m-0 text-muted fw-bold uppercase">Dernier scan effectué il y a 12 minutes : Aucune faille critique détectée.</p>
             </div>
           </div>
        </div>

      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import api from '@/services/api';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';

const loading = ref(true);
const stats = ref(null);

const masterKpis = ref([
  { label: 'Total Orgs', val: '...', icon: 'fa-solid fa-building', bg: '#eff6ff', color: '#3b82f6' },
  { label: 'Utilisateurs', val: '...', icon: 'fa-solid fa-users', bg: '#f5f3ff', color: '#8b5cf6' },
  { label: 'Tests IA', val: '...', icon: 'fa-solid fa-brain', bg: '#fffbeb', color: '#f59e0b' },
  { label: 'Uptime', val: '99.9%', icon: 'fa-solid fa-gauge-high', bg: '#ecfdf5', color: '#10b981' },
]);

const growthData = ref([]);

const resources = ref([
  { name: 'Charge API IA', usage: 0, limit: '0 Tokens', icon: 'fa-solid fa-microchip', class: 'bg-warning' },
  { name: 'Stockage Cloud', usage: 0, limit: '0 To / 0 To', icon: 'fa-solid fa-database', class: 'bg-primary' },
  { name: 'CPU Cluster', usage: 0, limit: 'Offline', icon: 'fa-solid fa-bolt', class: 'bg-success' }
]);

const formatNumber = (num) => {
  if (num >= 1000) return (num / 1000).toFixed(1) + 'k';
  return num.toString();
};

const fetchStats = async () => {
  loading.value = true;
  try {
    const res = await api.get('/SuperAdmin/stats');
    const data = res.data;
    stats.value = data;

    // Mise à jour des KPIs cardinaux
    masterKpis.value[0].val = formatNumber(data.totalEntreprises);
    masterKpis.value[1].val = formatNumber(data.totalUtilisateurs);
    masterKpis.value[2].val = formatNumber(data.totalTests);
    
    // Données de croissance
    growthData.value = data.croissanceStats || [];
  } catch (err) {
    console.error("Erreur lors de la récupération des stats:", err);
  } finally {
    loading.value = false;
  }
};

onMounted(fetchStats);
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;500;600;700;800&family=JetBrains+Mono:wght@400;700&display=swap');

/* --- THEME CORE & BACKGROUND --- */
.admin-body { min-height: 100vh; background-color: #f8fafc; font-family: 'Plus Jakarta Sans', sans-serif; overflow: hidden; }
.background-overlay { position: absolute; inset: 0; background: radial-gradient(circle at 30% 30%, #ffffff 0%, #f1f5f9 100%); z-index: 0; }
.tech-grid-subtle { position: absolute; inset: 0; background-image: radial-gradient(#e2e8f0 1px, transparent 1px); background-size: 40px 40px; opacity: 0.5; z-index: 1; }
.glow-orb { position: absolute; border-radius: 50%; filter: blur(120px); z-index: 1; opacity: 0.1; }
.orb-amber { width: 500px; height: 500px; background: #fbbf24; top: -10%; right: -5%; }
.orb-indigo { width: 400px; height: 400px; background: #4f46e5; bottom: -5%; left: -5%; }
.main-viewport { position: relative; z-index: 10; }

/* --- TYPOGRAPHY --- */
.display-title-pro { font-weight: 800; font-size: 32px; color: #0f172a; letter-spacing: -1.5px; }
.display-title-pro span { color: #3b82f6; }
.breadcrumb-cyber { font-family: 'JetBrains Mono'; font-size: 10px; color: #94a3b8; letter-spacing: 2px; }
.fw-800 { font-weight: 800; }
.tiny { font-size: 10px; }
.uppercase { text-transform: uppercase; }
.ls-1 { letter-spacing: 1px; }

/* --- GLASS SURFACES --- */
.glass-surface {
  background: rgba(255, 255, 255, 0.75);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(226, 232, 240, 0.8);
  border-radius: 24px;
}

/* --- ICON BOXES --- */
.icon-vessel-v2 { width: 44px; height: 44px; border-radius: 12px; display: flex; align-items: center; justify-content: center; font-size: 1.1rem; }

/* --- CHART HISTOGRAMME --- */
.chart-container-pro { height: 300px; }
.chart-column-pro {
  display: flex; flex-direction: column; align-items: center; justify-content: flex-end;
  height: 100%; position: relative;
}
.bar-fill-pro {
  width: 40px; border-radius: 10px 10px 4px 4px;
  background: linear-gradient(to top, #3b82f6, #6366f1);
  position: relative; transition: 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
}
.bar-fill-pro:hover { transform: scaleX(1.1); filter: brightness(1.1); }
.bar-glow { position: absolute; top: 0; left: 0; width: 100%; height: 30%; background: white; opacity: 0.2; border-radius: 10px 10px 0 0; }
.bar-value { margin-bottom: 8px; }

/* --- RESOURCE MONITOR --- */
.progress-track-pro { height: 10px; background: #f1f5f9; border-radius: 20px; overflow: hidden; border: 1px solid #e2e8f0; }
.progress-fill-pro { height: 100%; border-radius: 20px; transition: 1s ease; }
.bg-warning { background: linear-gradient(90deg, #f59e0b, #fbbf24); }
.bg-primary { background: linear-gradient(90deg, #3b82f6, #60a5fa); }
.bg-success { background: linear-gradient(90deg, #10b981, #34d399); }

/* --- BUTTONS & STATUS --- */
.btn-primary-pro-sm {
  background: #0f172a; color: white; border: none; padding: 10px 20px;
  border-radius: 12px; font-size: 12px; font-weight: 700; transition: 0.3s;
}
.btn-primary-pro-sm:hover { background: #eab308; color: #0f172a; transform: translateY(-2px); }

.status-dot { width: 8px; height: 8px; background: #10b981; border-radius: 50%; display: inline-block; margin-right: 8px; }
.pulse { animation: statusPulse 2s infinite; }
@keyframes statusPulse { 0% { box-shadow: 0 0 0 0 rgba(16, 185, 129, 0.4); } 70% { box-shadow: 0 0 0 10px rgba(16, 185, 129, 0); } 100% { box-shadow: 0 0 0 0 rgba(16, 185, 129, 0); } }

.dot-sm { width: 8px; height: 8px; border-radius: 50%; display: inline-block; margin-right: 4px; }
</style>