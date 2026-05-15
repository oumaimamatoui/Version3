<template>
  <div class="d-flex admin-layout" dir="ltr">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css">

    <AppSidebar />

    <div class="content-right flex-grow-1">
      <AppNavbar />

      <main class="p-4 pt-2" :class="{ 'rtl-content': locale === 'ar' || locale === 'AR' }">
        <div class="ai-generator-container animate-fade-in">

          <!-- ══ BREADCRUMB ══ -->
          <div class="breadcrumb-bar mb-3">
            <span class="bc-item">
              <i class="fa-solid fa-house-chimney bc-icon"></i>
              {{ t('sidebar.links.overview') }}
            </span>
            <i class="fa-solid fa-chevron-right bc-sep"></i>
            <span class="bc-item">{{ t('sidebar.links.bank') }}</span>
            <i class="fa-solid fa-chevron-right bc-sep"></i>
            <span class="bc-item bc-active">{{ t('sidebar.links.ai') }}</span>
          </div>

          <!-- ══ PAGE HEADER ══ -->
          <div class="page-header mb-5 d-flex justify-content-between align-items-end flex-wrap gap-3">
            <div>
              <div class="badge-ai-powered mb-2">
                <span class="ai-badge-dot"></span>
                <i class="fa-solid fa-sparkles me-1"></i>
                {{ t('iaGenerator.badgeLabel') }}
              </div>
              <h2 class="page-title">{{ t('iaGenerator.title') }}</h2>
              <p class="page-subtitle text-muted">{{ t('iaGenerator.subtitle') }}</p>
            </div>

            <div class="usage-pill" v-if="generatedQuestions.length > 0">
              <div class="usage-ring">
                <svg viewBox="0 0 36 36" class="ring-svg">
                  <circle cx="18" cy="18" r="15" fill="none" stroke="#f1f5f9" stroke-width="3"/>
                  <circle cx="18" cy="18" r="15" fill="none" stroke="#eab308" stroke-width="3"
                    :stroke-dasharray="`${(generatedQuestions.length / settings.count) * 94} 94`"
                    stroke-dashoffset="23.5" stroke-linecap="round"
                    style="transition:stroke-dasharray 0.6s ease;"/>
                </svg>
                <span class="ring-val">{{ generatedQuestions.length }}</span>
              </div>
              <div>
                <p class="usage-label m-0">{{ t('iaGenerator.questionsGenerated') }}</p>
                <p class="usage-sub m-0">{{ t('iaGenerator.outOf') }} {{ settings.count }}</p>
              </div>
            </div>
          </div>

          <div class="row g-4">
            <!-- ════════ LEFT COLUMN ════════ -->
            <div class="col-lg-5">

              <!-- Upload -->
              <div class="glass-card p-4 mb-4">
                <div class="section-title-pro mb-4">
                  <div class="icon-pill bg-navy"><i class="fa-solid fa-file-arrow-up"></i></div>
                  <div>
                    <h6 class="card-section-title">{{ t('iaGenerator.contextDoc') }}</h6>
                    <p class="card-section-sub">{{ t('iaGenerator.contextDocSub') }}</p>
                  </div>
                </div>

                <div class="upload-zone" @click="triggerUpload" :class="{ 'upload-zone-active': files.length }">
                  <div class="upload-zone-inner">
                    <i class="fa-solid fa-cloud-arrow-up upload-cloud mb-2"></i>
                    <div v-if="!files.length">
                      <p class="upload-title">{{ t('iaGenerator.uploadClick') }}</p>
                    </div>
                    <div v-else class="upload-success-msg">{{ files[0].name }}</div>
                  </div>
                  <input type="file" ref="fileRef" hidden @change="handleFile" accept=".pdf,.docx">
                </div>
              </div>

              <!-- Paramètres IA -->
              <div class="glass-card p-4">
                <div class="section-title-pro mb-4">
                  <div class="icon-pill bg-amber"><i class="fa-solid fa-sliders"></i></div>
                  <div>
                    <h6 class="card-section-title">{{ t('iaGenerator.aiParams') }}</h6>
                    <p class="card-section-sub">{{ t('iaGenerator.aiParamsSub') }}</p>
                  </div>
                </div>

                <!-- Nombre de questions -->
                <div class="setting-block mb-4">
                  <label class="setting-label mb-2">{{ t('iaGenerator.questionCount') }} ({{ settings.count }})</label>
                  <input type="range" v-model="settings.count" min="5" max="100" step="5" class="custom-range">
                  <div class="range-labels">
                    <span>5</span>
                    <span>25</span>
                    <span>50</span>
                    <span>75</span>
                    <span>100</span>
                  </div>
                </div>

                <!-- ═══ THÈME DYNAMIQUE ═══ -->
                <div class="setting-block mb-3">
                  <label class="setting-label mb-2">
                    <i class="fa-solid fa-folder-open me-1 text-amber-soft"></i>
                    Thème
                    <span v-if="isLoadingBank" class="loading-indicator ms-2">
                      <i class="fa-solid fa-circle-notch fa-spin"></i>
                    </span>
                  </label>

                  <div v-if="isLoadingBank" class="skeleton-select"></div>

                  <div v-else-if="themesList.length === 0" class="empty-bank-hint">
                    <i class="fa-solid fa-triangle-exclamation me-1"></i>
                    Aucun thème dans la banque de questions.
                  </div>

                  <div v-else class="theme-select-wrapper">
                    <i class="fa-solid fa-folder theme-ico"></i>
                    <select v-model="settings.theme" class="form-select-custom" @change="onThemeChange">
                      <option value="">— Choisir un thème —</option>
                      <option v-for="th in themesList" :key="th" :value="th">{{ th }}</option>
                    </select>
                    <i class="fa-solid fa-chevron-down select-arrow"></i>
                  </div>
                </div>

                <!-- ═══ SOUS-THÈME DYNAMIQUE ═══ -->
                <div class="setting-block mb-4">
                  <label class="setting-label mb-2">
                    <i class="fa-solid fa-tags me-1 text-amber-soft"></i>
                    Sous-thème
                    <span class="optional-badge ms-1">optionnel</span>
                  </label>

                  <div class="theme-select-wrapper" :class="{ 'disabled-wrapper': !settings.theme || sousThemesList.length === 0 }">
                    <i class="fa-solid fa-tag theme-ico"></i>
                    <select
                      v-model="settings.sousTheme"
                      class="form-select-custom"
                      :disabled="!settings.theme || sousThemesList.length === 0"
                    >
                      <option value="">
                        {{ sousThemesList.length === 0 && settings.theme
                          ? 'Aucun sous-thème disponible'
                          : '— Tous les sous-thèmes —' }}
                      </option>
                      <option v-for="st in sousThemesList" :key="st" :value="st">{{ st }}</option>
                    </select>
                    <i class="fa-solid fa-chevron-down select-arrow"></i>
                  </div>

                  <!-- Pills des sous-thèmes disponibles -->
                  <div v-if="sousThemesList.length > 0" class="sous-themes-pills mt-2">
                    <button
                      v-for="st in sousThemesList"
                      :key="st"
                      @click="settings.sousTheme = settings.sousTheme === st ? '' : st"
                      :class="['sous-theme-pill', { active: settings.sousTheme === st }]"
                    >
                      <i class="fa-solid fa-tag me-1" style="font-size:8px"></i>
                      {{ st }}
                    </button>
                  </div>
                </div>

                <!-- Résumé sélection -->
                <div v-if="settings.theme" class="selection-summary mb-4">
                  <div class="sel-row">
                    <span class="sel-label">Thème :</span>
                    <span class="sel-value">
                      <span class="sel-badge">{{ settings.theme }}</span>
                    </span>
                  </div>
                  <div class="sel-row" v-if="settings.sousTheme">
                    <span class="sel-label">Sous-thème :</span>
                    <span class="sel-value">
                      <span class="sel-badge sel-badge-sub">{{ settings.sousTheme }}</span>
                    </span>
                  </div>
                  <div class="sel-row">
                    <span class="sel-label">Questions dans la banque :</span>
                    <span class="sel-value fw-bold text-amber-soft">{{ questionCountForSelection }}</span>
                  </div>
                </div>

                <!-- Langue -->
                <div class="setting-block mb-4">
                  <label class="setting-label mb-2">{{ t('iaGenerator.language') }}</label>
                  <div class="lang-pills">
                    <button v-for="lang in languages" :key="lang.code"
                            @click="settings.langue = lang.code"
                            :class="['lang-pill', { active: settings.langue === lang.code }]">
                      {{ lang.label }}
                    </button>
                  </div>
                </div>

                <button class="btn-generate w-100" @click="startGeneration" :disabled="isGenerating || !settings.theme">
                  <span v-if="!isGenerating">
                    <i class="fa-solid fa-wand-magic-sparkles me-2"></i>
                    {{ t('iaGenerator.generateBtn') }}
                  </span>
                  <span v-else>
                    <i class="fa-solid fa-spinner fa-spin me-2"></i>
                    {{ t('iaGenerator.generating') }}
                  </span>
                </button>

                <p v-if="!settings.theme" class="btn-hint mt-2">
                  <i class="fa-solid fa-circle-info me-1"></i>
                  Sélectionnez un thème pour activer la génération.
                </p>
              </div>
            </div>

            <!-- ════════ RIGHT COLUMN (Results) ════════ -->
            <div class="col-lg-7">
              <div class="glass-card d-flex flex-column h-100">
                <div class="p-4 d-flex justify-content-between align-items-center border-bottom">
                  <h6 class="card-section-title mb-0">{{ t('iaGenerator.extractedQuestions') }}</h6>
                  <button class="btn-save" @click="saveAllQuestions" :disabled="!generatedQuestions.length">
                    {{ t('iaGenerator.saveAll') }}
                  </button>
                </div>

                <div class="questions-list custom-scrollbar p-4 flex-grow-1">
                  <div v-if="!generatedQuestions.length && !isGenerating" class="empty-state">
                    <i class="fa-solid fa-robot fa-3x mb-3 text-light"></i>
                    <h5>{{ t('iaGenerator.emptyTitle') }}</h5>
                  </div>

                  <div v-for="(q, idx) in generatedQuestions" :key="idx" class="q-card mb-4">
                    <div class="q-card-header mb-2">
                      <span class="badge bg-dark">#{{ idx + 1 }}</span>
                    </div>
                    <p class="q-text mb-3"><strong>{{ q.question }}</strong></p>
                    <div class="options-grid">
                      <div v-for="(opt, oIdx) in q.options" :key="oIdx"
                           class="opt-item"
                           :class="{ 'opt-correct': q.answer === oIdx }">
                        <span class="opt-letter me-2">{{ String.fromCharCode(65 + oIdx) }}</span>
                        <span class="opt-text">{{ opt }}</span>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import axios from 'axios';
