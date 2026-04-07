<template>
  <div class="d-flex admin-body">
    <!-- 1. SIDEBAR -->
    <AppSidebar />

    <div class="content flex-grow-1">
      <!-- 2. NAVBAR -->
      <AppNavbar />

      <main class="p-4 animate-fade-in">
        <!-- HEADER SECTION -->
        <div class="d-flex justify-content-between align-items-end mb-4">
          <div>
            <nav class="breadcrumb-cyber mb-2">TABLEAU DE BORD / MON_HISTORIQUE</nav>
            <h2 class="fw-800 text-navy m-0">
              <i class="fa-solid fa-clock-rotate-left me-2 text-amber"></i> 
              Mon <span>Historique</span>
            </h2>
          </div>
          <div class="text-end d-none d-md-block">
            <span class="badge-status actives px-3">
              {{ history.length }} TESTS TERMINÉS
            </span>
          </div>
        </div>

        <!-- FILTRES & RECHERCHE (Look Pro) -->
        <div class="glass-card p-3 mb-4 d-flex flex-wrap gap-3 align-items-center shadow-sm border-0">
          <div class="search-vessel flex-grow-1">
            <i class="fa-solid fa-magnifying-glass ms-3 text-muted"></i>
            <input type="text" v-model="searchQuery" placeholder="Rechercher un test ou un groupe..." class="cyber-input">
          </div>
          <select class="form-select-pro" v-model="filterScore">
            <option value="all">Tous les scores</option>
            <option value="high">Plus de 80%</option>
            <option value="medium">50% - 80%</option>
            <option value="low">Moins de 50%</option>
          </select>
        </div>

        <!-- LISTE DE L'HISTORIQUE -->
        <div class="history-stack">
          <TransitionGroup name="list">
            <div class="col-12 mb-3" v-for="test in filteredHistory" :key="test.id">
              <div class="history-card glass-card p-4 d-flex flex-wrap justify-content-between align-items-center">
                
                <!-- Info Test -->
                <div class="d-flex align-items-center gap-4 flex-grow-1">
                  <div class="icon-vessel-lg shadow-sm" :class="getScoreClass(test.score)">
                    <i class="fa-solid fa-file-invoice"></i>
                  </div>
                  <div>
                    <h5 class="fw-bold text-navy m-0">{{ test.title }}</h5>
                    <div class="d-flex gap-3 mt-1 align-items-center">
                      <span class="tiny fw-bold text-muted uppercase"><i class="fa-solid fa-layer-group me-1"></i> {{ test.group }}</span>
                      <span class="tiny fw-bold text-muted uppercase"><i class="fa-solid fa-calendar-day me-1"></i> {{ test.date }}</span>
                    </div>
                  </div>
                </div>

                <!-- Métriques Techniques -->
                <div class="d-flex gap-5 px-4 my-3 my-lg-0 border-start-cyber border-end-cyber">
                  <div class="text-center">
                    <div class="tiny fw-bold text-muted uppercase mb-1">Questions</div>
                    <div class="fw-800 text-navy">{{ test.qCount }}</div>
                  </div>
                  <div class="text-center">
                    <div class="tiny fw-bold text-muted uppercase mb-1">Durée</div>
                    <div class="fw-bold text-navy">{{ test.timeSpent }} min</div>
                  </div>
                </div>

                <!-- Score IA & Action -->
                <div class="d-flex align-items-center gap-4 ps-lg-4">
                  <div class="text-center">
                    <div class="tiny fw-bold text-muted uppercase mb-1">Index IA</div>
                    <div class="h3 fw-800 m-0" :style="{ color: getScoreColor(test.score) }">
                      {{ test.score }}%
                    </div>
                  </div>
                  <router-link to="/results" class="btn-ia-action">
                    <span>ANALYSE IA</span>
                    <i class="fa-solid fa-wand-magic-sparkles"></i>
                  </router-link>
                </div>

              </div>
            </div>
          </TransitionGroup>
        </div>

        <!-- EMPTY STATE -->
        <div v-if="filteredHistory.length === 0" class="text-center py-5">
          <i class="fa-solid fa-box-open fa-3x text-muted opacity-20 mb-3"></i>
          <p class="text-muted">Aucun test ne correspond à vos critères.</p>
        </div>
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';

