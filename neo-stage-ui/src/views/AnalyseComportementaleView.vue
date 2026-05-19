<template>
  <div class="ac-root" @mousemove="handleParallax">

    <!-- DÉCOR D'ARRIÈRE-PLAN -->
    <div class="ac-bg" aria-hidden="true">
      <div class="ac-orb ac-orb-amber"  :style="orbStyle(0.04)"></div>
      <div class="ac-orb ac-orb-blue"   :style="orbStyle(0.015)"></div>
      <div class="ac-orb ac-orb-purple" :style="orbStyle(0.025)"></div>
      <div class="ac-grid-dots"></div>
    </div>

    <AppSidebar />

    <div class="ac-main flex-grow-1 d-flex flex-column">
      <AppNavbar />

      <!-- ÉCRAN DE CHARGEMENT -->
      <div v-if="loading" class="ac-loader-viewport">
        <div class="ac-spinner-ring">
          <div></div><div></div><div></div><div></div>
        </div>
        <p class="ac-loading-text mt-3">SYNCHRONISATION AVEC L'IA .NET...</p>
      </div>

      <!-- CONTENU PRINCIPAL -->
      <main v-else class="ac-canvas flex-grow-1 overflow-auto">
        <div class="ac-inner p-4 p-lg-5">

          <!-- EN-TÊTE -->
          <header class="ac-page-header mb-5">
            <div class="ac-header-left">
              <div class="ac-breadcrumb mb-2">
                <span class="root" @click="$router.push('/dashboard')">Dashboard</span>
                <i class="fa-solid fa-chevron-right mx-2 sep"></i>
                <span class="current">Analyse Comportementale IA</span>
              </div>
              <h2 class="ac-page-title">
                Analyses <span class="ac-title-accent">Prédictives</span>
              </h2>
              <p class="ac-subtitle">
                {{ allAnalyses.length }} profil(s) synchronisé(s)
                · <span class="ac-ia-badge"><i class="fa-solid fa-brain me-1"></i>IA .NET Active</span>
              </p>
            </div>

            <div class="ac-header-right">
              <!-- BOUTON UPLOAD GÉNÉRIQUE -->
              <label class="ac-btn-upload" title="Analyser un nouveau CV">
                <i class="fa-solid fa-file-arrow-up me-2"></i>Analyser un CV
                <input type="file" @change="(e) => onUploadCv(e)" hidden accept=".pdf,.docx" />
              </label>
              
              <button class="ac-btn-icon" @click="initOrchestrator" :disabled="loading">
                <i class="fa-solid fa-rotate" :class="{ 'fa-spin': loading }"></i>
              </button>
            </div>
          </header>

          <!-- KPI CARDS -->
          <div class="ac-kpi-grid mb-5">
            <div class="ac-kpi-card" v-for="stat in kpiStats" :key="stat.label">
              <div class="ac-kpi-icon-wrap" :style="{ background: stat.bg }">
                <i :class="stat.icon" :style="{ color: stat.color }"></i>
              </div>
              <div class="ac-kpi-body">
                <div class="ac-kpi-value">{{ stat.value }}</div>
                <div class="ac-kpi-label">{{ stat.label }}</div>
              </div>
            </div>
          </div>

          <!-- BARRE DE RECHERCHE ET FILTRES -->
          <div class="ac-filters-row mb-4">
            <div class="ac-tabs-wrap">
              <button 
                v-for="tab in filterTabs" :key="tab.value"
                :class="['ac-tab', { active: activeTab === tab.value }]"
                @click="activeTab = tab.value"
              >
                {{ tab.label }}
              </button>
            </div>
            <div class="ac-search-box">
              <i class="fa-solid fa-magnifying-glass ac-search-icon"></i>
              <input type="text" v-model="searchQuery" placeholder="Rechercher un candidat..." class="ac-search-input" />
            </div>
          </div>

          <!-- ÉTAT VIDE -->
          <div v-if="filteredAnalyses.length === 0 && !loading" class="ac-empty-state">
            <i class="fa-solid fa-users-slash fa-3x mb-3"></i>
            <h5>Aucun candidat trouvé</h5>
            <p class="text-muted">Modifiez vos filtres ou synchronisez la liste.</p>
          </div>

          <!-- GRILLE DES CANDIDATS -->
          <div v-else class="ac-grid">
            <div
              v-for="(a, idx) in filteredAnalyses"
              :key="a.id"
              class="ac-profile-card"
              :style="{ animationDelay: (idx * 0.05) + 's' }"
              @click="openDetailModal(a)"
            >
              <div v-if="a.iaAnalyzed" class="ac-ia-tag-float">
                <i class="fa-solid fa-robot me-1"></i> IA ANALYSÉ
              </div>

              <div class="ac-card-top">
                <div class="ac-avatar" :style="{ background: avatarBg(a.candidat_nom) }">
                  {{ initials(a.candidat_nom) }}
                </div>
                <span class="ac-tier-badge" :class="'tier-' + a.tier_raw">
                  {{ a.tier_raw }}
                </span>
              </div>

              <div class="ac-card-body">
                <h5 class="ac-candidate-name">{{ a.candidat_nom }}</h5>
                <p class="ac-candidate-profile">{{ a.profile_type }}</p>
              </div>

              <div class="ac-card-score-area">
                <div class="ac-score-row">
                  <span class="ac-score-label">MATCH SCORE</span>
                  <span class="ac-score-value" :style="{ color: scoreColor(a.global_score) }">
                    {{ a.global_score }}%
                  </span>
                </div>
                <div class="ac-progress-track">
                  <div
                    class="ac-progress-fill"
                    :style="{ width: a.global_score + '%', background: scoreColor(a.global_score) }"
                  ></div>
                </div>
              </div>

              <div v-if="a.quick_insight" class="ac-quick-insight">
                <i class="fa-solid fa-comment-dots me-2"></i>
                <span>{{ a.quick_insight }}</span>
              </div>
            </div>
          </div>

        </div>
      </main>
    </div>

    <!-- MODAL D'ANALYSE DÉTAILLÉE -->
    <transition name="ac-modal-anim">
      <div v-if="detailModal.open" class="ac-modal-overlay" @click.self="closeModal">
        <div class="ac-modal">
          
          <div class="ac-modal-header">
            <div class="d-flex align-items-center gap-3">
              <div
                class="ac-modal-avatar"
                :style="{ background: avatarBg(detailModal.analysis?.candidat_nom ?? '') }"
              >
                {{ initials(detailModal.analysis?.candidat_nom ?? '?') }}
              </div>
              <h4 class="m-0">{{ detailModal.analysis?.candidat_nom }}</h4>
            </div>
            <button class="ac-modal-close ms-auto" @click="closeModal">
              <i class="fa-solid fa-xmark"></i>
            </button>
          </div>

          <div v-if="detailModal.loading" class="ac-modal-loader">
            <i class="fa-solid fa-circle-notch fa-spin fa-3x text-amber"></i>
            <p class="mt-3">Analyse IA en cours...</p>
          </div>

          <div v-else class="ac-modal-body">
            <!-- CAS 1 : ANALYSE DISPONIBLE -->
            <div v-if="detailModal.analysis?.ai_details">
              <div class="text-center mb-5">
                <div
                  class="ac-big-score"
                  :style="{ color: scoreColor(detailModal.analysis.global_score) }"
                >
                  {{ detailModal.analysis.global_score }}%
                </div>
                <p class="text-muted small uppercase fw-bold letter-spacing-2">
                  Score de compatibilité globale
                </p>
              </div>

              <div class="row g-4">
                <div class="col-md-6">
                  <div class="ac-insight-box ac-insight-force">
                    <div class="ac-insight-title">
                      <i class="fa-solid fa-thumbs-up me-2"></i>Points Forts
                    </div>
                    <ul class="ac-insight-list">
                      <li
                        v-for="(f, i) in detailModal.analysis.ai_details.points_forts"
                        :key="'fort-' + i"
                      >{{ f }}</li>
                    </ul>
                  </div>
                </div>
                <div class="col-md-6">
                  <div class="ac-insight-box ac-insight-axe">
                    <div class="ac-insight-title">
                      <i class="fa-solid fa-triangle-exclamation me-2"></i>Axes d'amélioration
                    </div>
                    <ul class="ac-insight-list">
                      <li
                        v-for="(p, i) in detailModal.analysis.ai_details.points_faibles"
                        :key="'faible-' + i"
                      >{{ p }}</li>
                    </ul>
                  </div>
                </div>
                <div class="col-12">
                  <div class="ac-insight-box ac-insight-tips">
                    <div class="ac-insight-title">
                      <i class="fa-solid fa-lightbulb me-2"></i>Recommandations Stratégiques
                    </div>
                    <ul class="ac-insight-list">
                      <li
                        v-for="(c, i) in detailModal.analysis.ai_details.conseils"
                        :key="'conseil-' + i"
                      >{{ c }}</li>
                    </ul>
                  </div>
                </div>
              </div>

              <div class="ac-decision-strip mt-4">
                <i class="fa-solid fa-gavel me-2"></i>
                <strong>Conclusion IA :</strong> {{ detailModal.analysis.ai_details.decision }}
              </div>

              <!-- DATE D'ANALYSE -->
              <div v-if="detailModal.analysis.ai_details.createdAt" class="ac-analysis-date mt-3">
                <i class="fa-regular fa-clock me-1"></i>
                Analysé le {{ formatDate(detailModal.analysis.ai_details.createdAt) }}
              </div>
            </div>

            <!-- CAS 2 : AUCUNE ANALYSE (SCORE 0%) -->
            <div v-else class="text-center py-5">
              <div class="ac-empty-robot mb-4">
                <i class="fa-solid fa-robot fa-4x"></i>
              </div>
              <h5>Aucune analyse IA disponible</h5>
              <p class="text-muted mb-4">
                Lancez une analyse comparative pour ce candidat en téléchargeant son CV.
              </p>
              
              <label class="ac-btn-upload-big">
                <i class="fa-solid fa-wand-magic-sparkles me-2"></i> 
                Analyser le CV de {{ firstName(detailModal.analysis?.candidat_nom) }}
                <input
                  type="file"
                  @change="(e) => onUploadCv(e, detailModal.analysis?.id)"
                  hidden
                  accept=".pdf,.docx"
                />
              </label>
            </div>
          </div>

        </div>
      </div>
    </transition>

    <!-- TOAST NOTIFICATIONS -->
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
import api from '@/services/api';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar  from '../components/AppNavbar.vue';