import Swal from 'sweetalert2';
import api from '@/services/api';

const { t, locale } = useI18n();

// ══════════════════════════════════════════════════════════════
// STATE
// ══════════════════════════════════════════════════════════════
const isGenerating   = ref(false);
const isLoadingBank  = ref(false);
const generatedQuestions = ref([]);
const fileRef        = ref(null);
const files          = ref([]);

// Données brutes de la banque : liste de questions avec theme + sousTheme
const bankQuestions  = ref([]);
// Catégories complètes depuis /api/Categories (thèmes + sous-thèmes)
const categoriesList = ref([]);

const settings = reactive({
  count:     10,
  theme:     '',
  sousTheme: '',
  difficulty:'Medium',
  langue:    'fr',
});

const languages = [
  { code: 'fr', label: 'Français' },
  { code: 'en', label: 'English' },
  { code: 'ar', label: 'العربية' },
];

// ══════════════════════════════════════════════════════════════
// COMPUTED — thèmes & sous-thèmes depuis la banque
// FIX: normalisation casse (UPPERCASE/lowercase/TitleCase)
//      + support double-casse JSON (theme / Theme)
// ══════════════════════════════════════════════════════════════

// Normalise un thème : "FRONTEND" ou "frontend" → "Frontend"
const normalizeStr = (str) => {
  if (!str) return '';
  const s = str.trim();
  if (!s) return '';
  return s.charAt(0).toUpperCase() + s.slice(1).toLowerCase();
};

