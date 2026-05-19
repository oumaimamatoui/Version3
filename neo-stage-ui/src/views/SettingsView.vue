<template>
  <div class="settings-master-root" @mousemove="handleParallax">

    <!-- BACKGROUND -->
    <div class="cyber-engine-bg">
      <div class="bg-vignette"></div>
      <div class="glow-orb orb-amber" :style="orbStyle(0.04)"></div>
      <div class="glow-orb orb-indigo" :style="orbStyle(0.015)"></div>
      <div class="quantum-grid"></div>
    </div>

    <AppSidebar />

    <div class="main-viewport flex-grow-1 d-flex flex-column position-relative">
      <AppNavbar />

      <main class="canvas-engine flex-grow-1 overflow-auto custom-scrollbar">
        <div class="content-area p-4 p-lg-5">

          <div v-if="!loading" class="animate__animated animate__fadeIn">

            <!-- HEADER -->
            <header class="page-header mb-5">
              <div class="header-left">
                <div class="breadcrumb-pro mb-2">
                  <span class="root">Administration</span>
                  <i class="fa-solid fa-chevron-right mx-2 separator"></i>
                  <span class="current">{{ t('settings.terminal') }}</span>
                </div>
                <h2 class="premium-title">
                  {{ t('settings.title') }}
                  <span class="gradient-text">{{ t('settings.titleSpan') }}</span>
                </h2>
                <p class="brand-subtitle mt-1">{{ t('settings.subtitle', { role: roleDisplay }) }}</p>
              </div>
              <div class="header-right">
                <div class="autosave-indicator" :class="{ saving: saving }">
                  <div class="autosave-dot" :class="{ pulse: saving }"></div>
                  <i :class="saving ? 'fa-solid fa-spinner fa-spin' : 'fa-solid fa-cloud-arrow-up'"></i>
                  <span>{{ saving ? t('settings.actions.syncing') : t('settings.actions.save') }}</span>
                </div>
              </div>
            </header>

            <div class="row g-4">

              <!-- SIDEBAR NAV -->
              <div class="col-lg-3">
                <div class="nav-panel sticky-top" style="top: 20px; z-index: 10;">

                  <div class="nav-panel-header">
                    <div class="avatar-display">
                      <img :src="profileDisplayUrl" :alt="t('profile.avatarAlt')" class="avatar-img">
                      <div class="avatar-status-ring"></div>
                    </div>
                    <div class="avatar-info ms-3">
                      <h6 class="fw-900 m-0">{{ userForm.prenom }} {{ userForm.nom }}</h6>
                      <span class="role-badge-inline">{{ roleDisplay }}</span>
                    </div>
                  </div>

                  <div class="settings-nav-matrix">
                    <button
                      v-for="tab in filteredTabs"
                      :key="tab.id"
                      @click="activeTab = tab.id"
                      :class="['nav-matrix-btn', { active: activeTab === tab.id }]"
                    >
                      <div class="icon-shell">
                        <i :class="tab.icon"></i>
                      </div>
                      <span>{{ t(`settings.tabs.${tab.id}`).toUpperCase() }}</span>
                      <i v-if="activeTab === tab.id" class="fa-solid fa-chevron-right ms-auto nav-arrow"></i>
                    </button>
                  </div>

                  <div class="nav-panel-footer">
                    <div class="join-date-widget">
                      <i class="fa-regular fa-calendar-check text-amber me-2"></i>
                      <span class="small text-muted">{{ t('profile.joinedSince') }} {{ userForm.joinDate }}</span>
                    </div>
                  </div>
                </div>
              </div>

              <!-- CONTENT PANEL -->
              <div class="col-lg-9">
                <div class="content-panel">

                  <!-- SECTION HEADER -->
                  <div class="section-title-bar mb-5">
                    <div class="section-icon-box">
                      <i :class="filteredTabs.find(t => t.id === activeTab)?.icon"></i>
                    </div>
                    <div>
                      <h5 class="fw-900 m-0">{{ t(`settings.tabs.${activeTab}`) }}</h5>
                      <p class="text-muted small m-0">{{ getSectionSubtitle(activeTab) }}</p>
                    </div>
                    <div class="ms-auto">
                      <span class="status-badge status-active">
                        <span class="status-dot"></span> ACTIF
                      </span>
                    </div>
                  </div>

                  <!-- ══════════════════════════
                       SECTION : PROFIL
                  ══════════════════════════ -->
                  <div v-if="activeTab === 'profile'" class="settings-section animate__animated animate__fadeIn">

                    <div class="profile-hero-card mb-5">
                      <div class="profile-hero-bg"></div>
                      <div class="profile-hero-content">
                        <div class="avatar-upload-zone" @click="triggerPhotoUpload">
                          <img :src="profileDisplayUrl" :alt="t('profile.avatarAlt')" class="avatar-large">
                          <div class="avatar-upload-overlay">
                            <i class="fa-solid fa-camera"></i>
                            <span>Modifier</span>
                          </div>
                          <input type="file" ref="photoInput" @change="handlePhotoChange" hidden accept="image/*">
                        </div>
                        <div class="profile-hero-data ms-4">
                          <h3 class="fw-900 m-0">{{ userForm.prenom }} {{ userForm.nom }}</h3>
                          <p class="text-muted mb-3">{{ userForm.email }}</p>
                          <div class="d-flex gap-2 flex-wrap">
                            <span class="hero-badge"><i class="fa-solid fa-shield-check me-1 text-amber"></i>{{ roleDisplay }}</span>
                            <span class="hero-badge"><i class="fa-regular fa-calendar me-1 text-indigo"></i>{{ userForm.joinDate }}</span>
                          </div>
                        </div>
                      </div>
                    </div>

                    <div class="row g-4">
                      <div class="col-md-6">
                        <div class="enigma-input-wrap">
                          <label>{{ t('settings.labels.firstName').toUpperCase() }}</label>
                          <div class="input-with-icon">
                            <i class="fa-solid fa-user input-icon"></i>
                            <input type="text" class="enigma-field" v-model="userForm.prenom" :placeholder="t('settings.labels.firstName')">
                          </div>
                        </div>
                      </div>
                      <div class="col-md-6">
                        <div class="enigma-input-wrap">
                          <label>{{ t('settings.labels.lastName').toUpperCase() }}</label>
                          <div class="input-with-icon">
                            <i class="fa-solid fa-user input-icon"></i>
                            <input type="text" class="enigma-field" v-model="userForm.nom" :placeholder="t('settings.labels.lastName')">
                          </div>
                        </div>
                      </div>
                      <div class="col-12">
                        <div class="enigma-input-wrap">
                          <label>{{ t('settings.labels.email').toUpperCase() }}</label>
                          <div class="input-with-icon">
                            <i class="fa-solid fa-envelope input-icon"></i>
                            <input type="email" class="enigma-field" v-model="userForm.email" :placeholder="t('settings.labels.email')">
                          </div>
                        </div>
                      </div>
                      <div class="col-12">
                        <div class="enigma-input-wrap">
                          <label>{{ t('settings.labels.bio').toUpperCase() }}</label>
                          <textarea class="enigma-field no-icon" v-model="userForm.bio" rows="4" :placeholder="t('settings.labels.bioPlaceholder')"></textarea>
                        </div>
                      </div>
                    </div>
                  </div>

                  <!-- ══════════════════════════
                       SECTION : SÉCURITÉ
                  ══════════════════════════ -->
                  <div v-if="activeTab === 'security'" class="settings-section animate__animated animate__fadeIn">

                    <div class="security-status-widget mb-5">
                      <div class="sec-status-left">
                        <div class="sec-icon-ring">
                          <i class="fa-solid fa-shield-halved"></i>
                        </div>
                        <div class="ms-4">
                          <h6 class="fw-900 m-0">Niveau de sécurité</h6>
                          <p class="text-muted small m-0">Protégez votre compte avec un mot de passe robuste</p>
                        </div>
                      </div>
                      <div class="sec-health-bar-wrap">
                        <div class="sec-health-bar">
                          <div class="sec-health-fill" style="width: 75%"></div>
                        </div>
                        <span class="sec-health-label">BON</span>
                      </div>
                    </div>

                    <div class="row g-4">
                      <div class="col-12">
                        <div class="enigma-input-wrap">
                          <label>{{ t('settings.labels.currentPassword').toUpperCase() }}</label>
                          <div class="input-with-icon">
                            <i class="fa-solid fa-lock input-icon"></i>
                            <input type="password" class="enigma-field" v-model="securityForm.currentPassword" placeholder="••••••••">
                          </div>
                        </div>
                      </div>
                      <div class="col-md-6">
                        <div class="enigma-input-wrap">
                          <label>{{ t('settings.labels.newPassword').toUpperCase() }}</label>
                          <div class="input-with-icon">
                            <i class="fa-solid fa-key input-icon"></i>
                            <input type="password" class="enigma-field" v-model="securityForm.newPassword" placeholder="••••••••">
                          </div>
                        </div>
                      </div>
                      <div class="col-md-6">
                        <div class="enigma-input-wrap">
                          <label>{{ t('settings.labels.confirmPassword').toUpperCase() }}</label>
                          <div class="input-with-icon">
                            <i class="fa-solid fa-key input-icon"></i>
                            <input type="password" class="enigma-field" v-model="securityForm.confirmPassword" placeholder="••••••••">
                          </div>
                        </div>
                      </div>

                      <div class="col-12" v-if="securityForm.newPassword">
                        <div class="password-strength-analyzer">
                          <div class="psa-header">
                            <div class="psa-label">ROBUSTESSE DU MOT DE PASSE</div>
                            <span class="psa-text" :style="{ color: strengthColor }">{{ strengthLabel }}</span>
                          </div>
                          <div class="psa-bars">
                            <div
                              v-for="(_, i) in 5"
                              :key="i"
                              class="psa-bar"
                              :class="{ filled: i < passwordStrength }"
                              :style="i < passwordStrength ? { background: strengthColor } : {}"
                            ></div>
                          </div>
                        </div>
                      </div>

                      <div class="col-12 mt-2" v-if="authStore.role !== 'Candidat'">
                        <div class="protocol-row">
                          <div class="p-icon"><i class="fa-solid fa-mobile-screen-button"></i></div>
                          <div class="p-data">
                            <h6>Authentification 2FA</h6>
                            <p>Renforcez votre sécurité avec un second facteur d'authentification.</p>
                          </div>
                          <span class="badge-coming-soon">BIENTÔT</span>
                        </div>
                      </div>
                    </div>
                  </div>

                  <!-- ══════════════════════════
                       SECTION : BRANDING
                  ══════════════════════════ -->
                  <div v-if="activeTab === 'branding'" class="settings-section animate__animated animate__fadeIn">
                    <div class="row g-4">
                      <div class="col-12">
                        <div class="enigma-input-wrap">
                          <label>{{ t('settings.labels.companyName').toUpperCase() }}</label>
                          <div class="input-with-icon">
                            <i class="fa-solid fa-building input-icon"></i>
                            <input type="text" class="enigma-field" v-model="brandForm.companyName">
                          </div>
                        </div>
                      </div>

                      <div class="col-12">
                        <div class="enigma-input-wrap">
                          <label>{{ t('settings.labels.signatureColor').toUpperCase() }}</label>
                          <div class="color-picker-pro">
                            <div class="color-swatch-large" :style="{ background: brandForm.color }">
                              <input type="color" class="color-input-hidden" v-model="brandForm.color">
                            </div>
                            <div class="color-meta">
                              <code class="color-hex fw-900">{{ brandForm.color.toUpperCase() }}</code>
                              <p class="small text-muted m-0">Cliquez sur la pastille pour modifier</p>
                            </div>
                            <div class="color-presets">
                              <button
                                v-for="preset in colorPresets"
                                :key="preset"
                                class="preset-dot"
                                :style="{ background: preset }"
                                :class="{ active: brandForm.color === preset }"
                                @click="brandForm.color = preset"
                              ></button>
                            </div>
                          </div>
                        </div>
                      </div>

                      <div class="col-12">
                        <div class="brand-preview-card">
                          <div class="brand-preview-label">APERÇU EN DIRECT</div>
                          <div class="brand-preview-ui" :style="{ '--brand-color': brandForm.color }">
                            <div class="bp-header" :style="{ background: brandForm.color }">
                              <span class="fw-900 text-white">{{ brandForm.companyName || 'Votre Entreprise' }}</span>
                            </div>
                            <div class="bp-body">
                              <div class="bp-btn" :style="{ background: brandForm.color }">Démarrer l'évaluation</div>
                              <div class="bp-tag" :style="{ color: brandForm.color, borderColor: brandForm.color }">Certifié</div>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>

                  <!-- ══════════════════════════
                       SECTION : INTÉGRATIONS
                  ══════════════════════════ -->
                  <div v-if="activeTab === 'integrations'" class="settings-section animate__animated animate__fadeIn">

                    <!-- Stats Row -->
                    <div class="integrations-stats-row mb-5">
                      <div class="int-stat-card">
                        <div class="int-stat-icon" style="background: #ecfdf5; color: #10b981;">
                          <i class="fa-solid fa-link"></i>
                        </div>
                        <div>
                          <div class="int-stat-value">{{ integrationStats.isGoogleConnected ? '1' : '0' }}</div>
                          <div class="int-stat-label">Connecté(s)</div>
                        </div>
                      </div>
                      <div class="int-stat-card">
                        <div class="int-stat-icon" style="background: #fffbeb; color: #f59e0b;">
                          <i class="fa-solid fa-plug-circle-bolt"></i>
                        </div>
                        <div>
                          <div class="int-stat-value">1</div>
                          <div class="int-stat-label">Disponible(s)</div>
                        </div>
                      </div>
                      <div class="int-stat-card">
                        <div class="int-stat-icon" style="background: #f0f9ff; color: #0ea5e9;">
                          <i class="fa-solid fa-shield-check"></i>
                        </div>
                        <div>
                          <div class="int-stat-value">OAuth2</div>
                          <div class="int-stat-label">Protocole</div>
                        </div>
                      </div>
                    </div>

                    <!-- Google Integration -->
                    <div class="integrations-grid">
                      <div class="integration-card" :class="{ connected: integrationStats.isGoogleConnected }">
                        <div class="integration-card-inner">
                          <div class="int-icon-shell">
                            <svg width="24" height="24" viewBox="0 0 24 24" fill="none">
                              <path d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z" fill="#4285F4"/>
                              <path d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z" fill="#34A853"/>
                              <path d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z" fill="#FBBC05"/>
                              <path d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z" fill="#EA4335"/>
                            </svg>
                          </div>
                          <div class="int-data flex-grow-1">
                            <div class="d-flex align-items-center gap-2 mb-1 flex-wrap">
                              <h6 class="fw-900 m-0">{{ t('settings.labels.googleTitle') }}</h6>
                              <span v-if="integrationStats.isGoogleConnected" class="int-connected-badge">
                                <i class="fa-solid fa-circle-check me-1"></i>Connecté
                              </span>
                            </div>
                            <p class="text-muted small m-0 mb-2">{{ t('settings.labels.googleDesc') }}</p>
                            <span v-if="integrationStats.isGoogleConnected" class="int-email-chip">
                              <i class="fa-regular fa-envelope me-1"></i>{{ integrationStats.connectedEmail }}
                            </span>
                          </div>
                          <div class="int-actions">
                            <button v-if="!integrationStats.isGoogleConnected" @click="connectGmail" class="btn-enigma-primary py-2 px-4">
                              <div class="btn-content"><i class="fa-solid fa-link me-2"></i>{{ t('settings.actions.connect') }}</div>
                              <div class="btn-glow"></div>
                            </button>
                            <button v-else @click="disconnectGmail" class="btn-disconnect">
                              <i class="fa-solid fa-link-slash me-2"></i>{{ t('settings.actions.disconnect') }}
                            </button>
                          </div>
                        </div>
                        <div v-if="integrationStats.isGoogleConnected" class="int-active-bar"></div>
                      </div>
                    </div>

                    <!-- Security Note -->
                    <div class="security-note mt-4">
                      <i class="fa-solid fa-shield-halved me-2 text-amber"></i>
                      <span>Toutes les connexions sont sécurisées via <strong>OAuth 2.0</strong>. Vos données restent protégées en permanence.</span>
                    </div>
                  </div>

                  <!-- FOOTER ACTIONS -->
                  <div class="footer-actions mt-5 pt-4">
                    <button class="btn-ghost-action" @click="resetForm">
                      <i class="fa-solid fa-rotate-left me-2"></i>{{ t('settings.actions.cancel') }}
                    </button>
                    <button @click="saveChanges" class="btn-enigma-primary px-5 py-3" :disabled="saving">
                      <div class="btn-content">
                        <span v-if="saving"><i class="fa-solid fa-spinner fa-spin me-2"></i>{{ t('settings.actions.syncing') }}</span>
                        <span v-else>{{ t('settings.actions.save') }} <i class="fa-solid fa-cloud-arrow-up ms-2"></i></span>
                      </div>
                      <div class="btn-glow"></div>
                    </button>
                  </div>

                </div>
              </div>
            </div>
          </div>

          <!-- LOADING STATE -->
          <div v-else class="loading-state">
            <div class="spinner-pro-premium"></div>
            <p class="mt-4 fw-800 text-muted text-uppercase small tracking-wider">{{ t('settings.actions.loadingCore') }}</p>
          </div>

        </div><!-- /content-area -->
      </main>
    </div><!-- /main-viewport -->

    <!-- TOAST -->
    <transition name="toast-slide">
      <div v-if="globalToast.active" class="enigma-toast" :class="globalToast.type">
        <div class="t-ico"><i :class="globalToast.icon"></i></div>
        <div class="t-body">
          <strong>SYSTEM MESSAGE</strong>
          <p class="m-0 small">{{ globalToast.message }}</p>
        </div>
      </div>
    </transition>

  </div>