// ─────────────────────────────────────────
// ÉTAT
// ─────────────────────────────────────────
const loading     = ref(true);
const allAnalyses = ref([]);
const searchQuery = ref('');
const activeTab   = ref('all');
const mouse       = reactive({ x: 0, y: 0 });
const toast       = reactive({ active: false, message: '', type: 'info', icon: '' });

const detailModal = reactive({
  open: false,
  loading: false,
  analysis: null
});

// ─────────────────────────────────────────
// LOGIQUE API .NET
// ─────────────────────────────────────────

/** Récupère l'historique IA pour un GUID donné */
const fetchAiHistory = async (guid) => {
  try {
    const { data } = await api.get(`/Ai/cv-history/${guid}`);
    return Array.isArray(data) ? data : [];
  } catch {
    return [];
  }
};

/** Initialise la liste des candidats enrichie par l'IA */
const initOrchestrator = async () => {
  loading.value = true;
  try {
    const { data: candidats } = await api.get('/Candidates');

    const enriched = await Promise.all(candidats.map(async (c) => {
      const id        = c.id ?? c.Id;
      const aiHistory = await fetchAiHistory(id);
      const latestAi  = aiHistory.length > 0 ? aiHistory[0] : null;
      const score     = latestAi?.score ?? 0;

      return {
        id,
        candidat_nom:  c.name ?? c.nom ?? 'Candidat',
        profile_type:  c.poste ?? 'Développeur',
        global_score:  score,
        tier_raw:      score >= 85 ? 'élite' : score >= 70 ? 'standard' : 'basique',
        iaAnalyzed:    !!latestAi,
        ai_details:    latestAi,   // contient points_forts, points_faibles, conseils, decision
        quick_insight: latestAi ? latestAi.decision : 'Analyse requise'
      };
    }));

    allAnalyses.value = enriched;
  } catch {
    showToast('Erreur de synchronisation avec le serveur .NET', 'error');
  } finally {
    loading.value = false;
  }
};