// Lit le champ theme d'une question quelle que soit la casse JSON
const getTheme    = (q) => (q.theme     || q.Theme     || '').trim();
const getSousTheme = (q) => (q.sousTheme || q.SousTheme || '').trim();

/**
 * Liste unique et triée des thèmes.
 * SOURCE 1 : /api/Categories → toutes les catégories créées (même sans questions)
 * SOURCE 2 : /api/Questions  → thèmes des questions existantes
 * Les deux sont fusionnés et dédupliqués (insensible à la casse).
 */
const themesList = computed(() => {
  const map = new Map(); // KEY_UPPER → displayLabel

  // Source 1 : catégories du référentiel
  categoriesList.value.forEach(cat => {
    const raw = (cat.nom || cat.Nom || '').trim();
    if (!raw) return;
    const key = raw.toUpperCase();
    if (!map.has(key)) map.set(key, raw); // garde le nom tel quel (déjà propre)
  });

  // Source 2 : thèmes des questions existantes (fallback si pas de catégories)
  bankQuestions.value.forEach(q => {
    const raw = getTheme(q);
    if (!raw) return;
    const key = raw.toUpperCase();
    if (!map.has(key)) map.set(key, normalizeStr(raw));
  });

  const list = [...map.values()].sort((a, b) =>
    a.localeCompare(b, 'fr', { sensitivity: 'base' })
  );
  console.log('[IAGenerator] thèmes fusionnés:', list,
    '| catégories:', categoriesList.value.length,
    '| questions:', bankQuestions.value.length);
  return list;
});