</template>

<script setup>
import { ref, computed, onMounted, reactive } from 'vue';
import { useI18n } from 'vue-i18n';
import api from '@/services/api';
import { useAuthStore } from '@/stores/auth';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';

const { t } = useI18n();
const authStore = useAuthStore();

const loading = ref(true);
const saving = ref(false);
const role = ref('');
const activeTab = ref('profile');
const integrationStats = ref({ isGoogleConnected: false, connectedEmail: null });
const photoInput = ref(null);

const userForm = ref({ nom: '', prenom: '', email: '', photoUrl: '', joinDate: '' });
const securityForm = ref({ currentPassword: '', newPassword: '', confirmPassword: '' });
const brandForm = ref({ companyName: '', color: '#eab308' });

const mousePos = reactive({ x: 0, y: 0 });
const globalToast = reactive({ active: false, message: '', type: '', icon: '' });

const colorPresets = ['#eab308', '#f43f5e', '#6366f1', '#10b981', '#06b6d4', '#f97316', '#0f172a'];

const profileDisplayUrl = computed(() => {
  if (userForm.value.photoUrl) return `http://localhost:5172${userForm.value.photoUrl}`;
  return `https://ui-avatars.com/api/?name=${userForm.value.prenom}+${userForm.value.nom}&background=0f172a&color=eab308&size=128`;
});

