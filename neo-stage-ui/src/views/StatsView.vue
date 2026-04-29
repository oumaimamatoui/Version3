<template>
  <div class="d-flex admin-body">
    <!-- NAVIGATION LATERALE -->
    <AppSidebar />

    <div class="content flex-grow-1 bg-luxury-main">
      <!-- BARRE DE NAVIGATION HAUTE -->
      <AppNavbar />

      <!-- ZONE DE CAPTURE POUR LE RAPPORT PDF -->
      <main id="capture-zone" class="p-4 p-lg-5 animate__animated animate__fadeIn" v-if="!loading">
        
        <!-- ==========================================
             SECTION 1 : EN-TÊTE DYNAMIQUE
             ========================================== -->
        <header class="d-flex flex-column flex-md-row justify-content-between align-items-md-end mb-5 gap-4">
          <div class="header-text-group">
            <nav class="breadcrumb-luxury">SYSTEME / ANALYTIQUE_TEMPS_REEL</nav>
            <h2 class="main-title-luxury m-0">
              Tableau de <span class="text-gold-gradient">Bord Global</span>
            </h2>
            <p class="subtitle-luxury m-0">Surveillance intelligente des performances et des certifications.</p>
          </div>
          
          <div class="header-actions d-flex gap-3" data-html2canvas-ignore="true">
            <button @click="fetchStats" class="btn-luxury-secondary shadow-sm" :disabled="loading">
              <i class="fa-solid fa-arrows-rotate me-2" :class="{ 'fa-spin': loading }"></i>Actualiser
            </button>
            <button @click="downloadPDF" class="btn-luxury-primary shadow-premium" :disabled="downloading">
              <i class="fa-solid me-2" :class="downloading ? 'fa-spinner fa-spin' : 'fa-file-pdf'"></i>
              {{ downloading ? 'GÉNÉRATION...' : 'EXPORTER RAPPORT' }}
            </button>
          </div>
        </header>

        <!-- ==========================================
             SECTION 2 : GRILLE KPI (BENTO STYLE)
             ========================================== -->
        <div class="row g-4 mb-5">
          <div class="col-md-3" v-for="kpi in kpiArray" :key="kpi.label">
            <div class="bento-card kpi-card-luxury">
              <div class="icon-vessel" :style="{ background: kpi.bg, color: kpi.color }">
                <i :class="kpi.icon"></i>
              </div>
              <div class="ms-3">
                <div class="kpi-value-lux">{{ kpi.value }}</div>
                <div class="kpi-label-lux">{{ kpi.label }}</div>
              </div>
              <div class="card-glass-sweep"></div>
            </div>
          </div>
        </div>

        <div class="row g-4 mb-5">
          <!-- ==========================================
               SECTION 3 : GRAPHIQUE DE PERFORMANCE
               ========================================== -->
          <div class="col-lg-8">
            <div class="bento-card chart-luxury-container h-100">
              <div class="d-flex justify-content-between align-items-center mb-5">
                <h6 class="section-title-luxury m-0">Performance par Campagne</h6>
                <div class="badge-tech">MOYENNE DES SCORES (%)</div>
              </div>
              
              <div class="chart-pro-viewport" v-if="stats.chart.length">
                <div v-for="item in stats.chart" :key="item.name" class="bar-column-group">
                  <div class="bar-score-floating">{{ item.score }}%</div>
                  <div class="bar-track-premium">
                    <div class="bar-fill-premium" :style="{ height: item.score + '%' }">
                      <div class="bar-gloss-effect"></div>
                    </div>
                  </div>
                  <div class="bar-label-premium">{{ item.name }}</div>
                </div>
              </div>
              <div v-else class="empty-state-container py-5 text-center">
                 <i class="fa-solid fa-chart-simple mb-3"></i>
                 <p>Données insuffisantes pour l'affichage graphique.</p>
              </div>
            </div>
          </div>

          <!-- ==========================================
               SECTION 4 : LEADERBOARD ÉLITE
               ========================================== -->
          <div class="col-lg-4">
            <div class="bento-card leaderboard-premium h-100">
              <h6 class="section-title-luxury mb-4">Top Candidatures</h6>
              <div v-if="stats.leaders.length" class="leaders-stack">
                <div v-for="(user, i) in stats.leaders" :key="i" class="leader-row-elite">
                  <div class="rank-node" :class="'rank-' + (i+1)">{{ i + 1 }}</div>
                  <div class="squircle-avatar">
                    {{ user.name.charAt(0) }}
                  </div>
                  <div class="flex-grow-1 ms-3">
                    <div class="leader-name-lux">{{ user.name }}</div>
                    <div class="leader-test-lux">{{ user.test }}</div>
                  </div>
                  <div class="leader-score-lux">{{ user.score }}%</div>
                </div>
              </div>
              <div v-else class="empty-state-mini">Aucun leader identifié</div>
            </div>
          </div>
        </div>

        <!-- ==========================================
             SECTION 5 : TABLEAU DES RÉSULTATS RÉCENTS
             ========================================== -->
        <div class="bento-card table-luxury-wrapper overflow-hidden">
          <div class="table-header-luxury">
             <h6 class="section-title-luxury m-0">Dernières Évaluations Certifiées</h6>
             <button class="btn-text-luxury" @click="$router.push('/candidates')">
               TOUS LES CANDIDATS <i class="fa-solid fa-arrow-right-long ms-2"></i>
             </button>
          </div>
          
          <div class="table-responsive">
            <table class="table table-luxury-pro m-0">
              <thead>
                <tr>
                  <th>CANDIDAT</th>
                  <th>CAMPAGNE / TEST</th>
                  <th>DATE</th>
                  <th>SCORE</th>
                  <th>INTEGRITÉ</th>
                  <th class="text-center">ACTION</th>
                </tr>
              </thead>
              <tbody v-if="stats.recentResults.length">
                <tr v-for="res in stats.recentResults" :key="res.id">
                  <td>
                    <div class="d-flex align-items-center">
                      <div class="mini-squircle-avatar">{{ res.candidateName.charAt(0) }}</div>
                      <div class="ms-3 fw-700 text-dark-lux">{{ res.candidateName }}</div>
                    </div>
                  </td>
                  <td><span class="text-muted-lux">{{ res.testName }}</span></td>
                  <td><span class="text-mono-lux">{{ res.date }}</span></td>
                  <td><div class="score-badge-lux">{{ res.score }}%</div></td>
                  <td>
                    <div class="integrity-badge" :class="res.integrity < 80 ? 'risk' : 'safe'">
                      <i class="fa-solid fa-shield-halved me-1"></i> {{ res.integrity }}%
                    </div>
                  </td>
                  <td class="text-center">
                    <button class="btn-action-view" @click="$router.push(`/details-candidat/${res.candidateId}`)">
                      <i class="fa-solid fa-eye"></i>
                    </button>
                  </td>
                </tr>
              </tbody>
              <tbody v-else>
                <tr>
                   <td colspan="6" class="text-center py-5 text-muted">Aucun résultat récent à afficher.</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </main>

      <!-- ÉTAT DE CHARGEMENT ÉLITE -->
      <div v-else class="loader-viewport-luxury">
        <div class="spinner-premium"></div>
        <p class="loading-text-lux">SYNCHRONISATION DES SYSTÈMES ANALYTIQUES...</p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import axios from 'axios';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';