/**
 * Sous-thèmes disponibles pour le thème sélectionné.
 * SOURCE 1 : /api/Categories → sousCategories de la catégorie choisie
 * SOURCE 2 : /api/Questions  → sousTheme des questions existantes
 */
const sousThemesList = computed(() => {
  if (!settings.theme) return [];
  const map = new Map();

  // Source 1 : sous-catégories du référentiel
  const matchedCat = categoriesList.value.find(cat =>
    (cat.nom || cat.Nom || '').toUpperCase() === settings.theme.toUpperCase()
  );
  if (matchedCat) {
    const subs = matchedCat.sousCategories || matchedCat.SousCategories || [];
    subs.forEach(sub => {
      const raw = (sub.nom || sub.Nom || '').trim();
      if (!raw) return;
      const key = raw.toUpperCase();
      if (!map.has(key)) map.set(key, raw);
    });
  }

  // Source 2 : sous-thèmes des questions existantes
  bankQuestions.value.forEach(q => {
    const rawT = getTheme(q);
    if (rawT.toUpperCase() !== settings.theme.toUpperCase()) return;
    const rawS = getSousTheme(q);
    if (!rawS) return;
    const key = rawS.toUpperCase();
    if (!map.has(key)) map.set(key, normalizeStr(rawS));
  });

  return [...map.values()].sort((a, b) =>
    a.localeCompare(b, 'fr', { sensitivity: 'base' })
  );
});

/** Nombre de questions dans la banque pour la sélection courante */
const questionCountForSelection = computed(() => {
  return bankQuestions.value.filter(q => {
    const themeMatch = getTheme(q).toUpperCase() === settings.theme.toUpperCase();
    const sousMatch  = !settings.sousTheme ||
      getSousTheme(q).toUpperCase() === settings.sousTheme.toUpperCase();
    return themeMatch && sousMatch;
  }).length;
});

// ══════════════════════════════════════════════════════════════
// HANDLERS
// ══════════════════════════════════════════════════════════════

const onThemeChange = () => {
  settings.sousTheme = '';
};

const triggerUpload = () => fileRef.value.click();

const handleFile = (e) => {
  const file = e.target.files[0];
  if (file) files.value = [{ name: file.name, raw: file }];
};

// ══════════════════════════════════════════════════════════════
// FETCH — Questions ET Catégories en parallèle
// Les deux sources sont nécessaires pour afficher TOUS les thèmes
// ══════════════════════════════════════════════════════════════
const fetchBankQuestions = async () => {
  isLoadingBank.value = true;
  try {
    // Chargement parallèle des deux endpoints
    const [resQ, resC] = await Promise.allSettled([
      api.get('/Questions'),
      api.get('/Categories'),
    ]);

    // Questions
    if (resQ.status === 'fulfilled') {
      bankQuestions.value = resQ.value.data || [];
      console.log('[IAGenerator] Questions chargées:', bankQuestions.value.length);
    } else {
      console.warn('[IAGenerator] /Questions échoué:', resQ.reason);
      bankQuestions.value = [];
    }

    // Catégories
    if (resC.status === 'fulfilled') {
      categoriesList.value = resC.value.data || [];
      console.log('[IAGenerator] Catégories chargées:', categoriesList.value.length,
        categoriesList.value.map(c => c.nom || c.Nom));
    } else {
      console.warn('[IAGenerator] /Categories échoué:', resC.reason);
      categoriesList.value = [];
    }

  } catch (err) {
    console.error('[IAGenerator] Erreur chargement :', err);
  } finally {
    isLoadingBank.value = false;
  }
};

