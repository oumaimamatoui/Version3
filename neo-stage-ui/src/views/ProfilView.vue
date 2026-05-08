<template>
  <div class="profile-root" @mousemove="handleParallax">

    <!-- BACKGROUND -->
    <div class="profile-bg">
      <div class="glow-orb orb-1" :style="orbStyle(0.04)"></div>
      <div class="glow-orb orb-2" :style="orbStyle(0.02)"></div>
      <div class="dot-grid"></div>
    </div>

    <AppSidebar />

    <div class="main-viewport flex-grow-1 d-flex flex-column position-relative">
      <AppNavbar />

      <main class="profile-canvas flex-grow-1 overflow-auto custom-scrollbar">
        <div class="content-area p-4 p-lg-5">

          <!-- LOADING -->
          <div v-if="loading" class="loading-state">
            <div class="spinner-orb"></div>
            <p class="mt-3 fw-800 text-uppercase small text-muted tracking-wide">Chargement du profil...</p>
          </div>

          <div v-else class="animate__animated animate__fadeIn">

            <!-- PAGE HEADER -->
            <header class="page-header mb-5">
              <div>
                <div class="breadcrumb-trail mb-2">
                  <span>Tableau de bord</span>
                  <i class="fa-solid fa-chevron-right sep"></i>
                  <span class="current">Mon Profil</span>
                </div>
                <h2 class="page-title">Mon <span class="highlight">Profil</span></h2>
                <p class="page-sub">Gérez vos informations personnelles et votre présence</p>
              </div>
              <div class="header-actions">
                <button class="btn-outline-action" @click="goToSettings">
                  <i class="fa-solid fa-gear me-2"></i>Paramètres
                </button>
                <button class="btn-primary-action">
                  <i class="fa-solid fa-share-nodes me-2"></i>Partager
                </button>
              </div>
            </header>

            <div class="profile-grid">

              <!-- LEFT COLUMN -->
              <div class="profile-left">

                <!-- IDENTITY CARD -->
                <div class="identity-card">
                  <div class="identity-card-bg">
                    <div class="id-card-shine"></div>
                  </div>

                  <div class="identity-card-body">
                    <div class="avatar-wrap" @click="triggerFileInput">
                      <img :src="profileDisplayUrl" class="avatar-img" :alt="t('profile.myProfile')">
                      <div class="avatar-overlay">
                        <i class="fa-solid fa-camera"></i>
                        <span>{{ t('profile.changePhoto') }}</span>
                      </div>
                      <div class="avatar-status"></div>
                      <input type="file" ref="fileInput" @change="onFileChange" hidden accept="image/*">
                    </div>

                    <h4 class="identity-name">{{ user.prenom }} {{ user.nom }}</h4>
                    <div class="identity-role">
                      <i class="fa-solid fa-circle-check me-1"></i>{{ roleDisplay }}
                    </div>

                    <div class="identity-divider"></div>

                    <div class="identity-stats">
                      <div class="id-stat">
                        <div class="id-stat-icon" style="background:#fffbeb; color:#f59e0b;">
                          <i class="fa-regular fa-calendar-check"></i>
                        </div>
                        <div>
                          <div class="id-stat-label">Membre depuis</div>
                          <div class="id-stat-value">{{ user.joinDate || '—' }}</div>
                        </div>
                      </div>
                      <div class="id-stat" v-if="user.entrepriseNom && authStore.role !== 'Candidat'">
                        <div class="id-stat-icon" style="background:#eef2ff; color:#6366f1;">
                          <i class="fa-solid fa-building"></i>
                        </div>
                        <div>
                          <div class="id-stat-label">Organisation</div>
                          <div class="id-stat-value">{{ user.entrepriseNom }}</div>
                        </div>
                      </div>
                    </div>

                    <div class="identity-actions">
                      <button class="id-btn-edit" @click="goToSettings">
                        <i class="fa-solid fa-pen me-2"></i>{{ t('profile.edit') }}
                        <div class="btn-shine"></div>
                      </button>
                      <button class="id-btn-share">
                        <i class="fa-solid fa-share-nodes"></i>
                      </button>
                    </div>
                  </div>
                </div>

                <!-- UPLOAD PROGRESS (when uploading) -->
                <div v-if="uploading" class="upload-progress-card mt-3">
                  <div class="up-icon"><i class="fa-solid fa-cloud-arrow-up fa-beat-fade"></i></div>
                  <div class="flex-grow-1">
                    <div class="up-label">Mise à jour de la photo...</div>
                    <div class="up-bar">
                      <div class="up-fill"></div>
                    </div>
                  </div>
                </div>

              </div>

              <!-- RIGHT COLUMN -->
              <div class="profile-right">

                <!-- INFO CARD -->
                <div class="info-card">
                  <div class="info-card-header">
                    <div class="info-header-icon">
                      <i class="fa-solid fa-address-card"></i>
                    </div>
                    <div>
                      <h6 class="fw-900 m-0">{{ t('profile.generalInfo') }}</h6>
                      <p class="text-muted small m-0">Informations de votre compte</p>
                    </div>
                    <div class="ms-auto">
                      <span class="live-badge">
                        <span class="live-dot"></span>En ligne
                      </span>
                    </div>
                  </div>

                  <div class="info-fields-grid">

                    <div class="info-field-item">
                      <div class="field-icon-box" style="background:#eef2ff; color:#6366f1;">
                        <i class="fa-solid fa-envelope"></i>
                      </div>
                      <div class="field-content">
                        <div class="field-label">{{ t('profile.email') }}</div>
                        <div class="field-value">{{ user.email || '—' }}</div>
                      </div>
                      <div class="field-verified-badge" v-if="user.email">
                        <i class="fa-solid fa-circle-check"></i>
                      </div>
                    </div>

                    <div class="info-field-item">
                      <div class="field-icon-box" style="background:#ecfdf5; color:#10b981;">
                        <i class="fa-regular fa-calendar-check"></i>
                      </div>
                      <div class="field-content">
                        <div class="field-label">{{ t('profile.joinedSince') }}</div>
                        <div class="field-value">{{ user.joinDate || '—' }}</div>
                      </div>
                    </div>

                    <div class="info-field-item">
                      <div class="field-icon-box" style="background:#fffbeb; color:#f59e0b;">
                        <i class="fa-solid fa-user-shield"></i>
                      </div>
                      <div class="field-content">
                        <div class="field-label">Rôle</div>
                        <div class="field-value">{{ roleDisplay }}</div>
                      </div>
                    </div>

                    <div class="info-field-item" v-if="user.entrepriseNom && authStore.role !== 'Candidat'">
                      <div class="field-icon-box" style="background:#f0f9ff; color:#0ea5e9;">
                        <i class="fa-solid fa-building"></i>
                      </div>
                      <div class="field-content">
                        <div class="field-label">{{ t('profile.organization') }}</div>
                        <div class="field-value">{{ user.entrepriseNom }}</div>
                      </div>
                    </div>

                  </div>
                </div>

                <!-- BIO CARD -->
                <div class="bio-card" v-if="user.bio">
                  <div class="bio-card-header">
                    <div class="bio-header-icon">
                      <i class="fa-solid fa-quote-left"></i>
                    </div>
                    <div>
                      <h6 class="fw-900 m-0">{{ t('profile.bio') }}</h6>
                      <p class="text-muted small m-0">À propos de vous</p>
                    </div>
                  </div>
                  <div class="bio-text">{{ user.bio }}</div>
                </div>

                <!-- QUICK ACTIONS -->
                <div class="quick-actions-card">
                  <div class="qa-header">
                    <i class="fa-solid fa-bolt-lightning me-2 text-amber"></i>
                    <span class="fw-900">Actions rapides</span>
                  </div>
                  <div class="qa-grid">
                    <button class="qa-item" @click="goToSettings">
                      <div class="qa-icon" style="background:#fffbeb; color:#f59e0b;">
                        <i class="fa-solid fa-user-pen"></i>
                      </div>
                      <span>Modifier le profil</span>
                      <i class="fa-solid fa-chevron-right qa-arrow"></i>
                    </button>
                    <button class="qa-item" @click="goToSettings">
                      <div class="qa-icon" style="background:#f0f9ff; color:#0ea5e9;">
                        <i class="fa-solid fa-lock"></i>
                      </div>
                      <span>Changer le mot de passe</span>
                      <i class="fa-solid fa-chevron-right qa-arrow"></i>
                    </button>
                    <button class="qa-item" @click="triggerFileInput">
                      <div class="qa-icon" style="background:#ecfdf5; color:#10b981;">
                        <i class="fa-solid fa-camera"></i>
                      </div>
                      <span>Changer la photo</span>
                      <i class="fa-solid fa-chevron-right qa-arrow"></i>
                    </button>
                  </div>
                </div>

              </div>
            </div>

          </div>
        </div><!-- /content-area -->
      </main>
    </div><!-- /main-viewport -->
  </div>
