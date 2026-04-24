<template>
  <div class="admin-layout">
    <!-- COUCHES DÉCORATIVES HAUT DE GAMME -->
    <div class="background-overlay"></div>
    <div class="tech-grid-subtle"></div>
    <div class="glow-orb orb-amber"></div>
    <div class="glow-orb orb-indigo"></div>

    <AppSidebar />
    
    <div class="main-viewport animate__animated animate__fadeIn">
      <AppNavbar />

      <div class="container-fluid px-lg-5 pt-4">
        
        <!-- 1. TERMINAL STATUS BAR : REVISITÉ -->
        <div class="terminal-status-bar mb-4">
          <div class="d-flex justify-content-between align-items-center px-4">
            <div class="breadcrumb-cyber">
              <span class="root">SYSTEM</span>
              <span class="sep">/</span>
              <span class="current">{{ roleLabel }}</span>
            </div>
            <div class="system-meta d-none d-md-flex">
              <div class="meta-pill">
                <span class="pulse-dot"></span>
                <span class="tag-text">ID: {{ authStore.user?.name.toUpperCase() }}</span>
              </div>
              <div class="meta-pill ms-3">
                <i class="fa-solid fa-circle-check me-2"></i>ENC_V2.0
              </div>
            </div>
          </div>
        </div>

        <!-- 2. HERO SECTION : ÉLÉGANCE ET SOBRIÉTÉ -->
        <div class="hero-cyber-card mb-5">
          <div class="row align-items-center g-0">
            <div class="col-lg-8 p-5">
              <div class="badge-premium mb-3">
                <i class="fa-solid fa-wand-magic-sparkles me-2"></i> {{ roleContext }}
              </div>
              <h1 class="display-title-cyber">
                Bonjour, <span class="text-gradient-amber">{{ authStore.user?.name || 'Utilisateur' }}</span>
              </h1>
              <p class="hero-subtitle text-muted mb-4">Bienvenue sur votre console de pilotage. Voici l'état actuel de votre périmètre.</p>
              
              <div class="ai-insight-card">
                <div class="ai-icon-pulse">
                  <i class="fa-solid fa-robot"></i>
                </div>
                <div class="ai-content">
                  <span class="ai-label">ANALYSE PRÉDICTIVE</span>
                  <p class="ai-text">{{ aiInsight }}</p>
                </div>
              </div>
            </div>
            
            <div class="col-lg-4 d-none d-lg-flex justify-content-center">
              <div class="visual-abstract">
                <div class="blob-shape"></div>
                <div class="robot-float">
                  <svg class="modern-robot" viewBox="0 0 200 200" width="200">
                    <rect x="55" y="50" width="90" height="85" rx="24" fill="white" stroke="#e2e8f0" stroke-width="1"/>
                    <rect x="65" y="65" width="70" height="32" rx="12" fill="#0f172a" />
                    <circle cx="85" cy="81" r="4" fill="#fbbf24" />
                    <circle cx="115" cy="81" r="4" fill="#fbbf24" />
                  </svg>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- 3. KPI BENTO GRID : MODERNISÉ -->
        <div class="row g-4 mb-5">
          <div class="col-xl-3 col-md-6" v-for="stat in dynamicStats" :key="stat.label">
            <router-link :to="getStatLink(stat.label)" class="stat-card-premium text-decoration-none">
              <div class="d-flex justify-content-between align-items-start">
                <div class="stat-icon-container" :style="{ '--brand-color': stat.color, '--brand-bg': stat.bg }">
                  <i :class="stat.icon"></i>
                </div>
                <div class="stat-trend" :class="{ 'trend-up': stat.trendUp, 'trend-down': !stat.trendUp }">
                  {{ stat.trend }} <i class="fa-solid" :class="stat.trendUp ? 'fa-caret-up' : 'fa-caret-down'"></i>
                </div>
              </div>
              <div class="stat-body mt-4">
                <h2 class="stat-value">{{ stat.value }}</h2>
                <p class="stat-label">{{ stat.label }}</p>
              </div>
              <div class="stat-footer-progress">
                <div class="progress-track">
                  <div class="progress-fill" :style="{ width: '70%', background: stat.color }"></div>
                </div>
              </div>
            </router-link>
          </div>
        </div>

        <!-- 4. SECTION CANDIDATS : LISTE DES CAMPAGNES -->
        <div v-if="userRole === 'Candidat'" class="campaign-candidate-section mb-5">
          <div class="d-flex justify-content-between align-items-end mb-4">
            <div>
              <h4 class="section-title-main">Evaluations prioritaires</h4>
              <p class="text-muted small m-0">Veuillez compléter vos tests avant la date limite.</p>
            </div>
            <span class="badge bg-slate-100 text-slate-600 px-3 py-2 rounded-pill fw-bold">{{ filteredCampaigns.length }} Tests</span>
          </div>

          <div v-if="loading" class="text-center py-5">
            <div class="loader-ripple"><div></div><div></div></div>
            <p class="mt-3 text-muted fw-semibold">Chargement de votre environnement...</p>
          </div>

          <div v-else class="row g-4">
            <div v-if="filteredCampaigns.length === 0" class="col-12">
              <div class="empty-state-panel">
                <i class="fa-solid fa-inbox fa-3x mb-3 text-slate-200"></i>
                <p class="fw-bold text-slate-500">Aucun test ne requiert votre attention pour le moment.</p>
              </div>
            </div>

            <div v-for="c in filteredCampaigns" :key="c.id" class="col-md-6 col-xl-4">
              <div class="campaign-card-glass">
                <div class="card-header-tech">
                  <span class="category-tag">{{ c.categorie || 'TECH' }}</span>
                  <div class="duration-tag"><i class="fa-regular fa-clock me-1"></i> {{ c.dureeMinutes }}min</div>
                </div>
                
                <div class="p-4 pt-3">
                  <h5 class="campaign-title">{{ c.nom }}</h5>
                  <p class="campaign-description">{{ c.description || 'Test de compétences techniques de haut niveau.' }}</p>
                  
                  <div class="campaign-info-grid">
                    <div class="info-item">
                      <span class="label">Passage</span>
                      <span class="value text-success">{{ c.scorePassage }}%</span>
                    </div>
                    <div class="info-item">
                      <span class="label">Échéance</span>
                      <span class="value">{{ formatDate(c.dateFin) }}</span>
                    </div>
                  </div>

                  <button 
                    @click="startExam(c)" 
                    class="btn-primary-cyber" 
                    :disabled="new Date(c.dateDebut) > new Date()"
                    :title="new Date(c.dateDebut) > new Date() ? 'Ce test n\'est pas encore ouvert' : ''"
                  >
                    <span v-if="new Date(c.dateDebut) > new Date()">
                      Ouvre le {{ formatDate(c.dateDebut) }}
                    </span>
                    <span v-else>
                      Démarrer la session <i class="fa-solid fa-arrow-right ms-2"></i>
                    </span>
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- 5. ANALYTICS (Pour Admins) -->
        <div v-if="userRole !== 'Candidat'" class="row g-4 pb-5">
            <div class="col-lg-12">
                <div class="glass-panel-chart">
                    <div class="d-flex justify-content-between align-items-center mb-4">
                      <h5 class="panel-title-cyber">Performance des flux</h5>
                      <div class="chart-filters">
                        <button class="btn-filter active">7 Jours</button>
                        <button class="btn-filter">30 Jours</button>
                      </div>
                    </div>
                    <div class="chart-height-wrapper"><canvas id="mainActivityChart"></canvas></div>
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
import api from '@/services/api';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';