/**
 * ÉTATS ET VARIABLES
 */
const loading = ref(true);
const downloading = ref(false);
const stats = ref({ 
  kpis: {}, 
  chart: [], 
  leaders: [], 
  recentResults: [] 
});

/**
 * CONFIGURATION DES CARTES KPI
 */
const kpiArray = computed(() => [
  { label: 'Certifications', value: stats.value.kpis.totalTests || 0, icon: 'fa-solid fa-certificate', color: '#fbbf24', bg: '#fffbeb' },
  { label: 'Moyenne Globale', value: (stats.value.kpis.moyenne || 0) + '%', icon: 'fa-solid fa-chart-line', color: '#3b82f6', bg: '#eff6ff' },
  { label: 'Analyses IA', value: stats.value.kpis.iaProcessed || 0, icon: 'fa-solid fa-robot', color: '#8b5cf6', bg: '#f5f3ff' },
  { label: 'Taux Rejet', value: (stats.value.kpis.tauxEchec || 0) + '%', icon: 'fa-solid fa-user-xmark', color: '#f43f5e', bg: '#fff1f2' },
]);

/**
 * RÉCUPÉRATION DES DONNÉES
 */
const fetchStats = async () => {
  loading.value = true;
  try {
    const token = localStorage.getItem('token');
    const res = await axios.get("http://localhost:5172/api/Dashboard/global-stats", {
      headers: { Authorization: `Bearer ${token}` }
    });
    stats.value = res.data;
  } catch (err) {
    console.error("Dashboard API Error:", err);
  } finally {
    setTimeout(() => { loading.value = false; }, 800);
  }
};