const allTabs = [
  { id: 'profile',      icon: 'fa-solid fa-user-gear',     roles: ['SuperAdmin','AdminEntreprise','Recruteur','Evaluateur','Candidat'] },
  { id: 'security',     icon: 'fa-solid fa-shield-halved', roles: ['SuperAdmin','AdminEntreprise','Recruteur','Evaluateur','Candidat'] },
  { id: 'branding',     icon: 'fa-solid fa-palette',       roles: ['AdminEntreprise'] },
  { id: 'integrations', icon: 'fa-solid fa-plug',          roles: ['AdminEntreprise','SuperAdmin'] },
];

const filteredTabs = computed(() => {
  const currentRole = (role.value || '').toLowerCase();
  return allTabs.filter(tab => {
    if (tab.id === 'profile' || tab.id === 'security') return true;
    return tab.roles.some(r => r.toLowerCase() === currentRole);
  });
});

const roleDisplay = computed(() => {
  if (!role.value) return t('roles.User');
  return t(`roles.${role.value}`);
});

const passwordStrength = computed(() => {
  const p = securityForm.value.newPassword;
  if (!p) return 0;
  let score = 0;
  if (p.length >= 8)  score++;
  if (p.length >= 12) score++;
  if (/[A-Z]/.test(p)) score++;
  if (/[0-9]/.test(p)) score++;
  if (/[^A-Za-z0-9]/.test(p)) score++;
  return score;
});

