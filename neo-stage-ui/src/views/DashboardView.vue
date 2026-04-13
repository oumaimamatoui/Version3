<template>
  <div class="admin-layout">
    <!-- COUCHES DÉCORATIVES -->
    <div class="background-overlay"></div>
    <div class="glow-orb orb-amber"></div>
    <div class="glow-orb orb-blue"></div>
    <div class="tech-grid-subtle"></div>

    <AppSidebar />
    
    <div class="main-viewport animate__animated animate__fadeIn">
      <AppNavbar />

      <div class="container-fluid px-4 pt-3">
        
        <!-- 1. TERMINAL STATUS BAR -->
        <div class="terminal-status-bar mb-4">
          <div class="d-flex justify-content-between align-items-center px-3">
            <div class="breadcrumb-cyber">
              <span class="root">EVALUATECH</span>
              <span class="sep">//</span>
              <span class="current">{{ roleLabel }}</span>
            </div>
            <div class="system-tag">
              <span class="pulse-dot"></span>
              <span class="tag-text">LOGGED_AS: {{ authStore.user?.name.toUpperCase() }}</span>
            </div>
          </div>
        </div>

        <!-- 2. HERO SECTION -->
        <div class="hero-cyber-card mb-5">
          <div class="row align-items-center">
            <div class="col-lg-8 p-5">
              <div class="badge-amber-glow mb-3">
                <i class="fa-solid fa-shield-halved me-2"></i> SESSION SÉCURISÉE : {{ roleContext }}
              </div>
              <h1 class="display-title-cyber">
                Ravi de vous revoir, <br>
                <span class="text-amber">{{ authStore.user?.name || 'Utilisateur' }}</span>
              </h1>
              <div class="ai-insight-box mt-4">
                <div class="ai-box-icon">
                  <i class="fa-solid fa-microchip"></i>
                </div>
                <p class="ai-insight-text">
                  <strong>SYSTÈME IA :</strong> {{ aiInsight }}
                </p>
              </div>
            </div>
            
            <div class="col-lg-4 text-center d-none d-lg-block">
              <div class="ai-robot-dashboard">
                <div class="robot-float">
                  <svg class="modern-robot" viewBox="0 0 200 200" width="180">
                    <rect x="55" y="50" width="90" height="85" rx="28" fill="#fff" stroke="#e2e8f0" stroke-width="2"/>
                    <rect x="65" y="65" width="70" height="32" rx="16" fill="#1e293b" />
                    <circle cx="85" cy="81" r="5" fill="#eab308" />
                    <circle cx="115" cy="81" r="5" fill="#eab308" />
                  </svg>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- 3. KPI BENTO GRID -->
        <div class="row g-4 mb-5">
          <div class="col-md-6 col-xl-3" v-for="stat in dynamicStats" :key="stat.label">
            <div class="kpi-cyber-card">
              <div class="d-flex justify-content-between align-items-start">
                <div class="kpi-icon-box" :style="{ color: stat.color, backgroundColor: stat.bg }">
                  <i :class="stat.icon"></i>
                </div>
              </div>
              <div class="mt-4">
                <h2 class="kpi-value">{{ stat.value }}</h2>
                <span class="kpi-label">{{ stat.label }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- 4. SECTION CANDIDATS : LISTE DES CAMPAGNES -->
        <div v-if="userRole === 'Candidat'" class="campaign-candidate-section mb-5">
          <div class="d-flex justify-content-between align-items-center mb-4">
            <h4 class="section-title-cyber"><i class="fa-solid fa-layer-group text-amber me-2"></i> VOS ÉVALUATIONS DISPONIBLES</h4>
            <span class="badge bg-dark rounded-pill">{{ filteredCampaigns.length }} Tests Actifs</span>
          </div>

          <div v-if="loading" class="text-center py-5">
            <div class="spinner-pro-premium"></div>
            <p class="mt-2 text-muted fw-bold">Synchronisation des tests...</p>
          </div>

          <div v-else class="row g-4">
            <div v-if="filteredCampaigns.length === 0" class="col-12 text-center py-5 glass-panel">
              <i class="fa-solid fa-folder-open fa-3x text-muted mb-3"></i>
              <p class="text-muted fw-bold">Aucune évaluation n'est disponible pour le moment.</p>
            </div>

            <div v-for="c in filteredCampaigns" :key="c.id" class="col-md-6 col-xl-4">
              <div class="campaign-card-candidate">
                <div class="card-status-strip active"></div>
                <div class="p-4">
                  <div class="d-flex justify-content-between mb-3">
                    <span class="badge-tech">{{ c.categorie || 'TECHNIQUE' }}</span>
                    <span class="time-limit"><i class="fa-regular fa-clock me-1"></i> {{ c.dureeMinutes }} min</span>
                  </div>
                  <h5 class="campaign-name">{{ c.nom }}</h5>
                  <p class="campaign-desc text-muted small">{{ c.description || 'Évaluation des compétences techniques.' }}</p>
                  
                  <div class="campaign-meta-grid mb-4">
                    <div class="meta-item">
                      <span class="meta-label">SCORE MIN.</span>
                      <span class="meta-value text-amber">{{ c.scorePassage }}%</span>
                    </div>
                    <div class="meta-item">
                      <span class="meta-label">FINIT LE</span>
                      <span class="meta-value">{{ formatDate(c.dateFin) }}</span>
                    </div>
                  </div>

                  <button @click="startExam(c.id)" class="btn-start-exam w-100">
                    LANCER LE TEST <i class="fa-solid fa-chevron-right ms-2"></i>
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- 5. ANALYTICS (Pour Admins) -->
        <div v-if="userRole !== 'Candidat'" class="row g-4 pb-5">
            <div class="col-lg-12">
                <div class="glass-panel">
                    <h5 class="panel-title mb-4">COURBE DE PERFORMANCE GLOBAL</h5>
                    <div class="chart-height"><canvas id="mainActivityChart"></canvas></div>
                </div>
            </div>
        </div>

      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useRouter } from 'vue-router';
