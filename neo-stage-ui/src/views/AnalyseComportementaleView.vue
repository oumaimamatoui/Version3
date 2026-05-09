<template>
  <div class="analyse-root d-flex overflow-hidden" @mousemove="handleParallax">

    <!-- ── BACKGROUND EFFECTS (respecte le thème) ── -->
    <div class="ac-bg-layer" aria-hidden="true">
      <div class="ac-orb ac-orb-amber" :style="orbStyle(0.04)"></div>
      <div class="ac-orb ac-orb-blue"  :style="orbStyle(0.015)"></div>
      <div class="ac-grid-dots"></div>
    </div>

    <AppSidebar />

    <div class="ac-main flex-grow-1 d-flex flex-column position-relative">
      <AppNavbar />

      <main class="ac-canvas flex-grow-1 overflow-auto">
        <div class="ac-inner p-4 p-lg-5">

          <!-- ══ HEADER ══ -->
          <header class="ac-header mb-5">
            <div class="ac-breadcrumb mb-2">
              <span>Administration</span>
              <i class="fa-solid fa-chevron-right mx-2"></i>
              <span class="ac-breadcrumb-current">Analyses comportementales</span>
            </div>
            <div class="d-flex justify-content-between align-items-end flex-wrap gap-3">
              <div>
                <h2 class="ac-page-title">
                  Analyses <span class="ac-title-accent">&amp; Intelligence</span>
                </h2>
                <p class="ac-page-subtitle">
                  {{ analyses.length }} profil{{ analyses.length > 1 ? 's' : '' }} analysé{{ analyses.length > 1 ? 's' : '' }}
                  · Score moyen
                  <strong>{{ avgScore !== null ? avgScore + '%' : '—' }}</strong>
                </p>
              </div>

              <div class="d-flex gap-2 align-items-center flex-wrap">
                <button class="ac-btn-icon" @click="initOrchestrator" :disabled="loading" title="Rafraîchir">
                  <i class="fa-solid fa-rotate" :class="{ 'fa-spin': loading }"></i>
                </button>
                <div class="ac-view-toggle">
                  <button :class="['ac-toggle-btn', { active: viewMode === 'grid' }]" @click="viewMode = 'grid'" title="Grille">
                    <i class="fa-solid fa-table-cells-large"></i>
                  </button>
                  <button :class="['ac-toggle-btn', { active: viewMode === 'list' }]" @click="viewMode = 'list'" title="Liste">
                    <i class="fa-solid fa-list-ul"></i>
                  </button>
                </div>
              </div>
            </div>
          </header>

          <!-- ══ KPI STATS ══ -->
          <div class="ac-kpi-grid mb-5">
            <div v-for="stat in kpiStats" :key="stat.label" class="ac-kpi-card">
              <div class="ac-kpi-icon" :style="{ background: stat.bg, color: stat.color }">
                <i :class="stat.icon"></i>
              </div>
              <div class="ac-kpi-body">
                <div class="ac-kpi-value">{{ stat.value }}</div>
                <div class="ac-kpi-label">{{ stat.label }}</div>
              </div>
              <div class="ac-kpi-sparkline" :style="{ background: stat.color + '18' }">
                <div class="ac-kpi-bar" :style="{ background: stat.color, height: stat.bar + '%' }"></div>
              </div>
            </div>
          </div>

          <!-- ══ FILTRES & RECHERCHE ══ -->
          <div class="ac-filters mb-4">
            <!-- Tabs -->
            <div class="ac-tabs">
              <button
                v-for="tab in filterTabs"
                :key="tab.value"
                :class="['ac-tab', { active: activeTab === tab.value }]"
                @click="activeTab = tab.value"
              >
                {{ tab.label }}
                <span class="ac-tab-count">{{ tab.count }}</span>
              </button>
            </div>

            <!-- Droite : tri + recherche -->
            <div class="d-flex gap-2 align-items-center">
              <select class="ac-select" v-model="sortBy">
                <option value="score_desc">Score ↓</option>
                <option value="score_asc">Score ↑</option>
                <option value="name">Nom A–Z</option>
                <option value="date_desc">Plus récent</option>
              </select>
              <div class="ac-search-box">
                <i class="fa-solid fa-magnifying-glass"></i>
                <input
                  type="text"
                  v-model="searchQuery"
                  placeholder="Rechercher..."
                  class="ac-search-input"
                />
                <button v-if="searchQuery" class="ac-search-clear" @click="searchQuery = ''">
                  <i class="fa-solid fa-xmark"></i>
                </button>
              </div>
            </div>
          </div>

          <!-- ══ LOADING ══ -->
          <div v-if="loading" class="ac-loading-state">
            <div class="ac-spinner"></div>
            <p>Chargement des profils…</p>
          </div>

          <!-- ══ EMPTY ══ -->
          <div v-else-if="filteredAnalyses.length === 0" class="ac-empty-state">
            <div class="ac-empty-icon">
              <i class="fa-solid fa-user-slash"></i>
            </div>
            <h5>Aucun profil trouvé</h5>
            <p>Modifiez vos filtres ou ajoutez des candidats.</p>
          </div>

          <!-- ══ GRID VIEW ══ -->
          <div v-else-if="viewMode === 'grid'" class="ac-grid">
            <div
              v-for="(a, idx) in filteredAnalyses"
              :key="a.id"
              class="ac-profile-card"
              :style="{ animationDelay: (idx * 0.04) + 's' }"
              @click="goToDetail(a)"
            >
              <!-- Card top -->
              <div class="ac-card-top">
                <div class="ac-avatar" :style="{ background: avatarBg(a.candidat_nom) }">
                  {{ initials(a.candidat_nom) }}
                </div>
                <div class="ac-card-badges">
                  <span class="ac-tier-badge" :class="'tier-' + tierKey(a.neural_tier)">
                    <span class="ac-tier-dot"></span>
                    {{ a.neural_tier }}
                  </span>
                  <button
                    class="ac-delete-btn"
                    @click.stop="handleDelete(a.id)"
                    title="Supprimer"
                  >
                    <i class="fa-solid fa-trash-can"></i>
                  </button>
                </div>
              </div>

              <!-- Card body -->
              <div class="ac-card-body">
                <h5 class="ac-candidate-name">{{ a.candidat_nom }}</h5>
                <p class="ac-candidate-profile">{{ a.profile_type }}</p>
              </div>

              <!-- Score -->
              <div class="ac-card-score-area">
                <div class="ac-score-row">
                  <span class="ac-score-label">Score global</span>
                  <span class="ac-score-value" :style="{ color: scoreColor(a.global_score) }">
                    {{ a.global_score > 0 ? a.global_score + '%' : 'N/A' }}
                  </span>
                </div>
                <div class="ac-progress-track">
                  <div
                    class="ac-progress-fill"
                    :style="{
                      width: a.global_score + '%',
                      background: scoreColor(a.global_score)
                    }"
                  ></div>
                </div>
              </div>

              <!-- Card footer -->
              <div class="ac-card-footer">
                <span class="ac-date-label">
                  <i class="fa-regular fa-calendar"></i>
                  {{ formatDate(a.created_at) }}
                </span>
                <span class="ac-tests-label">
                  <i class="fa-solid fa-file-lines"></i>
                  {{ a.tests_count }} test{{ a.tests_count > 1 ? 's' : '' }}
                </span>
              </div>

              <!-- Hover CTA -->
              <div class="ac-card-cta">
                <i class="fa-solid fa-arrow-right"></i>
                Voir le profil
              </div>
            </div>
          </div>

          <!-- ══ LIST VIEW ══ -->
          <div v-else class="ac-list-container">
            <!-- List header -->
            <div class="ac-list-head">
              <div class="col-name">Candidat</div>
              <div class="col-profile">Profil</div>
              <div class="col-tier">Tier</div>
              <div class="col-score">Score</div>
              <div class="col-date">Date</div>
              <div class="col-actions"></div>
            </div>

            <!-- List rows -->
            <div
              v-for="(a, idx) in filteredAnalyses"
              :key="a.id"
              class="ac-list-row"
              :style="{ animationDelay: (idx * 0.03) + 's' }"
              @click="goToDetail(a)"
            >
              <div class="col-name">
                <div class="ac-list-avatar" :style="{ background: avatarBg(a.candidat_nom) }">
                  {{ initials(a.candidat_nom) }}
                </div>
                <span class="ac-list-name">{{ a.candidat_nom }}</span>
              </div>
              <div class="col-profile">
                <span class="ac-list-profile">{{ a.profile_type }}</span>
              </div>
              <div class="col-tier">
                <span class="ac-tier-badge" :class="'tier-' + tierKey(a.neural_tier)">
                  <span class="ac-tier-dot"></span>
                  {{ a.neural_tier }}
                </span>
              </div>
              <div class="col-score">
                <div class="ac-list-score-wrap">
                  <div class="ac-list-progress">
                    <div
                      class="ac-list-progress-fill"
                      :style="{
                        width: a.global_score + '%',
                        background: scoreColor(a.global_score)
                      }"
                    ></div>
                  </div>
                  <span class="ac-list-score-val" :style="{ color: scoreColor(a.global_score) }">
                    {{ a.global_score > 0 ? a.global_score + '%' : '—' }}
                  </span>
                </div>
              </div>
              <div class="col-date">
                <span class="ac-date-label">{{ formatDate(a.created_at) }}</span>
              </div>
              <div class="col-actions">
                <button class="ac-btn-icon ac-btn-icon-sm" @click.stop="goToDetail(a)" title="Voir">
                  <i class="fa-solid fa-eye"></i>
                </button>
                <button class="ac-btn-icon ac-btn-icon-sm ac-btn-danger" @click.stop="handleDelete(a.id)" title="Supprimer">
                  <i class="fa-solid fa-trash-can"></i>
                </button>
              </div>
            </div>
          </div>

        </div>
      </main>
    </div>

    <!-- ══ TOAST ══ -->
    <transition name="ac-toast-anim">
      <div v-if="toast.active" class="ac-toast" :class="'ac-toast-' + toast.type">
        <i :class="toast.icon"></i>
        <span>{{ toast.message }}</span>
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