/**
 * GÉNÉRATION DU RAPPORT PDF
 */
const downloadPDF = async () => {
  downloading.value = true;
  try {
    const element = document.getElementById('capture-zone');
    const canvas = await html2canvas(element, {
      scale: 2,
      useCORS: true,
      backgroundColor: '#f8fafc'
    });

    const imgData = canvas.toDataURL('image/png');
    const pdf = new jsPDF('p', 'mm', 'a4');
    const pdfWidth = pdf.internal.pageSize.getWidth();
    const pdfHeight = (canvas.height * pdfWidth) / canvas.width;
    
    pdf.addImage(imgData, 'PNG', 0, 0, pdfWidth, pdfHeight);
    pdf.save(`EvaluaTech_Rapport_${new Date().getTime()}.pdf`);
  } catch (err) {
    console.error("PDF Generation Error:", err);
  } finally {
    downloading.value = false;
  }
};

onMounted(fetchStats);
</script>

<style scoped>
/**
 * ==========================================
 * DESIGN SYSTEM : LUXURY LIGHT MODE
 * ==========================================
 */

/* POLICES */
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&family=JetBrains+Mono:wght@600;800&display=swap');

.admin-body { 
  background: #fdfdfd; 
  min-height: 100vh; 
  font-family: 'Plus Jakarta Sans', sans-serif;
  color: #0f172a;
}