const authStore = useAuthStore();
const router = useRouter();
const userRole = computed(() => authStore.role);
const campaigns = ref([]);
const loading = ref(true);

const fetchCampaigns = async () => {
  loading.value = true;
  try {
    const res = await api.get('/Campagnes');
    campaigns.value = res.data;
  } catch (err) { console.error(err); } 
  finally { loading.value = false; }
};

const filteredCampaigns = computed(() => {
  return campaigns.value; // On laisse le bouton "Démarrer" se gérer avec la date
});

const startExam = (campaign) => { 
  const targetId = campaign.candidatureId || campaign.id;
  router.push(`/exam-lobby/${targetId}`); 
};
const formatDate = (d) => d ? new Date(d).toLocaleDateString('fr-FR', { day: '2-digit', month: 'short' }) : 'N/A';

const roleLabel = computed(() => {
  const map = { 
    'SuperAdmin': 'ADMINISTRATEUR MAÎTRE', 
    'AdminEntreprise': 'WORKSPACE CORPORATE', 
    'Recruteur': 'TALENT ACQUISITION',
    'Evaluateur': 'ASSESSMENT EXPERT', 
    'Candidat': 'ESPACE ÉVALUATION' 
  };
  return map[userRole.value] || 'TABLEAU DE BORD';
});