// ── STATE ──────────────────────────────────────────────────────
const router      = useRouter();
const loading     = ref(true);
const viewMode    = ref('grid');
const searchQuery = ref('');
const activeTab   = ref('all');
const sortBy      = ref('score_desc');
const analyses    = ref([]);
const mouse       = reactive({ x: 0, y: 0 });
const toast       = reactive({ active: false, message: '', type: 'info', icon: '' });

// ── CHARGEMENT ────────────────────────────────────────────────
const initOrchestrator = async () => {
  loading.value = true;
  try {
    const { data: candidats } = await api.get('/Candidates');
    if (!Array.isArray(candidats) || candidats.length === 0) {
      analyses.value = [];
      return;
    }
    const results = await Promise.allSettled(candidats.map(enrichCandidat));
    analyses.value = results
      .filter(r => r.status === 'fulfilled')
      .map(r => r.value);
  } catch (err) {
    showToast('Erreur de connexion API', 'error');
    console.error(err);
  } finally {
    loading.value = false;
  }
};

const enrichCandidat = async (c) => {
  const id = c.id || c.Id;
  let tests = [];
  const endpoints = [
    `/Examen/resultats/${id}`,
    `/Examen/historique/${id}`,
    `/Examen/candidate-report/${id}`,
    `/Results/${id}`,
  ];
  for (const ep of endpoints) {
    try {
      const { data } = await api.get(ep);
      if (data) {
        tests = Array.isArray(data) ? data : [data];
        if (tests.length > 0) break;
      }
    } catch (_) {}
  }

  let globalScore = 0;
  if (tests.length > 0) {
    const scores = tests
      .map(t => Number(t.scoreGlobal ?? t.scoreTotal ?? t.score ?? t.Score ?? t.pourcentage ?? 0))
      .filter(s => s > 0);
    if (scores.length > 0)
      globalScore = Math.round(scores.reduce((a, b) => a + b, 0) / scores.length);
  } else {
    globalScore = Number(c.global_score ?? c.score ?? c.scoreGlobal ?? c.Score ?? 0);
  }

  return {
    id,
    candidat_nom : c.name || c.fullName || c.nom || c.Name || 'Candidat',
    profile_type : c.group || c.campaignName || c.poste || detectProfile(globalScore, tests),
    neural_tier  : globalScore >= 85 ? 'Élite' : globalScore >= 70 ? 'Standard' : 'Basique',
    global_score : globalScore,
    created_at   : c.created_at || c.createdAt || c.CreatedAt || new Date().toISOString(),
    tests_count  : tests.length,
  };
};

