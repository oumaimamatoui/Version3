<template>
  <div class="admin-layout">
    <div class="bg-mesh"></div>
    
    <!-- Navigation latérale -->
    <AppSidebar />

    <div class="main-viewport animate__animated animate__fadeIn">
      <!-- Barre de navigation haute -->
      <AppNavbar />

      <div class="container-fluid px-lg-5 pt-4">
        
        <!-- BREADCRUMB / STATUS BAR -->
        <div class="status-bar mb-4">
          <div class="d-flex justify-content-between align-items-center px-4">
            <div class="breadcrumb-path">
              <span class="root">TABLEAU DE BORD</span>
              <span class="sep">/</span>
              <span class="current">MON HISTORIQUE DES TESTS</span>
            </div>
          </div>
        </div>

        <!-- HEADER SECTION -->
        <div class="d-flex justify-content-between align-items-end mb-5">
          <div>
            <h1 class="page-title mb-1">Mon <span class="accent-word">Historique</span></h1>
            <p class="page-subtitle">Consultez vos scores et l'analyse détaillée de vos sessions terminées.</p>
          </div>
          <div class="stats-row">
            <div class="stat-pill dark">
              <span class="pill-count">{{ historyData.length }}</span>
              <span class="pill-label">Tests validés</span>
            </div>
          </div>
        </div>

        <!-- BARRE DE RECHERCHE ET FILTRES -->
        <div class="glass-card-filter p-3 mb-4 d-flex flex-wrap gap-3 align-items-center shadow-sm">
          <div class="search-vessel flex-grow-1">
            <i class="fa-solid fa-magnifying-glass ms-3 text-muted"></i>
            <input 
              type="text" 
              v-model="searchQuery" 
              placeholder="Rechercher un test par son nom..." 
              class="cyber-input"
            >
          </div>
          <select class="form-select-pro" v-model="filterScore">
            <option value="all">Tous les scores</option>
            <option value="high">Excellence (> 80%)</option>
            <option value="mid">Réussis (50% - 80%)</option>
            <option value="low">À améliorer (< 50%)</option>
          </select>
        </div>

        <!-- LOADER PENDANT LA RÉCUPÉRATION -->
        <div v-if="loading" class="text-center py-5">
          <div class="spinner-ring"></div>
          <p class="mt-4 loading-text">Extraction de vos performances...</p>
        </div>

        <!-- LISTE DES TESTS TERMINÉS -->
        <div v-else>
          <div class="row g-4">
            <!-- État vide -->
            <div v-if="filteredHistory.length === 0" class="col-12">
              <div class="empty-panel py-5">
                <i class="fa-solid fa-卒業生 fa-3x mb-3 opacity-20"></i>
                <p class="m-0">Aucun historique disponible pour le moment.</p>
                <small class="text-muted">Terminez votre premier test pour voir vos résultats ici.</small>
              </div>
            </div>

            <!-- Cartes d'historique -->
            <div v-for="test in filteredHistory" :key="test.id" class="col-12">
              <div class="history-item-card animate__animated animate__fadeInUp">
                
                <!-- Section Identité du Test -->
                <div class="d-flex align-items-center gap-4 flex-grow-1 min-w-0">
                  <div class="score-icon-vessel" :class="getScoreCategory(test.score)">
                    <i class="fa-solid fa-award"></i>
                  </div>
                  <div class="text-truncate">
                    <h5 class="test-name text-truncate">{{ test.nom }}</h5>
                    <div class="test-meta">
                      <span><i class="fa-solid fa-calendar-check me-1"></i> Terminé le {{ formatDate(test.dateFin) }}</span>
                      <span class="mx-3 text-silver">|</span>
                      <span><i class="fa-solid fa-stopwatch me-1"></i> Session de {{ test.dureeMinutes }} min</span>
                    </div>
                  </div>
                </div>

                <!-- Section Score Central -->
                <div class="score-section px-lg-5">
                  <div class="score-label">RÉSULTAT FINAL</div>
                  <div class="score-value" :style="{ color: getScoreColor(test.score) }">
                    {{ test.score }}%
                  </div>
                </div>

                <!-- Section Action (Lien vers la correction) -->
                <div class="action-section">
                  <router-link :to="`/exam-lobby/${test.id}`" class="btn-action-pro">
                    <span>REVOIR L'AUDIT</span>
                    <i class="fa-solid fa-chevron-right"></i>
                  </router-link>
                </div>

              </div>
            </div>
          </div>
        </div>

      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import api from '@/services/api';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';

const loading = ref(true);
const searchQuery = ref('');
const filterScore = ref('all');
const historyData = ref([]);

// Récupération des données depuis le nouvel endpoint
const fetchHistory = async () => {
  loading.value = true;
  try {
    const res = await api.get('/Examen/mon-historique');
    historyData.value = res.data;
  } catch (err) {
    console.error("Erreur lors de la récupération de l'historique:", err);
  } finally {
    loading.value = false;
  }
};

// Filtrage dynamique (Recherche + Score)
const filteredHistory = computed(() => {
  return historyData.value.filter(item => {
    const matchesSearch = item.nom.toLowerCase().includes(searchQuery.value.toLowerCase());
    
    let matchesScore = true;
    if (filterScore.value === 'high') matchesScore = item.score >= 80;
    if (filterScore.value === 'mid') matchesScore = item.score >= 50 && item.score < 80;
    if (filterScore.value === 'low') matchesScore = item.score < 50;

    return matchesSearch && matchesScore;
  });
});