/** Upload de CV — générique ou spécifique à un candidat */
const onUploadCv = async (event, specificCandidatId = null) => {
  const file = event.target.files?.[0];
  if (!file) return;

  // Réinitialise l'input pour permettre un re-upload du même fichier
  event.target.value = '';

  const jobDesc  = detailModal.analysis?.profile_type ?? 'Poste Fullstack';
  const formData = new FormData();
  formData.append('file', file);
  formData.append('File', file);
  formData.append('lang', 'fr');
  formData.append('Lang', 'fr');
  formData.append('job_description', jobDesc);
  formData.append('jobDescription', jobDesc);
  formData.append('JobDescription', jobDesc);

  if (specificCandidatId) {
    formData.append('candidat_id', specificCandidatId);
    formData.append('candidatId', specificCandidatId);
    formData.append('CandidatId', specificCandidatId);
    detailModal.loading = true;
  }

  showToast('Traitement IA en cours...', 'info');

  try {
    const { data } = await api.post('/Ai/analyze-cv', formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    });

    if (data.is_cv === false || data.isCv === false || data.status === 'NOT_A_CV') {
      const msg = data.alert?.subtitle || data.alert?.title || "Ce document n'est pas reconnu comme un CV.";
      showToast(msg, 'error');
      return;
    }

    showToast('Analyse terminée avec succès !', 'success');

    // Mise à jour locale si on est dans la modale
    if (specificCandidatId && detailModal.analysis?.id === specificCandidatId) {
      detailModal.analysis.ai_details   = data;
      detailModal.analysis.global_score  = data.score ?? 0;
      detailModal.analysis.quick_insight = data.decision ?? '';
      detailModal.analysis.iaAnalyzed    = true;
    }

    await initOrchestrator();
  } catch {
    showToast("Erreur lors de l'analyse du document", 'error');
  } finally {
    if (specificCandidatId) detailModal.loading = false;
  }
};

