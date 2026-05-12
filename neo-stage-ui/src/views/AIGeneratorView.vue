<template>
  <!-- ثبتنا الـ dir="ltr" ديما باش السيدبار يبقى على اليسار -->
  <div class="d-flex admin-layout" dir="ltr">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css">
    
    <!-- السيدبار يبقى في بلاصتو على اليسار -->
    <AppSidebar />

    <div class="content-right flex-grow-1">
      <AppNavbar />

      <!-- نزيدو Class "rtl-content" فقط إذا كانت اللغة عربية باش نعدلو النصوص -->
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

              <div class="glass-card p-4">
                <div class="section-title-pro mb-4">
                  <div class="icon-pill bg-amber"><i class="fa-solid fa-sliders"></i></div>
                  <div>
                    <h6 class="card-section-title">{{ t('iaGenerator.aiParams') }}</h6>
                    <p class="card-section-sub">{{ t('iaGenerator.aiParamsSub') }}</p>
                  </div>
                </div>

                <div class="setting-block mb-4">
                  <label class="setting-label mb-2">{{ t('iaGenerator.questionCount') }} ({{ settings.count }})</label>
                  <input type="range" v-model="settings.count" min="5" max="20" class="custom-range">
                </div>

                <div class="setting-block mb-4">
                  <label class="setting-label mb-2">{{ t('iaGenerator.expertise') }}</label>
                  <input v-model="settings.theme" class="form-input" :placeholder="t('iaGenerator.expertisePlaceholder')">
                </div>

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

                <button class="btn-generate w-100" @click="startGeneration" :disabled="isGenerating">
                  <span v-if="!isGenerating">{{ t('iaGenerator.generateBtn') }}</span>
                  <span v-else><i class="fa-solid fa-spinner fa-spin me-2"></i>{{ t('iaGenerator.generating') }}</span>
                </button>
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
                      <div v-for="(opt, oIdx) in q.options" :key="oIdx" class="opt-item" :class="{ 'opt-correct': q.answer === oIdx }">
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
import { ref, reactive } from 'vue';
import { useI18n } from 'vue-i18n';
import axios from 'axios';
import Swal from 'sweetalert2';
import api from '@/services/api';

const { t, locale } = useI18n();

const isGenerating = ref(false);
const generatedQuestions = ref([]);
const fileRef = ref(null);
const files = ref([]);

const settings = reactive({
  count: 10,
  theme: '',
  difficulty: 'Medium',
  langue: 'fr',
});

const languages = [
  { code: 'fr', label: 'Français' },
  { code: 'en', label: 'English' },
  { code: 'ar', label: 'العربية' },
];

const triggerUpload = () => fileRef.value.click();
const handleFile = (e) => {
  const file = e.target.files[0];
  if (file) files.value = [{ name: file.name, raw: file }];
};

const startGeneration = async () => {
  if (!settings.theme) {
    Swal.fire({ icon: 'warning', title: t('iaGenerator.warningTitle'), text: t('iaGenerator.warningText') });
    return;
  }
  isGenerating.value = true;
  try {
    const fd = new FormData();
    if (files.value.length) fd.append('file', files.value[0].raw);
    fd.append('nombre', settings.count);
    fd.append('themetique', settings.theme);
    fd.append('difficulte', settings.difficulty);
    fd.append('langue', settings.langue);

    const res = await axios.post('http://127.0.0.1:8000/ia/generate-pro', fd);
    if (res.data.status === 'IA_SUCCESS') generatedQuestions.value = res.data.questions;
  } catch (err) {
    Swal.fire({ icon: 'error', title: 'Error', text: 'Generation failed' });
  } finally { isGenerating.value = false; }
};

const saveAllQuestions = async () => {
  try {
    for (const q of generatedQuestions.value) {
      await api.post('/Questions', {
        enonce: q.question,
        type: 0,
        points: 2,
        theme: settings.theme.toUpperCase(),
        choix: q.options,
        bonneReponse: q.options[q.answer],
      });
    }
    Swal.fire({ icon: 'success', title: t('iaGenerator.saveSuccess') });
    generatedQuestions.value = [];
  } catch (err) {
    Swal.fire({ icon: 'error', title: 'Error', text: 'Save failed' });
  }
};
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:ital,wght@0,400;0,500;0,600;0,700;0,800;1,700&display=swap');
 