const detectProfile = (score, tests) => {
  if (tests.length === 0) return 'À évaluer';
  const themes = [...new Set(tests.map(t => t.theme || t.Theme || t.categorie).filter(Boolean))];
  if (themes.length > 0) return themes.slice(0, 2).join(' · ');
  return score >= 85 ? 'Expert' : score >= 70 ? 'Confirmé' : 'Junior';
};

// ── NAVIGATION & DELETE ───────────────────────────────────────
const goToDetail = (a) => {
  router.push({
    path: `/analyse-comportementale/${a.id}`,
    query: { score: a.global_score, nom: a.candidat_nom, tier: a.neural_tier },
  });
};

const handleDelete = async (id) => {
  if (!confirm('Supprimer cette analyse ?')) return;
  try {
    await api.delete(`/Candidates/${id}`);
  } catch (_) {}
  analyses.value = analyses.value.filter(a => a.id !== id);
  showToast('Analyse supprimée', 'warn');
};

// ── COMPUTED ──────────────────────────────────────────────────
const filteredAnalyses = computed(() => {
  let list = [...analyses.value];

  if (activeTab.value === 'elite')    list = list.filter(a => a.neural_tier === 'Élite');
  if (activeTab.value === 'standard') list = list.filter(a => a.neural_tier === 'Standard');
  if (activeTab.value === 'basique')  list = list.filter(a => a.neural_tier === 'Basique');
  if (activeTab.value === 'noeval')   list = list.filter(a => a.global_score === 0);

  const q = searchQuery.value.toLowerCase().trim();
  if (q) list = list.filter(a =>
    a.candidat_nom.toLowerCase().includes(q) ||
    a.profile_type.toLowerCase().includes(q)
  );

  if (sortBy.value === 'score_desc') list.sort((a, b) => b.global_score - a.global_score);
  if (sortBy.value === 'score_asc')  list.sort((a, b) => a.global_score - b.global_score);
  if (sortBy.value === 'name')       list.sort((a, b) => a.candidat_nom.localeCompare(b.candidat_nom));
  if (sortBy.value === 'date_desc')  list.sort((a, b) => new Date(b.created_at) - new Date(a.created_at));

  return list;
});