const roleContext = computed(() => {
  if (userRole.value === 'SuperAdmin') return 'CORE INFRASTRUCTURE ACTIVE';
  if (userRole.value === 'AdminEntreprise' || userRole.value === 'Recruteur') return 'GESTION DES TALENTS';
  return 'ÉVALUATION DES COMPÉTENCES';
});

const getStatLink = (label) => {
  const l = label.toLowerCase();
  if (l.includes('candidat') || l.includes('talent')) return '/candidates-list';
  if (l.includes('évaluation') || l.includes('campagne') || l.includes('session')) return '/campaigns';
  return '/dashboard';
};

const aiInsight = computed(() => {
  if (userRole.value === 'Candidat') return "Votre profil correspond à 94% des attentes techniques de ce test.";
  if (userRole.value === 'Recruteur') return "Alerte : 3 nouveaux candidats à haut potentiel viennent de finaliser leurs tests.";
  return "Analyse terminée : Le taux de réussite global a augmenté de 12% ce mois-ci.";
});

const dynamicStats = computed(() => {
  const stats = {
    AdminEntreprise: [
      { label: 'TALENTS ACTIFS', value: '12', icon: 'fa-solid fa-user-group', bg: '#eff6ff', color: '#3b82f6', trend: '+4', trendUp: true },
      { label: 'ÉVALUATIONS', value: '284', icon: 'fa-solid fa-file-lines', bg: '#fffbeb', color: '#f59e0b', trend: '+15%', trendUp: true },
      { label: 'TAUX RÉUSSITE', value: '78%', icon: 'fa-solid fa-chart-simple', bg: '#f5f3ff', color: '#8b5cf6', trend: '+2%', trendUp: true },
      { label: 'ALERTES IA', value: '02', icon: 'fa-solid fa-triangle-exclamation', bg: '#fef2f2', color: '#ef4444', trend: 'STABLE', trendUp: true }
    ],
    Candidat: [
        { label: 'À PASSER', value: '01', icon: 'fa-solid fa-hourglass-half', bg: '#fffbeb', color: '#f59e0b', trend: 'Urgent', trendUp: false },
        { label: 'TERMINÉS', value: '04', icon: 'fa-solid fa-circle-check', bg: '#ecfdf5', color: '#10b981', trend: '+1', trendUp: true },
        { label: 'SCORE MOY.', value: '82%', icon: 'fa-solid fa-bolt-lightning', bg: '#f5f3ff', color: '#8b5cf6', trend: '+5%', trendUp: true },
        { label: 'RANG ACTUEL', value: '#12', icon: 'fa-solid fa-trophy', bg: '#eff6ff', color: '#3b82f6', trend: '+2', trendUp: true }
    ]
  };
  return stats[userRole.value] || stats.AdminEntreprise;
});

onMounted(fetchCampaigns);
</script>

<style scoped>
/* Le CSS reste inchangé comme demandé */
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&display=swap');

.admin-layout { 
  min-height: 100vh; 
  background-color: #f1f5f9; 
  font-family: 'Plus Jakarta Sans', sans-serif; 
  display: flex; 
  position: relative; 
  overflow-x: hidden;
}

.main-viewport { flex-grow: 1; z-index: 5; position: relative; padding-bottom: 50px; }