// ══════════════════════════════════════════════════════════════
// GÉNÉRATION IA
// ══════════════════════════════════════════════════════════════
const startGeneration = async () => {
  if (!settings.theme) {
    Swal.fire({
      icon: 'warning',
      title: t('iaGenerator.warningTitle'),
      text: 'Veuillez sélectionner un thème avant de générer.',
    });
    return;
  }

  isGenerating.value = true;
  try {
    const fd = new FormData();
    if (files.value.length) fd.append('file', files.value[0].raw);
    fd.append('nombre',     settings.count);
    fd.append('themetique', settings.sousTheme || settings.theme);
    fd.append('theme',      settings.theme);
    fd.append('sousTheme',  settings.sousTheme);
    fd.append('difficulte', settings.difficulty);
    fd.append('langue',     settings.langue);

    const res = await axios.post('http://127.0.0.1:8000/ia/generate-pro', fd);
    if (res.data.status === 'IA_SUCCESS') {
      generatedQuestions.value = res.data.questions;
    }
  } catch (err) {
    Swal.fire({ icon: 'error', title: 'Error', text: 'Generation failed' });
  } finally {
    isGenerating.value = false;
  }
};

// ══════════════════════════════════════════════════════════════
// SAUVEGARDE
// ══════════════════════════════════════════════════════════════
const saveAllQuestions = async () => {
  try {
    for (const q of generatedQuestions.value) {
      await api.post('/Questions', {
        enonce:       q.question,
        type:         0,
        points:       2,
        theme:        settings.theme,
        sousTheme:    settings.sousTheme || '',
        choix:        q.options,
        bonneReponse: q.options[q.answer],
        langue:       settings.langue,
      });
    }
    Swal.fire({ icon: 'success', title: t('iaGenerator.saveSuccess') });
    generatedQuestions.value = [];
    // Rafraîchir la banque après sauvegarde
    await fetchBankQuestions();
  } catch (err) {
    Swal.fire({ icon: 'error', title: 'Error', text: 'Save failed' });
  }
};

// ══════════════════════════════════════════════════════════════
// INIT
// ══════════════════════════════════════════════════════════════
onMounted(() => fetchBankQuestions());
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:ital,wght@0,400;0,500;0,600;0,700;0,800;1,700&display=swap');

/* ════════ BASE ════════ */
.admin-layout {
  min-height: 100vh;
  background-color: #f8fafc;
  font-family: 'Plus Jakarta Sans', sans-serif;
  display: flex !important;
  flex-direction: row !important;
}