const filterTabs = computed(() => [
  { label: 'Tous',     value: 'all',      count: analyses.value.length },
  { label: 'Élite',   value: 'elite',    count: analyses.value.filter(a => a.neural_tier === 'Élite').length },
  { label: 'Standard',value: 'standard', count: analyses.value.filter(a => a.neural_tier === 'Standard').length },
  { label: 'Basique', value: 'basique',  count: analyses.value.filter(a => a.neural_tier === 'Basique').length },
  { label: 'Non éval.',value: 'noeval',  count: analyses.value.filter(a => a.global_score === 0).length },
]);

const avgScore = computed(() => {
  const avecScore = analyses.value.filter(a => a.global_score > 0);
  if (!avecScore.length) return null;
  return Math.round(avecScore.reduce((s, a) => s + a.global_score, 0) / avecScore.length);
});

const kpiStats = computed(() => {
  const total   = analyses.value.length;
  const elite   = analyses.value.filter(a => a.neural_tier === 'Élite').length;
  const noScore = analyses.value.filter(a => a.global_score === 0).length;
  return [
    {
      label: 'Total profils',
      value: total,
      icon:  'fa-solid fa-users',
      color: '#f59e0b',
      bg:    'rgba(245,158,11,0.12)',
      bar:   total > 0 ? 60 : 0,
    },
    {
      label: 'Score moyen',
      value: avgScore.value !== null ? avgScore.value + '%' : '—',
      icon:  'fa-solid fa-chart-line',
      color: '#10b981',
      bg:    'rgba(16,185,129,0.12)',
      bar:   avgScore.value ?? 0,
    },
    {
      label: 'Tier Élite',
      value: elite,
      icon:  'fa-solid fa-crown',
      color: '#6366f1',
      bg:    'rgba(99,102,241,0.12)',
      bar:   total > 0 ? Math.round((elite / total) * 100) : 0,
    },
    {
      label: 'Sans évaluation',
      value: noScore,
      icon:  'fa-solid fa-hourglass-half',
      color: '#f43f5e',
      bg:    'rgba(244,63,94,0.12)',
      bar:   total > 0 ? Math.round((noScore / total) * 100) : 0,
    },
  ];
});

// ── HELPERS ───────────────────────────────────────────────────
const scoreColor = (s) => {
  if (!s || s === 0) return 'var(--text-light)';
  return s >= 85 ? '#10b981' : s >= 70 ? '#f59e0b' : '#f43f5e';
};

const tierKey = (t) =>
  t === 'Élite' ? 'elite' : t === 'Standard' ? 'standard' : 'basique';

const formatDate = (d) =>
  d ? new Date(d).toLocaleDateString('fr-FR', { day: '2-digit', month: 'short', year: '2-digit' }) : '—';

const initials = (name) => {
  if (!name) return '?';
  const parts = name.trim().split(/\s+/);
  return parts.length >= 2
    ? (parts[0][0] + parts[1][0]).toUpperCase()
    : name.slice(0, 2).toUpperCase();
};

// Palette reproductible par initiales
const AVATAR_PALETTES = [
  { bg: 'rgba(245,158,11,0.18)', color: '#d97706' },
  { bg: 'rgba(99,102,241,0.18)', color: '#6366f1' },
  { bg: 'rgba(16,185,129,0.18)', color: '#059669' },
  { bg: 'rgba(244,63,94,0.18)',  color: '#e11d48' },
  { bg: 'rgba(59,130,246,0.18)', color: '#2563eb' },
  { bg: 'rgba(168,85,247,0.18)', color: '#9333ea' },
];
const avatarBg = (name) => {
  const idx = (name || '?').charCodeAt(0) % AVATAR_PALETTES.length;
  return AVATAR_PALETTES[idx].bg;
};