const strengthColor = computed(() => {
  return ['#f43f5e','#f97316','#eab308','#06b6d4','#10b981'][passwordStrength.value - 1] || '#f43f5e';
});

const strengthLabel = computed(() => {
  return ['Très faible','Faible','Moyen','Fort','Excellent'][passwordStrength.value - 1] || 'Très faible';
});

const getSectionSubtitle = (tab) => {
  const map = {
    profile:      'Gérez vos informations personnelles',
    security:     "Sécurisez l'accès à votre compte",
    branding:     "Personnalisez l'identité de votre entreprise",
    integrations: 'Connectez vos outils externes',
  };
  return map[tab] || '';
};

const fetchInitialData = async () => {
  loading.value = true;
  try {
    const resUser = await api.get('/Settings/me');
    userForm.value = resUser.data;
    if (resUser.data.photoUrl && authStore.user.photoUrl !== resUser.data.photoUrl) {
      authStore.user.photoUrl = resUser.data.photoUrl;
      localStorage.setItem('user', JSON.stringify(authStore.user));
    }
    if (role.value.toLowerCase() === 'adminentreprise') {
      const resBrand = await api.get('/Settings/branding');
      brandForm.value = resBrand.data;
      integrationStats.value.isGoogleConnected = resBrand.data.isGoogleConnected;
      integrationStats.value.connectedEmail = resBrand.data.connectedEmail;
    }
  } catch (error) {
    console.error(error);
  } finally {
    loading.value = false;
  }
};

const saveChanges = async () => {
  saving.value = true;
  try {
    let endpoint = '';
    let payload = {};
    if (activeTab.value === 'profile') {
      endpoint = '/Settings/update-profile';
      payload = userForm.value;
    } else if (activeTab.value === 'security') {
      if (securityForm.value.newPassword !== securityForm.value.confirmPassword) {
        showToast(t('settings.alerts.passMismatch'), 'error', 'fa-solid fa-triangle-exclamation');
        saving.value = false;
        return;
      }
      endpoint = '/Settings/change-password';
      payload = securityForm.value;
    } else if (activeTab.value === 'branding') {
      endpoint = '/Settings/update-branding';
      payload = brandForm.value;
    }
    await api.post(endpoint, payload);
    showToast(t('settings.alerts.syncSuccess'), 'success', 'fa-solid fa-cloud-arrow-up');
  } catch (error) {
    showToast(error.response?.data?.message || t('profile.uploadError'), 'error', 'fa-solid fa-triangle-exclamation');
  } finally {
    saving.value = false;
  }
};

const handlePhotoChange = async (e) => {
  const file = e.target.files[0];
  if (!file) return;
  const formData = new FormData();
  formData.append('file', file);
  try {
    saving.value = true;
    const res = await api.post('/Settings/upload-photo', formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    });
    userForm.value.photoUrl = res.data.photoUrl;
    authStore.user.photoUrl = res.data.photoUrl;
    localStorage.setItem('user', JSON.stringify(authStore.user));
    showToast(t('profile.uploadSuccess'), 'success', 'fa-solid fa-camera');
  } catch {
    showToast(t('profile.uploadError'), 'error', 'fa-solid fa-triangle-exclamation');
  } finally {
    saving.value = false;
  }
};

const disconnectGmail = async () => {
  if (!confirm(t('settings.alerts.disconnectConfirm'))) return;
  try {
    await api.post('/GoogleAuth/disconnect');
    integrationStats.value.isGoogleConnected = false;
    showToast(t('settings.alerts.disconnectSuccess'), 'success', 'fa-solid fa-link-slash');
  } catch {
    showToast(t('settings.alerts.authError'), 'error', 'fa-solid fa-triangle-exclamation');
  }
};

const connectGmail = async () => {
  try {
    const res = await api.get('/GoogleAuth/auth-url');
    window.location.href = res.data.url;
  } catch {
    showToast(t('settings.alerts.authError'), 'error', 'fa-solid fa-triangle-exclamation');
  }
};

let _toastTimer = null;
const showToast = (msg, type = 'success', icon = 'fa-solid fa-check') => {
  clearTimeout(_toastTimer);
  Object.assign(globalToast, { message: msg, type: `t-${type}`, icon, active: true });
  _toastTimer = setTimeout(() => { globalToast.active = false; }, 4000);
};

const resetForm = () => fetchInitialData();
const triggerPhotoUpload = () => photoInput.value.click();