/* ════════ BASE ════════ */
.admin-layout {
  min-height: 100vh;
  background-color: #f8fafc;
  font-family: 'Plus Jakarta Sans', sans-serif;
}
 
/* ════════ BREADCRUMB ════════ */
.breadcrumb-bar {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 12px;
  font-weight: 600;
  color: #94a3b8;
}
.bc-icon { font-size: 10px; }
.bc-sep { font-size: 8px; color: #cbd5e1; }
.bc-active { color: #eab308; }
.bc-item { cursor: default; }
 
/* ════════ PAGE HEADER ════════ */
.badge-ai-powered {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  background: linear-gradient(90deg, #0f172a, #0891b2);
  color: white;
  padding: 5px 14px;
  border-radius: 100px;
  font-size: 10px;
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 1px;
}
.ai-badge-dot {
  width: 6px; height: 6px;
  background: #4ade80;
  border-radius: 50%;
  animation: blink 1.5s ease-in-out infinite;
}
@keyframes blink { 0%,100%{opacity:1} 50%{opacity:0.3} }
 
.page-title {
  font-size: 32px;
  font-weight: 800;
  color: #0f172a;
  letter-spacing: -1px;
  margin: 6px 0 4px;
}
.page-subtitle {
  font-size: 14px;
  color: #64748b;
  margin: 0;
}
 
/* Usage pill */
.usage-pill {
  display: flex;
  align-items: center;
  gap: 14px;
  background: white;
  border: 1.5px solid #f1f5f9;
  border-radius: 20px;
  padding: 16px 22px;
  box-shadow: 0 4px 20px rgba(0,0,0,0.04);
}
.usage-ring {
  position: relative;
  width: 48px;
  height: 48px;
  flex-shrink: 0;
}
.ring-svg {
  width: 48px;
  height: 48px;
  transform: rotate(-90deg);
}
.ring-val {
  position: absolute;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 14px;
  font-weight: 800;
  color: #0f172a;
}
.usage-label { font-size: 13px; font-weight: 700; color: #0f172a; margin: 0 0 2px; }
.usage-sub { font-size: 11px; color: #94a3b8; margin: 0; font-weight: 600; }
 
/* ════════ GLASS CARDS ════════ */
.glass-card {
  background: white;
  border-radius: 24px;
  border: 1px solid #f1f5f9;
  box-shadow: 0 8px 32px rgba(0,0,0,0.04);
  position: relative;
  overflow: hidden;
  transition: box-shadow 0.3s;
}
.glass-card:hover {
  box-shadow: 0 12px 40px rgba(0,0,0,0.06);
}
 
/* Section title inside cards */
.section-title-pro {
  display: flex;
  align-items: flex-start;
  gap: 14px;
}
.icon-pill {
  width: 38px;
  height: 38px;
  min-width: 38px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 15px;
  color: white;
  flex-shrink: 0;
}
.bg-navy { background-color: #0f172a; }
.bg-amber { background-color: #eab308; color: #0f172a !important; }
.bg-indigo { background-color: #6366f1; }
 
.card-section-title {
  font-size: 15px;
  font-weight: 800;
  color: #0f172a;
  margin: 0 0 2px;
  letter-spacing: -0.3px;
}
.card-section-sub {
  font-size: 12px;
  color: #94a3b8;
  margin: 0;
  font-weight: 600;
}
 
/* ════════ UPLOAD ZONE ════════ */
.upload-zone {
  border: 2px dashed #cbd5e1;
  border-radius: 20px;
  cursor: pointer;
  transition: 0.3s;
  background: #fcfcfd;
  min-height: 120px;
  display: flex;
  align-items: center;
  justify-content: center;
}
.upload-zone:hover,
.upload-zone-drag {
  border-color: #eab308;
  background: #fffbeb;
}
.upload-zone-active {
  border-color: #22c55e;
  background: #f0fdf4;
  border-style: solid;
}
.upload-zone-inner {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
  padding: 28px 20px;
  text-align: center;
  width: 100%;
}
.upload-cloud-wrap {
  width: 52px;
  height: 52px;
  background: #f1f5f9;
  border-radius: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: 0.3s;
}
.upload-zone:hover .upload-cloud-wrap {
  background: #fef9c3;
}
.cloud-uploaded {
  background: #dcfce7 !important;
}
.upload-cloud {
  font-size: 22px;
  color: #94a3b8;
  transition: 0.3s;
}
.upload-zone:hover .upload-cloud { color: #eab308; }
.cloud-uploaded .upload-cloud { color: #22c55e !important; }
 
.upload-title { font-size: 13px; font-weight: 800; color: #0f172a; margin: 0; }
.upload-hint { font-size: 11px; color: #94a3b8; margin: 0; font-weight: 600; }
.upload-success-msg {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 13px;
  font-weight: 700;
  color: #16a34a;
}
.success-dot {
  width: 8px; height: 8px;
  background: #22c55e;
  border-radius: 50%;
  animation: blink 1s infinite;
}
 
/* File pill */
.file-pill {
  display: flex;
  align-items: center;
  gap: 12px;
  background: #f8fafc;
  border: 1px solid #f1f5f9;
  padding: 12px 16px;
  border-radius: 14px;
  transition: 0.2s;
}
.file-pill:hover { border-color: #e2e8f0; }
.file-pill-icon {
  width: 36px;
  height: 36px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 16px;
  flex-shrink: 0;
}
.icon-pdf { background: #fef2f2; color: #ef4444; }
.icon-docx { background: #eff6ff; color: #3b82f6; }
.file-pill-info { flex: 1; min-width: 0; }
.file-name { display: block; font-size: 13px; font-weight: 700; color: #0f172a; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
.file-size { font-size: 11px; color: #94a3b8; font-weight: 600; }
.btn-remove {
  width: 28px;
  height: 28px;
  border: none;
  background: #f1f5f9;
  border-radius: 8px;
  color: #94a3b8;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
  transition: 0.2s;
  flex-shrink: 0;
}
.btn-remove:hover { background: #fef2f2; color: #ef4444; }
 
/* ════════ SETTINGS ════════ */
.setting-label {
  font-size: 12px;
  font-weight: 700;
  color: #475569;
  text-transform: uppercase;
  letter-spacing: 0.8px;
  display: block;
}
.text-amber-soft { color: #d97706; }
 
/* Range */
.range-wrap { position: relative; }
.custom-range {
  -webkit-appearance: none;
  width: 100%;
  height: 6px;
  background: #f1f5f9;
  border-radius: 10px;
  outline: none;
  cursor: pointer;
  position: relative;
  z-index: 2;
}
.custom-range::-webkit-slider-thumb {
  -webkit-appearance: none;
  width: 20px; height: 20px;
  background: #eab308;
  border-radius: 50%;
  cursor: pointer;
  border: 3px solid white;
  box-shadow: 0 2px 8px rgba(234,179,8,0.4);
  transition: 0.2s;
}
.custom-range::-webkit-slider-thumb:hover {
  transform: scale(1.2);
  box-shadow: 0 4px 14px rgba(234,179,8,0.5);
}
.count-badge {
  background: #0f172a;
  color: #eab308;
  width: 36px;
  height: 36px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}
.count-val { font-size: 16px; font-weight: 800; }
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
 
/* Input */
.input-wrapper { position: relative; }
.form-input {
  width: 100%;
  padding: 12px 16px;
  border: 1.5px solid #e2e8f0;
  border-radius: 14px;
  font-size: 14px;
  font-family: inherit;
  font-weight: 600;
  color: #0f172a;
  outline: none;
  transition: 0.2s;
  background: white;
}
.form-input:focus { border-color: #eab308; box-shadow: 0 0 0 3px rgba(234,179,8,0.1); }
.form-input::placeholder { color: #cbd5e1; font-weight: 500; }
.input-suffix {
  position: absolute;
  right: 14px;
  top: 50%;
  transform: translateY(-50%);
  font-size: 14px;
}
 
/* Lang pills */
.lang-pills {
  display: flex;
  gap: 8px;
}
.lang-pill {
  flex: 1;
  border: 1.5px solid #e2e8f0;
  background: #f8fafc;
  padding: 10px 8px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 700;
  color: #64748b;
  cursor: pointer;
  transition: 0.2s;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 6px;
  font-family: inherit;
}
.lang-pill:hover { border-color: #fde68a; color: #0f172a; background: #fefce8; }
.lang-pill.active { border-color: #eab308; background: #fffbeb; color: #92400e; font-weight: 800; }
.lang-flag { font-size: 16px; }
 
/* Difficulty grid */
.difficulty-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 10px;
}
.diff-card {
  border: 1.5px solid #f1f5f9;
  background: white;
  border-radius: 16px;
  padding: 14px 8px;
  text-align: center;
  cursor: pointer;
  transition: 0.25s;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 6px;
  font-family: inherit;
}
.diff-card:hover { border-color: #fde68a; background: #fffbeb; transform: translateY(-2px); }
.diff-icon { font-size: 20px; color: #94a3b8; transition: 0.2s; }
.diff-label { font-size: 11px; font-weight: 800; color: #64748b; text-transform: uppercase; letter-spacing: 0.5px; }
.diff-pts { font-size: 10px; font-weight: 700; color: #94a3b8; }
 
.diff-card.diff-easy.active  { border-color: #22c55e; background: #f0fdf4; }
.diff-card.diff-easy.active .diff-icon  { color: #16a34a; }
.diff-card.diff-easy.active .diff-label { color: #16a34a; }
 
.diff-card.diff-medium.active { border-color: #f59e0b; background: #fffbeb; }
.diff-card.diff-medium.active .diff-icon  { color: #d97706; }
.diff-card.diff-medium.active .diff-label { color: #d97706; }
 
.diff-card.diff-hard.active  { border-color: #ef4444; background: #fef2f2; }
.diff-card.diff-hard.active .diff-icon  { color: #dc2626; }
.diff-card.diff-hard.active .diff-label { color: #dc2626; }
 
/* ════════ GENERATE BUTTON ════════ */
.btn-generate {
  position: relative;
  background: #eab308;
  color: #0f172a;
  border: none;
  padding: 16px 24px;
  border-radius: 18px;
  font-weight: 800;
  font-size: 15px;
  font-family: inherit;
  cursor: pointer;
  transition: 0.3s;
  overflow: hidden;
  letter-spacing: 0.5px;
}
.btn-generate:hover:not(:disabled) {
  background: #ca8a04;
  transform: translateY(-2px);
  box-shadow: 0 14px 30px rgba(234,179,8,0.4);
}
.btn-generate:disabled { opacity: 0.7; cursor: not-allowed; }
.btn-gen-inner { position: relative; z-index: 2; }
.btn-gen-glow {
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, rgba(255,255,255,0.25) 0%, transparent 60%);
  pointer-events: none;
}
.gen-spinner {
  width: 16px; height: 16px;
  border: 2.5px solid rgba(15,23,42,0.3);
  border-top-color: #0f172a;
  border-radius: 50%;
  display: inline-block;
  animation: spin 0.7s linear infinite;
}
@keyframes spin { to { transform: rotate(360deg); } }
 
/* Generation progress */
.gen-progress { text-align: center; }
.gen-progress-bar {
  height: 4px;
  background: #f1f5f9;
  border-radius: 10px;
  overflow: hidden;
  margin-bottom: 8px;
}
.gen-progress-fill {
  height: 100%;
  background: linear-gradient(90deg, #eab308, #f59e0b, #eab308);
  background-size: 200% 100%;
  border-radius: 10px;
  animation: progress-shimmer 1.5s ease-in-out infinite;
  width: 60%;
}
@keyframes progress-shimmer {
  0%   { background-position: 200% 0; }
  100% { background-position: -200% 0; }
}
.gen-status-text {
  font-size: 12px;
  color: #94a3b8;
  font-weight: 600;
  margin: 0;
  animation: fade-pulse 2s ease-in-out infinite;
}
@keyframes fade-pulse { 0%,100%{opacity:0.6} 50%{opacity:1} }
 
/* ════════ RIGHT PANEL ════════ */
.btn-export {
  background: #f8fafc;
  border: 1.5px solid #e2e8f0;
  color: #64748b;
  padding: 8px 16px;
  border-radius: 10px;
  font-size: 12px;
  font-weight: 700;
  cursor: pointer;
  transition: 0.2s;
  font-family: inherit;
}
.btn-export:hover { border-color: #eab308; color: #0f172a; background: #fffbeb; }
.btn-save {
  background: #0f172a;
  color: white;
  border: none;
  padding: 8px 20px;
  border-radius: 10px;
  font-size: 12px;
  font-weight: 700;
  cursor: pointer;
  transition: 0.2s;
  font-family: inherit;
}
.btn-save:hover:not(:disabled) { background: #1e293b; transform: translateY(-1px); }
.btn-save:disabled { opacity: 0.6; cursor: not-allowed; }
.save-spinner {
  width: 12px; height: 12px;
  border: 2px solid rgba(255,255,255,0.3);
  border-top-color: white;
  border-radius: 50%;
  display: inline-block;
  animation: spin 0.7s linear infinite;
}
 
/* ════════ EMPTY STATE ════════ */
.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 60px 30px;
  text-align: center;
  position: relative;
}
.empty-orb {
  position: absolute;
  width: 200px; height: 200px;
  background: radial-gradient(circle, rgba(234,179,8,0.06) 0%, transparent 70%);
  border-radius: 50%;
  pointer-events: none;
}
.empty-robot {
  width: 70px; height: 70px;
  background: linear-gradient(135deg, #f8fafc, #f1f5f9);
  border-radius: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 28px;
  color: #cbd5e1;
  margin-bottom: 18px;
  border: 1.5px solid #f1f5f9;
}
.empty-title {
  font-size: 17px;
  font-weight: 800;
  color: #0f172a;
  margin: 0 0 8px;
}
.empty-sub {
  font-size: 13px;
  color: #94a3b8;
  font-weight: 600;
  margin: 0 0 24px;
}
.empty-steps {
  display: flex;
  flex-direction: column;
  gap: 10px;
  width: 100%;
  max-width: 280px;
}
.empty-step {
  display: flex;
  align-items: center;
  gap: 12px;
  background: #f8fafc;
  border-radius: 12px;
  padding: 10px 14px;
  text-align: left;
  font-size: 12px;
  font-weight: 600;
  color: #64748b;
}
.step-num {
  width: 22px; height: 22px;
  background: #eab308;
  color: #0f172a;
  border-radius: 7px;
  font-size: 11px;
  font-weight: 800;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}
 
/* ════════ SKELETON ════════ */
.skeleton-list { display: flex; flex-direction: column; gap: 16px; padding: 4px 0; }
.skeleton-card {
  background: #f8fafc;
  border-radius: 18px;
  padding: 20px;
  border: 1px solid #f1f5f9;
}
.skeleton-line {
  height: 10px;
  background: linear-gradient(90deg, #f1f5f9 25%, #e2e8f0 50%, #f1f5f9 75%);
  background-size: 200% 100%;
  border-radius: 8px;
  animation: skeleton-shimmer 1.4s ease-in-out infinite;
}
.skeleton-opt {
  height: 44px;
  background: linear-gradient(90deg, #f1f5f9 25%, #e2e8f0 50%, #f1f5f9 75%);
  background-size: 200% 100%;
  border-radius: 12px;
  margin-bottom: 8px;
  animation: skeleton-shimmer 1.4s ease-in-out infinite;
}
@keyframes skeleton-shimmer {
  0%   { background-position: 200% 0; }
  100% { background-position: -200% 0; }
}
.w-20 { width: 20%; }
.w-80 { width: 80%; }
 
/* ════════ QUESTION CARDS ════════ */
.questions-list {
  max-height: 60vh;
  overflow-y: auto;
  padding-right: 4px;
}
.custom-scrollbar::-webkit-scrollbar { width: 4px; }
.custom-scrollbar::-webkit-scrollbar-track { background: transparent; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: #e2e8f0; border-radius: 10px; }
.custom-scrollbar::-webkit-scrollbar-thumb:hover { background: #cbd5e1; }
 
.q-card {
  background: white;
  border: 1.5px solid #f1f5f9;
  border-radius: 20px;
  padding: 20px;
  margin-bottom: 12px;
  transition: 0.25s;
  cursor: default;
}
.q-card:hover { border-color: #fde68a; transform: translateY(-2px); box-shadow: 0 8px 24px rgba(0,0,0,0.05); }
.q-card-selected { border-color: #eab308 !important; background: #fffbeb; }
 
.q-card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
  flex-wrap: wrap;
  gap: 6px;
}
.q-num {
  font-size: 11px;
  font-weight: 800;
  color: #94a3b8;
  background: #f1f5f9;
  padding: 3px 8px;
  border-radius: 6px;
  font-family: 'Courier New', monospace;
}
.q-badge-ia {
  font-size: 9px;
  font-weight: 800;
  background: linear-gradient(90deg, #0f172a, #1e40af);
  color: white;
  padding: 3px 8px;
  border-radius: 6px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}
.q-diff-badge {
  font-size: 9px;
  font-weight: 800;
  padding: 3px 8px;
  border-radius: 6px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}
.q-diff-badge.diff-easy   { background: #dcfce7; color: #16a34a; }
.q-diff-badge.diff-medium { background: #fef9c3; color: #a16207; }
.q-diff-badge.diff-hard   { background: #fef2f2; color: #dc2626; }
 
.btn-select-q, .btn-remove-q {
  width: 28px; height: 28px;
  border: none;
  background: #f8fafc;
  border-radius: 8px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
  color: #94a3b8;
  transition: 0.2s;
  border: 1px solid #f1f5f9;
}
.btn-select-q:hover { background: #fffbeb; color: #eab308; border-color: #fde68a; }
.btn-select-q.selected { background: #fffbeb; color: #eab308; border-color: #eab308; }
.btn-remove-q:hover { background: #fef2f2; color: #ef4444; border-color: #fecaca; }
 
.q-text {
  font-size: 14px;
  font-weight: 700;
  color: #0f172a;
  margin: 0 0 14px;
  line-height: 1.6;
}
 
.opt-item {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 10px 14px;
  border-radius: 12px;
  background: #f8fafc;
  margin-bottom: 6px;
  font-size: 13px;
  color: #475569;
  font-weight: 600;
  border: 1px solid transparent;
  transition: 0.2s;
}
.opt-correct {
  background: #f0fdf4;
  border-color: #86efac;
  color: #166534;
  font-weight: 700;
}
.opt-letter {
  width: 24px; height: 24px;
  min-width: 24px;
  border-radius: 8px;
  border: 1.5px solid #cbd5e1;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 10px;
  font-weight: 800;
  color: #94a3b8;
  transition: 0.2s;
}
.letter-correct {
  background: #22c55e;
  border-color: #22c55e;
  color: white !important;
}
.opt-text { flex: 1; }
.opt-check { color: #22c55e; font-size: 14px; margin-left: auto; }
 
.q-card-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 14px;
  padding-top: 12px;
  border-top: 1px solid #f1f5f9;
}
.q-pts {
  font-size: 11px;
  font-weight: 800;
  color: #eab308;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}
.q-theme {
  font-size: 10px;
  font-weight: 700;
  color: #94a3b8;
  background: #f1f5f9;
  padding: 3px 10px;
  border-radius: 6px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}
 
/* ════════ SELECTION BAR ════════ */
.selection-bar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: #0f172a;
  border-radius: 14px;
  padding: 12px 18px;
  margin-top: 16px;
}
.sel-count {
  font-size: 13px;
  font-weight: 800;
  color: #eab308;
}
.btn-sel-clear {
  background: none;
  border: none;
  color: #94a3b8;
  font-size: 12px;
  font-weight: 700;
  cursor: pointer;
  font-family: inherit;
  transition: 0.2s;
}
.btn-sel-clear:hover { color: white; }
.btn-sel-save {
  background: #eab308;
  border: none;
  color: #0f172a;
  padding: 8px 18px;
  border-radius: 10px;
  font-size: 12px;
  font-weight: 800;
  cursor: pointer;
  font-family: inherit;
  transition: 0.2s;
}
.btn-sel-save:hover { background: #ca8a04; }
 
/* ════════ TRANSITIONS ════════ */
.slide-up-enter-active { transition: all 0.3s ease; }
.slide-up-leave-active { transition: all 0.2s ease; }
.slide-up-enter-from { opacity: 0; transform: translateY(10px); }
.slide-up-leave-to   { opacity: 0; transform: translateY(-6px); }
 
.q-fade-enter-active { transition: all 0.4s ease; }
.q-fade-leave-active { transition: all 0.25s ease; }
.q-fade-enter-from { opacity: 0; transform: translateY(12px) scale(0.98); }
.q-fade-leave-to   { opacity: 0; transform: translateY(-8px) scale(0.98); }
 
.animate-fade-in {
  animation: fadeIn 0.5s ease-out;
}
@keyframes fadeIn {
  from { opacity: 0; transform: translateY(12px); }
  to   { opacity: 1; transform: translateY(0); }
}
/* 1. التخطيط الأساسي ديما LTR */
.admin-layout {
  min-height: 100vh;
  background-color: #f8fafc;
  display: flex !important;
  flex-direction: row !important; /* ديما السيدبار يسار */
}

/* 2. التحكم في النصوص عند اختيار اللغة العربية */
.rtl-content {
  direction: rtl; /* يقلب اتجاه النص فقط */
  text-align: right; /* يحاذي النص لليمين */
}

/* 3. استثناءات لضمان عدم انقلاب التصميم (يبقى الأيقونات في مكانها الأصلي) */
.rtl-content .section-title-pro {
  display: flex;
  flex-direction: row; /* نحافظ على الأيقونة على يسار النص حتى في العربي */
  text-align: right;
}

.rtl-content .icon-pill {
  margin-left: 15px; /* مسافة من اليمين بدل اليسار */
  margin-right: 0;
}

/* باقي الستايليست */
.glass-card { background: white; border-radius: 24px; border: 1px solid #f1f5f9; box-shadow: 0 4px 20px rgba(0,0,0,0.02); }
.questions-list { max-height: 70vh; overflow-y: auto; }
.custom-scrollbar::-webkit-scrollbar { width: 5px; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: #e2e8f0; border-radius: 10px; }

.upload-zone { border: 2px dashed #cbd5e1; border-radius: 15px; padding: 20px; text-align: center; cursor: pointer; }
.form-input { width: 100%; padding: 12px; border: 1.5px solid #e2e8f0; border-radius: 10px; }
.lang-pills { display: flex; gap: 10px; }
.lang-pill { flex: 1; padding: 10px; border: 1px solid #e2e8f0; border-radius: 10px; background: white; cursor: pointer; }
.lang-pill.active { background: #0f172a; color: white; }
.btn-generate { background: #eab308; border: none; padding: 15px; border-radius: 12px; font-weight: 700; }
.btn-save { background: #0f172a; color: white; border: none; padding: 8px 20px; border-radius: 8px; }

.q-card { background: #f8fafc; border-radius: 15px; padding: 20px; border: 1px solid #f1f5f9; }
.opt-item { padding: 10px; background: white; border-radius: 10px; margin-bottom: 5px; border: 1px solid #f1f5f9; }
.opt-correct { border-color: #22c55e; background: #f0fdf4; }
</style>