</template>

<script setup>
import { ref, computed, onMounted, reactive } from 'vue';
import { useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import api from '@/services/api';
import { useAuthStore } from '@/stores/auth';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';

const { t } = useI18n();
const router = useRouter();
const authStore = useAuthStore();
const fileInput = ref(null);

const user = ref({ nom: '', prenom: '', email: '', photoUrl: '', joinDate: '', entrepriseNom: '', bio: '' });
const loading = ref(true);
const uploading = ref(false);

const mousePos = reactive({ x: 0, y: 0 });

const profileDisplayUrl = computed(() => {
  if (user.value.photoUrl) return `http://localhost:5172${user.value.photoUrl}`;
  return `https://ui-avatars.com/api/?name=${user.value.prenom}+${user.value.nom}&background=0f172a&color=eab308&size=256&bold=true`;
});

const roleDisplay = computed(() => {
  if (!authStore.role) return '...';
  return t(`roles.${authStore.role}`);
});

const fetchProfile = async () => {
  try {
    const res = await api.get('/Settings/me');
    user.value = res.data;
  } catch (error) {
    console.error('Erreur profil:', error);
  } finally {
    loading.value = false;
  }
};

onMounted(fetchProfile);

const triggerFileInput = () => fileInput.value.click();

const onFileChange = async (e) => {
  const file = e.target.files[0];
  if (!file) return;
  const formData = new FormData();
  formData.append('file', file);
  try {
    uploading.value = true;
    const res = await api.post('/Settings/upload-photo', formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    });
    user.value.photoUrl = res.data.photoUrl;
    authStore.user.photoUrl = res.data.photoUrl;
    localStorage.setItem('user', JSON.stringify(authStore.user));
  } catch (error) {
    console.error('Erreur upload :', error);
    alert(t('profile.uploadError'));
  } finally {
    uploading.value = false;
  }
};

