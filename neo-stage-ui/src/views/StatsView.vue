<template>
  <div class="d-flex admin-body">
    <AppSidebar />

    <div class="content flex-grow-1 bg-main-soft">
      <AppNavbar />

      <!-- ID unique pour la capture -->
      <main id="capture-zone" class="p-4 animate-fade-in" v-if="!loading">
        <!-- HEADER -->
        <div class="d-flex justify-content-between align-items-end mb-4">
          <div>
            <nav class="breadcrumb-cyber mb-2 text-indigo fw-bold">ANALYTICS / TEMPS_REEL</nav>
            <h2 class="fw-800 text-navy m-0">
              Statistiques <span>Globales</span>
            </h2>
          </div>
          <div class="d-flex gap-2" data-html2canvas-ignore="true">
            <button @click="fetchStats" class="btn-refresh">
              <i class="fa-solid fa-sync me-2"></i>Actualiser
            </button>
            <button 
              @click="downloadPDF" 
              class="btn-primary-pro-sm" 
              :disabled="downloading"
            >
              <i class="fa-solid me-2" :class="downloading ? 'fa-spinner fa-spin' : 'fa-download'"></i>
              {{ downloading ? 'Génération...' : 'Rapport PDF' }}
            </button>
          </div>
        </div>

        <!-- KPI CARDS -->
        <div class="row g-3 mb-4">
          <div class="col-md-3" v-for="kpi in kpiArray" :key="kpi.label">
            <div class="glass-card p-3 border-0 shadow-sm d-flex align-items-center gap-3 bg-white rounded-4">
              <div class="icon-vessel-sm" :style="{ background: kpi.bg, color: kpi.color }">
                <i :class="kpi.icon"></i>
              </div>
              <div>
                <div class="fw-800 fs-4 text-navy">{{ kpi.value }}</div>
                <div class="tiny fw-bold text-muted uppercase">{{ kpi.label }}</div>
              </div>
            </div>
          </div>
        </div>

        <div class="row g-4">
          <!-- CHART -->
          <div class="col-lg-8">
            <div class="glass-card p-4 h-100 shadow-sm border-0 bg-white rounded-4">
              <h6 class="fw-bold mb-5 text-navy uppercase tiny ls-1">Performance par Projet</h6>
              <div class="chart-container-pro d-flex align-items-end gap-3" v-if="chartData.length">
                <div v-for="(group, idx) in chartData" :key="idx" class="chart-column-wrapper">
                  <div class="chart-value-hint">{{ group.score }}%</div>
                  <div class="chart-bar-pro" :style="{ height: group.score + '%' }">
                    <!-- On masque le glow pour le PDF -->
                    <div class="bar-glow" data-html2canvas-ignore="true"></div>
                  </div>
                  <span class="chart-label-pro">{{ group.name }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- LEADERBOARD -->
          <div class="col-lg-4">
            <div class="glass-card p-4 h-100 shadow-sm border-0 bg-white rounded-4">
              <h6 class="fw-bold mb-4 text-navy uppercase tiny ls-1">Top Candidatures</h6>
              <div v-for="(user, index) in topUsers" :key="index" class="user-rank-card mb-3">
                <div class="rank-number" :class="'rank-' + (index + 1)">{{ index + 1 }}</div>
                <div class="avatar-sm-circle bg-navy text-white fw-bold">{{ user.name.charAt(0) }}</div>
                <div class="flex-grow-1 ms-3">
                  <div class="small fw-800 text-navy">{{ user.name }}</div>
                  <div class="tiny text-muted fw-bold">{{ user.test }}</div>
                </div>
                <div class="score-badge-pro">{{ user.score }}%</div>
              </div>
            </div>
          </div>
        </div>
      </main>

      <div v-else class="text-center py-5">
          <div class="spinner-border text-indigo mt-5"></div>
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

const loading = ref(true);
const downloading = ref(false);
const stats = ref({ kpis: {}, chart: [], leaders: [] });

const kpiArray = computed(() => [
  { label: 'Tests Réussis', value: stats.value.kpis.totalTests || 0, icon: 'fa-solid fa-file-signature', color: '#10B981', bg: '#ecfdf5' },
  { label: 'Moyenne Scores', value: (stats.value.kpis.moyenne?.toFixed(1) || 0) + '%', icon: 'fa-solid fa-chart-bar', color: '#3B82F6', bg: '#eff6ff' },
  { label: 'Processus IA', value: stats.value.kpis.iaProcessed || 0, icon: 'fa-solid fa-microchip', color: '#EAB308', bg: '#fff7ed' },
  { label: 'Indicateur Échec', value: (stats.value.kpis.tauxEchec?.toFixed(1) || 0) + '%', icon: 'fa-solid fa-user-xmark', color: '#EF4444', bg: '#fff1f2' },
]);

const chartData = computed(() => stats.value.chart);
const topUsers = computed(() => stats.value.leaders);

const fetchStats = async () => {
    try {
        const res = await axios.get("http://localhost:5172/api/Dashboard/global-stats");
        stats.value = res.data;
    } finally { loading.value = false; }
};

const downloadPDF = async () => {
    downloading.value = true;
    try {
        const element = document.getElementById('capture-zone');

        const canvas = await html2canvas(element, {
            scale: 2,
            useCORS: true,
            backgroundColor: '#f1f5f9',
            logging: false,
            // CETTE PARTIE CORRIGE L'ERREUR addColorStop
            onclone: (clonedDoc) => {
                // 1. On force l'arrêt de toutes les transitions CSS
                const allElems = clonedDoc.getElementsByTagName("*");
                for (let i = 0; i < allElems.length; i++) {
                    allElems[i].style.transition = "none";
                    allElems[i].style.animation = "none";
                }

                // 2. On cible spécifiquement les barres de graphique
                const bars = clonedDoc.querySelectorAll('.chart-bar-pro');
                bars.forEach(bar => {
                    // On remplace le linear-gradient par une couleur solide
                    // pour éviter le calcul NaN/non-finite sur le Canvas
                    bar.style.background = '#4f46e5'; 
                    bar.style.backgroundImage = 'none';
                    
                    // Si la hauteur est trop petite (0%), on force 1px pour le rendu
                    if (parseInt(bar.style.height) === 0) {
                        bar.style.height = '2px';
                    }
                });
            }
        });

        const imgData = canvas.toDataURL('image/png');
        const pdf = new jsPDF('p', 'mm', 'a4');
        const pdfWidth = pdf.internal.pageSize.getWidth();
        const pdfHeight = (canvas.height * pdfWidth) / canvas.width;

        pdf.addImage(imgData, 'PNG', 0, 0, pdfWidth, pdfHeight);
        pdf.save(`Rapport_${new Date().getTime()}.pdf`);
    } catch (error) {
        console.error("Erreur PDF:", error);
        alert("Erreur de génération du PDF. Réessayez dans un instant.");
    } finally {
        downloading.value = false;
    }
};

onMounted(fetchStats);
</script>

<style scoped>
.admin-body { background-color: #f8fafc; min-height: 100vh; font-family: 'Plus Jakarta Sans', sans-serif; }
.text-navy { color: #0f172a; }
.bg-main-soft { background-color: #f1f5f9; }
.ls-1 { letter-spacing: 1px; }

.icon-vessel-sm { width: 45px; height: 45px; border-radius: 12px; display: flex; align-items: center; justify-content: center; }

.chart-container-pro { height: 280px; padding-bottom: 20px; border-bottom: 1px solid #f1f5f9; }
.chart-column-wrapper { flex: 1; display: flex; flex-direction: column; align-items: center; justify-content: flex-end; }
.chart-bar-pro { 
    width: 38px; 
    background: linear-gradient(to top, #4f46e5, #818cf8); 
    border-radius: 6px 6px 0 0; 
    position: relative; 
    transition: height 0.8s ease-out; /* L'origine du bug est ici, corrigé par le script */
}

.user-rank-card { display: flex; align-items: center; padding: 12px; background: #fafbfc; border: 1px solid #f1f5f9; border-radius: 15px; }
.score-badge-pro { background: #ecfdf5; color: #10b981; padding: 5px 12px; border-radius: 10px; font-weight: 800; font-size: 13px; }
.avatar-sm-circle { width: 34px; height: 34px; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-size: 12px; }

.rank-number { width: 20px; font-weight: 900; color: #cbd5e1; }
.rank-1 { color: #eab308; }

.btn-refresh { background: white; border: 1px solid #e2e8f0; padding: 8px 15px; border-radius: 10px; font-weight: 800; font-size: 11px; cursor: pointer; }
.btn-primary-pro-sm { 
    background: #4f46e5; color: white; border: none; padding: 8px 18px; border-radius: 10px; font-weight: 800; font-size: 11px; cursor: pointer;
}
.btn-primary-pro-sm:disabled { opacity: 0.6; }

.bg-indigo-soft { background: #eef2ff; color: #4f46e5; }
.chart-value-hint { font-size: 10px; font-weight: 900; color: #4f46e5; margin-bottom: 5px; }
</style>