// ─────────────────────────────────────────
// COMPUTED
// ─────────────────────────────────────────

const filteredAnalyses = computed(() => {
  let list = allAnalyses.value;
  if (activeTab.value !== 'all') {
    list = list.filter(a => a.tier_raw === activeTab.value);
  }
  if (searchQuery.value.trim()) {
    const q = searchQuery.value.toLowerCase().trim();
    list = list.filter(a => a.candidat_nom.toLowerCase().includes(q));
  }
  return list;
});

/** Score moyen calculé dynamiquement sur les candidats analysés */
const avgScore = computed(() => {
  const analyzed = allAnalyses.value.filter(a => a.iaAnalyzed);
  if (!analyzed.length) return 'N/A';
  const avg = analyzed.reduce((sum, a) => sum + a.global_score, 0) / analyzed.length;
  return Math.round(avg) + '%';
});

const kpiStats = computed(() => [
  {
    label: 'CANDIDATS',
    value: allAnalyses.value.length,
    icon:  'fa-solid fa-users',
    color: '#f59e0b',
    bg:    'rgba(245,158,11,0.1)'
  },
  {
    label: 'ANALYSÉS IA',
    value: allAnalyses.value.filter(a => a.iaAnalyzed).length,
    icon:  'fa-solid fa-robot',
    color: '#8b5cf6',
    bg:    'rgba(139,92,246,0.1)'
  },
  {
    label: 'SCORE MOYEN',
    value: avgScore.value,
    icon:  'fa-solid fa-bolt',
    color: '#10b981',
    bg:    'rgba(16,185,129,0.1)'
  }
]);

const filterTabs = [
  { label: 'Tous les profils', value: 'all'      },
  { label: 'Niveau Élite',     value: 'élite'    },
  { label: 'Standard',         value: 'standard' },
  { label: 'Basique',          value: 'basique'  }
];

// ─────────────────────────────────────────
// HELPERS UI
// ─────────────────────────────────────────

const openDetailModal = (a) => {
  detailModal.analysis = a;
  detailModal.open     = true;
};

const closeModal = () => {
  detailModal.open    = false;
  detailModal.loading = false;
};