const handleParallax = (e) => {
  mouse.x = (e.clientX - window.innerWidth  / 2) / 30;
  mouse.y = (e.clientY - window.innerHeight / 2) / 30;
};
const orbStyle = (f) => ({
  transform: `translate(${mouse.x * f * 10}px, ${mouse.y * f * 10}px)`,
});

const showToast = (message, type) => {
  const icons = {
    error:   'fa-solid fa-circle-xmark',
    warn:    'fa-solid fa-triangle-exclamation',
    success: 'fa-solid fa-circle-check',
    info:    'fa-solid fa-circle-info',
  };
  Object.assign(toast, { active: true, message, type, icon: icons[type] ?? icons.info });
  setTimeout(() => { toast.active = false; }, 3500);
};

onMounted(initOrchestrator);
</script>

<style scoped>
/* ══════════════════════════════════════════════════════════════
   ROOT — utilise 100 % les variables du thème global App.vue
   var(--bg-page), var(--bg-card), var(--text-main),
   var(--text-muted), var(--text-light), var(--border-color),
   var(--primary), var(--primary-light), var(--shadow-*), etc.
   ══════════════════════════════════════════════════════════════ */

.analyse-root {
  min-height: 100vh;
  background: var(--bg-page);
  color: var(--text-main);
  font-family: 'Segoe UI', system-ui, sans-serif;
  position: relative;
  transition: background 0.35s ease, color 0.35s ease;
}

/* ── BACKGROUND ── */
.ac-bg-layer {
  position: fixed; inset: 0; z-index: 0; pointer-events: none; overflow: hidden;
}
.ac-grid-dots {
  position: absolute; inset: 0;
  background-image: radial-gradient(var(--border-color) 1px, transparent 1px);
  background-size: 36px 36px;
  opacity: 0.6;
  transition: opacity 0.35s;
}
.ac-orb {
  position: absolute; width: 500px; height: 500px;
  border-radius: 50%; filter: blur(100px); opacity: 0.08;
  transition: transform 0.3s ease-out, opacity 0.35s;
}
.ac-orb-amber { background: var(--primary); top: -180px; right: -80px; }
.ac-orb-blue  { background: var(--accent);  bottom: -180px; left: -80px; }

/* ── LAYOUT ── */
.ac-main  { z-index: 5; background: transparent; }
.ac-canvas { height: calc(100vh - 64px); }
.ac-inner  { max-width: 1400px; margin: 0 auto; }

/* ── HEADER ── */
.ac-breadcrumb {
  font-size: 0.72rem; font-weight: 700;
  color: var(--text-light); letter-spacing: 0.04em;
}
.ac-breadcrumb i { font-size: 0.5rem; opacity: 0.5; }
.ac-breadcrumb-current { color: var(--text-main); }

.ac-page-title {
  font-size: clamp(1.6rem, 3vw, 2.4rem);
  font-weight: 800; letter-spacing: -1px;
  color: var(--text-main); margin: 0;
}
.ac-title-accent {
  background: linear-gradient(135deg, var(--primary), #fbbf24);
  -webkit-background-clip: text; -webkit-text-fill-color: transparent;
  background-clip: text;
}
.ac-page-subtitle {
  margin: 6px 0 0; font-size: 0.85rem;
  color: var(--text-muted);
}
.ac-page-subtitle strong { color: var(--text-main); }

/* ── BUTTON BASE ── */
.ac-btn-icon {
  width: 42px; height: 42px; border-radius: 13px;
  border: 1.5px solid var(--border-color);
  background: var(--bg-card);
  color: var(--text-muted);
  cursor: pointer; display: flex; align-items: center; justify-content: center;
  font-size: 0.9rem; transition: all 0.2s;
}
.ac-btn-icon:hover:not(:disabled) {
  border-color: var(--primary); color: var(--primary);
  background: var(--primary-light);
}
.ac-btn-icon:disabled { opacity: 0.45; cursor: not-allowed; }
.ac-btn-icon-sm { width: 34px; height: 34px; font-size: 0.78rem; border-radius: 10px; }
.ac-btn-danger:hover { border-color: var(--danger) !important; color: var(--danger) !important; background: var(--danger-bg) !important; }

/* ── VIEW TOGGLE ── */
.ac-view-toggle {
  display: flex; background: var(--bg-card);
  border: 1.5px solid var(--border-color); border-radius: 14px;
  padding: 3px; gap: 3px;
}
.ac-toggle-btn {
  width: 36px; height: 36px; border-radius: 10px;
  border: none; background: transparent;
  color: var(--text-muted); cursor: pointer;
  display: flex; align-items: center; justify-content: center;
  font-size: 0.85rem; transition: all 0.2s;
}
.ac-toggle-btn.active {
  background: var(--secondary); color: var(--primary);
}
[data-theme="dark"] .ac-toggle-btn.active {
  background: var(--primary); color: var(--bg-page);
}

/* ── KPI GRID ── */
.ac-kpi-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 16px;
}
.ac-kpi-card {
  background: var(--bg-card);
  border: 1.5px solid var(--border-color);
  border-radius: 20px; padding: 20px 22px;
  display: flex; align-items: center; gap: 14px;
  transition: all 0.25s; cursor: default;
  box-shadow: var(--shadow-xs);
}
.ac-kpi-card:hover {
  transform: translateY(-3px);
  box-shadow: var(--shadow-md);
  border-color: var(--border-color);
}
.ac-kpi-icon {
  width: 48px; height: 48px; border-radius: 14px;
  display: flex; align-items: center; justify-content: center;
  font-size: 1.15rem; flex-shrink: 0;
}
.ac-kpi-body { flex: 1; }
.ac-kpi-value { font-size: 1.5rem; font-weight: 800; color: var(--text-main); line-height: 1; }
.ac-kpi-label { font-size: 0.68rem; font-weight: 700; color: var(--text-light); text-transform: uppercase; letter-spacing: 0.06em; margin-top: 4px; }
.ac-kpi-sparkline {
  width: 36px; height: 36px; border-radius: 10px;
  display: flex; align-items: flex-end; justify-content: center;
  padding: 6px; flex-shrink: 0;
}
.ac-kpi-bar { width: 10px; border-radius: 4px; min-height: 4px; transition: height 1s ease; }