const orbStyle = (f) => ({
  transform: `translate(${mousePos.x * f * 10}px, ${mousePos.y * f * 10}px)`
});
const handleParallax = (e) => {
  mousePos.x = (e.clientX - window.innerWidth / 2) / 20;
  mousePos.y = (e.clientY - window.innerHeight / 2) / 20;
};

onMounted(() => {
  role.value = authStore.role || localStorage.getItem('role') || 'Candidat';
  fetchInitialData();
});
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;600;700;800;900&family=JetBrains+Mono:wght@600;700&display=swap');
@import url('https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.2/css/all.min.css');

/* ═══════════════════════════════════════════
   ROOT & LAYOUT  (identique au Dashboard)
═══════════════════════════════════════════ */
.settings-master-root {
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
.canvas-engine { height: calc(100vh - 64px); }
.content-area  { position: relative; z-index: 20; }

.custom-scrollbar::-webkit-scrollbar       { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: transparent; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: rgba(148,163,184,0.35); border-radius: 10px; }
.custom-scrollbar::-webkit-scrollbar-thumb:hover { background: rgba(148,163,184,0.6); }

/* ═══════════════════════════════════════════
   BACKGROUND
═══════════════════════════════════════════ */
.cyber-engine-bg { position: fixed; inset: 0; z-index: 0; pointer-events: none; }
.quantum-grid {
  position: absolute; inset: 0;
  background-image: radial-gradient(#cbd5e1 1px, transparent 1px);
  background-size: 40px 40px;
  opacity: 0.18;
}
[data-theme="dark"] .quantum-grid { opacity: 0.25; }
.glow-orb {
  position: absolute; width: 600px; height: 600px;
  filter: blur(130px); opacity: 0.1; border-radius: 50%;
  transition: transform 0.4s ease-out;
}
[data-theme="dark"] .glow-orb { opacity: 0.16; }
.orb-amber  { background: #f59e0b; top: -200px; right: -100px; }
.orb-indigo { background: #6366f1; bottom: -200px; left: -100px; }

/* ═══════════════════════════════════════════
   PAGE HEADER
═══════════════════════════════════════════ */
.page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  flex-wrap: wrap;
  gap: 16px;
}
.premium-title {
  font-weight: 900;
  font-size: clamp(1.4rem, 2.5vw, 2.1rem);
  letter-spacing: -1.5px;
  margin: 0;
  line-height: 1.1;
  color: var(--text-main, #0f172a);
  transition: color 0.3s;
}
.gradient-text {
  background: linear-gradient(135deg, #f59e0b 0%, #fbbf24 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}
.breadcrumb-pro { font-size: 0.7rem; font-weight: 700; color: #94a3b8; }
.breadcrumb-pro .root:hover { color: #f59e0b; cursor: pointer; }
.breadcrumb-pro .separator { font-size: 0.5rem; opacity: 0.4; }
.breadcrumb-pro .current { color: #0f172a; font-weight: 800; }
.brand-subtitle {
  color: #94a3b8;
  font-size: 0.68rem;
  font-weight: 800;
  letter-spacing: 1.5px;
  text-transform: uppercase;
}

/* AUTOSAVE */
.autosave-indicator {
  display: flex; align-items: center; gap: 10px;
  font-size: 0.7rem; font-weight: 700; color: #94a3b8;
  background: white; padding: 10px 18px;
  border-radius: 14px; border: 1.5px solid #eef2f6;
  box-shadow: 0 2px 8px rgba(0,0,0,0.04);
  transition: all 0.3s;
}
.autosave-indicator.saving { color: #f59e0b; border-color: #fde68a; background: #fffbeb; }
.autosave-dot {
  width: 7px; height: 7px; border-radius: 50%;
  background: #10b981;
}
.autosave-dot.pulse { background: #f59e0b; animation: dotPulse 1s infinite; }
@keyframes dotPulse { 0%,100%{opacity:1;transform:scale(1)} 50%{opacity:0.4;transform:scale(1.4)} }

/* ═══════════════════════════════════════════
   NAV PANEL
═══════════════════════════════════════════ */
.nav-panel {
  background: white;
  border-radius: 28px;
  padding: 24px;
  border: 1px solid #eef2f6;
  box-shadow: 0 4px 24px rgba(0,0,0,0.05);
}
.nav-panel-header {
  display: flex; align-items: center;
  padding-bottom: 20px;
  border-bottom: 1px solid #f1f5f9;
  margin-bottom: 16px;
}
.avatar-display { position: relative; width: 50px; height: 50px; flex-shrink: 0; }
.avatar-img {
  width: 50px; height: 50px; border-radius: 16px;
  object-fit: cover; border: 3px solid #eef2f6;
}
.avatar-status-ring {
  position: absolute; bottom: -2px; right: -2px;
  width: 13px; height: 13px; border-radius: 50%;
  background: #10b981; border: 2px solid white;
}
.avatar-info h6 { font-size: 0.88rem; color: #0f172a; }
.role-badge-inline {
  font-size: 0.58rem; font-weight: 900;
  background: #fffbeb; color: #f59e0b;
  padding: 3px 10px; border-radius: 8px;
  text-transform: uppercase; letter-spacing: 0.5px;
}
.settings-nav-matrix { display: flex; flex-direction: column; gap: 5px; }
.nav-matrix-btn {
  display: flex; align-items: center; gap: 12px;
  padding: 12px 14px; background: transparent;
  border: none; border-radius: 16px;
  color: #64748b; font-weight: 800; font-size: 0.68rem;
  transition: all 0.2s cubic-bezier(0.4,0,0.2,1);
  cursor: pointer; font-family: inherit; width: 100%;
  text-align: left; letter-spacing: 0.3px;
}
.nav-matrix-btn:hover { background: #f8fafc; color: #0f172a; }
.nav-matrix-btn.active {
  background: #0f172a; color: #fff;
  box-shadow: 0 8px 24px rgba(15,23,42,0.18);
}
.nav-matrix-btn.active .icon-shell { background: rgba(255,255,255,0.1); color: #f59e0b; }
.nav-arrow { font-size: 0.55rem; opacity: 0.5; }
.icon-shell {
  width: 34px; height: 34px; background: #f1f5f9;
  border-radius: 10px; display: flex;
  align-items: center; justify-content: center;
  flex-shrink: 0; font-size: 0.82rem; transition: 0.2s;
}
.nav-panel-footer { border-top: 1px solid #f1f5f9; padding-top: 16px; margin-top: 16px; }
.join-date-widget { display: flex; align-items: center; font-size: 0.7rem; }
.text-amber { color: #f59e0b !important; }
.text-indigo { color: #6366f1 !important; }

/* ═══════════════════════════════════════════
   CONTENT PANEL
═══════════════════════════════════════════ */
.content-panel {
  background: white;
  border-radius: 28px;
  padding: 36px 40px;
  border: 1px solid #eef2f6;
  border-top: 4px solid #f59e0b;
  box-shadow: 0 4px 24px rgba(0,0,0,0.05);
}
.section-title-bar {
  display: flex; align-items: center; gap: 18px;
  padding-bottom: 24px;
  border-bottom: 1px solid #f1f5f9;
}
.section-icon-box {
  width: 50px; height: 50px;
  background: #fffbeb; color: #f59e0b;
  border-radius: 16px; display: flex;
  align-items: center; justify-content: center;
  font-size: 1.15rem; flex-shrink: 0;
}
.section-title-bar h5 { color: #0f172a; font-size: 0.98rem; }
.status-badge {
  padding: 5px 14px; border-radius: 10px;
  font-size: 0.6rem; font-weight: 800;
  text-transform: uppercase;
  display: inline-flex; align-items: center;
}
.status-active { background: #ecfdf5; color: #10b981; }
.status-dot {
  width: 6px; height: 6px; border-radius: 50%;
  background: currentColor; margin-right: 6px;
  animation: pulse 2s infinite;
}
@keyframes pulse { 0%,100%{opacity:1} 50%{opacity:0.3} }

/* ═══════════════════════════════════════════
   PROFILE HERO
═══════════════════════════════════════════ */
.profile-hero-card {
  border-radius: 20px;
  border: 1px solid #eef2f6;
  overflow: hidden;
  position: relative;
}
.profile-hero-bg {
  height: 88px;
  background: linear-gradient(135deg, #0f172a 0%, #1e293b 60%, #0f172a 100%);
  position: relative;
}
.profile-hero-bg::after {
  content: '';
  position: absolute; inset: 0;
  background: radial-gradient(circle at 70% 50%, rgba(245,158,11,0.15), transparent 60%);
}
.profile-hero-content {
  padding: 0 28px 24px;
  display: flex; align-items: flex-end;
  margin-top: -30px;
}
.avatar-upload-zone {
  position: relative; width: 88px; height: 88px;
  cursor: pointer; flex-shrink: 0;
  border-radius: 20px; overflow: hidden;
  border: 4px solid white;
  box-shadow: 0 8px 24px rgba(0,0,0,0.14);
}
.avatar-large { width: 100%; height: 100%; object-fit: cover; }
.avatar-upload-overlay {
  position: absolute; inset: 0;
  background: rgba(15,23,42,0.78);
  display: flex; flex-direction: column;
  align-items: center; justify-content: center;
  color: white; font-size: 0.68rem; font-weight: 800;
  opacity: 0; transition: 0.25s; gap: 5px;
}
.avatar-upload-zone:hover .avatar-upload-overlay { opacity: 1; }
.profile-hero-data h3 { color: #0f172a; font-size: 1.3rem; }
.hero-badge {
  font-size: 0.68rem; font-weight: 800;
  background: #f8fafc; border: 1.5px solid #eef2f6;
  padding: 4px 12px; border-radius: 10px; color: #64748b;
}

/* ═══════════════════════════════════════════
   FORM INPUTS
═══════════════════════════════════════════ */
.enigma-input-wrap label {
  font-size: 0.6rem; font-weight: 900;
  color: #94a3b8; letter-spacing: 1px;
  margin-bottom: 8px; display: block;
}
.input-with-icon { position: relative; }
.input-icon {
  position: absolute; left: 16px; top: 50%;
  transform: translateY(-50%); color: #f59e0b;
  font-size: 0.78rem; z-index: 2;
}
.enigma-field {
  width: 100%; padding: 14px 18px 14px 44px;
  background: #f8fafc;
  border: 2px solid #eef2f6;
  border-radius: 14px; outline: none;
  font-weight: 700; font-family: inherit;
  transition: all 0.2s; font-size: 0.88rem;
  color: #0f172a;
}
.enigma-field.no-icon { padding-left: 18px; }
textarea.enigma-field { padding-left: 18px; resize: vertical; }
.enigma-field:focus {
  border-color: #f59e0b; background: white;
  box-shadow: 0 0 0 4px rgba(245,158,11,0.08);
}

/* ═══════════════════════════════════════════
   SECURITY
═══════════════════════════════════════════ */
.security-status-widget {
  background: linear-gradient(135deg, #f8fafc 0%, #fffbeb 100%);
  border: 1.5px solid #fde68a;
  border-radius: 20px; padding: 22px 24px;
  display: flex; align-items: center;
  justify-content: space-between; gap: 20px;
  flex-wrap: wrap;
}
.sec-status-left { display: flex; align-items: center; }
.sec-icon-ring {
  width: 54px; height: 54px;
  background: linear-gradient(135deg, #f59e0b, #fbbf24);
  border-radius: 16px; display: flex;
  align-items: center; justify-content: center;
  color: white; font-size: 1.25rem;
  box-shadow: 0 8px 20px rgba(245,158,11,0.28);
}
.sec-health-bar-wrap { display: flex; align-items: center; gap: 12px; }
.sec-health-bar {
  width: 120px; height: 6px; background: #e2e8f0;
  border-radius: 10px; overflow: hidden;
}
.sec-health-fill {
  height: 100%; background: linear-gradient(90deg, #10b981, #34d399);
  border-radius: 10px; transition: width 0.8s ease;
}
.sec-health-label { font-size: 0.62rem; font-weight: 900; color: #10b981; letter-spacing: 1px; }
.password-strength-analyzer {
  background: #f8fafc; border-radius: 14px;
  padding: 16px 20px; border: 1.5px solid #eef2f6;
}
.psa-header { display: flex; align-items: center; justify-content: space-between; margin-bottom: 10px; }
.psa-label { font-size: 0.55rem; font-weight: 900; color: #94a3b8; letter-spacing: 1.2px; }
.psa-bars { display: flex; gap: 6px; }
.psa-bar {
  flex: 1; height: 5px; background: #e2e8f0;
  border-radius: 10px; transition: background 0.4s;
}
.psa-text { font-size: 0.7rem; font-weight: 800; }
.protocol-row {
  display: flex; align-items: center; gap: 16px;
  background: #f8fafc; border-radius: 18px; padding: 20px;
  border: 1.5px solid #eef2f6;
}
.p-icon {
  width: 44px; height: 44px; background: #fffbeb;
  border-radius: 12px; display: flex;
  align-items: center; justify-content: center;
  font-size: 1rem; color: #f59e0b; flex-shrink: 0;
}
.p-data { flex: 1; }
.p-data h6 { font-weight: 800; margin: 0; font-size: 0.88rem; }
.p-data p  { font-size: 0.72rem; color: #94a3b8; margin: 0; }
.badge-coming-soon {
  font-size: 0.55rem; font-weight: 900;
  background: #f1f5f9; color: #94a3b8;
  padding: 4px 10px; border-radius: 8px;
  letter-spacing: 0.8px; white-space: nowrap;
}

/* ═══════════════════════════════════════════
   BRANDING
═══════════════════════════════════════════ */
.color-picker-pro {
  display: flex; align-items: center; gap: 20px;
  background: #f8fafc; border: 2px solid #eef2f6;
  border-radius: 18px; padding: 20px 24px;
  flex-wrap: wrap; transition: border-color 0.2s;
}
.color-picker-pro:hover { border-color: #fde68a; }
.color-swatch-large {
  width: 58px; height: 58px; border-radius: 16px;
  cursor: pointer; position: relative;
  box-shadow: 0 8px 20px rgba(0,0,0,0.15);
  flex-shrink: 0; overflow: hidden; transition: transform 0.2s;
}
.color-swatch-large:hover { transform: scale(1.08); }
.color-input-hidden {
  position: absolute; inset: 0; opacity: 0;
  cursor: pointer; width: 100%; height: 100%;
}
.color-hex { font-size: 1.05rem; color: #0f172a; font-family: 'Courier New', monospace; }
.color-presets { display: flex; gap: 8px; flex-wrap: wrap; }
.preset-dot {
  width: 26px; height: 26px; border-radius: 50%;
  border: none; cursor: pointer; transition: 0.2s;
  outline: 3px solid transparent; outline-offset: 2px;
}
.preset-dot:hover { transform: scale(1.18); }
.preset-dot.active { outline-color: #0f172a; }
.brand-preview-card {
  background: #f8fafc; border-radius: 18px;
  padding: 20px; border: 1.5px solid #eef2f6;
}
.brand-preview-label {
  font-size: 0.55rem; font-weight: 900;
  color: #94a3b8; letter-spacing: 1.5px; margin-bottom: 14px;
}
.brand-preview-ui { border-radius: 12px; overflow: hidden; border: 1.5px solid #eef2f6; }
.bp-header { padding: 16px 20px; }
.bp-body { padding: 16px 20px; background: white; display: flex; align-items: center; gap: 12px; }
.bp-btn { padding: 8px 18px; border-radius: 10px; color: white; font-weight: 800; font-size: 0.78rem; }
.bp-tag { padding: 5px 14px; border-radius: 8px; font-weight: 800; font-size: 0.72rem; border: 2px solid; background: transparent; }

/* ═══════════════════════════════════════════
   INTEGRATIONS
═══════════════════════════════════════════ */
.integrations-stats-row {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 12px;
}
.int-stat-card {
  background: #f8fafc;
  border: 1.5px solid #eef2f6;
  border-radius: 16px; padding: 16px 18px;
  display: flex; align-items: center; gap: 14px;
  transition: 0.2s;
}
.int-stat-card:hover { border-color: #fde68a; box-shadow: 0 4px 14px rgba(0,0,0,0.05); }
.int-stat-icon {
  width: 40px; height: 40px; border-radius: 12px;
  display: flex; align-items: center; justify-content: center;
  font-size: 1rem; flex-shrink: 0;
}
.int-stat-value { font-weight: 900; font-size: 0.95rem; color: #0f172a; }
.int-stat-label { font-size: 0.62rem; font-weight: 700; color: #94a3b8; letter-spacing: 0.5px; }

.integrations-grid { display: flex; flex-direction: column; gap: 14px; }
.integration-card {
  border: 2px solid #eef2f6;
  border-radius: 22px; overflow: hidden;
  background: white; transition: 0.25s;
  position: relative;
}
.integration-card:hover { border-color: #cbd5e1; box-shadow: 0 8px 28px rgba(0,0,0,0.07); transform: translateY(-1px); }
.integration-card.connected { border-color: #bbf7d0; background: linear-gradient(135deg, #f0fdf4 0%, white 100%); }
.integration-card-inner {
  display: flex; align-items: center; gap: 20px;
  padding: 24px 28px; flex-wrap: wrap;
}
.int-icon-shell {
  width: 54px; height: 54px;
  background: white; border-radius: 16px;
  display: flex; align-items: center; justify-content: center;
  flex-shrink: 0; box-shadow: 0 4px 14px rgba(0,0,0,0.08);
  border: 1.5px solid #eef2f6;
}
.int-connected-badge {
  font-size: 0.58rem; font-weight: 900;
  background: #ecfdf5; color: #10b981;
  padding: 3px 10px; border-radius: 8px;
}
.int-email-chip {
  font-size: 0.72rem; font-weight: 700;
  background: white; color: #64748b;
  border: 1.5px solid #eef2f6;
  padding: 4px 12px; border-radius: 8px;
  display: inline-flex; align-items: center;
}
.int-active-bar {
  height: 3px;
  background: linear-gradient(90deg, #10b981, #34d399, #6ee7b7);
}
.btn-disconnect {
  background: white; color: #f43f5e;
  border: 2px solid #fecdd3; border-radius: 12px;
  padding: 10px 20px; font-weight: 800;
  font-size: 0.78rem; cursor: pointer;
  transition: 0.2s; font-family: inherit;
}
.btn-disconnect:hover { background: #fff1f2; border-color: #f43f5e; }

.security-note {
  background: #fffbeb;
  border: 1.5px solid #fde68a;
  border-radius: 14px;
  padding: 14px 18px;
  font-size: 0.78rem;
  color: #92400e;
  font-weight: 600;
  display: flex;
  align-items: center;
}

/* ═══════════════════════════════════════════
   BUTTONS
═══════════════════════════════════════════ */
.btn-enigma-primary {
  background: #0f172a; color: white;
  border: none; border-radius: 16px;
  font-weight: 800; position: relative;
  overflow: hidden; cursor: pointer;
  font-family: inherit; font-size: 0.82rem;
  transition: transform 0.2s;
}
.btn-enigma-primary:hover { transform: translateY(-1px); }
.btn-enigma-primary .btn-glow {
  position: absolute; inset: 0;
  background: linear-gradient(135deg, #f59e0b, #fbbf24);
  opacity: 0; transition: 0.3s; z-index: 1;
}
.btn-enigma-primary:hover .btn-glow { opacity: 1; }
.btn-enigma-primary .btn-content {
  position: relative; z-index: 2;
  display: flex; align-items: center; justify-content: center;
}
.btn-enigma-primary:hover .btn-content { color: #0f172a; }
.btn-enigma-primary:disabled { opacity: 0.5; cursor: not-allowed; transform: none; }
.btn-ghost-action {
  background: transparent; border: none;
  color: #94a3b8; font-weight: 800;
  font-size: 0.78rem; cursor: pointer;
  font-family: inherit; padding: 12px 20px;
  border-radius: 12px; transition: 0.2s;
}
.btn-ghost-action:hover { background: #f8fafc; color: #0f172a; }

/* ═══════════════════════════════════════════
   FOOTER ACTIONS
═══════════════════════════════════════════ */
.footer-actions {
  display: flex; justify-content: space-between;
  align-items: center; border-top: 1px solid #f1f5f9;
}

/* ═══════════════════════════════════════════
   LOADING
═══════════════════════════════════════════ */
.loading-state {
  display: flex; flex-direction: column;
  align-items: center; justify-content: center;
  min-height: 60vh;
}
.spinner-pro-premium {
  width: 48px; height: 48px;
  border: 4px solid #f1f5f9;
  border-top: 4px solid #f59e0b;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}
@keyframes spin { to { transform: rotate(360deg); } }
.tracking-wider { letter-spacing: 2px; }

/* ═══════════════════════════════════════════
   TOAST
═══════════════════════════════════════════ */
.enigma-toast {
  position: fixed; bottom: 30px; right: 30px;
  background: #0f172a; color: white;
  padding: 18px 28px; border-radius: 18px;
  display: flex; align-items: center; gap: 14px;
  z-index: 3000; border-left: 5px solid #f59e0b;
  box-shadow: 0 20px 40px rgba(0,0,0,0.25);
  min-width: 280px;
}
.t-success { border-left-color: #10b981; }
.t-error   { border-left-color: #f43f5e; }
.t-warn    { border-left-color: #f59e0b; }
.t-ico     { font-size: 1.15rem; }
.t-body strong { font-size: 0.62rem; letter-spacing: 1px; opacity: 0.5; display: block; margin-bottom: 3px; }
.toast-slide-enter-active { animation: slideIn 0.35s cubic-bezier(0.4,0,0.2,1); }
.toast-slide-leave-active { animation: slideIn 0.25s cubic-bezier(0.4,0,0.2,1) reverse; }
@keyframes slideIn { from { transform: translateX(110%); opacity: 0; } to { transform: translateX(0); opacity: 1; } }

/* ═══════════════════════════════════════════
   UTILS
═══════════════════════════════════════════ */
.fw-900 { font-weight: 900 !important; }
.fw-800 { font-weight: 800 !important; }

/* ═══════════════════════════════════════════
   DARK MODE
═══════════════════════════════════════════ */
[data-theme="dark"] .settings-master-root { background: #0d1117; color: #f0f6fc; }
[data-theme="dark"] .nav-panel { background: #161b22; border-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .nav-matrix-btn { color: #8b949e; }
[data-theme="dark"] .nav-matrix-btn:hover { background: rgba(255,255,255,0.05); color: #f0f6fc; }
[data-theme="dark"] .nav-matrix-btn.active { background: #0d1117; color: #f0f6fc; }
[data-theme="dark"] .icon-shell { background: rgba(255,255,255,0.06); }
[data-theme="dark"] .avatar-img { border-color: rgba(255,255,255,0.1); }
[data-theme="dark"] .nav-panel-header { border-bottom-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .nav-panel-footer { border-top-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .content-panel { background: #161b22; border-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .section-title-bar { border-bottom-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .section-title-bar h5 { color: #f0f6fc; }
[data-theme="dark"] .section-icon-box { background: rgba(245,158,11,0.1); }
[data-theme="dark"] .profile-hero-data h3 { color: #f0f6fc; }
[data-theme="dark"] .hero-badge { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.08); color: #8b949e; }
[data-theme="dark"] .enigma-field { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.08); color: #f0f6fc; }
[data-theme="dark"] .enigma-field:focus { border-color: #f59e0b; background: rgba(255,255,255,0.08); }
[data-theme="dark"] .security-status-widget { background: rgba(245,158,11,0.05); border-color: rgba(245,158,11,0.2); }
[data-theme="dark"] .password-strength-analyzer { background: rgba(255,255,255,0.03); border-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .protocol-row { background: rgba(255,255,255,0.03); border-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .p-data h6 { color: #f0f6fc; }
[data-theme="dark"] .color-picker-pro { background: rgba(255,255,255,0.03); border-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .color-hex { color: #f0f6fc; }
[data-theme="dark"] .brand-preview-card { background: rgba(255,255,255,0.03); border-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .brand-preview-ui { border-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .bp-body { background: #0d1117; }
[data-theme="dark"] .integration-card { background: #161b22; border-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .integration-card.connected { background: rgba(16,185,129,0.07); border-color: rgba(16,185,129,0.2); }
[data-theme="dark"] .int-icon-shell { background: #0d1117; border-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .int-email-chip { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .int-stat-card { background: rgba(255,255,255,0.03); border-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .int-stat-value { color: #f0f6fc; }
[data-theme="dark"] .footer-actions { border-top-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .autosave-indicator { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .premium-title { color: #f0f6fc; }
[data-theme="dark"] .breadcrumb-pro .current { color: #f0f6fc; }
[data-theme="dark"] .avatar-info h6 { color: #f0f6fc; }
[data-theme="dark"] .security-note { background: rgba(245,158,11,0.07); border-color: rgba(245,158,11,0.2); color: #fde68a; }
[data-theme="dark"] .profile-hero-card { border-color: rgba(255,255,255,0.06); }
</style>