const scoreColor = (s) =>
  s >= 85 ? '#10b981' : s >= 70 ? '#f59e0b' : '#ef4444';

/** Retourne les initiales d'un nom (2 lettres max) */
const initials = (n) =>
  (n || '?').split(' ').map(p => p[0] ?? '').join('').toUpperCase().slice(0, 2);

/** Retourne le prénom (premier mot) */
const firstName = (n) => (n ?? '').split(' ')[0] || 'ce candidat';

const avatarBg = (name) =>
  `hsl(${((name ?? '').length * 45) % 360}, 65%, 85%)`;

const orbStyle = (f) => ({
  transform: `translate(${mouse.x * f}px, ${mouse.y * f}px)`
});

const handleParallax = (e) => {
  mouse.x = e.clientX - window.innerWidth  / 2;
  mouse.y = e.clientY - window.innerHeight / 2;
};

const formatDate = (iso) => {
  if (!iso) return '';
  return new Date(iso).toLocaleDateString('fr-FR', {
    day:   '2-digit',
    month: 'long',
    year:  'numeric',
    hour:  '2-digit',
    minute:'2-digit'
  });
};

const showToast = (message, type) => {
  const icons = {
    success: 'fa-solid fa-circle-check',
    error:   'fa-solid fa-triangle-exclamation',
    info:    'fa-solid fa-circle-info'
  };
  Object.assign(toast, { active: true, message, type, icon: icons[type] ?? icons.info });
  setTimeout(() => { toast.active = false; }, 3500);
};

onMounted(initOrchestrator);
</script>

<style scoped>
/* ────────────────────────────────────────
   ROOT & BACKGROUND
──────────────────────────────────────── */
.ac-root {
  font-family: 'Plus Jakarta Sans', sans-serif;
  min-height: 100vh;
  position: relative;
  overflow: hidden;
  background: #f8fafc;
  display: flex;
}

.ac-bg {
  position: fixed;
  inset: 0;
  pointer-events: none;
  z-index: 0;
}