/* ── FILTERS ── */
.ac-filters {
  display: flex; justify-content: space-between; align-items: center;
  gap: 12px; flex-wrap: wrap;
}
.ac-tabs {
  display: flex; background: var(--bg-card);
  border: 1.5px solid var(--border-color);
  border-radius: 16px; padding: 4px; gap: 4px;
  flex-wrap: wrap;
}
.ac-tab {
  padding: 8px 16px; border-radius: 12px; border: none;
  background: transparent; color: var(--text-muted);
  font-weight: 700; font-size: 0.8rem; cursor: pointer;
  transition: all 0.2s; white-space: nowrap;
  font-family: inherit;
}
.ac-tab.active { background: var(--secondary); color: var(--primary); }
[data-theme="dark"] .ac-tab.active { background: var(--primary); color: var(--bg-page); }
.ac-tab-count {
  background: var(--bg-hover); color: var(--text-muted);
  border-radius: 8px; padding: 1px 7px;
  font-size: 0.65rem; margin-left: 5px;
}
.ac-tab.active .ac-tab-count { background: rgba(255,255,255,0.18); color: inherit; }

.ac-select {
  height: 42px; padding: 0 14px; border-radius: 13px;
  border: 1.5px solid var(--border-color);
  background: var(--bg-card); color: var(--text-main);
  font-size: 0.82rem; font-weight: 600; cursor: pointer;
  outline: none; font-family: inherit;
  transition: border-color 0.2s;
}
.ac-select:focus { border-color: var(--primary); }

.ac-search-box {
  display: flex; align-items: center; gap: 8px;
  background: var(--bg-card); border: 1.5px solid var(--border-color);
  border-radius: 13px; padding: 0 14px; height: 42px;
  color: var(--text-muted); transition: border-color 0.2s;
}
.ac-search-box:focus-within { border-color: var(--primary); }
.ac-search-input {
  border: none; outline: none; background: transparent;
  color: var(--text-main); font-size: 0.85rem; font-weight: 600;
  width: 200px; font-family: inherit;
}
.ac-search-input::placeholder { color: var(--text-light); font-weight: 400; }
.ac-search-clear {
  border: none; background: none; cursor: pointer;
  color: var(--text-light); padding: 0; font-size: 0.8rem;
  transition: color 0.2s;
}
.ac-search-clear:hover { color: var(--danger); }

/* ── LOADING ── */
.ac-loading-state {
  text-align: center; padding: 80px 20px; color: var(--text-muted);
}
.ac-spinner {
  width: 48px; height: 48px; margin: 0 auto 16px;
  border: 3px solid var(--border-color);
  border-top: 3px solid var(--primary);
  border-radius: 50%; animation: ac-spin 0.9s linear infinite;
}
@keyframes ac-spin { to { transform: rotate(360deg); } }
.ac-loading-state p { font-size: 0.82rem; font-weight: 700; text-transform: uppercase; letter-spacing: 0.08em; }