/* ════════ BREADCRUMB ════════ */
.breadcrumb-bar {
  display: flex; align-items: center; gap: 8px;
  font-size: 12px; font-weight: 600; color: #94a3b8;
}
.bc-icon { font-size: 10px; }
.bc-sep  { font-size: 8px; color: #cbd5e1; }
.bc-active { color: #eab308; }

/* ════════ PAGE HEADER ════════ */
.badge-ai-powered {
  display: inline-flex; align-items: center; gap: 6px;
  background: linear-gradient(90deg, #0f172a, #0891b2);
  color: white; padding: 5px 14px; border-radius: 100px;
  font-size: 10px; font-weight: 800; text-transform: uppercase; letter-spacing: 1px;
}
.ai-badge-dot {
  width: 6px; height: 6px; background: #4ade80; border-radius: 50%;
  animation: blink 1.5s ease-in-out infinite;
}
@keyframes blink { 0%,100%{opacity:1} 50%{opacity:0.3} }

.page-title {
  font-size: 32px; font-weight: 800; color: #0f172a;
  letter-spacing: -1px; margin: 6px 0 4px;
}
.page-subtitle { font-size: 14px; color: #64748b; margin: 0; }

/* Usage pill */
.usage-pill {
  display: flex; align-items: center; gap: 14px;
  background: white; border: 1.5px solid #f1f5f9;
  border-radius: 20px; padding: 16px 22px;
  box-shadow: 0 4px 20px rgba(0,0,0,0.04);
}
.usage-ring { position: relative; width: 48px; height: 48px; flex-shrink: 0; }
.ring-svg   { width: 48px; height: 48px; transform: rotate(-90deg); }
.ring-val {
  position: absolute; inset: 0;
  display: flex; align-items: center; justify-content: center;
  font-size: 14px; font-weight: 800; color: #0f172a;
}
.usage-label { font-size: 13px; font-weight: 700; color: #0f172a; margin: 0 0 2px; }
.usage-sub   { font-size: 11px; color: #94a3b8; margin: 0; font-weight: 600; }

/* ════════ GLASS CARDS ════════ */
.glass-card {
  background: white; border-radius: 24px;
  border: 1px solid #f1f5f9;
  box-shadow: 0 8px 32px rgba(0,0,0,0.04);
  transition: box-shadow 0.3s;
}
.glass-card:hover { box-shadow: 0 12px 40px rgba(0,0,0,0.06); }

.section-title-pro  { display: flex; align-items: flex-start; gap: 14px; }
.icon-pill {
  width: 38px; height: 38px; min-width: 38px; border-radius: 12px;
  display: flex; align-items: center; justify-content: center;
  font-size: 15px; color: white; flex-shrink: 0;
}
.bg-navy  { background-color: #0f172a; }
.bg-amber { background-color: #eab308; color: #0f172a !important; }

.card-section-title { font-size: 15px; font-weight: 800; color: #0f172a; margin: 0 0 2px; letter-spacing: -0.3px; }
.card-section-sub   { font-size: 12px; color: #94a3b8; margin: 0; font-weight: 600; }

/* ════════ UPLOAD ZONE ════════ */
.upload-zone {
  border: 2px dashed #cbd5e1; border-radius: 20px;
  cursor: pointer; transition: 0.3s; background: #fcfcfd;
  min-height: 120px; display: flex; align-items: center; justify-content: center;
}
.upload-zone:hover { border-color: #eab308; background: #fffbeb; }
.upload-zone-active { border-color: #22c55e; background: #f0fdf4; border-style: solid; }
.upload-zone-inner {
  display: flex; flex-direction: column; align-items: center;
  gap: 8px; padding: 28px 20px; text-align: center; width: 100%;
}
.upload-cloud { font-size: 22px; color: #94a3b8; transition: 0.3s; }
.upload-zone:hover .upload-cloud { color: #eab308; }
.upload-title { font-size: 13px; font-weight: 800; color: #0f172a; margin: 0; }
.upload-success-msg { font-size: 13px; font-weight: 700; color: #16a34a; }

/* ════════ SETTINGS ════════ */
.setting-label {
  font-size: 12px; font-weight: 700; color: #475569;
  text-transform: uppercase; letter-spacing: 0.8px; display: block;
}
.text-amber-soft { color: #d97706; }

/* Range */
.custom-range {
  -webkit-appearance: none; width: 100%; height: 6px;
  background: #f1f5f9; border-radius: 10px; outline: none; cursor: pointer;
}
.custom-range::-webkit-slider-thumb {
  -webkit-appearance: none; width: 20px; height: 20px;
  background: #eab308; border-radius: 50%; cursor: pointer;
  border: 3px solid white; box-shadow: 0 2px 8px rgba(234,179,8,0.4); transition: 0.2s;
}
.custom-range::-webkit-slider-thumb:hover { transform: scale(1.2); }

.range-labels {
  display: flex;
  justify-content: space-between;
  margin-top: 6px;
}
.range-labels span {
  font-size: 10px;
  font-weight: 700;
  color: #cbd5e1;
}

/* ═══════════════════════════════════
   THEME SELECT WRAPPER
═══════════════════════════════════ */
.theme-select-wrapper {
  position: relative;
  display: flex;
  align-items: center;
}
.theme-ico {
  position: absolute; left: 14px; z-index: 2;
  font-size: 12px; color: #eab308; pointer-events: none;
}
.select-arrow {
  position: absolute; right: 14px; z-index: 2;
  font-size: 10px; color: #94a3b8; pointer-events: none;
}
.form-select-custom {
  width: 100%;
  padding: 12px 36px 12px 36px;
  border: 1.5px solid #e2e8f0;
  border-radius: 14px;
  font-size: 13px;
  font-family: inherit;
  font-weight: 600;
  color: #0f172a;
  outline: none;
  appearance: none;
  -webkit-appearance: none;
  background: white;
  cursor: pointer;
  transition: 0.2s;
}
.form-select-custom:focus {
  border-color: #eab308;
  box-shadow: 0 0 0 3px rgba(234,179,8,0.1);
}
.form-select-custom:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}
.disabled-wrapper {
  opacity: 0.5;
  pointer-events: none;
}

/* ═══════════════════════════════════
   SKELETON LOADER
═══════════════════════════════════ */
.skeleton-select {
  height: 48px;
  border-radius: 14px;
  background: linear-gradient(90deg, #f1f5f9 25%, #e2e8f0 50%, #f1f5f9 75%);
  background-size: 200% 100%;
  animation: skeletonShimmer 1.4s ease-in-out infinite;
}
@keyframes skeletonShimmer {
  0%   { background-position: 200% 0; }
  100% { background-position: -200% 0; }
}

/* ═══════════════════════════════════
   LOADING INDICATOR
═══════════════════════════════════ */
.loading-indicator { color: #94a3b8; font-size: 11px; }

/* ═══════════════════════════════════
   EMPTY BANK HINT
═══════════════════════════════════ */
.empty-bank-hint {
  padding: 12px 16px;
  background: #fffbeb;
  border: 1.5px dashed #fde68a;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 600;
  color: #92400e;
}

/* ═══════════════════════════════════
   SOUS-THÈMES PILLS
═══════════════════════════════════ */
.sous-themes-pills {
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
}
.sous-theme-pill {
  padding: 5px 12px;
  border: 1.5px solid #e2e8f0;
  background: #f8fafc;
  border-radius: 50px;
  font-size: 11px;
  font-weight: 700;
  color: #64748b;
  cursor: pointer;
  transition: 0.2s;
  font-family: inherit;
  display: flex;
  align-items: center;
}
.sous-theme-pill:hover {
  border-color: #fde68a;
  background: #fffbeb;
  color: #92400e;
}
.sous-theme-pill.active {
  border-color: #eab308;
  background: #fffbeb;
  color: #92400e;
  font-weight: 800;
}

/* ═══════════════════════════════════
   SELECTION SUMMARY
═══════════════════════════════════ */
.selection-summary {
  background: #f8fafc;
  border: 1.5px solid #e2e8f0;
  border-radius: 16px;
  padding: 14px 16px;
  display: flex;
  flex-direction: column;
  gap: 8px;
}
.sel-row {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 12px;
}
.sel-label {
  font-size: 11px;
  font-weight: 700;
  color: #94a3b8;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  white-space: nowrap;
  min-width: 130px;
}
.sel-value { flex: 1; }
.sel-badge {
  display: inline-flex;
  align-items: center;
  background: #0f172a;
  color: #eab308;
  padding: 3px 10px;
  border-radius: 8px;
  font-size: 11px;
  font-weight: 800;
  letter-spacing: 0.3px;
}
.sel-badge-sub {
  background: #fffbeb;
  color: #92400e;
  border: 1px solid #fde68a;
}
.fw-bold { font-weight: 700 !important; }

/* ════════ OPTIONAL BADGE ════════ */
.optional-badge {
  font-size: 9px;
  font-weight: 700;
  background: #f1f5f9;
  color: #94a3b8;
  padding: 2px 7px;
  border-radius: 20px;
  text-transform: lowercase;
  letter-spacing: 0;
}

/* ════════ INPUT ════════ */
.form-input {
  width: 100%; padding: 12px 16px;
  border: 1.5px solid #e2e8f0; border-radius: 14px;
  font-size: 14px; font-family: inherit; font-weight: 600;
  color: #0f172a; outline: none; transition: 0.2s; background: white;
}
.form-input:focus { border-color: #eab308; box-shadow: 0 0 0 3px rgba(234,179,8,0.1); }
.form-input::placeholder { color: #cbd5e1; font-weight: 500; }

/* ════════ LANG PILLS ════════ */
.lang-pills { display: flex; gap: 8px; }
.lang-pill {
  flex: 1; border: 1.5px solid #e2e8f0; background: #f8fafc;
  padding: 10px 8px; border-radius: 12px; font-size: 12px;
  font-weight: 700; color: #64748b; cursor: pointer; transition: 0.2s;
  display: flex; align-items: center; justify-content: center;
  font-family: inherit;
}
.lang-pill:hover  { border-color: #fde68a; color: #0f172a; background: #fefce8; }
.lang-pill.active { border-color: #eab308; background: #fffbeb; color: #92400e; font-weight: 800; }

/* ════════ GENERATE BUTTON ════════ */
.btn-generate {
  background: #eab308; color: #0f172a; border: none;
  padding: 16px 24px; border-radius: 18px; font-weight: 800;
  font-size: 15px; font-family: inherit; cursor: pointer; transition: 0.3s;
  letter-spacing: 0.5px;
}
.btn-generate:hover:not(:disabled) {
  background: #ca8a04; transform: translateY(-2px);
  box-shadow: 0 14px 30px rgba(234,179,8,0.4);
}
.btn-generate:disabled { opacity: 0.6; cursor: not-allowed; }

.btn-hint {
  text-align: center; font-size: 11px; color: #94a3b8;
  font-weight: 600; margin: 0;
}

/* ════════ RIGHT PANEL ════════ */
.btn-save {
  background: #0f172a; color: white; border: none;
  padding: 8px 20px; border-radius: 10px; font-size: 12px;
  font-weight: 700; cursor: pointer; transition: 0.2s; font-family: inherit;
}
.btn-save:hover:not(:disabled) { background: #1e293b; transform: translateY(-1px); }
.btn-save:disabled { opacity: 0.6; cursor: not-allowed; }

/* ════════ EMPTY STATE ════════ */
.empty-state {
  display: flex; flex-direction: column; align-items: center;
  justify-content: center; padding: 60px 30px; text-align: center;
}

/* ════════ QUESTION CARDS ════════ */
.questions-list { max-height: 60vh; overflow-y: auto; padding-right: 4px; }
.custom-scrollbar::-webkit-scrollbar { width: 4px; }
.custom-scrollbar::-webkit-scrollbar-track { background: transparent; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: #e2e8f0; border-radius: 10px; }
.custom-scrollbar::-webkit-scrollbar-thumb:hover { background: #cbd5e1; }

.q-card {
  background: white; border: 1.5px solid #f1f5f9;
  border-radius: 20px; padding: 20px; margin-bottom: 12px;
  transition: 0.25s; cursor: default;
}
.q-card:hover { border-color: #fde68a; transform: translateY(-2px); box-shadow: 0 8px 24px rgba(0,0,0,0.05); }
.q-card-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 12px; }
.q-text { font-size: 14px; font-weight: 700; color: #0f172a; margin: 0 0 14px; line-height: 1.6; }

.opt-item {
  display: flex; align-items: center; gap: 10px;
  padding: 10px 14px; border-radius: 12px; background: #f8fafc;
  margin-bottom: 6px; font-size: 13px; color: #475569; font-weight: 600;
  border: 1px solid transparent; transition: 0.2s;
}
.opt-correct { background: #f0fdf4; border-color: #86efac; color: #166534; font-weight: 700; }
.opt-letter {
  width: 24px; height: 24px; min-width: 24px; border-radius: 8px;
  border: 1.5px solid #cbd5e1; display: flex; align-items: center; justify-content: center;
  font-size: 10px; font-weight: 800; color: #94a3b8;
}
.opt-text { flex: 1; }

/* ════════ ANIMATIONS ════════ */
.animate-fade-in { animation: fadeIn 0.5s ease-out; }
@keyframes fadeIn {
  from { opacity: 0; transform: translateY(12px); }
  to   { opacity: 1; transform: translateY(0); }
}

/* ════════ RTL SUPPORT ════════ */
.rtl-content { direction: rtl; text-align: right; }
.rtl-content .section-title-pro { flex-direction: row; text-align: right; }
.rtl-content .icon-pill { margin-left: 15px; margin-right: 0; }
</style>