.ac-orb {
  position: absolute;
  border-radius: 50%;
  filter: blur(100px);
  opacity: 0.15;
  transition: transform 0.3s ease;
}
.ac-orb-amber  { width: 500px; height: 500px; background: #f59e0b; top: -100px; right: -100px; }
.ac-orb-blue   { width: 400px; height: 400px; background: #3b82f6; bottom: -50px; left: -50px; }
.ac-orb-purple { width: 300px; height: 300px; background: #a855f7; top: 30%; left: 20%; }

.ac-grid-dots {
  position: absolute;
  inset: 0;
  background-image: radial-gradient(circle, #cbd5e1 1px, transparent 1px);
  background-size: 30px 30px;
  opacity: 0.4;
}

/* ────────────────────────────────────────
   LAYOUT
──────────────────────────────────────── */
.ac-main    { z-index: 10; width: 100%; }
.ac-canvas  { background: transparent; }
.ac-inner   { max-width: 1400px; margin: 0 auto; }

/* ────────────────────────────────────────
   LOADER
──────────────────────────────────────── */
.ac-loader-viewport {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 60vh;
}

.ac-spinner-ring {
  display: inline-block;
  position: relative;
  width: 60px;
  height: 60px;
}
.ac-spinner-ring div {
  box-sizing: border-box;
  display: block;
  position: absolute;
  width: 48px;
  height: 48px;
  margin: 6px;
  border: 5px solid #f59e0b;
  border-radius: 50%;
  animation: ac-spin 1.2s cubic-bezier(0.5, 0, 0.5, 1) infinite;
  border-color: #f59e0b transparent transparent transparent;
}
.ac-spinner-ring div:nth-child(1) { animation-delay: -0.45s; }
.ac-spinner-ring div:nth-child(2) { animation-delay: -0.3s;  }
.ac-spinner-ring div:nth-child(3) { animation-delay: -0.15s; }

@keyframes ac-spin {
  0%   { transform: rotate(0deg);   }
  100% { transform: rotate(360deg); }
}

.ac-loading-text {
  font-size: 0.75rem;
  font-weight: 800;
  letter-spacing: 2px;
  color: #94a3b8;
  text-transform: uppercase;
}

/* ────────────────────────────────────────
   PAGE HEADER
──────────────────────────────────────── */
.ac-page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  flex-wrap: wrap;
  gap: 20px;
}

.ac-breadcrumb {
  font-size: 0.8rem;
  color: #94a3b8;
  display: flex;
  align-items: center;
}
.ac-breadcrumb .root  { cursor: pointer; }
.ac-breadcrumb .root:hover { color: #f59e0b; }
.ac-breadcrumb .sep   { font-size: 0.65rem; }
.ac-breadcrumb .current { color: #475569; font-weight: 600; }

.ac-page-title {
  font-size: 2rem;
  font-weight: 900;
  color: #0f172a;
  margin: 0;
}
.ac-title-accent { color: #f59e0b; }

.ac-subtitle {
  font-size: 0.85rem;
  color: #64748b;
  margin: 6px 0 0;
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  gap: 6px;
}

.ac-ia-badge {
  background: linear-gradient(135deg, #8b5cf6, #6d28d9);
  color: white;
  padding: 2px 10px;
  border-radius: 20px;
  font-size: 0.72rem;
  font-weight: 800;
}

.ac-header-right {
  display: flex;
  align-items: center;
  gap: 12px;
}

.ac-btn-upload {
  background: #0f172a;
  color: #fbbf24;
  padding: 10px 20px;
  border-radius: 12px;
  cursor: pointer;
  font-weight: 700;
  font-size: 0.85rem;
  transition: 0.25s;
  display: inline-flex;
  align-items: center;
}
.ac-btn-upload:hover { background: #1e293b; transform: translateY(-1px); }

.ac-btn-icon {
  width: 42px;
  height: 42px;
  border-radius: 12px;
  border: 1px solid #e2e8f0;
  background: white;
  color: #64748b;
  cursor: pointer;
  transition: 0.25s;
  display: flex;
  align-items: center;
  justify-content: center;
}
.ac-btn-icon:hover:not(:disabled) { border-color: #f59e0b; color: #f59e0b; }
.ac-btn-icon:disabled { opacity: 0.5; cursor: not-allowed; }

/* ────────────────────────────────────────
   KPI CARDS
──────────────────────────────────────── */
.ac-kpi-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 20px;
}

.ac-kpi-card {
  background: white;
  padding: 20px;
  border-radius: 20px;
  border: 1px solid #e2e8f0;
  display: flex;
  align-items: center;
  gap: 15px;
  transition: 0.25s;
}
.ac-kpi-card:hover { transform: translateY(-3px); box-shadow: 0 8px 20px rgba(0,0,0,0.06); }

.ac-kpi-icon-wrap {
  width: 50px;
  height: 50px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.2rem;
  flex-shrink: 0;
}

.ac-kpi-value {
  font-size: 1.6rem;
  font-weight: 900;
  color: #0f172a;
  line-height: 1;
}
.ac-kpi-label {
  font-size: 0.7rem;
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 1.5px;
  color: #94a3b8;
  margin-top: 3px;
}

/* ────────────────────────────────────────
   FILTERS & SEARCH
──────────────────────────────────────── */
.ac-filters-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex-wrap: wrap;
  gap: 15px;
}

.ac-tabs-wrap {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
}

.ac-tab {
  padding: 8px 18px;
  border-radius: 30px;
  border: 1px solid #e2e8f0;
  background: white;
  color: #64748b;
  font-size: 0.82rem;
  font-weight: 600;
  cursor: pointer;
  transition: 0.2s;
}
.ac-tab:hover  { border-color: #f59e0b; color: #f59e0b; }
.ac-tab.active { background: #0f172a; color: #fbbf24; border-color: #0f172a; }

.ac-search-box {
  position: relative;
  display: flex;
  align-items: center;
}
.ac-search-icon {
  position: absolute;
  left: 14px;
  color: #94a3b8;
  font-size: 0.85rem;
}
.ac-search-input {
  padding: 9px 14px 9px 38px;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  background: white;
  font-size: 0.85rem;
  color: #334155;
  outline: none;
  width: 250px;
  transition: 0.2s;
  font-family: inherit;
}
.ac-search-input:focus { border-color: #f59e0b; box-shadow: 0 0 0 3px rgba(245,158,11,0.1); }

/* ────────────────────────────────────────
   EMPTY STATE
──────────────────────────────────────── */
.ac-empty-state {
  text-align: center;
  padding: 60px 20px;
  color: #94a3b8;
}

/* ────────────────────────────────────────
   CANDIDATE CARDS GRID
──────────────────────────────────────── */
.ac-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 20px;
}

.ac-profile-card {
  background: white;
  padding: 25px;
  border-radius: 24px;
  border: 1px solid #e2e8f0;
  cursor: pointer;
  transition: transform 0.25s, border-color 0.25s, box-shadow 0.25s;
  position: relative;
  animation: ac-fade-in 0.4s ease both;
}
.ac-profile-card:hover {
  transform: translateY(-5px);
  border-color: #f59e0b;
  box-shadow: 0 15px 30px rgba(0,0,0,0.06);
}

@keyframes ac-fade-in {
  from { opacity: 0; transform: translateY(12px); }
  to   { opacity: 1; transform: translateY(0);    }
}

.ac-ia-tag-float {
  position: absolute;
  top: 15px;
  right: 15px;
  background: #8b5cf6;
  color: white;
  padding: 3px 10px;
  border-radius: 20px;
  font-size: 0.65rem;
  font-weight: 800;
  letter-spacing: 0.5px;
}

.ac-card-top {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 15px;
}

.ac-avatar {
  width: 48px;
  height: 48px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 800;
  font-size: 1rem;
  color: #334155;
}

.ac-modal-avatar {
  width: 44px;
  height: 44px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 800;
  font-size: 0.95rem;
  color: #334155;
  flex-shrink: 0;
}

.ac-tier-badge {
  font-size: 0.68rem;
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 1px;
  padding: 4px 12px;
  border-radius: 20px;
}
.tier-élite    { background: #ecfdf5; color: #10b981; border: 1px solid #10b981; }
.tier-standard { background: #fffbeb; color: #f59e0b; border: 1px solid #f59e0b; }
.tier-basique  { background: #fff1f2; color: #ef4444; border: 1px solid #ef4444; }

.ac-candidate-name    { font-size: 1rem; font-weight: 800; color: #0f172a; margin: 0 0 4px; }
.ac-candidate-profile { font-size: 0.82rem; color: #64748b; margin: 0; }

.ac-card-score-area { margin-top: 18px; }
.ac-score-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 6px;
}
.ac-score-label { font-size: 0.68rem; font-weight: 800; text-transform: uppercase; letter-spacing: 1px; color: #94a3b8; }
.ac-score-value { font-size: 1.1rem; font-weight: 900; }

.ac-progress-track {
  background: #f1f5f9;
  border-radius: 99px;
  height: 6px;
  overflow: hidden;
}
.ac-progress-fill {
  height: 100%;
  border-radius: 99px;
  transition: width 0.6s ease;
}

.ac-quick-insight {
  margin-top: 14px;
  background: #f8fafc;
  border-radius: 12px;
  padding: 10px 14px;
  font-size: 0.8rem;
  color: #64748b;
  display: flex;
  align-items: flex-start;
  gap: 6px;
  line-height: 1.4;
}

/* ────────────────────────────────────────
   MODAL
──────────────────────────────────────── */
.ac-modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(15, 23, 42, 0.6);
  backdrop-filter: blur(5px);
  z-index: 9999;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
}

.ac-modal {
  background: white;
  border-radius: 30px;
  width: 100%;
  max-width: 750px;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 25px 50px rgba(0,0,0,0.2);
}

.ac-modal-header {
  padding: 22px 25px;
  border-bottom: 1px solid #f1f5f9;
  display: flex;
  align-items: center;
  gap: 12px;
  position: sticky;
  top: 0;
  background: white;
  z-index: 1;
  border-radius: 30px 30px 0 0;
}

.ac-modal-close {
  background: #f1f5f9;
  border: none;
  width: 36px;
  height: 36px;
  border-radius: 50%;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #64748b;
  transition: 0.2s;
  flex-shrink: 0;
}
.ac-modal-close:hover { background: #fee2e2; color: #ef4444; }

.ac-modal-loader {
  padding: 60px 30px;
  text-align: center;
  color: #64748b;
}

.ac-modal-body { padding: 30px; }

.ac-big-score {
  font-size: 4.5rem;
  font-weight: 900;
  line-height: 1;
}

.ac-insight-box {
  padding: 20px;
  border-radius: 20px;
  height: 100%;
}
.ac-insight-force { background: #ecfdf5; border: 1px solid #10b981; }
.ac-insight-axe   { background: #fff1f2; border: 1px solid #f43f5e; }
.ac-insight-tips  { background: #f5f3ff; border: 1px solid #8b5cf6; }

.ac-insight-title {
  font-size: 0.7rem;
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 1.5px;
  margin-bottom: 12px;
  color: #64748b;
  display: flex;
  align-items: center;
}

.ac-insight-list {
  list-style: none;
  padding: 0;
  margin: 0;
  font-size: 0.85rem;
}
.ac-insight-list li {
  margin-bottom: 7px;
  color: #334155;
  padding-left: 16px;
  position: relative;
  line-height: 1.5;
}
.ac-insight-list li::before {
  content: "•";
  position: absolute;
  left: 0;
  font-weight: bold;
}

.ac-decision-strip {
  background: #0f172a;
  color: #fbbf24;
  padding: 16px 22px;
  border-radius: 15px;
  font-size: 0.9rem;
  text-align: center;
  line-height: 1.5;
}

.ac-analysis-date {
  text-align: center;
  font-size: 0.78rem;
  color: #94a3b8;
}

.ac-empty-robot {
  color: #cbd5e1;
}

.ac-btn-upload-big {
  background: #8b5cf6;
  color: white;
  padding: 14px 28px;
  border-radius: 15px;
  cursor: pointer;
  font-weight: 800;
  font-size: 0.9rem;
  display: inline-flex;
  align-items: center;
  transition: 0.25s;
}
.ac-btn-upload-big:hover { background: #7c3aed; transform: scale(1.04); }

/* ────────────────────────────────────────
   TOAST
──────────────────────────────────────── */
.ac-toast {
  position: fixed;
  bottom: 30px;
  right: 30px;
  padding: 14px 22px;
  border-radius: 14px;
  color: white;
  display: flex;
  align-items: center;
  gap: 10px;
  font-weight: 700;
  font-size: 0.88rem;
  box-shadow: 0 10px 25px rgba(0,0,0,0.15);
  z-index: 99999;
  max-width: 360px;
}
.ac-toast-success { background: #10b981; }
.ac-toast-error   { background: #f43f5e; }
.ac-toast-info    { background: #3b82f6; }

/* ────────────────────────────────────────
   ANIMATIONS TRANSITION
──────────────────────────────────────── */
.ac-modal-anim-enter-active,
.ac-modal-anim-leave-active {
  transition: opacity 0.25s ease;
}
.ac-modal-anim-enter-active .ac-modal,
.ac-modal-anim-leave-active .ac-modal {
  transition: transform 0.25s ease, opacity 0.25s ease;
}
.ac-modal-anim-enter-from,
.ac-modal-anim-leave-to {
  opacity: 0;
}
.ac-modal-anim-enter-from .ac-modal,
.ac-modal-anim-leave-to .ac-modal {
  transform: scale(0.95) translateY(10px);
  opacity: 0;
}

.ac-toast-anim-enter-active,
.ac-toast-anim-leave-active { transition: all 0.3s ease; }
.ac-toast-anim-enter-from,
.ac-toast-anim-leave-to { opacity: 0; transform: translateY(15px); }

/* ────────────────────────────────────────
   RESPONSIVE
──────────────────────────────────────── */
@media (max-width: 768px) {
  .ac-kpi-grid         { grid-template-columns: 1fr; }
  .ac-page-header      { flex-direction: column; align-items: flex-start; }
  .ac-filters-row      { flex-direction: column; align-items: flex-start; }
  .ac-search-input     { width: 100%; }
  .ac-search-box       { width: 100%; }
  .ac-grid             { grid-template-columns: 1fr; }
  .ac-big-score        { font-size: 3rem; }
  .ac-modal            { border-radius: 20px; }
}
</style>