import axios from 'axios';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';

const authStore = useAuthStore();
const router = useRouter();
const userRole = computed(() => authStore.role);
const campaigns = ref([]);
const loading = ref(true);

const API_ENDPOINT = 'http://localhost:5172/api';

const fetchCampaigns = async () => {
  loading.value = true;
  try {
    const res = await axios.get(`${API_ENDPOINT}/Campagnes`);
    campaigns.value = res.data;
  } catch (err) { console.error(err); } 
  finally { loading.value = false; }
};

const filteredCampaigns = computed(() => {
  return userRole.value === 'Candidat' ? campaigns.value.filter(c => c.statut === 1) : campaigns.value;
});

const startExam = (id) => { router.push(`/exam-lobby/${id}`); };
const formatDate = (d) => d ? new Date(d).toLocaleDateString() : 'Non définie';

const roleLabel = computed(() => {
  const map = { 'SuperAdmin': 'ADMINISTRATEUR MAÎTRE', 'AdminEntreprise': 'ESPACE CORPORATE', 'Evaluateur': 'ESPACE ÉVALUATEUR', 'Candidat': 'PORTAIL CANDIDAT' };
  return map[userRole.value] || 'TABLEAU DE BORD';
});

const roleContext = computed(() => {
  if (userRole.value === 'SuperAdmin') return 'CORE SYSTEM INFRA';
  if (userRole.value === 'AdminEntreprise') return 'CORP METRICS';
  if (userRole.value === 'Evaluateur') return 'EVAL_METRICS';
  return 'EVAL_PROCESS';
});

const aiInsight = computed(() => {
  if (userRole.value === 'Candidat') return "VOTRE PROFIL EST OPTIMISÉ À 94% POUR L'EXAMEN ACTUEL.";
  return "ANALYSE COMPLÉTÉE : 12 NOUVELLES INSCRIPTIONS DÉTECTÉES CE MATIN.";
});

const dynamicStats = computed(() => {
  const stats = {
    AdminEntreprise: [
      { label: 'TALENTS', value: '12', icon: 'fa-solid fa-users-viewfinder', bg: '#eff6ff', color: '#3b82f6', trend: '+4', trendUp: true },
      { label: 'ÉVALUATIONS', value: '284', icon: 'fa-solid fa-file-signature', bg: '#fffbeb', color: '#f59e0b', trend: '+15%', trendUp: true },
      { label: 'TAUX SUCCÈS', value: '78%', icon: 'fa-solid fa-chart-pie', bg: '#f5f3ff', color: '#8b5cf6', trend: '+2%', trendUp: true },
      { label: 'ALERTES IA', value: '02', icon: 'fa-solid fa-bolt-lightning', bg: '#fef2f2', color: '#ef4444', trend: 'BAS', trendUp: false }
    ],
    Evaluateur: [
      { label: 'SESSIONS', value: '05', icon: 'fa-solid fa-calendar-check', bg: '#fffbeb', color: '#f59e0b', trend: '+1', trendUp: true },
      { label: 'QUESTIONS', value: '248', icon: 'fa-solid fa-database', bg: '#eff6ff', color: '#3b82f6', trend: '+12', trendUp: true },
      { label: 'À CORRIGER', value: '03', icon: 'fa-solid fa-pen-nib', bg: '#fef2f2', color: '#ef4444', trend: 'Urgent', trendUp: false },
      { label: 'SCORE MOY.', value: '14.5', icon: 'fa-solid fa-star', bg: '#f5f3ff', color: '#8b5cf6', trend: '+0.5', trendUp: true }
    ],
    Candidat: [
        { label: 'À PASSER', value: '01', icon: 'fa-solid fa-hourglass-half', bg: '#fffbeb', color: '#f59e0b', trend: 'Urg.', trendUp: false },
        { label: 'TERMINÉS', value: '04', icon: 'fa-solid fa-check-double', bg: '#ecfdf5', color: '#10b981', trend: '+1', trendUp: true },
        { label: 'SCORE MOY.', value: '82%', icon: 'fa-solid fa-brain', bg: '#f5f3ff', color: '#8b5cf6', trend: '+5%', trendUp: true },
        { label: 'RANG', value: '#12', icon: 'fa-solid fa-trophy', bg: '#eff6ff', color: '#3b82f6', trend: '+2', trendUp: true }
    ]
  };
  return stats[userRole.value] || stats.AdminEntreprise;
});