/* DECORATIONS */
.background-overlay { position: absolute; inset: 0; background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%); z-index: 0; }
.tech-grid-subtle { position: absolute; inset: 0; background-image: radial-gradient(#cbd5e1 0.8px, transparent 0.8px); background-size: 32px 32px; opacity: 0.3; }
.glow-orb { position: absolute; border-radius: 50%; filter: blur(120px); opacity: 0.1; }
.orb-amber { width: 500px; height: 500px; background: #fbbf24; top: -10%; right: -5%; }
.orb-indigo { width: 400px; height: 400px; background: #6366f1; bottom: 5%; left: -5%; }

/* TERMINAL BAR */
.terminal-status-bar { 
  background: rgba(255, 255, 255, 0.6); 
  backdrop-filter: blur(12px); 
  border: 1px solid rgba(255, 255, 255, 0.8); 
  border-radius: 20px; 
  padding: 12px 0; 
  box-shadow: 0 4px 20px rgba(0,0,0,0.02);
}
.breadcrumb-cyber { font-size: 11px; font-weight: 800; letter-spacing: 1px; color: #64748b; }
.breadcrumb-cyber .root { color: #94a3b8; }
.breadcrumb-cyber .sep { margin: 0 10px; color: #cbd5e1; }
.breadcrumb-cyber .current { color: #0f172a; }
.meta-pill { background: #0f172a; color: white; padding: 6px 14px; border-radius: 100px; font-size: 10px; font-weight: 700; display: flex; align-items: center; }
.pulse-dot { width: 6px; height: 6px; background: #fbbf24; border-radius: 50%; margin-right: 8px; animation: pulse 2s infinite; }

/* HERO CARD */
.hero-cyber-card { 
  background: white; 
  border-radius: 32px; 
  border: 1px solid rgba(255,255,255,0.7); 
  box-shadow: 0 20px 50px rgba(0,0,0,0.04); 
  position: relative; 
  overflow: hidden; 
}
.badge-premium { display: inline-flex; align-items: center; padding: 6px 16px; background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 100px; font-size: 11px; font-weight: 700; color: #475569; }
.display-title-cyber { font-weight: 800; font-size: 2.8rem; color: #0f172a; letter-spacing: -1px; margin-bottom: 0.5rem; }
.text-gradient-amber { background: linear-gradient(90deg, #f59e0b, #d97706); -webkit-background-clip: text; -webkit-text-fill-color: transparent; }
.ai-insight-card { 
  background: #f8fafc; 
  border: 1px solid #f1f5f9; 
  padding: 20px; 
  border-radius: 24px; 
  display: flex; 
  gap: 15px; 
  align-items: center; 
  max-width: 550px; 
}
.ai-icon-pulse { font-size: 24px; color: #f59e0b; background: white; width: 50px; height: 50px; border-radius: 15px; display: flex; align-items: center; justify-content: center; box-shadow: 0 8px 15px rgba(245, 158, 11, 0.1); }
.ai-label { font-size: 10px; font-weight: 800; color: #94a3b8; letter-spacing: 0.5px; }
.ai-text { font-size: 13.5px; font-weight: 600; color: #334155; margin: 0; }

/* STAT CARDS (KPI) */
.stat-card-premium { 
  background: white; 
  padding: 24px; 
  border-radius: 24px; 
  border: 1px solid #f1f5f9; 
  transition: all 0.4s cubic-bezier(0.165, 0.84, 0.44, 1); 
  display: block; 
}
.stat-card-premium:hover { transform: translateY(-5px); box-shadow: 0 15px 35px rgba(0,0,0,0.05); border-color: #e2e8f0; }
.stat-icon-container { 
  width: 52px; height: 52px; border-radius: 16px; 
  background: var(--brand-bg); color: var(--brand-color); 
  display: flex; align-items: center; justify-content: center; font-size: 20px; 
}
.stat-value { font-size: 32px; font-weight: 800; color: #0f172a; margin: 0; letter-spacing: -1px; }
.stat-label { font-size: 12px; font-weight: 700; color: #94a3b8; text-transform: uppercase; letter-spacing: 0.5px; margin: 0; }
.stat-trend { font-size: 12px; font-weight: 700; padding: 4px 10px; border-radius: 100px; }
.trend-up { background: #ecfdf5; color: #10b981; }
.trend-down { background: #fef2f2; color: #ef4444; }
.progress-track { height: 6px; background: #f1f5f9; border-radius: 100px; overflow: hidden; margin-top: 15px; }
.progress-fill { height: 100%; border-radius: 100px; }

/* CAMPAIGN CARDS (GLASS) */
.campaign-card-glass { 
  background: rgba(255, 255, 255, 0.7); 
  backdrop-filter: blur(10px); 
  border: 1px solid rgba(255, 255, 255, 0.8); 
  border-radius: 28px; 
  transition: 0.3s ease; 
  height: 100%; 
  box-shadow: 0 10px 30px rgba(0,0,0,0.02);
}
.campaign-card-glass:hover { transform: translateY(-10px); background: #fff; box-shadow: 0 25px 50px rgba(0,0,0,0.06); }
.card-header-tech { padding: 20px 24px 10px; display: flex; justify-content: space-between; align-items: center; }
.category-tag { background: #0f172a; color: white; padding: 4px 12px; border-radius: 8px; font-size: 10px; font-weight: 800; }
.duration-tag { font-size: 12px; font-weight: 700; color: #64748b; }
.campaign-title { font-weight: 800; color: #0f172a; font-size: 18px; margin-bottom: 8px; }
.campaign-description { font-size: 13px; color: #64748b; line-height: 1.5; height: 40px; overflow: hidden; }
.campaign-info-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 10px; margin: 20px 0; background: #f8fafc; padding: 15px; border-radius: 20px; }
.info-item .label { display: block; font-size: 10px; font-weight: 700; color: #94a3b8; text-transform: uppercase; }
.info-item .value { font-size: 14px; font-weight: 800; color: #1e293b; }
.btn-primary-cyber { 
  width: 100%; padding: 14px; border-radius: 18px; border: none; 
  background: #0f172a; color: white; font-weight: 700; font-size: 14px; 
  transition: 0.3s ease; 
}
.btn-primary-cyber:hover { background: #f59e0b; color: #0f172a; transform: scale(1.02); }

/* CHARTS PANEL */
.glass-panel-chart { background: white; padding: 32px; border-radius: 32px; border: 1px solid #f1f5f9; }
.btn-filter { background: #f1f5f9; border: none; padding: 6px 16px; border-radius: 10px; font-size: 12px; font-weight: 700; color: #64748b; margin-left: 8px; }
.btn-filter.active { background: #0f172a; color: white; }

/* ANIMATIONS & SPINNER */
@keyframes pulse { 0% { opacity: 1; transform: scale(1); } 50% { opacity: 0.5; transform: scale(1.2); } 100% { opacity: 1; transform: scale(1); } }
.loader-ripple { display: inline-block; position: relative; width: 60px; height: 60px; }
.loader-ripple div { position: absolute; border: 4px solid #f59e0b; opacity: 1; border-radius: 50%; animation: loader-ripple 1s cubic-bezier(0, 0.2, 0.8, 1) infinite; }
.loader-ripple div:nth-child(2) { animation-delay: -0.5s; }
@keyframes loader-ripple { 0% { top: 30px; left: 30px; width: 0; height: 0; opacity: 1; } 100% { top: 0px; left: 0px; width: 60px; height: 60px; opacity: 0; } }

/* ROBOT FLOAT */
.visual-abstract { position: relative; }
.blob-shape { position: absolute; width: 300px; height: 300px; background: linear-gradient(135deg, #fffbeb 0%, #fef3c7 100%); border-radius: 50%; top: 50%; left: 50%; transform: translate(-50%, -50%); z-index: 1; filter: blur(40px); }
.robot-float { position: relative; z-index: 2; animation: float 6s ease-in-out infinite; }
@keyframes float { 0%, 100% { transform: translateY(0); } 50% { transform: translateY(-20px); } }
</style>