// Utilitaires de formatage
const formatDate = (d) => {
  if (!d) return 'N/A';
  return new Date(d).toLocaleDateString('fr-FR', { 
    day: '2-digit', 
    month: 'long', 
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  });
};

const getScoreColor = (score) => {
  if (score >= 80) return '#10b981'; // Vert
  if (score >= 50) return '#f59e0b'; // Ambre
  return '#ef4444'; // Rouge
};

const getScoreCategory = (score) => {
  if (score >= 80) return 'cat-high';
  if (score >= 50) return 'cat-mid';
  return 'cat-low';
};

onMounted(fetchHistory);
</script>

<style scoped>
/* ─── Layout & Fond ─── */
.admin-layout { min-height: 100vh; display: flex; background: #f8fafc; position: relative; }
.bg-mesh {
  position: fixed; inset: 0; z-index: 0; pointer-events: none;
  background: radial-gradient(circle at 80% 20%, rgba(251, 191, 36, 0.08) 0%, transparent 50%),
              linear-gradient(160deg, #f8fafc 0%, #f1f5f9 100%);
}
.main-viewport { flex-grow: 1; z-index: 5; position: relative; padding-bottom: 50px; }

/* ─── Barre de statut ─── */
.status-bar {
  background: rgba(255, 255, 255, 0.8); backdrop-filter: blur(10px);
  border: 1px solid white; border-radius: 16px; padding: 12px 0;
  box-shadow: 0 4px 15px rgba(0,0,0,0.02);
}
.breadcrumb-path { font-size: 10px; font-weight: 800; letter-spacing: 1.2px; color: #94a3b8; }

/* ─── Titres ─── */
.page-title { font-size: 32px; font-weight: 900; color: #0f172a; letter-spacing: -0.5px; }
.accent-word { color: #fbbf24; position: relative; }
.page-subtitle { color: #64748b; font-size: 15px; }

/* ─── Filtres Glass ─── */
.glass-card-filter {
  background: white; border-radius: 20px; border: 1px solid #e2e8f0;
}
.search-vessel { display: flex; align-items: center; background: #f1f5f9; border-radius: 12px; }
.cyber-input { border: none; background: transparent; outline: none; width: 100%; padding: 12px; font-size: 14px; color: #1e293b; }
.form-select-pro {
  border: 1px solid #e2e8f0; border-radius: 12px; padding: 10px 20px; font-size: 14px; font-weight: 600; color: #475569; outline: none; cursor: pointer;
}

/* ─── Cartes Historique ─── */
.history-item-card {
  background: white; border: 1px solid #f1f5f9; border-radius: 24px;
  padding: 24px 35px; display: flex; align-items: center; justify-content: space-between;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}
.history-item-card:hover {
  transform: translateY(-5px) scale(1.005);
  box-shadow: 0 20px 40px rgba(15, 23, 42, 0.06);
  border-color: #fbbf24;
}

.score-icon-vessel {
  width: 56px; height: 56px; border-radius: 18px;
  display: flex; align-items: center; justify-content: center; font-size: 1.4rem;
}
.cat-high { background: #ecfdf5; color: #10b981; box-shadow: 0 10px 20px rgba(16, 185, 129, 0.1); }
.cat-mid { background: #fffbeb; color: #f59e0b; box-shadow: 0 10px 20px rgba(245, 158, 11, 0.1); }
.cat-low { background: #fef2f2; color: #ef4444; box-shadow: 0 10px 20px rgba(239, 68, 68, 0.1); }

.test-name { font-weight: 800; color: #1e293b; margin: 0; font-size: 1.2rem; }
.test-meta { font-size: 13px; color: #94a3b8; font-weight: 600; margin-top: 6px; }

.score-section { text-align: center; border-left: 1px solid #f1f5f9; border-right: 1px solid #f1f5f9; min-width: 200px; }
.score-label { font-size: 10px; font-weight: 800; color: #cbd5e1; letter-spacing: 1.5px; margin-bottom: 5px; }
.score-value { font-size: 32px; font-weight: 950; line-height: 1; }

.btn-action-pro {
  background: #0f172a; color: white; text-decoration: none;
  padding: 14px 28px; border-radius: 16px; font-weight: 700; font-size: 13px;
  display: flex; align-items: center; gap: 12px; transition: 0.3s;
}
.btn-action-pro:hover { background: #fbbf24; color: #0f172a; transform: scale(1.05); }

/* ─── Utils ─── */
.stat-pill.dark { background: #0f172a; color: white; padding: 12px 25px; border-radius: 100px; box-shadow: 0 10px 20px rgba(15, 23, 42, 0.2); }
.pill-count { color: #fbbf24; font-weight: 900; font-size: 1.4rem; margin-right: 10px; }
.pill-label { font-size: 11px; font-weight: 700; text-transform: uppercase; letter-spacing: 1px; opacity: 0.8; }

.spinner-ring {
  width: 45px; height: 45px; border: 4px solid #e2e8f0;
  border-top-color: #fbbf24; border-radius: 50%; animation: spin 0.8s linear infinite;
  display: inline-block;
}
@keyframes spin { to { transform: rotate(360deg); } }

.empty-panel { 
    text-align: center; color: #94a3b8; font-weight: 600; 
    border: 2px dashed #e2e8f0; border-radius: 30px; 
    background: rgba(255,255,255,0.4);
}

@media (max-width: 992px) {
  .history-item-card { flex-direction: column; text-align: center; gap: 25px; }
  .score-section { border: none; padding: 0; }
  .action-section { width: 100%; }
  .btn-action-pro { justify-content: center; }
}
</style>