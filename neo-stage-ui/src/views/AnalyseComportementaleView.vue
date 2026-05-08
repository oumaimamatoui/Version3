<template>
  <div class="enigma-master-root d-flex overflow-hidden" @mousemove="handleParallax">

    <!-- BACKGROUND EFFECTS -->
    <div class="cyber-engine-bg">
      <div class="bg-vignette"></div>
      <div class="glow-orb orb-amber" :style="orbStyle(0.04)"></div>
      <div class="glow-orb orb-blue"  :style="orbStyle(0.015)"></div>
      <div class="quantum-grid"></div>
    </div>

    <AppSidebar />

    <div class="main-orchestrator flex-grow-1 d-flex flex-column position-relative">
      <AppNavbar />

      <main class="canvas-engine flex-grow-1 overflow-auto custom-scrollbar">

        <!-- ═══════ DASHBOARD VIEW ═══════ -->
        <div class="dashboard-view animate__animated animate__fadeIn p-4 p-lg-5">

          <!-- HEADER -->
          <header class="d-flex justify-content-between align-items-end mb-5 flex-wrap gap-3">
            <div>
              <div class="breadcrumb-pro mb-2">
                <span class="root">Administration</span>
                <i class="fa-solid fa-chevron-right mx-2 separator"></i>
                <span class="current">Terminal Analyses IA</span>
              </div>
              <h2 class="premium-title">Analyses &amp; <span class="gradient-text">Intelligence</span></h2>
            </div>
            <div class="d-flex gap-3 flex-wrap align-items-center">
              <button class="btn-refresh-pro" @click="initOrchestrator" :disabled="loading" :title="'Rafraîchir'">
                <i class="fa-solid fa-rotate" :class="{ 'fa-spin': loading }"></i>
              </button>
              <div class="view-toggle-cluster">
                <button :class="['btn-view-toggle', { active: viewMode === 'grid' }]" @click="viewMode = 'grid'">
                  <i class="fa-solid fa-table-cells-large"></i>
                </button>
                <button :class="['btn-view-toggle', { active: viewMode === 'list' }]" @click="viewMode = 'list'">
                  <i class="fa-solid fa-list-ul"></i>
                </button>
              </div>
            </div>
          </header>

          <!-- KPI STATS -->
          <div class="row g-4 mb-5">
            <div class="col-xl-3 col-md-6" v-for="stat in kpiStats" :key="stat.label">
              <div class="stat-card-premium">
                <div class="stat-icon-wrapper" :style="{ background: stat.bg, color: stat.color }">
                  <i :class="stat.icon"></i>
                </div>
                <div class="stat-details">
                  <div class="stat-value">{{ stat.value }}</div>
                  <div class="stat-label">{{ stat.label }}</div>
                </div>
              </div>
            </div>
          </div>

          <!-- FILTERS -->
          <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-3">
            <div class="tabs-container">
              <div class="d-flex gap-2 p-1 bg-white rounded-4 shadow-sm border">
                <button
                  v-for="tab in filterTabs"
                  :key="tab.label"
                  class="nav-tab-btn-modern"
                  :class="{ active: activeTab === tab.value }"
                  @click="activeTab = tab.value"
                >
                  {{ tab.label }} <span class="tab-count">{{ tab.count }}</span>
                </button>
              </div>
            </div>
            <div class="search-inline-box">
              <i class="fa-solid fa-magnifying-glass"></i>
              <input
                type="text"
                v-model="dashboardSearch"
                placeholder="Rechercher un candidat..."
                class="search-inline-input"
              />
            </div>
          </div>

          <!-- LOADING STATE -->
          <div v-if="loading" class="col-12 text-center py-5">
            <div class="spinner-pro-premium"></div>
            <p class="mt-3 text-muted fw-700" style="font-size:0.85rem;">CHARGEMENT DES ANALYSES...</p>
          </div>

          <!-- EMPTY STATE -->
          <div v-else-if="filteredAnalyses.length === 0" class="empty-state-box text-center py-5">
            <i class="fa-solid fa-brain fa-3x mb-3" style="color:#e2e8f0;"></i>
            <p class="text-muted fw-700">Aucune analyse trouvée.</p>
          </div>

          <!-- ── GRID VIEW ── -->
          <div v-else-if="viewMode !== 'list'" class="row g-4">
            <div
              v-for="a in filteredAnalyses"
              :key="a.id"
              class="col-xl-4 col-md-6 animate__animated animate__fadeInUp"
            >
              <div class="campaign-card-modern" @click="goToDetail(a)">
                <div class="card-header-modern mb-3 d-flex justify-content-between align-items-start">
                  <span class="status-badge" :class="getTierClass(a.neural_tier)">
                    <span class="status-dot"></span> {{ a.neural_tier }}
                  </span>
                  <button class="btn-options-round" @click.stop="handleDelete(a.id)" title="Supprimer">
                    <i class="fa-solid fa-trash-can"></i>
                  </button>
                </div>
                <h5 class="campaign-title-modern fw-800">{{ a.candidat_nom }}</h5>
                <p class="text-muted small mb-0">{{ a.profile_type }}</p>

                <div class="progress-slim mb-2 mt-3">
                  <div
                    class="progress-fill"
                    :style="{ width: a.global_score + '%', background: getScoreColor(a.global_score) }"
                  ></div>
                </div>
                <div class="d-flex justify-content-between align-items-center">
                  <span class="fw-800 small" :style="{ color: getScoreColor(a.global_score) }">
                    {{ a.global_score > 0 ? a.global_score + '%' : 'Non évalué' }}
                  </span>
                  <span class="small text-muted">{{ formatDate(a.created_at) }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- ── LIST VIEW ── -->
          <div v-else class="list-view-pro">
            <div class="list-header d-flex align-items-center px-4 py-2 mb-2">
              <div class="flex-grow-1" style="font-size:0.7rem;font-weight:800;color:#94a3b8;text-transform:uppercase;">Candidat</div>
              <div class="flex-grow-1" style="font-size:0.7rem;font-weight:800;color:#94a3b8;text-transform:uppercase;">Profil</div>
              <div class="px-3" style="font-size:0.7rem;font-weight:800;color:#94a3b8;text-transform:uppercase;">Score</div>
              <div style="width:40px;"></div>
            </div>
            <div
              v-for="a in filteredAnalyses"
              :key="a.id"
              class="list-row-item d-flex align-items-center px-4 py-3 mb-2"
              @click="goToDetail(a)"
            >
              <div class="flex-grow-1 fw-800">{{ a.candidat_nom }}</div>
              <div class="flex-grow-1 text-muted small">{{ a.profile_type }}</div>
              <div class="px-3">
                <span
                  class="slot-badge"
                  :style="{ background: getScoreColor(a.global_score) + '20', color: getScoreColor(a.global_score) }"
                >
                  {{ a.global_score > 0 ? a.global_score + '%' : '—' }}
                </span>
              </div>
              <button class="btn-icon-sm" @click.stop="goToDetail(a)" title="Voir le détail">
                <i class="fa-solid fa-eye"></i>
              </button>
            </div>
          </div>

        </div>
      </main>
    </div>

    <!-- TOAST NOTIFICATION -->
    <transition name="toast-slide">
      <div v-if="globalToast.active" class="enigma-toast" :class="globalToast.type">
        <i :class="globalToast.icon"></i> {{ globalToast.message }}
      </div>
    </transition>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import api from '@/services/api';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar  from '../components/AppNavbar.vue';

// ─── STATE ───────────────────────────────────────────────────────
const router        = useRouter();
const loading       = ref(true);
const viewMode      = ref('grid');
const dashboardSearch = ref('');
const activeTab     = ref('all');
const analyses      = ref([]);
const globalToast   = reactive({ active: false, message: '', type: '', icon: '' });
const mousePos      = reactive({ x: 0, y: 0 });

// ─── CHARGEMENT PRINCIPAL ────────────────────────────────────────
// Stratégie corrigée :
//   1. Récupérer la liste des candidats
//   2. Pour chaque candidat, appeler l'endpoint résultats individuel
//   3. Calculer le score réel depuis les données retournées
// ────────────────────────────────────────────────────────────────
const initOrchestrator = async () => {
  loading.value = true;
  try {
    // Étape 1 : liste des candidats
    const { data: candidats } = await api.get('/Candidates');

    if (!Array.isArray(candidats) || candidats.length === 0) {
      analyses.value = [];
      return;
    }

    // Étape 2 : pour chaque candidat, récupérer ses résultats
    const enriched = await Promise.allSettled(
      candidats.map(c => enrichCandidat(c))
    );

    analyses.value = enriched
      .filter(r => r.status === 'fulfilled')
      .map(r => r.value);

  } catch (err) {
    showToast('Erreur de connexion API', 'error');
    console.error('initOrchestrator error:', err);
  } finally {
    loading.value = false;
  }
};

// ─── ENRICHISSEMENT D'UN CANDIDAT ───────────────────────────────
// Tente plusieurs endpoints pour trouver les résultats d'examen
// du candidat, puis calcule son score réel.
// ────────────────────────────────────────────────────────────────
const enrichCandidat = async (c) => {
  const id = c.id || c.Id;
  let tests = [];

  // Liste des endpoints à essayer pour les résultats
  const endpointsToTry = [
    `/Examen/resultats/${id}`,
    `/Examen/historique/${id}`,
    `/Examen/candidate-report/${id}`,
    `/ExamenCandidats/${id}`,
    `/Results/${id}`,
  ];

  for (const ep of endpointsToTry) {
    try {
      const { data } = await api.get(ep);
      if (data) {
        tests = Array.isArray(data) ? data : [data];
        if (tests.length > 0) break; // On a trouvé des données, on arrête
      }
    } catch (_) {
      // Endpoint non disponible, on essaie le suivant
    }
  }

  // Calcul du score global réel
  let globalScore = 0;
  if (tests.length > 0) {
    const scores = tests.map(t =>
      Number(
        t.scoreGlobal   ??
        t.scoreTotal    ??
        t.score         ??
        t.Score         ??
        t.ScoreGlobal   ??
        t.pourcentage   ??
        0
      )
    ).filter(s => s > 0);

    if (scores.length > 0) {
      globalScore = Math.round(scores.reduce((a, b) => a + b, 0) / scores.length);
    }
  } else {
    // Fallback : score stocké directement sur le candidat
    globalScore = Number(
      c.global_score  ??
      c.score         ??
      c.scoreGlobal   ??
      c.Score         ??
      0
    );
  }

  const tier =
    globalScore >= 85 ? 'Élite' :
    globalScore >= 70 ? 'Standard' : 'Basique';

  return {
    id,
    candidat_nom:  c.name || c.fullName || c.nom || c.Name || 'Candidat',
    profile_type:  c.group || c.campaignName || c.poste || detectProfile(globalScore, tests),
    neural_tier:   tier,
    global_score:  globalScore,
    created_at:    c.created_at || c.createdAt || c.CreatedAt || new Date().toISOString(),
    tests_count:   tests.length,
    // Stocker les données brutes pour la page détail
    _raw_candidate: c,
    _raw_tests:     tests,
  };
};

// ─── DÉTECTION DU PROFIL (fallback) ─────────────────────────────
const detectProfile = (score, tests) => {
  if (tests.length === 0) return 'À évaluer';
  const themes = [...new Set(tests.map(t => t.theme || t.Theme || t.categorie).filter(Boolean))];
  if (themes.length > 0) return themes.slice(0, 2).join(' · ');
  return score >= 85 ? 'Profil Expert' : score >= 70 ? 'Profil Confirmé' : 'Profil Junior';
};

// ─── NAVIGATION VERS LE DÉTAIL ───────────────────────────────────
const goToDetail = (a) => {
  router.push({
    path: `/analyse-comportementale/${a.id}`,
    query: {
      score: a.global_score,
      nom:   a.candidat_nom,
      tier:  a.neural_tier,
    },
  });
};

// ─── SUPPRESSION ────────────────────────────────────────────────
const handleDelete = async (id) => {
  if (!confirm('Supprimer cette analyse ?')) return;
  try {
    await api.delete(`/Candidates/${id}`);
    analyses.value = analyses.value.filter(a => a.id !== id);
    showToast('Analyse supprimée', 'warn');
  } catch {
    // Suppression locale si l'API n'a pas d'endpoint DELETE
    analyses.value = analyses.value.filter(a => a.id !== id);
    showToast('Analyse retirée de la liste', 'warn');
  }
};

// ─── COMPUTED ────────────────────────────────────────────────────
const filteredAnalyses = computed(() => {
  let list = [...analyses.value];

  if (activeTab.value === 'elite')    list = list.filter(a => a.neural_tier === 'Élite');
  if (activeTab.value === 'standard') list = list.filter(a => a.neural_tier === 'Standard');
  if (activeTab.value === 'basique')  list = list.filter(a => a.neural_tier === 'Basique');

  if (dashboardSearch.value.trim()) {
    const q = dashboardSearch.value.toLowerCase().trim();
    list = list.filter(a =>
      a.candidat_nom.toLowerCase().includes(q) ||
      a.profile_type.toLowerCase().includes(q)
    );
  }
  return list;
});

const filterTabs = computed(() => [
  { label: 'Tout',     value: 'all',      count: analyses.value.length },
  { label: 'Élite',    value: 'elite',    count: analyses.value.filter(a => a.neural_tier === 'Élite').length },
  { label: 'Standard', value: 'standard', count: analyses.value.filter(a => a.neural_tier === 'Standard').length },
  { label: 'Basique',  value: 'basique',  count: analyses.value.filter(a => a.neural_tier === 'Basique').length },
]);

const kpiStats = computed(() => {
  const avecScore = analyses.value.filter(a => a.global_score > 0);
  const avgScore  = avecScore.length
    ? Math.round(avecScore.reduce((s, a) => s + a.global_score, 0) / avecScore.length)
    : null;

  return [
    {
      label: 'Analyses',
      value: analyses.value.length,
      icon: 'fa-solid fa-brain',
      color: '#f59e0b', bg: '#fffbeb',
    },
    {
      label: 'Score Moyen Réel',
      value: avgScore !== null ? avgScore + '%' : '—',
      icon: 'fa-solid fa-chart-line',
      color: '#10b981', bg: '#ecfdf5',
    },
    {
      label: 'Tier Élite',
      value: analyses.value.filter(a => a.neural_tier === 'Élite').length,
      icon: 'fa-solid fa-crown',
      color: '#6366f1', bg: '#eef2ff',
    },
    {
      label: 'Sans données',
      value: analyses.value.filter(a => a.global_score === 0).length,
      icon: 'fa-solid fa-clock',
      color: '#f43f5e', bg: '#fff1f2',
    },
  ];
});

// ─── HELPERS ────────────────────────────────────────────────────
const getScoreColor = (s) => {
  if (!s || s === 0) return '#94a3b8';
  return s >= 85 ? '#10b981' : s >= 70 ? '#f59e0b' : '#f43f5e';
};

const getTierClass = (t) =>
  t === 'Élite' ? 'status-1' : t === 'Standard' ? 'status-0' : 'status-2';

const formatDate = (d) =>
  d ? new Date(d).toLocaleDateString('fr-FR', { day: '2-digit', month: 'short' }) : '—';

const showToast = (msg, type) => {
  const icons = { error: 'fa-solid fa-circle-xmark', warn: 'fa-solid fa-triangle-exclamation', success: 'fa-solid fa-circle-check' };
  globalToast.message = msg;
  globalToast.type    = `t-${type}`;
  globalToast.icon    = icons[type] || icons.error;
  globalToast.active  = true;
  setTimeout(() => { globalToast.active = false; }, 3500);
};

const handleParallax = (e) => {
  mousePos.x = (e.clientX - window.innerWidth  / 2) / 30;
  mousePos.y = (e.clientY - window.innerHeight / 2) / 30;
};

const orbStyle = (f) => ({
  transform: `translate(${mousePos.x * f * 10}px, ${mousePos.y * f * 10}px)`,
});

onMounted(initOrchestrator);
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&family=JetBrains+Mono:wght@400;500;700&display=swap');

/* ── ROOT ── */
.enigma-master-root {
  min-height: 100vh;
  background: #f8fafc;
  font-family: 'Plus Jakarta Sans', sans-serif;
  color: #0f172a;
}

/* ── BACKGROUND ── */
.cyber-engine-bg { position: fixed; inset: 0; z-index: 0; pointer-events: none; }
.quantum-grid {
  position: absolute; inset: 0;
  background-image: radial-gradient(#cbd5e1 1px, transparent 1px);
  background-size: 40px 40px;
  opacity: 0.2;
}
.glow-orb {
  position: absolute; width: 600px; height: 600px;
  filter: blur(120px); opacity: 0.15; border-radius: 50%;
  transition: transform 0.3s ease-out;
}
.orb-amber { background: #f59e0b; top: -200px; right: -100px; }
.orb-blue  { background: #6366f1; bottom: -200px; left: -100px; }

/* ── LAYOUT ── */
.main-orchestrator { z-index: 5; }
.canvas-engine { height: calc(100vh - 64px); }

/* ── TYPOGRAPHY ── */
.premium-title { font-weight: 800; font-size: 2.2rem; letter-spacing: -1px; }
.gradient-text {
  background: linear-gradient(135deg, #f59e0b, #fbbf24);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}
.breadcrumb-pro { font-size: 0.72rem; font-weight: 700; color: #94a3b8; }
.breadcrumb-pro .separator { font-size: 0.55rem; opacity: 0.5; }
.breadcrumb-pro .current { color: #0f172a; font-weight: 800; }

/* ── KPI CARDS ── */
.stat-card-premium {
  background: white; border-radius: 24px; padding: 24px;
  display: flex; align-items: center;
  border: 1px solid #eef2f6;
  box-shadow: 0 4px 12px rgba(0,0,0,0.03);
  transition: transform 0.2s, box-shadow 0.2s;
}
.stat-card-premium:hover { transform: translateY(-3px); box-shadow: 0 8px 24px rgba(0,0,0,0.06); }
.stat-icon-wrapper {
  width: 50px; height: 50px; border-radius: 14px;
  display: flex; align-items: center; justify-content: center;
  font-size: 1.2rem; margin-right: 15px; flex-shrink: 0;
}
.stat-value { font-size: 1.4rem; font-weight: 800; }
.stat-label { font-size: 0.7rem; color: #94a3b8; text-transform: uppercase; font-weight: 700; }

/* ── GRID CARDS ── */
.campaign-card-modern {
  background: white; border-radius: 24px; padding: 25px;
  border: 1.5px solid #eef2f6; cursor: pointer;
  transition: all 0.3s ease; height: 100%;
}
.campaign-card-modern:hover {
  transform: translateY(-5px);
  box-shadow: 0 12px 30px rgba(0,0,0,0.08);
  border-color: #fbbf24;
}
.campaign-title-modern { font-size: 1rem; font-weight: 800; color: #0f172a; }
.progress-slim { height: 6px; background: #f1f5f9; border-radius: 10px; overflow: hidden; }
.progress-fill { height: 100%; border-radius: 10px; transition: width 1s ease; }

.btn-options-round {
  width: 32px; height: 32px; border-radius: 10px;
  border: 1.5px solid #eef2f6; background: white;
  cursor: pointer; font-size: 0.8rem; color: #94a3b8;
  display: flex; align-items: center; justify-content: center;
  transition: 0.2s;
}
.btn-options-round:hover { background: #fff1f2; color: #f43f5e; border-color: #fecdd3; }

/* ── LIST VIEW ── */
.list-header { background: #f8fafc; border-radius: 12px; }
.list-row-item {
  background: white; border: 1.5px solid #eef2f6;
  border-radius: 16px; cursor: pointer; transition: 0.2s;
}
.list-row-item:hover { border-color: #fbbf24; background: #fffbeb; }
.slot-badge { padding: 4px 10px; border-radius: 8px; font-weight: 800; font-size: 0.7rem; }
.btn-icon-sm {
  width: 32px; height: 32px; border-radius: 10px;
  border: 1.5px solid #eef2f6; background: white; color: #64748b;
  cursor: pointer; font-size: .75rem;
  display: flex; align-items: center; justify-content: center;
  transition: 0.2s;
}
.btn-icon-sm:hover { background: #f8fafc; color: #0f172a; border-color: #cbd5e1; }

/* ── STATUS BADGES ── */
.status-badge {
  font-size: 0.6rem; font-weight: 800; padding: 4px 10px;
  border-radius: 8px; text-transform: uppercase;
  display: inline-flex; align-items: center; gap: 4px;
}
.status-dot { width: 6px; height: 6px; border-radius: 50%; background: currentColor; }
.status-1 { background: #ecfdf5; color: #10b981; }
.status-0 { background: #eef2ff; color: #6366f1; }
.status-2 { background: #f1f5f9; color: #64748b; }

/* ── FILTER / TABS ── */
.view-toggle-cluster {
  display: flex; background: white;
  border: 1.5px solid #e2e8f0; border-radius: 16px;
  padding: 4px; gap: 4px;
}
.btn-view-toggle {
  width: 38px; height: 38px; border-radius: 12px;
  border: none; background: transparent; color: #94a3b8;
  cursor: pointer; transition: 0.3s;
  display: flex; align-items: center; justify-content: center;
}
.btn-view-toggle.active { background: #0f172a; color: #fbbf24; }

.btn-refresh-pro {
  width: 44px; height: 44px; background: white;
  border: 1.5px solid #e2e8f0; border-radius: 14px;
  color: #64748b; cursor: pointer; transition: 0.3s;
  display: flex; align-items: center; justify-content: center;
}
.btn-refresh-pro:hover:not(:disabled) { border-color: #fbbf24; color: #f59e0b; }
.btn-refresh-pro:disabled { opacity: 0.5; cursor: not-allowed; }

.search-inline-box {
  display: flex; align-items: center;
  background: white; border: 1.5px solid #eef2f6;
  border-radius: 14px; padding: 0 14px; gap: 10px; color: #94a3b8;
}
.search-inline-input {
  border: none; outline: none; background: transparent;
  padding: 10px 0; font-weight: 700; font-size: 0.85rem;
  width: 220px; font-family: inherit;
}

.nav-tab-btn-modern {
  padding: 8px 18px; border-radius: 12px;
  border: none; background: transparent;
  font-weight: 800; font-size: 0.8rem; color: #94a3b8;
  cursor: pointer; transition: 0.2s; font-family: inherit;
}
.nav-tab-btn-modern.active { background: #0f172a; color: white; }
.tab-count {
  background: #f1f5f9; color: #64748b;
  padding: 2px 7px; border-radius: 8px; font-size: 0.65rem; margin-left: 6px;
}
.nav-tab-btn-modern.active .tab-count { background: rgba(255,255,255,0.15); color: white; }

/* ── EMPTY STATE ── */
.empty-state-box { color: #94a3b8; }

/* ── SPINNER ── */
.spinner-pro-premium {
  width: 50px; height: 50px;
  border: 4px solid #f1f5f9;
  border-top: 4px solid #f59e0b;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 40px auto;
}
@keyframes spin { to { transform: rotate(360deg); } }

/* ── TOAST ── */
.enigma-toast {
  position: fixed; bottom: 30px; right: 30px;
  padding: 15px 25px; border-radius: 14px;
  background: #0f172a; color: white;
  z-index: 10000; box-shadow: 0 10px 30px rgba(0,0,0,0.25);
  display: flex; align-items: center; gap: 10px;
  font-weight: 700; font-size: 0.85rem;
}
.t-error   { border-left: 4px solid #f43f5e; }
.t-warn    { border-left: 4px solid #f59e0b; }
.t-success { border-left: 4px solid #10b981; }

.toast-slide-enter-active { animation: slideIn .4s ease-out; }
.toast-slide-leave-active { animation: slideIn .3s ease-in reverse; }
@keyframes slideIn {
  from { transform: translateX(120%); opacity: 0; }
  to   { transform: translateX(0);    opacity: 1; }
}

/* ── UTILITIES ── */
.fw-800 { font-weight: 800 !important; }
.fw-700 { font-weight: 700 !important; }
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: #eef2f6; border-radius: 10px; }
</style>