const goToSettings = () => router.push('/settings');

const orbStyle = (f) => ({
  transform: `translate(${mousePos.x * f * 10}px, ${mousePos.y * f * 10}px)`
});
const handleParallax = (e) => {
  mousePos.x = (e.clientX - window.innerWidth / 2) / 20;
  mousePos.y = (e.clientY - window.innerHeight / 2) / 20;
};
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;600;700;800;900&family=JetBrains+Mono:wght@600;700&display=swap');
@import url('https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.2/css/all.min.css');

/* ═══════════════════════════════════════════
   ROOT  (identique au Dashboard)
═══════════════════════════════════════════ */
.profile-root {
  font-family: 'Plus Jakarta Sans', sans-serif;
  min-height: 100vh;
  background: var(--bg-page, #f8fafc);
  color: var(--text-main, #0f172a);
  position: relative;
  overflow-x: hidden;
  display: flex;
  transition: background-color 0.35s ease;
}

.main-viewport { z-index: 10; }

/* ─── SCROLL ─── */
.profile-canvas { height: calc(100vh - 64px); }
.content-area   { position: relative; z-index: 20; }

.custom-scrollbar::-webkit-scrollbar       { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: transparent; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: rgba(148,163,184,0.35); border-radius: 10px; }
.custom-scrollbar::-webkit-scrollbar-thumb:hover { background: rgba(148,163,184,0.6); }

/* BACKGROUND */
.profile-bg { position: fixed; inset: 0; z-index: 0; pointer-events: none; }
.dot-grid {
  position: absolute; inset: 0;
  background-image: radial-gradient(#cbd5e1 1px, transparent 1px);
  background-size: 36px 36px;
  opacity: 0.16;
}
[data-theme="dark"] .dot-grid { opacity: 0.22; }
.glow-orb {
  position: absolute; border-radius: 50%;
  filter: blur(130px); opacity: 0.1;
  transition: transform 0.4s ease-out;
}
[data-theme="dark"] .glow-orb { opacity: 0.16; }
.orb-1 { width: 500px; height: 500px; background: #f59e0b; top: -150px; right: 0; }
.orb-2 { width: 400px; height: 400px; background: #6366f1; bottom: -100px; left: -50px; }

/* ═══════════════════════════════════════════
   LOADING
═══════════════════════════════════════════ */
.loading-state {
  display: flex; flex-direction: column;
  align-items: center; justify-content: center;
  min-height: 60vh;
}
.spinner-orb {
  width: 46px; height: 46px;
  border: 4px solid #f1f5f9;
  border-top: 4px solid #f59e0b;
  border-radius: 50%;
  animation: spin 0.9s linear infinite;
}
@keyframes spin { to { transform: rotate(360deg); } }
.tracking-wide { letter-spacing: 2px; }

/* ═══════════════════════════════════════════
   PAGE HEADER
═══════════════════════════════════════════ */
.page-header {
  display: flex;
  align-items: flex-end;
  justify-content: space-between;
  flex-wrap: wrap;
  gap: 16px;
}
.breadcrumb-trail {
  font-size: 0.68rem; font-weight: 700; color: #94a3b8;
  display: flex; align-items: center; gap: 6px;
}
.breadcrumb-trail .sep { font-size: 0.5rem; opacity: 0.4; }
.breadcrumb-trail .current { color: #0f172a; font-weight: 800; }
.page-title {
  font-weight: 900;
  font-size: clamp(1.4rem, 2.5vw, 2.1rem);
  letter-spacing: -1.5px; margin: 0; line-height: 1.1;
  color: var(--text-main, #0f172a);
  transition: color 0.3s;
}
.page-title .highlight {
  background: linear-gradient(135deg, #f59e0b, #fbbf24);
  -webkit-background-clip: text; -webkit-text-fill-color: transparent;
  background-clip: text;
}
.page-sub { color: #94a3b8; font-size: 0.72rem; font-weight: 700; letter-spacing: 0.5px; margin: 4px 0 0; }
.header-actions { display: flex; gap: 10px; align-items: center; }
.btn-outline-action {
  background: white; color: #64748b;
  border: 1.5px solid #e2e8f0; border-radius: 14px;
  padding: 10px 20px; font-weight: 800; font-size: 0.8rem;
  cursor: pointer; font-family: inherit; transition: 0.2s;
}
.btn-outline-action:hover { border-color: #f59e0b; color: #0f172a; }
.btn-primary-action {
  background: #0f172a; color: white;
  border: none; border-radius: 14px;
  padding: 10px 22px; font-weight: 800; font-size: 0.8rem;
  cursor: pointer; font-family: inherit; transition: 0.2s;
}
.btn-primary-action:hover { background: #1e293b; transform: translateY(-1px); }

/* ═══════════════════════════════════════════
   PROFILE GRID
═══════════════════════════════════════════ */
.profile-grid {
  display: grid;
  grid-template-columns: 300px 1fr;
  gap: 20px;
  align-items: start;
}
@media (max-width: 900px) {
  .profile-grid { grid-template-columns: 1fr; }
}
.profile-right { display: flex; flex-direction: column; gap: 18px; }

/* ═══════════════════════════════════════════
   IDENTITY CARD
═══════════════════════════════════════════ */
.identity-card {
  background: white;
  border-radius: 28px;
  border: 1px solid #eef2f6;
  box-shadow: 0 4px 24px rgba(0,0,0,0.06);
  overflow: hidden;
  position: relative;
}
.identity-card-bg {
  height: 96px;
  background: linear-gradient(135deg, #0f172a 0%, #1e293b 60%, #0f172a 100%);
  position: relative;
  overflow: hidden;
}
.identity-card-bg::before {
  content: '';
  position: absolute; inset: 0;
  background: radial-gradient(circle at 60% 50%, rgba(245,158,11,0.18), transparent 55%);
}
.id-card-shine {
  position: absolute; top: -40%; left: -40%;
  width: 80%; height: 180%;
  background: linear-gradient(105deg, transparent 30%, rgba(255,255,255,0.05) 50%, transparent 70%);
  transform: rotate(15deg);
}
.identity-card-body {
  padding: 0 28px 28px;
  display: flex; flex-direction: column; align-items: center;
  text-align: center;
}

/* AVATAR */
.avatar-wrap {
  position: relative; width: 96px; height: 96px;
  cursor: pointer; border-radius: 24px; overflow: hidden;
  border: 4px solid white;
  box-shadow: 0 10px 28px rgba(0,0,0,0.14);
  margin-top: -32px; margin-bottom: 16px;
}
.avatar-img { width: 100%; height: 100%; object-fit: cover; display: block; }
.avatar-overlay {
  position: absolute; inset: 0;
  background: rgba(15,23,42,0.78);
  display: flex; flex-direction: column;
  align-items: center; justify-content: center;
  color: white; font-size: 0.65rem;
  font-weight: 800; text-transform: uppercase;
  opacity: 0; transition: 0.25s; gap: 5px;
}
.avatar-overlay i { font-size: 1.2rem; }
.avatar-wrap:hover .avatar-overlay { opacity: 1; }
.avatar-status {
  position: absolute; bottom: 4px; right: 4px;
  width: 14px; height: 14px; border-radius: 50%;
  background: #10b981; border: 2px solid white;
}

.identity-name { font-weight: 900; font-size: 1.2rem; color: #0f172a; margin: 0 0 6px; }
.identity-role {
  font-size: 0.72rem; font-weight: 800;
  background: linear-gradient(135deg, #fffbeb, #fef3c7);
  color: #d97706;
  padding: 5px 14px; border-radius: 10px;
  border: 1.5px solid #fde68a;
  display: inline-flex; align-items: center; gap: 5px;
}

.identity-divider {
  width: 100%; height: 1px;
  background: #f1f5f9; margin: 20px 0;
}

.identity-stats { width: 100%; display: flex; flex-direction: column; gap: 12px; }
.id-stat {
  display: flex; align-items: center; gap: 12px;
  text-align: left;
}
.id-stat-icon {
  width: 36px; height: 36px; border-radius: 10px;
  display: flex; align-items: center; justify-content: center;
  font-size: 0.85rem; flex-shrink: 0;
}
.id-stat-label { font-size: 0.6rem; font-weight: 700; color: #94a3b8; letter-spacing: 0.5px; text-transform: uppercase; }
.id-stat-value { font-size: 0.82rem; font-weight: 800; color: #0f172a; }

.identity-actions {
  display: flex; gap: 10px; width: 100%; margin-top: 20px;
}
.id-btn-edit {
  flex: 1; background: #0f172a; color: white;
  border: none; border-radius: 14px;
  padding: 12px 18px; font-weight: 800; font-size: 0.8rem;
  cursor: pointer; font-family: inherit; position: relative;
  overflow: hidden; transition: transform 0.2s;
  display: flex; align-items: center; justify-content: center;
}
.id-btn-edit:hover { transform: translateY(-1px); }
.btn-shine {
  position: absolute; inset: 0;
  background: linear-gradient(135deg, #f59e0b, #fbbf24);
  opacity: 0; transition: 0.3s; z-index: 1;
}
.id-btn-edit > * { position: relative; z-index: 2; }
.id-btn-edit:hover .btn-shine { opacity: 1; }
.id-btn-edit:hover { color: #0f172a; }
.id-btn-share {
  width: 44px; height: 44px; min-width: 44px;
  background: #f8fafc; border: 1.5px solid #eef2f6;
  border-radius: 14px; color: #64748b;
  cursor: pointer; font-size: 0.9rem;
  transition: 0.2s; display: flex;
  align-items: center; justify-content: center;
}
.id-btn-share:hover { border-color: #f59e0b; color: #f59e0b; background: #fffbeb; }

/* UPLOAD PROGRESS */
.upload-progress-card {
  background: #fffbeb;
  border: 1.5px solid #fde68a;
  border-radius: 16px; padding: 14px 18px;
  display: flex; align-items: center; gap: 14px;
}
.up-icon { color: #f59e0b; font-size: 1.1rem; }
.up-label { font-size: 0.72rem; font-weight: 800; color: #92400e; margin-bottom: 8px; }
.up-bar {
  height: 4px; background: #fde68a;
  border-radius: 10px; overflow: hidden;
}
.up-fill {
  height: 100%; width: 60%; background: #f59e0b;
  border-radius: 10px;
  animation: progressAnim 1.5s ease-in-out infinite alternate;
}
@keyframes progressAnim { from { width: 20%; } to { width: 90%; } }

/* ═══════════════════════════════════════════
   INFO CARD
═══════════════════════════════════════════ */
.info-card {
  background: white;
  border-radius: 24px;
  border: 1px solid #eef2f6;
  border-top: 3px solid #f59e0b;
  padding: 28px;
  box-shadow: 0 4px 20px rgba(0,0,0,0.04);
}
.info-card-header {
  display: flex; align-items: center; gap: 16px;
  padding-bottom: 22px; border-bottom: 1px solid #f1f5f9;
  margin-bottom: 22px;
}
.info-header-icon {
  width: 46px; height: 46px; background: #fffbeb; color: #f59e0b;
  border-radius: 14px; display: flex;
  align-items: center; justify-content: center;
  font-size: 1.1rem; flex-shrink: 0;
}
.live-badge {
  font-size: 0.62rem; font-weight: 800;
  background: #ecfdf5; color: #10b981;
  padding: 5px 12px; border-radius: 10px;
  display: inline-flex; align-items: center; gap: 6px;
}
.live-dot {
  width: 6px; height: 6px; border-radius: 50%;
  background: #10b981; animation: dotPulse 2s infinite;
}
@keyframes dotPulse { 0%,100%{opacity:1;transform:scale(1)} 50%{opacity:0.4;transform:scale(1.3)} }

.info-fields-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 14px;
}
@media (max-width: 600px) {
  .info-fields-grid { grid-template-columns: 1fr; }
}

.info-field-item {
  display: flex; align-items: center; gap: 14px;
  background: #f8fafc; border-radius: 16px;
  padding: 14px 16px; border: 1.5px solid #eef2f6;
  transition: 0.2s; position: relative;
}
.info-field-item:hover { border-color: #fde68a; background: #fffbeb; }
.field-icon-box {
  width: 38px; height: 38px; border-radius: 12px;
  display: flex; align-items: center; justify-content: center;
  font-size: 0.9rem; flex-shrink: 0;
}
.field-label {
  font-size: 0.58rem; font-weight: 900; color: #94a3b8;
  letter-spacing: 0.8px; text-transform: uppercase; margin-bottom: 3px;
}
.field-value { font-size: 0.85rem; font-weight: 800; color: #0f172a; }
.field-verified-badge {
  position: absolute; top: 10px; right: 12px;
  color: #10b981; font-size: 0.75rem;
}

/* ═══════════════════════════════════════════
   BIO CARD
═══════════════════════════════════════════ */
.bio-card {
  background: white;
  border-radius: 24px;
  border: 1px solid #eef2f6;
  padding: 28px;
  box-shadow: 0 4px 20px rgba(0,0,0,0.04);
}
.bio-card-header {
  display: flex; align-items: center; gap: 14px;
  margin-bottom: 18px;
}
.bio-header-icon {
  width: 44px; height: 44px; background: #eef2ff; color: #6366f1;
  border-radius: 14px; display: flex;
  align-items: center; justify-content: center; font-size: 1.1rem;
}
.bio-text {
  font-size: 0.9rem; color: #374151; font-weight: 600;
  line-height: 1.7; white-space: pre-line;
  border-left: 3px solid #f59e0b;
  padding-left: 18px;
}

/* ═══════════════════════════════════════════
   QUICK ACTIONS
═══════════════════════════════════════════ */
.quick-actions-card {
  background: white;
  border-radius: 24px;
  border: 1px solid #eef2f6;
  padding: 24px 28px;
  box-shadow: 0 4px 20px rgba(0,0,0,0.04);
}
.qa-header {
  font-size: 0.78rem; font-weight: 900;
  color: #0f172a; margin-bottom: 16px;
  display: flex; align-items: center;
}
.text-amber { color: #f59e0b !important; }
.qa-grid { display: flex; flex-direction: column; gap: 8px; }
.qa-item {
  display: flex; align-items: center; gap: 14px;
  background: #f8fafc; border: 1.5px solid #eef2f6;
  border-radius: 14px; padding: 13px 16px;
  cursor: pointer; font-family: inherit;
  font-size: 0.82rem; font-weight: 800; color: #374151;
  transition: 0.2s; text-align: left; width: 100%;
}
.qa-item:hover { border-color: #f59e0b; background: #fffbeb; color: #0f172a; }
.qa-icon {
  width: 36px; height: 36px; border-radius: 10px;
  display: flex; align-items: center; justify-content: center;
  font-size: 0.85rem; flex-shrink: 0;
}
.qa-arrow {
  margin-left: auto; font-size: 0.6rem;
  color: #cbd5e1; transition: 0.2s;
}
.qa-item:hover .qa-arrow { color: #f59e0b; transform: translateX(3px); }

/* ═══════════════════════════════════════════
   UTILS
═══════════════════════════════════════════ */
.fw-900 { font-weight: 900 !important; }
.fw-800 { font-weight: 800 !important; }

/* ═══════════════════════════════════════════
   DARK MODE
═══════════════════════════════════════════ */
[data-theme="dark"] .profile-root { background: #0d1117; color: #f0f6fc; }
[data-theme="dark"] .identity-card,
[data-theme="dark"] .info-card,
[data-theme="dark"] .bio-card,
[data-theme="dark"] .quick-actions-card { background: #161b22; border-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .identity-name { color: #f0f6fc; }
[data-theme="dark"] .id-stat-value { color: #f0f6fc; }
[data-theme="dark"] .field-value { color: #f0f6fc; }
[data-theme="dark"] .info-field-item { background: rgba(255,255,255,0.03); border-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .info-field-item:hover { background: rgba(245,158,11,0.06); border-color: rgba(245,158,11,0.2); }
[data-theme="dark"] .qa-item { background: rgba(255,255,255,0.03); border-color: rgba(255,255,255,0.06); color: #c9d1d9; }
[data-theme="dark"] .qa-item:hover { background: rgba(245,158,11,0.07); border-color: rgba(245,158,11,0.2); color: #f0f6fc; }
[data-theme="dark"] .id-btn-share { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.08); color: #8b949e; }
[data-theme="dark"] .identity-divider { background: rgba(255,255,255,0.06); }
[data-theme="dark"] .info-card-header { border-bottom-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .page-title { color: #f0f6fc; }
[data-theme="dark"] .breadcrumb-trail .current { color: #f0f6fc; }
[data-theme="dark"] .btn-outline-action { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.08); color: #8b949e; }
[data-theme="dark"] .bio-text { color: #c9d1d9; }
[data-theme="dark"] .qa-header { color: #f0f6fc; }
</style>