/* ── EMPTY STATE ── */
.ac-empty-state {
  text-align: center; padding: 80px 20px;
  color: var(--text-muted);
}
.ac-empty-icon {
  width: 72px; height: 72px; border-radius: 20px;
  background: var(--bg-hover); display: flex; align-items: center; justify-content: center;
  font-size: 1.6rem; color: var(--border-color);
  margin: 0 auto 20px;
}
.ac-empty-state h5 { font-weight: 800; color: var(--text-main); margin-bottom: 6px; }
.ac-empty-state p  { font-size: 0.85rem; }

/* ── GRID ── */
.ac-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 20px;
}

/* ── PROFILE CARD ── */
.ac-profile-card {
  background: var(--bg-card);
  border: 1.5px solid var(--border-color);
  border-radius: 24px; padding: 24px;
  cursor: pointer; position: relative; overflow: hidden;
  transition: all 0.28s cubic-bezier(0.4, 0, 0.2, 1);
  box-shadow: var(--shadow-xs);
  animation: ac-card-in 0.4s ease both;
}
@keyframes ac-card-in {
  from { opacity: 0; transform: translateY(16px); }
  to   { opacity: 1; transform: translateY(0); }
}
.ac-profile-card::before {
  content: ''; position: absolute; inset: 0;
  border-radius: 24px; opacity: 0; transition: opacity 0.28s;
  background: linear-gradient(135deg, var(--primary-light) 0%, transparent 60%);
}
.ac-profile-card:hover {
  transform: translateY(-5px);
  box-shadow: var(--shadow-md);
  border-color: var(--primary);
}
.ac-profile-card:hover::before { opacity: 1; }
.ac-profile-card:hover .ac-card-cta { opacity: 1; transform: translateY(0); }

/* Card top */
.ac-card-top {
  display: flex; justify-content: space-between; align-items: flex-start;
  margin-bottom: 16px; position: relative;
}
.ac-avatar {
  width: 52px; height: 52px; border-radius: 16px;
  display: flex; align-items: center; justify-content: center;
  font-size: 0.9rem; font-weight: 800; color: var(--text-main);
  letter-spacing: 0.5px; flex-shrink: 0;
}
.ac-card-badges { display: flex; align-items: center; gap: 8px; }
.ac-delete-btn {
  width: 30px; height: 30px; border-radius: 9px;
  border: 1.5px solid var(--border-color);
  background: var(--bg-card); color: var(--text-light);
  cursor: pointer; display: flex; align-items: center; justify-content: center;
  font-size: 0.72rem; transition: all 0.2s; opacity: 0;
}
.ac-profile-card:hover .ac-delete-btn { opacity: 1; }
.ac-delete-btn:hover { background: var(--danger-bg); color: var(--danger); border-color: var(--danger); }