.bg-luxury-main { 
  background: radial-gradient(at 0% 0%, #ffffff 0%, #f8fafc 100%); 
}

/* TYPOGRAPHIE ET TITRES */
.breadcrumb-luxury { font-size: 11px; font-weight: 800; color: #fbbf24; letter-spacing: 2px; margin-bottom: 8px; }
.main-title-luxury { font-weight: 900; font-size: 2.6rem; color: #0f172a; letter-spacing: -1.5px; }
.text-gold-gradient { 
  background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%); 
  -webkit-background-clip: text; 
  -webkit-text-fill-color: transparent; 
}
.subtitle-luxury { color: #94a3b8; font-weight: 500; font-size: 1.05rem; }

/* BENTO CARDS PREMIUM */
.bento-card {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(25px);
  border-radius: 35px;
  padding: 35px;
  border: 1px solid #ffffff;
  box-shadow: 0 20px 40px -15px rgba(0,0,0,0.04), 0 5px 15px -5px rgba(0,0,0,0.01);
  transition: all 0.4s cubic-bezier(0.165, 0.84, 0.44, 1);
  position: relative;
  overflow: hidden;
}
.bento-card:hover { transform: translateY(-10px); box-shadow: 0 40px 80px -20px rgba(0,0,0,0.08); }

/* KPI CARDS */
.icon-vessel { 
  width: 65px; height: 65px; border-radius: 20px; 
  display: flex; align-items: center; justify-content: center; font-size: 1.6rem; 
}
.kpi-value-lux { font-family: 'JetBrains Mono', monospace; font-size: 2.2rem; font-weight: 800; color: #0f172a; line-height: 1; }
.kpi-label-lux { font-size: 11px; font-weight: 800; color: #94a3b8; text-transform: uppercase; margin-top: 6px; letter-spacing: 0.5px; }

/* CHART STYLING */
.chart-pro-viewport { height: 320px; display: flex; align-items: flex-end; gap: 35px; padding: 20px 0; }
.bar-column-group { flex: 1; display: flex; flex-direction: column; align-items: center; }
.bar-track-premium { 
  width: 50px; height: 220px; background: #f8fafc; border-radius: 100px; 
  position: relative; overflow: hidden; border: 1px solid #f1f5f9; 
}
.bar-fill-premium { 
  width: 100%; position: absolute; bottom: 0; 
  background: linear-gradient(to top, #fbbf24, #f59e0b); 
  border-radius: 100px; transition: height 1.5s cubic-bezier(0.175, 0.885, 0.32, 1.275); 
}
.bar-score-floating { font-family: 'JetBrains Mono'; font-weight: 800; font-size: 12px; margin-bottom: 12px; color: #0f172a; }
.bar-label-premium { font-size: 10px; font-weight: 800; color: #64748b; margin-top: 15px; text-transform: uppercase; }

/* LEADERBOARD STYLING */
.leader-row-elite { 
  display: flex; align-items: center; padding: 20px; background: #ffffff; 
  border-radius: 24px; margin-bottom: 15px; border: 1px solid #f1f5f9; transition: 0.3s; 
}
.leader-row-elite:hover { border-color: #fbbf24; background: #fffdf7; }
.squircle-avatar { 
  width: 48px; height: 48px; background: #0f172a; color: #fff; 
  border-radius: 16px; display: flex; align-items: center; justify-content: center; font-weight: 800; 
}
.rank-node { width: 30px; font-weight: 900; font-size: 1rem; color: #cbd5e1; }
.rank-1 { color: #fbbf24; }
.leader-score-lux { font-family: 'JetBrains Mono'; font-weight: 800; color: #fbbf24; font-size: 1.2rem; }

/* TABLE LUXURY STYLING */
.table-luxury-wrapper { padding: 0 !important; }
.table-header-luxury { padding: 30px 35px; display: flex; justify-content: space-between; align-items: center; border-bottom: 1px solid #f1f5f9; }
.table-luxury-pro thead th { background: #f8fafc; color: #94a3b8; font-size: 11px; font-weight: 800; padding: 20px 35px; border: none; letter-spacing: 1px; }
.table-luxury-pro tbody td { padding: 22px 35px; vertical-align: middle; border-bottom: 1px solid #f8fafc; }
.mini-squircle-avatar { width: 38px; height: 38px; border-radius: 12px; background: #e2e8f0; display: flex; align-items: center; justify-content: center; font-weight: 800; font-size: 12px; }
.score-pill-luxury { 
  background: #fffbeb; color: #d97706; padding: 6px 14px; border-radius: 100px; 
  font-weight: 800; font-family: 'JetBrains Mono'; font-size: 13px; 
}
.integrity-badge { font-size: 11px; font-weight: 700; }
.integrity-badge.safe { color: #10b981; }
.integrity-badge.risk { color: #f43f5e; }

/* BOUTONS ACTIONS */
.btn-luxury-primary { 
  background: #0f172a; color: white; border: none; padding: 15px 30px; 
  border-radius: 18px; font-weight: 800; font-size: 13px; transition: 0.3s; 
}
.btn-luxury-primary:hover { transform: translateY(-3px); box-shadow: 0 15px 30px rgba(15,23,42,0.25); }
.btn-luxury-secondary { 
  background: white; border: 1.5px solid #e2e8f0; padding: 15px 30px; 
  border-radius: 18px; font-weight: 800; color: #475569; transition: 0.3s; 
}
.btn-action-view { 
  width: 42px; height: 42px; border-radius: 14px; border: none; 
  background: #f8fafc; color: #0f172a; transition: 0.3s; 
}
.btn-action-view:hover { background: #0f172a; color: white; transform: rotate(15deg); }

/* EFFET DE BRILLANCE */
.card-glass-sweep { 
  position: absolute; top: 0; left: -100%; width: 50%; height: 100%; 
  background: linear-gradient(90deg, transparent, rgba(255,255,255,0.4), transparent); 
  transition: 0.5s; 
}
.bento-card:hover .card-glass-sweep { left: 150%; transition: 0.8s; }

/* LOADER */
.loader-viewport-luxury { height: 70vh; display: flex; flex-direction: column; align-items: center; justify-content: center; gap: 25px; }
.spinner-premium { 
  width: 60px; height: 60px; border: 5px solid #f1f5f9; 
  border-top-color: #fbbf24; border-radius: 50%; animation: spin 1s infinite linear; 
}
@keyframes spin { to { transform: rotate(360deg); } }

/* SCROLLBAR */
::-webkit-scrollbar { width: 8px; }
::-webkit-scrollbar-track { background: #f8fafc; }
::-webkit-scrollbar-thumb { background: #e2e8f0; border-radius: 10px; border: 2px solid #f8fafc; }
::-webkit-scrollbar-thumb:hover { background: #fbbf24; }
</style>