const searchQuery = ref('');
const filterScore = ref('all');

const history = ref([
  { id: 1, title: 'Architecture Cloud Senior', group: 'Tech IT', score: 85, date: '15 Fév 2026', qCount: 40, timeSpent: 42 },
  { id: 2, title: 'Logique & Algorithmique', group: 'Stagiaires', score: 62, date: '02 Fév 2026', qCount: 20, timeSpent: 15 },
  { id: 3, title: 'Sécurité Réseaux v2', group: 'Tech IT', score: 45, date: '20 Jan 2026', qCount: 30, timeSpent: 28 },
]);

const filteredHistory = computed(() => {
  return history.value.filter(item => {
    const matchesSearch = item.title.toLowerCase().includes(searchQuery.value.toLowerCase()) || 
                         item.group.toLowerCase().includes(searchQuery.value.toLowerCase());
    
    let matchesScore = true;
    if (filterScore.value === 'high') matchesScore = item.score >= 80;
    else if (filterScore.value === 'medium') matchesScore = item.score >= 50 && item.score < 80;
    else if (filterScore.value === 'low') matchesScore = item.score < 50;

    return matchesSearch && matchesScore;
  });
});

const getScoreColor = (score) => {
  if (score >= 80) return '#10B981'; // Success
  if (score >= 50) return '#EAB308'; // Warning/Amber
  return '#EF4444'; // Danger
};

const getScoreClass = (score) => {
  if (score >= 80) return 'bg-success-soft text-success';
  if (score >= 50) return 'bg-amber-soft text-amber';
  return 'bg-danger-soft text-danger';
};
</script>

<style scoped>
/* --- TYPOGRAPHIE --- */
.breadcrumb-cyber { font-family: monospace; font-size: 10px; color: #94a3b8; letter-spacing: 2px; }
.fw-800 { font-weight: 800; }
.ls-1 { letter-spacing: 1px; }

/* --- CARDS & ITEMS --- */
.history-card {
  border: 1px solid #f1f5f9;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}
.history-card:hover {
  transform: translateY(-3px) scale(1.005);
  box-shadow: 0 15px 30px -10px rgba(0,0,0,0.05) !important;
  border-color: #EAB308;
}

.icon-vessel-lg {
  width: 55px; height: 55px;
  border-radius: 16px;
  display: flex; align-items: center; justify-content: center;
  font-size: 1.5rem;
}

.bg-success-soft { background: #ecfdf5; color: #10b981; }
.bg-amber-soft { background: #fffbeb; color: #eab308; }
.bg-danger-soft { background: #fff1f2; color: #ef4444; }

/* --- INPUTS PRO --- */
.cyber-input {
  flex: 1; background: transparent; border: none; padding: 10px;
  font-size: 14px; outline: none; color: #0F172A;
}
.form-select-pro {
  background-color: #f8fafc; border: 1px solid #e2e8f0;
  border-radius: 10px; padding: 8px 15px; font-size: 13px;
  font-weight: 700; color: #64748b; outline: none;
}

/* --- BUTTON IA --- */
.btn-ia-action {
  background: #0F172A; color: white;
  text-decoration: none; padding: 12px 20px;
  border-radius: 50px; font-weight: 800; font-size: 11px;
  display: flex; align-items: center; gap: 10px;
  transition: 0.3s;
  border: 2px solid transparent;
}
.btn-ia-action:hover {
  background: #EAB308; color: #0F172A;
  box-shadow: 0 5px 15px rgba(234, 179, 8, 0.3);
}

/* --- UTILS --- */
.border-start-cyber { border-left: 1px solid #f1f5f9; }
.border-end-cyber { border-right: 1px solid #f1f5f9; }

/* --- ANIMATIONS --- */
.list-move, .list-enter-active, .list-leave-active { transition: all 0.4s ease; }
.list-enter-from, .list-leave-to { opacity: 0; transform: translateX(30px); }
</style>