/* Tier badge */
.ac-tier-badge {
  font-size: 0.6rem; font-weight: 800; padding: 4px 10px;
  border-radius: 8px; text-transform: uppercase; letter-spacing: 0.05em;
  display: inline-flex; align-items: center; gap: 5px;
}
.ac-tier-dot { width: 5px; height: 5px; border-radius: 50%; background: currentColor; }
.tier-elite    { background: rgba(16,185,129,0.12);  color: #059669; }
.tier-standard { background: rgba(99,102,241,0.12);  color: #6366f1; }
.tier-basique  { background: var(--bg-hover);        color: var(--text-muted); }
[data-theme="dark"] .tier-elite    { background: rgba(16,185,129,0.2); color: #34d399; }
[data-theme="dark"] .tier-standard { background: rgba(99,102,241,0.2); color: #818cf8; }

/* Card body */
.ac-card-body { margin-bottom: 18px; position: relative; }
.ac-candidate-name    { font-size: 1rem; font-weight: 800; color: var(--text-main); margin: 0 0 4px; }
.ac-candidate-profile { font-size: 0.78rem; color: var(--text-muted); margin: 0; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }

/* Score area */
.ac-card-score-area { position: relative; }
.ac-score-row { display: flex; justify-content: space-between; align-items: center; margin-bottom: 8px; }
.ac-score-label { font-size: 0.7rem; font-weight: 700; color: var(--text-light); text-transform: uppercase; letter-spacing: 0.06em; }
.ac-score-value { font-size: 0.95rem; font-weight: 800; }
.ac-progress-track { height: 6px; background: var(--bg-hover); border-radius: 8px; overflow: hidden; }
.ac-progress-fill  { height: 100%; border-radius: 8px; transition: width 1.2s cubic-bezier(0.4, 0, 0.2, 1); }

/* Card footer */
.ac-card-footer {
  display: flex; justify-content: space-between; align-items: center;
  margin-top: 16px; padding-top: 14px; border-top: 1px solid var(--border-color);
  position: relative;
}
.ac-date-label, .ac-tests-label {
  font-size: 0.7rem; color: var(--text-light); font-weight: 600;
  display: flex; align-items: center; gap: 5px;
}

/* Hover CTA */
.ac-card-cta {
  position: absolute; bottom: 0; left: 0; right: 0;
  height: 44px; background: var(--primary);
  color: white; font-weight: 800; font-size: 0.8rem;
  display: flex; align-items: center; justify-content: center; gap: 8px;
  border-radius: 0 0 22px 22px;
  opacity: 0; transform: translateY(8px); transition: all 0.25s;
}

/* ── LIST VIEW ── */
.ac-list-container { display: flex; flex-direction: column; gap: 6px; }

.ac-list-head {
  display: grid;
  grid-template-columns: 2fr 2fr 1fr 2fr 1fr 80px;
  gap: 12px; padding: 10px 18px;
  background: var(--bg-hover); border-radius: 12px;
  font-size: 0.68rem; font-weight: 800; color: var(--text-light);
  text-transform: uppercase; letter-spacing: 0.07em;
}

.ac-list-row {
  display: grid;
  grid-template-columns: 2fr 2fr 1fr 2fr 1fr 80px;
  gap: 12px; padding: 14px 18px;
  background: var(--bg-card); border: 1.5px solid var(--border-color);
  border-radius: 16px; cursor: pointer; align-items: center;
  transition: all 0.2s; box-shadow: var(--shadow-xs);
  animation: ac-card-in 0.35s ease both;
}
.ac-list-row:hover {
  border-color: var(--primary);
  background: var(--primary-light);
  box-shadow: var(--shadow-sm);
}

.col-name    { display: flex; align-items: center; gap: 10px; min-width: 0; }
.col-profile { display: flex; align-items: center; }
.col-tier    { display: flex; align-items: center; }
.col-score   { display: flex; align-items: center; }
.col-date    { display: flex; align-items: center; }
.col-actions { display: flex; align-items: center; gap: 6px; justify-content: flex-end; }

.ac-list-avatar {
  width: 34px; height: 34px; border-radius: 10px;
  display: flex; align-items: center; justify-content: center;
  font-size: 0.72rem; font-weight: 800; color: var(--text-main);
  flex-shrink: 0;
}
.ac-list-name    { font-weight: 800; font-size: 0.88rem; color: var(--text-main); white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
.ac-list-profile { font-size: 0.78rem; color: var(--text-muted); white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }

.ac-list-score-wrap {
  display: flex; align-items: center; gap: 10px; width: 100%;
}
.ac-list-progress {
  flex: 1; height: 5px; background: var(--bg-hover); border-radius: 6px; overflow: hidden;
}
.ac-list-progress-fill { height: 100%; border-radius: 6px; transition: width 1s ease; }
.ac-list-score-val { font-size: 0.82rem; font-weight: 800; min-width: 36px; text-align: right; }

/* ── TOAST ── */
.ac-toast {
  position: fixed; bottom: 28px; right: 28px; z-index: 99999;
  display: flex; align-items: center; gap: 10px;
  padding: 14px 22px; border-radius: 14px;
  background: var(--secondary); color: var(--text-inverse);
  font-weight: 700; font-size: 0.85rem;
  box-shadow: var(--shadow-lg);
}
[data-theme="dark"] .ac-toast { background: var(--bg-card); color: var(--text-main); border: 1px solid var(--border-color); }
.ac-toast-error   { border-left: 4px solid var(--danger); }
.ac-toast-warn    { border-left: 4px solid var(--warning); }
.ac-toast-success { border-left: 4px solid var(--success); }
.ac-toast-info    { border-left: 4px solid var(--info); }

.ac-toast-anim-enter-active { animation: ac-toast-in 0.35s ease; }
.ac-toast-anim-leave-active { animation: ac-toast-in 0.25s ease reverse; }
@keyframes ac-toast-in {
  from { transform: translateX(110%); opacity: 0; }
  to   { transform: translateX(0);    opacity: 1; }
}

/* ── RESPONSIVE ── */
@media (max-width: 900px) {
  .ac-list-head, .ac-list-row {
    grid-template-columns: 1fr 1fr 80px;
  }
  .col-profile, .col-date { display: none; }
}
@media (max-width: 600px) {
  .ac-filters    { flex-direction: column; align-items: stretch; }
  .ac-search-box { width: 100%; }
  .ac-search-input { width: 100%; }
  .ac-grid { grid-template-columns: 1fr; }
  .ac-kpi-grid { grid-template-columns: 1fr 1fr; }
}
</style>