onMounted(fetchCampaigns);
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;600;700;800&display=swap');

.admin-layout { min-height: 100vh; background-color: #f8fafc; font-family: 'Plus Jakarta Sans', sans-serif; display: flex; position: relative; }
.main-viewport { 
  flex-grow: 1; 
  z-index: 5; 
  position: relative; 
}

/* DECO ELEMENTS */
.background-overlay { position: absolute; inset: 0; background: radial-gradient(circle at 30% 30%, #ffffff 0%, #f1f5f9 100%); z-index: 0; }
.tech-grid-subtle { position: absolute; inset: 0; background-image: radial-gradient(#e2e8f0 1.5px, transparent 1.5px); background-size: 40px 40px; opacity: 0.4; }
.glow-orb { position: absolute; border-radius: 50%; filter: blur(130px); opacity: 0.15; }
.orb-amber { width: 600px; height: 600px; background: #fbbf24; top: -100px; right: -100px; }
.orb-blue { width: 500px; height: 500px; background: #60a5fa; bottom: -100px; left: -100px; }

/* TERMINAL BAR */
.terminal-status-bar { background: rgba(255, 255, 255, 0.7); backdrop-filter: blur(10px); border-radius: 16px; border: 1px solid #fff; padding: 10px 0; }
.system-tag { display: flex; align-items: center; gap: 8px; background: #0f172a; padding: 6px 15px; border-radius: 100px; }
.tag-text { color: white; font-size: 9px; font-weight: 800; }
.pulse-dot { width: 6px; height: 6px; background: #eab308; border-radius: 50%; animation: blink 2s infinite; }

/* HERO CARD */
.hero-cyber-card { background: rgba(255, 255, 255, 0.85); border: 1px solid #fff; border-radius: 40px; box-shadow: 0 20px 40px rgba(0,0,0,0.03); backdrop-filter: blur(20px); overflow: hidden; position: relative; }
.display-title-cyber { font-weight: 800; font-size: 42px; letter-spacing: -1.5px; color: #0f172a; }
.text-amber { color: #eab308; }
.badge-amber-glow { display: inline-block; padding: 8px 18px; background: #0f172a; color: #fff; border-radius: 100px; font-size: 11px; font-weight: 800; }
.ai-insight-box { background: #f8fafc; border-left: 5px solid #eab308; padding: 20px; border-radius: 20px; display: flex; align-items: center; gap: 20px; }
.ai-box-icon { font-size: 24px; color: #eab308; }

/* CAMPAIGN CARDS */
.campaign-card-candidate { background: white; border-radius: 24px; border: 1px solid #f1f5f9; transition: 0.3s; position: relative; overflow: hidden; height: 100%; }
.campaign-card-candidate:hover { transform: translateY(-8px); box-shadow: 0 20px 30px rgba(0,0,0,0.05); border-color: #eab308; }
.card-status-strip { height: 6px; width: 100%; background: #eab308; }
.badge-tech { background: #f8fafc; color: #64748b; padding: 4px 12px; border-radius: 8px; font-size: 10px; font-weight: 800; text-transform: uppercase; }
.campaign-name { font-weight: 800; color: #0f172a; margin: 15px 0 10px; }
.campaign-meta-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 15px; background: #f8fafc; padding: 15px; border-radius: 16px; }
.meta-label { display: block; font-size: 9px; font-weight: 800; color: #94a3b8; margin-bottom: 4px; }
.meta-value { font-size: 13px; font-weight: 800; color: #0f172a; }

.btn-start-exam { background: #0f172a; color: white; border: none; padding: 14px; border-radius: 14px; font-weight: 800; font-size: 13px; transition: 0.3s; }
.btn-start-exam:hover { background: #eab308; color: #0f172a; }

/* KPI */
.kpi-cyber-card { background: white; border-radius: 24px; padding: 25px; border: 1px solid #f1f5f9; }
.kpi-icon-box { width: 45px; height: 45px; border-radius: 14px; display: flex; align-items: center; justify-content: center; font-size: 18px; }
.kpi-value { font-size: 28px; font-weight: 800; color: #0f172a; margin-top: 10px; }
.kpi-label { font-size: 11px; font-weight: 700; color: #94a3b8; text-transform: uppercase; }

.glass-panel { background: white; border-radius: 30px; padding: 30px; border: 1px solid #f1f5f9; }

/* UTILS */
.spinner-pro-premium { width: 50px; height: 50px; border: 4px solid #f1f5f9; border-top: 4px solid #eab308; border-radius: 50%; animation: spin 1s linear infinite; margin: 0 auto; }
@keyframes spin { to { transform: rotate(360deg); } }
@keyframes blink { 0%, 100% { opacity: 1; } 50% { opacity: 0.3; } }
@keyframes float { 0%, 100% { transform: translateY(0); } 50% { transform: translateY(-10px); } }
.robot-float { animation: float 4s ease-in-out infinite; }
</style>