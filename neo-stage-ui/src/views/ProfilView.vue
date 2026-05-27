<template>
  <div class="enigma-master-root d-flex overflow-hidden" @mousemove="handleParallax">

    <!-- BACKGROUND — identique Campagnes.vue -->
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
        <div class="dashboard-view animate__animated animate__fadeIn p-4 p-lg-5">

          <!-- LOADING -->
          <div v-if="loading" class="text-center py-5">
            <div class="spinner-pro-premium"></div>
            <p class="fw-700 text-muted mt-3" style="font-size:0.85rem;letter-spacing:1px;">CHARGEMENT DU PROFIL...</p>
          </div>

          <div v-else>

            <!-- ══════════════════════════════════════
                 HEADER
            ══════════════════════════════════════ -->
            <header class="d-flex justify-content-between align-items-end mb-5 flex-wrap gap-3">
              <div>
                <div class="breadcrumb-pro mb-2">
                  <span class="root">Tableau de bord</span>
                  <i class="fa-solid fa-chevron-right mx-2 separator"></i>
                  <span class="current">Mon Profil</span>
                </div>
                <h2 class="premium-title">Mon <span class="gradient-text">Profil</span></h2>
                <p class="brand-subtitle">GÉREZ VOS INFORMATIONS ET VOTRE PRÉSENCE</p>
              </div>
              <div class="d-flex gap-3 flex-wrap">
                <button class="btn-outline-pro" @click="goToSettings">
                  <i class="fa-solid fa-gear me-2"></i>Paramètres
                </button>
                <!-- BOUTON PARTAGER FONCTIONNEL -->
                <button class="btn-enigma-primary shadow-premium" @click="shareProfile">
                  <div class="btn-content">
                    <i class="fa-solid fa-share-nodes me-2"></i>Partager
                  </div>
                  <div class="btn-glow"></div>
                </button>
              </div>
            </header>

            <!-- ══════════════════════════════════════
                 PROFILE GRID
            ══════════════════════════════════════ -->
            <div class="profile-grid">

              <!-- ─── COLONNE GAUCHE ─── -->
              <div class="profile-left">

                <!-- IDENTITY CARD -->
                <div class="enigma-card identity-card overflow-hidden">
                  <!-- Banner -->
                  <div class="identity-banner">
                    <div class="banner-shine"></div>
                  </div>

                  <!-- Body -->
                  <div class="identity-body">
                    <div class="avatar-wrap" @click="triggerFileInput">
                      <img :src="profileDisplayUrl" class="avatar-img" :alt="user.prenom + ' ' + user.nom">
                      <div class="avatar-overlay">
                        <i class="fa-solid fa-camera"></i>
                        <span>{{ t('profile.changePhoto') }}</span>
                      </div>
                      <div class="avatar-status-dot"></div>
                      <input type="file" ref="fileInput" @change="onFileChange" hidden accept="image/*">
                    </div>

                    <h4 class="identity-name">{{ user.prenom }} {{ user.nom }}</h4>

                    <div class="identity-role-badge">
                      <i class="fa-solid fa-circle-check me-1"></i>{{ roleDisplay }}
                    </div>

                    <div class="identity-divider"></div>

                    <div class="identity-stats-list">
                      <div class="id-stat-row">
                        <div class="id-stat-icon-box" style="background:#fffbeb; color:#f59e0b;">
                          <i class="fa-regular fa-calendar-check"></i>
                        </div>
                        <div>
                          <div class="id-stat-label">Membre depuis</div>
                          <div class="id-stat-value">{{ user.joinDate || '—' }}</div>
                        </div>
                      </div>
                      <div class="id-stat-row" v-if="user.entrepriseNom && authStore.role !== 'Candidat'">
                        <div class="id-stat-icon-box" style="background:#eef2ff; color:#6366f1;">
                          <i class="fa-solid fa-building"></i>
                        </div>
                        <div>
                          <div class="id-stat-label">Organisation</div>
                          <div class="id-stat-value">{{ user.entrepriseNom }}</div>
                        </div>
                      </div>
                    </div>

                    <div class="identity-actions mt-4">
                      <button class="btn-enigma-primary flex-grow-1" @click="goToSettings">
                        <div class="btn-content"><i class="fa-solid fa-pen me-2"></i>{{ t('profile.edit') }}</div>
                        <div class="btn-glow"></div>
                      </button>
                      <button class="btn-share-round" @click="shareProfile" title="Partager le profil">
                        <i class="fa-solid fa-share-nodes"></i>
                      </button>
                    </div>
                  </div>
                </div>

                <!-- UPLOAD PROGRESS -->
                <div v-if="uploading" class="upload-progress-card mt-3">
                  <div class="up-icon"><i class="fa-solid fa-cloud-arrow-up fa-beat-fade text-amber"></i></div>
                  <div class="flex-grow-1">
                    <div class="up-label">Mise à jour de la photo...</div>
                    <div class="up-track">
                      <div class="up-fill"></div>
                    </div>
                  </div>
                </div>

              </div>

              <!-- ─── COLONNE DROITE ─── -->
              <div class="profile-right">

                <!-- INFO CARD -->
                <div class="enigma-card p-5">
                  <div class="d-flex align-items-center gap-4 pb-4 mb-4" style="border-bottom:1px solid #f1f5f9">
                    <div class="pane-icon-box amber">
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
                      <div v-if="user.email" class="field-verified">
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

                <!--  SUBSCRIPTION CARD (Uniquement pour AdminEntreprise) -->
                <div v-if="authStore.role === 'AdminEntreprise' && user.subscriptionPlan === 'EvaluaTech Go'" class="enigma-card p-5 border-amber shadow-premium">
                  <div class="d-flex align-items-center gap-4 pb-4 mb-4" style="border-bottom:1px solid #f1f5f9">
                    <div class="pane-icon-box amber-glow">
                      <i class="fa-solid fa-crown"></i>
                    </div>
                    <div class="flex-grow-1">
                      <h6 class="fw-900 m-0">{{ t('profile.subscriptionTitle') }}</h6>
                      <p class="text-muted small m-0">{{ t('profile.subscriptionSub') }}</p>
                    </div>
                    <div class="ms-auto">
                      <span v-if="daysRemaining > 7" class="status-badge-v2 success">
                        <i class="fa-solid fa-check-circle me-1"></i> {{ t('profile.statusActive') }}
                      </span>
                      <span v-else-if="daysRemaining > 0" class="status-badge-v2 warning">
                        <i class="fa-solid fa-clock me-1"></i> {{ t('profile.expiresIn', { days: daysRemaining }) }}
                      </span>
                      <span v-else class="status-badge-v2 danger">
                        <i class="fa-solid fa-triangle-exclamation me-1"></i> {{ t('profile.subscriptionExpired') }}
                      </span>
                    </div>
                  </div>

                  <div class="info-fields-grid mb-4">
                    <div class="info-field-item">
                      <div class="field-icon-box" style="background:#fffbeb; color:#f59e0b;">
                        <i class="fa-solid fa-gem"></i>
                      </div>
                      <div class="field-content">
                        <div class="field-label">{{ t('profile.currentPlan') }}</div>
                        <div class="field-value fw-900 text-amber">{{ user.subscriptionPlan || 'Starter' }}</div>
                      </div>
                    </div>

                    <div class="info-field-item">
                      <div class="field-icon-box" style="background:#f0f9ff; color:#0ea5e9;">
                        <i class="fa-solid fa-calendar-day"></i>
                      </div>
                      <div class="field-content">
                        <div class="field-label">{{ t('profile.activationDate') }}</div>
                        <div class="field-value">{{ user.subscriptionDate || '—' }}</div>
                      </div>
                    </div>

                    <div class="info-field-item">
                      <div class="field-icon-box" style="background:#fef2f2; color:#ef4444;">
                        <i class="fa-solid fa-hourglass-end"></i>
                      </div>
                      <div class="field-content">
                        <div class="field-label">{{ t('profile.expirationDate') }}</div>
                        <div class="field-value">{{ user.subscriptionExpiry || '—' }}</div>
                      </div>
                    </div>

                    <div class="info-field-item">
                      <div class="field-icon-box" :style="{ background: daysRemaining > 7 ? '#ecfdf5' : '#fff7ed', color: daysRemaining > 7 ? '#10b981' : '#f97316' }">
                        <i class="fa-solid fa-clock-rotate-left"></i>
                      </div>
                      <div class="field-content">
                        <div class="field-label">{{ t('profile.timeLeft') }}</div>
                        <div class="field-value" :class="{ 'text-warning fw-900': daysRemaining <= 7 }">
                          {{ daysRemaining > 0 ? daysRemaining + ' ' + t('profile.days') : t('profile.finished') }}
                        </div>
                      </div>
                    </div>
                  </div>

                  <!-- ACTIONS ABONNEMENT -->
                  <div class="d-flex gap-3">
                    <button v-if="daysRemaining <= 7 && daysRemaining > 0" class="btn-enigma-primary flex-grow-1 shadow-amber" @click="router.push('/pricing')">
                      <div class="btn-content"><i class="fa-solid fa-arrows-rotate me-2"></i>{{ t('profile.renewNow') }}</div>
                      <div class="btn-glow"></div>
                    </button>
                    <button v-if="daysRemaining <= 0" class="btn-enigma-primary flex-grow-1 shadow-danger" @click="router.push('/pricing')" style="background:#ef4444">
                      <div class="btn-content"><i class="fa-solid fa-power-off me-2"></i>{{ t('profile.reactivate') }}</div>
                      <div class="btn-glow"></div>
                    </button>
                  </div>
                </div>

                <!-- BIO CARD -->
                <div class="enigma-card p-5" v-if="user.bio">
                  <div class="d-flex align-items-center gap-4 pb-4 mb-4" style="border-bottom:1px solid #f1f5f9">
                    <div class="pane-icon-box indigo">
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
                <div class="enigma-card p-5">
                  <div class="d-flex align-items-center gap-2 mb-4">
                    <i class="fa-solid fa-bolt-lightning text-amber"></i>
                    <span class="fw-900" style="font-size:0.8rem;letter-spacing:0.5px;">ACTIONS RAPIDES</span>
                  </div>
                  <div class="d-flex flex-column gap-2">

                    <button class="btn-quick-action" @click="goToSettings">
                      <div class="qa-icon-box" style="background:#fffbeb; color:#f59e0b;">
                        <i class="fa-solid fa-user-pen"></i>
                      </div>
                      <span>Modifier le profil</span>
                      <i class="fa-solid fa-chevron-right qa-arrow ms-auto"></i>
                    </button>

                    <button class="btn-quick-action" @click="goToSettings">
                      <div class="qa-icon-box" style="background:#f0f9ff; color:#0ea5e9;">
                        <i class="fa-solid fa-lock"></i>
                      </div>
                      <span>Changer le mot de passe</span>
                      <i class="fa-solid fa-chevron-right qa-arrow ms-auto"></i>
                    </button>

                    <button class="btn-quick-action" @click="triggerFileInput">
                      <div class="qa-icon-box" style="background:#ecfdf5; color:#10b981;">
                        <i class="fa-solid fa-camera"></i>
                      </div>
                      <span>Changer la photo</span>
                      <i class="fa-solid fa-chevron-right qa-arrow ms-auto"></i>
                    </button>

                    <!-- PARTAGER DANS QUICK ACTIONS -->
                    <button class="btn-quick-action" @click="shareProfile">
                      <div class="qa-icon-box" style="background:#eef2ff; color:#6366f1;">
                        <i class="fa-solid fa-share-nodes"></i>
                      </div>
                      <span>Partager mon profil</span>
                      <i class="fa-solid fa-chevron-right qa-arrow ms-auto"></i>
                    </button>

                  </div>
                </div>

              </div>
            </div>

          </div>
        </div>
      </main>
    </div>

    <!-- ══════════════════════════════════════
         SHARE MODAL
    ══════════════════════════════════════ -->
    <transition name="modal-quantum">
      <div v-if="shareModal.show" class="quantum-vault-overlay" @click.self="shareModal.show = false">
        <div class="share-modal animate__animated animate__zoomIn animate__faster">
          <div class="d-flex justify-content-between align-items-center mb-4">
            <div class="d-flex align-items-center gap-3">
              <div class="pane-icon-box indigo"><i class="fa-solid fa-share-nodes"></i></div>
              <div>
                <h5 class="fw-900 m-0">Partager le Profil</h5>
                <p class="text-muted small m-0">Choisissez votre méthode de partage</p>
              </div>
            </div>
            <button @click="shareModal.show = false" class="btn-icon-sm">
              <i class="fa-solid fa-xmark"></i>
            </button>
          </div>

          <!-- URL preview -->
          <div class="share-url-box mb-4">
            <span class="share-url-text text-truncate">{{ shareModal.url }}</span>
            <button class="btn-copy-url" @click="copyToClipboard" :class="{ copied: shareModal.copied }">
              <i :class="shareModal.copied ? 'fa-solid fa-check' : 'fa-regular fa-copy'"></i>
              {{ shareModal.copied ? 'Copié !' : 'Copier' }}
            </button>
          </div>

          <!-- Share channels -->
          <div class="share-channels-grid">
            <button class="share-channel-btn" @click="shareVia('native')" v-if="canNativeShare">
              <div class="sc-icon" style="background:#fffbeb; color:#f59e0b;"><i class="fa-solid fa-share-from-square"></i></div>
              <span>Partager via</span>
            </button>
            <button class="share-channel-btn" @click="shareVia('email')">
              <div class="sc-icon" style="background:#eef2ff; color:#6366f1;"><i class="fa-solid fa-envelope"></i></div>
              <span>Email</span>
            </button>
            <button class="share-channel-btn" @click="shareVia('linkedin')">
              <div class="sc-icon" style="background:#f0f9ff; color:#0a66c2;"><i class="fa-brands fa-linkedin-in"></i></div>
              <span>LinkedIn</span>
            </button>
            <button class="share-channel-btn" @click="shareVia('whatsapp')">
              <div class="sc-icon" style="background:#ecfdf5; color:#25D366;"><i class="fa-brands fa-whatsapp"></i></div>
              <span>WhatsApp</span>
            </button>
            <button class="share-channel-btn" @click="shareVia('twitter')">
              <div class="sc-icon" style="background:#f8fafc; color:#000000;"><i class="fa-brands fa-x-twitter"></i></div>
              <span>X / Twitter</span>
            </button>
            <button class="share-channel-btn" @click="shareVia('teams')">
              <div class="sc-icon" style="background:#f0f4ff; color:#6264A7;"><i class="fa-brands fa-microsoft"></i></div>
              <span>Teams</span>
            </button>
          </div>

          <div class="mt-4 text-center">
            <p class="text-muted small">
              <i class="fa-solid fa-shield-halved me-1 text-amber"></i>
              Seules vos informations publiques seront partagées.
            </p>
          </div>
        </div>
      </div>
    </transition>

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
import { useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import api from '@/services/api';
import { useAuthStore } from '@/stores/auth';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';

const { t }       = useI18n();
const router      = useRouter();
const authStore   = useAuthStore();
const fileInput   = ref(null);

/* ── State ── */
const user      = ref({ nom: '', prenom: '', email: '', photoUrl: '', joinDate: '', entrepriseNom: '', bio: '', subscriptionPlan: '', subscriptionDate: '', subscriptionExpiry: '' });
const loading   = ref(true);
const uploading = ref(false);
const mousePos  = reactive({ x: 0, y: 0 });
const globalToast = reactive({ active: false, message: '', type: '', icon: '' });

/* ── Share modal ── */
const shareModal = reactive({
  show:   false,
  url:    '',
  copied: false,
});
const canNativeShare = computed(() => !!navigator.share);

/* ── Computed ── */
const profileDisplayUrl = computed(() => {
  if (user.value.photoUrl) return `http://localhost:5172${user.value.photoUrl}`;
  return `https://ui-avatars.com/api/?name=${encodeURIComponent(user.value.prenom + ' ' + user.value.nom)}&background=0f172a&color=eab308&size=256&bold=true`;
});

const roleDisplay = computed(() => {
  if (!authStore.role) return '...';
  return t(`roles.${authStore.role}`, authStore.role);
});

const daysRemaining = computed(() => {
  if (!user.value.subscriptionExpiry) return 0;
  
  // Parse "dd/MM/yyyy"
  const parts = user.value.subscriptionExpiry.split('/');
  if (parts.length === 3) {
    const day = parseInt(parts[0], 10);
    const month = parseInt(parts[1], 10) - 1; // Months are 0-indexed in JS
    const year = parseInt(parts[2], 10);
    const expiry = new Date(year, month, day);
    const now = new Date();
    const diff = expiry.getTime() - now.getTime();
    return Math.max(0, Math.ceil(diff / (1000 * 60 * 60 * 24)));
  }
  
  // Fallback
  const expiry = new Date(user.value.subscriptionExpiry);
  const now    = new Date();
  const diff   = expiry.getTime() - now.getTime();
  return Math.max(0, Math.ceil(diff / (1000 * 60 * 60 * 24)));
});

/* ── API ── */
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

/* ── Avatar upload ── */
const triggerFileInput = () => fileInput.value.click();

const onFileChange = async (e) => {
  const file = e.target.files[0];
  if (!file) return;
  const formData = new FormData();
  formData.append('file', file);
  try {
    uploading.value = true;
    const res = await api.post('/Settings/upload-photo', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    user.value.photoUrl = res.data.photoUrl;
    authStore.user.photoUrl = res.data.photoUrl;
    localStorage.setItem('user', JSON.stringify(authStore.user));
    showPulseToast('Photo mise à jour avec succès !', 'success', 'fa-solid fa-camera');
  } catch (error) {
    console.error('Erreur upload :', error);
    showPulseToast(t('profile.uploadError'), 'error', 'fa-solid fa-triangle-exclamation');
  } finally {
    uploading.value = false;
  }
};

/* ── Navigation ── */
const goToSettings = () => router.push('/settings');

/* ── Share fonctionnel ── */
const buildShareUrl = () => {
  const base = window.location.origin;
  const id   = authStore.user?.id || '';
  return id ? `${base}/profil/${id}` : base;
};

const shareProfile = async () => {
  shareModal.url    = buildShareUrl();
  shareModal.copied = false;

  // Tentative Web Share API native (mobile / desktop support)
  if (navigator.share) {
    try {
      await navigator.share({
        title: `Profil de ${user.value.prenom} ${user.value.nom}`,
        text:  `Découvrez le profil de ${user.value.prenom} ${user.value.nom} sur EvaluaArchitect.`,
        url:   shareModal.url,
      });
      showPulseToast('Profil partagé avec succès !', 'success', 'fa-solid fa-share-nodes');
      return;
    } catch (err) {
      // L'utilisateur a annulé → on ouvre la modale de fallback
      if (err.name === 'AbortError') return;
    }
  }

  // Fallback : modale avec options
  shareModal.show = true;
};

const copyToClipboard = async () => {
  try {
    await navigator.clipboard.writeText(shareModal.url);
    shareModal.copied = true;
    showPulseToast('Lien copié dans le presse-papiers !', 'success', 'fa-regular fa-copy');
    setTimeout(() => { shareModal.copied = false; }, 2500);
  } catch {
    // Fallback execCommand
    const ta = document.createElement('textarea');
    ta.value = shareModal.url;
    document.body.appendChild(ta);
    ta.select();
    document.execCommand('copy');
    document.body.removeChild(ta);
    shareModal.copied = true;
    showPulseToast('Lien copié !', 'success', 'fa-regular fa-copy');
    setTimeout(() => { shareModal.copied = false; }, 2500);
  }
};

const shareVia = (platform) => {
  const url  = encodeURIComponent(shareModal.url);
  const name = encodeURIComponent(`${user.value.prenom} ${user.value.nom}`);
  const text = encodeURIComponent(`Découvrez le profil de ${user.value.prenom} ${user.value.nom} sur EvaluaArchitect.`);

  const targets = {
    email:    `mailto:?subject=Profil%20de%20${name}&body=${text}%0A${url}`,
    linkedin: `https://www.linkedin.com/sharing/share-offsite/?url=${url}`,
    whatsapp: `https://wa.me/?text=${text}%20${url}`,
    twitter:  `https://twitter.com/intent/tweet?text=${text}&url=${url}`,
    teams:    `https://teams.microsoft.com/share?href=${url}&msgText=${text}`,
  };

  if (platform === 'native') {
    shareProfile();
    return;
  }

  if (targets[platform]) {
    window.open(targets[platform], '_blank', 'noopener,noreferrer');
    shareModal.show = false;
    showPulseToast(`Ouverture de ${platform}...`, 'success', 'fa-solid fa-share-nodes');
  }
};

/* ── Toast ── */
let _toastTimer = null;
const showPulseToast = (msg, type = 'success', icon = 'fa-solid fa-check') => {
  clearTimeout(_toastTimer);
  Object.assign(globalToast, { message: msg, type: `t-${type}`, icon, active: true });
  _toastTimer = setTimeout(() => { globalToast.active = false; }, 4000);
};

/* ── Parallax ── */
const orbStyle       = (f) => ({ transform: `translate(${mousePos.x * f * 10}px, ${mousePos.y * f * 10}px)` });
const handleParallax = (e) => {
  mousePos.x = (e.clientX - window.innerWidth  / 2) / 20;
  mousePos.y = (e.clientY - window.innerHeight / 2) / 20;
};
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;700;800;900&display=swap');

/* ════════════════════════════════════════
   BASE — identique Campagnes.vue
════════════════════════════════════════ */
.enigma-master-root {
  min-height: 100vh;
  background: #f8fafc;
  font-family: 'Plus Jakarta Sans', sans-serif;
  color: #0f172a;
}

/* BACKGROUND */
.cyber-engine-bg { position: fixed; inset: 0; z-index: 0; pointer-events: none; }
.quantum-grid {
  position: absolute; inset: 0;
  background-image: radial-gradient(#cbd5e1 1px, transparent 1px);
  background-size: 40px 40px; opacity: 0.2;
}
.glow-orb {
  position: absolute; width: 600px; height: 600px;
  filter: blur(120px); opacity: 0.15; border-radius: 50%;
  transition: transform 0.3s ease-out;
}
.orb-amber { background: #f59e0b; top: -200px; right: -100px; }
.orb-blue  { background: #6366f1; bottom: -200px; left: -100px; }
.main-orchestrator { z-index: 5; }
.canvas-engine { height: calc(100vh - 64px); }

/* SCROLLBAR */
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: transparent; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: #eef2f6; border-radius: 10px; }

/* ════════════════════════════════════════
   HEADER
════════════════════════════════════════ */
.premium-title { font-weight: 800; font-size: 2.2rem; letter-spacing: -1px; }
.gradient-text {
  background: linear-gradient(135deg, #f59e0b 0%, #fbbf24 100%);
  -webkit-background-clip: text; -webkit-text-fill-color: transparent;
}
.brand-subtitle { font-size: 0.6rem; font-weight: 800; color: #94a3b8; letter-spacing: 2px; margin-top: 4px; }
.breadcrumb-pro { font-size: 0.72rem; font-weight: 700; color: #94a3b8; }
.breadcrumb-pro .separator { font-size: 0.55rem; opacity: 0.5; }
.breadcrumb-pro .current  { color: #0f172a; font-weight: 800; }

/* ════════════════════════════════════════
   BUTTONS — copie exacte Campagnes.vue
════════════════════════════════════════ */
.btn-enigma-primary {
  background: #0f172a; color: white; border: none;
  padding: 14px 26px; border-radius: 18px; font-weight: 800;
  position: relative; overflow: hidden; cursor: pointer; font-family: inherit;
}
.btn-enigma-primary .btn-glow {
  position: absolute; inset: 0;
  background: linear-gradient(135deg, #f59e0b, #fbbf24);
  opacity: 0; transition: 0.3s; z-index: 1;
}
.btn-enigma-primary:hover .btn-glow { opacity: 1; }
.btn-enigma-primary .btn-content { position: relative; z-index: 2; display: flex; align-items: center; }
.btn-enigma-primary:hover .btn-content { color: #0f172a; }
.shadow-premium { box-shadow: 0 20px 60px rgba(0,0,0,0.12) !important; }

.btn-outline-pro {
  background: white; color: #0f172a;
  border: 1.5px solid #eef2f6; padding: 12px 22px;
  border-radius: 16px; font-weight: 800; font-size: 0.85rem;
  cursor: pointer; transition: 0.2s; font-family: inherit;
}
.btn-outline-pro:hover { border-color: #f59e0b; color: #f59e0b; background: #fffbeb; }

.btn-icon-sm {
  width: 34px; height: 34px; border-radius: 10px;
  border: 1.5px solid #eef2f6; background: white;
  color: #64748b; cursor: pointer; transition: 0.2s;
  font-size: 0.8rem; display: flex; align-items: center; justify-content: center;
}
.btn-icon-sm:hover { background: #f8fafc; color: #0f172a; }

.btn-quick-action {
  display: flex; align-items: center; gap: 14px;
  width: 100%; background: #f8fafc; border: 1.5px solid #eef2f6;
  border-radius: 16px; padding: 13px 16px; cursor: pointer;
  font-family: inherit; font-size: 0.82rem; font-weight: 800; color: #374151;
  transition: 0.2s; text-align: left;
}
.btn-quick-action:hover { border-color: #f59e0b; background: #fffbeb; color: #0f172a; }
.qa-icon-box {
  width: 36px; height: 36px; border-radius: 10px;
  display: flex; align-items: center; justify-content: center;
  font-size: 0.85rem; flex-shrink: 0;
}
.qa-arrow { font-size: 0.6rem; color: #cbd5e1; transition: 0.2s; }
.btn-quick-action:hover .qa-arrow { color: #f59e0b; transform: translateX(4px); }

/* ════════════════════════════════════════
   ENIGMA CARD
════════════════════════════════════════ */
.enigma-card { background: white; border-radius: 32px; border: 1px solid #eef2f6; }

/* ════════════════════════════════════════
   PROFILE GRID
════════════════════════════════════════ */
.profile-grid {
  display: grid;
  grid-template-columns: 300px 1fr;
  gap: 20px;
  align-items: start;
}
@media (max-width: 960px) { .profile-grid { grid-template-columns: 1fr; } }
.profile-right { display: flex; flex-direction: column; gap: 18px; }

/* ════════════════════════════════════════
   IDENTITY CARD
════════════════════════════════════════ */
.identity-card { overflow: hidden; }
.identity-banner {
  height: 90px;
  background: linear-gradient(135deg, #0f172a 0%, #1e293b 60%, #0f172a 100%);
  position: relative; overflow: hidden;
}
.identity-banner::before {
  content: '';
  position: absolute; inset: 0;
  background: radial-gradient(circle at 65% 50%, rgba(245,158,11,0.2), transparent 55%);
}
.banner-shine {
  position: absolute; top: -40%; left: -40%;
  width: 80%; height: 180%;
  background: linear-gradient(105deg, transparent 30%, rgba(255,255,255,0.04) 50%, transparent 70%);
  transform: rotate(15deg);
}
.identity-body {
  padding: 0 28px 28px;
  display: flex; flex-direction: column; align-items: center; text-align: center;
}

/* AVATAR */
.avatar-wrap {
  position: relative; width: 88px; height: 88px;
  cursor: pointer; border-radius: 22px; overflow: hidden;
  border: 4px solid white;
  box-shadow: 0 8px 24px rgba(0,0,0,0.12);
  margin-top: -30px; margin-bottom: 16px;
}
.avatar-img { width: 100%; height: 100%; object-fit: cover; display: block; }
.avatar-overlay {
  position: absolute; inset: 0;
  background: rgba(15,23,42,0.80);
  display: flex; flex-direction: column; align-items: center; justify-content: center;
  color: white; font-size: 0.6rem; font-weight: 800; text-transform: uppercase;
  opacity: 0; transition: 0.25s; gap: 4px;
}
.avatar-overlay i { font-size: 1.1rem; }
.avatar-wrap:hover .avatar-overlay { opacity: 1; }
.avatar-status-dot {
  position: absolute; bottom: 5px; right: 5px;
  width: 13px; height: 13px; border-radius: 50%;
  background: #10b981; border: 2px solid white;
}

.identity-name { font-weight: 900; font-size: 1.15rem; color: #0f172a; margin: 0 0 8px; }
.identity-role-badge {
  font-size: 0.7rem; font-weight: 800;
  background: #fffbeb; color: #d97706;
  padding: 5px 14px; border-radius: 10px;
  border: 1.5px solid #fde68a;
  display: inline-flex; align-items: center; gap: 5px;
}
.identity-divider { width: 100%; height: 1px; background: #f1f5f9; margin: 20px 0; }

.identity-stats-list { width: 100%; display: flex; flex-direction: column; gap: 12px; }
.id-stat-row { display: flex; align-items: center; gap: 12px; text-align: left; }
.id-stat-icon-box {
  width: 36px; height: 36px; border-radius: 10px;
  display: flex; align-items: center; justify-content: center;
  font-size: 0.85rem; flex-shrink: 0;
}
.id-stat-label { font-size: 0.58rem; font-weight: 700; color: #94a3b8; text-transform: uppercase; letter-spacing: 0.5px; }
.id-stat-value { font-size: 0.82rem; font-weight: 800; color: #0f172a; }

.identity-actions { display: flex; gap: 10px; width: 100%; }
.btn-share-round {
  width: 50px; height: 50px; min-width: 50px;
  background: #f8fafc; border: 1.5px solid #eef2f6;
  border-radius: 16px; color: #64748b;
  cursor: pointer; font-size: 0.95rem;
  transition: 0.2s; display: flex; align-items: center; justify-content: center;
}
.btn-share-round:hover { border-color: #6366f1; color: #6366f1; background: #eef2ff; }

/* UPLOAD PROGRESS */
.upload-progress-card {
  background: #fffbeb; border: 1.5px solid #fde68a;
  border-radius: 20px; padding: 16px 20px;
  display: flex; align-items: center; gap: 14px;
}
.up-label { font-size: 0.72rem; font-weight: 800; color: #92400e; margin-bottom: 8px; }
.up-track { height: 4px; background: #fde68a; border-radius: 10px; overflow: hidden; }
.up-fill {
  height: 100%; width: 60%; background: #f59e0b; border-radius: 10px;
  animation: progressAnim 1.5s ease-in-out infinite alternate;
}
@keyframes progressAnim { from { width: 20%; } to { width: 92%; } }

/* ════════════════════════════════════════
   PANE ICONS
════════════════════════════════════════ */
.pane-icon-box {
  width: 46px; height: 46px; border-radius: 14px;
  display: flex; align-items: center; justify-content: center;
  font-size: 1.1rem; flex-shrink: 0;
}
.pane-icon-box.amber  { background: #fffbeb; color: #f59e0b; }
.pane-icon-box.indigo { background: #eef2ff; color: #6366f1; }
.pane-icon-box.green  { background: #ecfdf5; color: #10b981; }
.pane-icon-box.amber-glow {
  background: #fffbeb; color: #f59e0b;
  box-shadow: 0 0 20px rgba(245,158,11,0.2);
  border: 1px solid rgba(245,158,11,0.2);
}

/* SUBSCRIPTION SPECIFIC */
.border-amber { border-color: #fde68a !important; }
.shadow-amber { box-shadow: 0 15px 40px rgba(245,158,11,0.15) !important; }
.shadow-danger { box-shadow: 0 15px 40px rgba(239,68,68,0.2) !important; }

.status-badge-v2 {
  font-size: 0.65rem; font-weight: 800;
  padding: 6px 14px; border-radius: 12px;
  display: inline-flex; align-items: center; gap: 6px;
  letter-spacing: 0.5px;
}
.status-badge-v2.success { background: #ecfdf5; color: #10b981; border: 1px solid #d1fae5; }
.status-badge-v2.warning { background: #fff7ed; color: #f97316; border: 1px solid #ffedd5; animation: pulse-warning 2s infinite; }
.status-badge-v2.danger  { background: #fef2f2; color: #ef4444; border: 1px solid #fee2e2; }

@keyframes pulse-warning {
  0% { box-shadow: 0 0 0 0 rgba(249,115,22,0.4); }
  70% { box-shadow: 0 0 0 10px rgba(249,115,22,0); }
  100% { box-shadow: 0 0 0 0 rgba(249,115,22,0); }
}

.text-amber { color: #d97706 !important; }
.text-warning { color: #f97316 !important; }

/* LIVE BADGE */
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

/* ════════════════════════════════════════
   INFO FIELDS
════════════════════════════════════════ */
.info-fields-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; }
@media (max-width: 600px) { .info-fields-grid { grid-template-columns: 1fr; } }

.info-field-item {
  display: flex; align-items: center; gap: 14px;
  background: #f8fafc; border-radius: 16px; padding: 14px 16px;
  border: 1.5px solid #eef2f6; transition: 0.2s; position: relative;
}
.info-field-item:hover { border-color: #fde68a; background: #fffbeb; }
.field-icon-box {
  width: 38px; height: 38px; border-radius: 12px;
  display: flex; align-items: center; justify-content: center;
  font-size: 0.9rem; flex-shrink: 0;
}
.field-label { font-size: 0.58rem; font-weight: 900; color: #94a3b8; letter-spacing: 0.8px; text-transform: uppercase; margin-bottom: 3px; }
.field-value { font-size: 0.85rem; font-weight: 800; color: #0f172a; }
.field-verified { position: absolute; top: 10px; right: 12px; color: #10b981; font-size: 0.75rem; }

/* BIO */
.bio-text {
  font-size: 0.9rem; color: #374151; font-weight: 600;
  line-height: 1.7; white-space: pre-line;
  border-left: 3px solid #f59e0b; padding-left: 18px;
}

/* ════════════════════════════════════════
   SHARE MODAL
════════════════════════════════════════ */
.quantum-vault-overlay {
  position: fixed; inset: 0;
  background: rgba(15,23,42,0.6); backdrop-filter: blur(10px);
  z-index: 2000; display: flex; align-items: center; justify-content: center;
}
.share-modal {
  background: white; border-radius: 32px; padding: 36px;
  width: 500px; max-width: 95vw;
  box-shadow: 0 40px 80px rgba(0,0,0,0.18);
}

.share-url-box {
  display: flex; align-items: center; gap: 10px;
  background: #f8fafc; border: 1.5px solid #eef2f6;
  border-radius: 16px; padding: 12px 16px; overflow: hidden;
}
.share-url-text {
  flex: 1; font-size: 0.78rem; font-weight: 700;
  color: #64748b; overflow: hidden; text-overflow: ellipsis; white-space: nowrap;
  font-family: monospace;
}
.btn-copy-url {
  flex-shrink: 0; background: #0f172a; color: white;
  border: none; border-radius: 10px; padding: 8px 14px;
  font-size: 0.72rem; font-weight: 800; cursor: pointer;
  font-family: inherit; transition: 0.2s;
  display: flex; align-items: center; gap: 6px;
}
.btn-copy-url.copied { background: #10b981; }
.btn-copy-url:hover:not(.copied) { background: #f59e0b; color: #0f172a; }

.share-channels-grid {
  display: grid; grid-template-columns: repeat(3, 1fr); gap: 10px;
}
.share-channel-btn {
  display: flex; flex-direction: column; align-items: center; gap: 8px;
  background: #f8fafc; border: 1.5px solid #eef2f6;
  border-radius: 16px; padding: 16px 10px;
  cursor: pointer; font-family: inherit;
  font-size: 0.72rem; font-weight: 800; color: #475569;
  transition: 0.2s;
}
.share-channel-btn:hover { border-color: #f59e0b; background: #fffbeb; color: #0f172a; transform: translateY(-2px); }
.sc-icon {
  width: 42px; height: 42px; border-radius: 14px;
  display: flex; align-items: center; justify-content: center; font-size: 1.1rem;
}

/* ════════════════════════════════════════
   TOAST — identique Campagnes.vue
════════════════════════════════════════ */
.enigma-toast {
  position: fixed; bottom: 30px; right: 30px;
  background: #0f172a; color: white; padding: 20px 30px;
  border-radius: 20px; display: flex; align-items: center; gap: 15px;
  z-index: 3000; border-left: 5px solid #f59e0b;
  box-shadow: 0 20px 40px rgba(0,0,0,0.2);
}
.t-success { border-left-color: #10b981; }
.t-error   { border-left-color: #f43f5e; }
.t-warn    { border-left-color: #f59e0b; }
.toast-slide-enter-active { animation: slideIn 0.4s ease-out; }
.toast-slide-leave-active { animation: slideIn 0.3s ease-in reverse; }
@keyframes slideIn { from { transform: translateX(120%); opacity: 0; } to { transform: translateX(0); opacity: 1; } }

/* TRANSITIONS */
.modal-quantum-enter-active { animation: zoomIn 0.25s ease-out; }
.modal-quantum-leave-active { animation: zoomIn 0.2s ease-in reverse; }
@keyframes zoomIn { from { opacity: 0; transform: scale(0.92); } to { opacity: 1; transform: scale(1); } }

/* ════════════════════════════════════════
   SPINNER
════════════════════════════════════════ */
.spinner-pro-premium {
  width: 48px; height: 48px;
  border: 4px solid #f1f5f9; border-top: 4px solid #f59e0b;
  border-radius: 50%; animation: spin 1s linear infinite; margin: 0 auto;
}
@keyframes spin { to { transform: rotate(360deg); } }

/* MISC */
.fw-800 { font-weight: 800 !important; }
.fw-900 { font-weight: 900 !important; }
.text-amber { color: #f59e0b !important; }

/* ANIMATE COMPAT */
.animate__animated { animation-fill-mode: both; }
.animate__fadeIn { animation: fadeIn 0.4s ease; }
@keyframes fadeIn { from { opacity: 0; transform: translateY(8px); } to { opacity: 1; transform: none; } }
.animate__zoomIn { animation: zoomInModal 0.25s ease-out; }
@keyframes zoomInModal { from { opacity:0; transform:scale(0.92); } to { opacity:1; transform:scale(1); } }
.animate__faster { animation-duration: 0.2s !important; }

/* ════════════════════════════════════════
   DARK MODE — même pattern Campagnes.vue
════════════════════════════════════════ */
[data-theme="dark"] .enigma-master-root { background: #0d1117; color: #f0f6fc; }
[data-theme="dark"] .canvas-engine      { background: #0d1117; }
[data-theme="dark"] .premium-title      { color: #f0f6fc; }
[data-theme="dark"] .brand-subtitle     { color: #8b949e; }
[data-theme="dark"] .breadcrumb-pro .current { color: #f0f6fc; }

[data-theme="dark"] .enigma-card { background: #161b22; border-color: rgba(255,255,255,0.08); }

[data-theme="dark"] .btn-outline-pro { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.1); color: #f0f6fc; }
[data-theme="dark"] .btn-outline-pro:hover { border-color: #d97706; color: #f59e0b; background: rgba(245,158,11,0.08); }
[data-theme="dark"] .btn-icon-sm { background: rgba(255,255,255,0.04); border-color: rgba(255,255,255,0.08); color: #8b949e; }
[data-theme="dark"] .btn-icon-sm:hover { background: rgba(255,255,255,0.08); color: #f0f6fc; }
[data-theme="dark"] .btn-share-round { background: rgba(255,255,255,0.04); border-color: rgba(255,255,255,0.08); color: #8b949e; }
[data-theme="dark"] .btn-share-round:hover { background: rgba(99,102,241,0.1); border-color: rgba(99,102,241,0.3); color: #a5b4fc; }
[data-theme="dark"] .btn-quick-action { background: rgba(255,255,255,0.03); border-color: rgba(255,255,255,0.06); color: #c9d1d9; }
[data-theme="dark"] .btn-quick-action:hover { background: rgba(245,158,11,0.07); border-color: rgba(245,158,11,0.2); color: #f0f6fc; }

[data-theme="dark"] .identity-name  { color: #f0f6fc; }
[data-theme="dark"] .id-stat-value  { color: #f0f6fc; }
[data-theme="dark"] .identity-divider { background: rgba(255,255,255,0.06); }

[data-theme="dark"] .info-field-item { background: rgba(255,255,255,0.03); border-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .info-field-item:hover { background: rgba(245,158,11,0.06); border-color: rgba(245,158,11,0.2); }
[data-theme="dark"] .field-value { color: #f0f6fc; }

[data-theme="dark"] .bio-text { color: #c9d1d9; }

[data-theme="dark"] .share-modal { background: #161b22; border: 1px solid rgba(255,255,255,0.08); }
[data-theme="dark"] .share-modal h5 { color: #f0f6fc; }
[data-theme="dark"] .share-url-box { background: rgba(255,255,255,0.04); border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .share-url-text { color: #8b949e; }
[data-theme="dark"] .share-channel-btn { background: rgba(255,255,255,0.03); border-color: rgba(255,255,255,0.06); color: #c9d1d9; }
[data-theme="dark"] .share-channel-btn:hover { background: rgba(245,158,11,0.07); border-color: rgba(245,158,11,0.2); color: #f0f6fc; }

[data-theme="dark"] .upload-progress-card { background: rgba(245,158,11,0.08); border-color: rgba(245,158,11,0.2); }
[data-theme="dark"] .up-label { color: #fbbf24; }
[data-theme="dark"] .up-track { background: rgba(245,158,11,0.15); }
</style>