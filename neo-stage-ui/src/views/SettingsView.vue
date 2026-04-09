<template>
  <div class="admin-dashboard-container">
    <!-- COUCHES D'ARRIÈRE-PLAN -->
    <div class="background-overlay"></div>
    <div class="glow-orb orb-amber"></div>
    <div class="glow-orb orb-indigo"></div>
    <div class="tech-grid-subtle"></div>

    <div class="d-flex position-relative z-index-10">
      <AppSidebar />

      <div class="content flex-grow-1">
        <AppNavbar />

        <div class="p-4 pt-0 main-viewport animate__animated animate__fadeIn">
          
          <!-- HEADER SECTION -->
          <header class="d-flex justify-content-between align-items-end mb-5 mt-4">
            <div class="header-content">
              <div class="badge-tech mb-2">
                <span class="pulse-dot"></span> PARAMÈTRES DU TERMINAL
              </div>
              <h1 class="display-title">Configuration <span>Système</span></h1>
              <p class="brand-subtitle text-uppercase">Espace {{ roleDisplay }} — Gestion de compte NeoEvaluation</p>
            </div>
          </header>

          <div class="row g-4" v-if="!loading">
            <!-- NAVIGATION LATÉRALE (Bento Style) -->
            <div class="col-lg-3">
              <div class="glass-surface p-3 sticky-top" style="top: 20px; z-index: 10;">
                <div class="settings-nav-matrix">
                  <button v-for="tab in filteredTabs" :key="tab.id" 
                          @click="activeTab = tab.id"
                          :class="['nav-matrix-btn', { active: activeTab === tab.id }]">
                    <div class="icon-shell">
                      <i :class="tab.icon"></i>
                    </div>
                    <span>{{ tab.label.toUpperCase() }}</span>
                  </button>
                </div>
              </div>
            </div>

            <!-- PANNEAU DE CONTENU -->
            <div class="col-lg-9">
              <div class="glass-surface p-5 border-amber-soft shadow-lg">
                
                <!-- SECTION : PROFIL (Adapté à votre modèle Utilisateur) -->
                <div v-if="activeTab === 'profile'" class="settings-section">
                  <h5 class="label-heading mb-4 text-dark">Informations Personnelles</h5>
                  <div class="profile-neural-upload mb-5">
                    <div class="avatar-vessel" @click="triggerPhotoUpload">
                      <img :src="profileDisplayUrl" alt="Avatar">
                      <button class="btn-edit-neural"><i class="fa-solid fa-camera"></i></button>
                      <input type="file" ref="photoInput" @change="handlePhotoChange" hidden accept="image/*">
                    </div>
                    <div class="ms-4">
                      <h4 class="fw-800 m-0 text-dark">{{ userForm.prenom }} {{ userForm.nom }}</h4>
                      <p class="text-slate-500 small m-0">Inscrit depuis {{ userForm.joinDate }}</p>
                    </div>
                  </div>
                  
                  <div class="row g-4">
                    <div class="col-md-6">
                      <label class="cyber-label">Prénom</label>
                      <input type="text" class="cyber-input" v-model="userForm.prenom">
                    </div>
                    <div class="col-md-6">
                      <label class="cyber-label">Nom de famille</label>
                      <input type="text" class="cyber-input" v-model="userForm.nom">
                    </div>
                    <div class="col-md-12">
                      <label class="cyber-label">Email de liaison</label>
                      <input type="email" class="cyber-input" v-model="userForm.email">
                    </div>
                    <div class="col-md-12">
                      <label class="cyber-label">Ma Description / Bio</label>
                      <textarea class="cyber-input" v-model="userForm.bio" rows="4" placeholder="Parlez-nous de vous..."></textarea>
                    </div>
                  </div>
                </div>

                <!-- SECTION : SÉCURITÉ -->
                <div v-if="activeTab === 'security'" class="settings-section">
                  <h5 class="label-heading mb-4 text-dark">Sécurité & Chiffrement</h5>
                  <div class="mb-4">
                    <label class="cyber-label">Mot de passe actuel</label>
                    <input type="password" class="cyber-input" v-model="securityForm.currentPassword" placeholder="••••••••">
                  </div>
                  <div class="row g-4 mb-4">
                    <div class="col-md-6">
                      <label class="cyber-label">Nouveau mot de passe</label>
                      <input type="password" class="cyber-input" v-model="securityForm.newPassword" placeholder="••••••••">
                    </div>
                    <div class="col-md-6">
                      <label class="cyber-label">Confirmer le mot de passe</label>
                      <input type="password" class="cyber-input" v-model="securityForm.confirmPassword" placeholder="••••••••">
                    </div>
                  </div>
                </div>

                <!-- SECTION : BRANDING (ADMIN) -->
                <div v-if="activeTab === 'branding' && role === 'AdminEntreprise'" class="settings-section">
                  <h5 class="label-heading mb-4 text-dark">Identité Visuelle Entreprise</h5>
                  <div class="mb-4">
                    <label class="cyber-label">Raison sociale (Entreprise)</label>
                    <input type="text" class="cyber-input" v-model="brandForm.companyName">
                  </div>
                  <div class="mb-4">
                    <label class="cyber-label">Couleur Signature</label>
                    <div class="d-flex gap-3 align-items-center">
                      <input type="color" class="color-picker-neural" v-model="brandForm.color">
                      <code class="text-indigo fw-800">{{ brandForm.color }}</code>
                    </div>
                  </div>
                </div>

                <!-- FOOTER ACTIONS -->
                <div class="mt-5 pt-4 border-top-light d-flex justify-content-between align-items-center">
                  <button class="btn-link-tech" @click="resetForm">ANNULER LES MODIFICATIONS</button>
                  <button @click="saveChanges" class="btn-primary-gradient px-5 py-3" :disabled="saving">
                    <span v-if="saving"><i class="fa-solid fa-spinner fa-spin me-2"></i>SYNCHRONISATION...</span>
                    <span v-else>SAUVEGARDER <i class="fa-solid fa-cloud-arrow-up ms-2"></i></span>
                  </button>
                </div>

              </div>
            </div>
          </div>

          <!-- ÉTAT DE CHARGEMENT -->
          <div v-else class="text-center py-5">
            <div class="spinner-border text-amber"></div>
            <p class="mt-3 fw-bold text-slate-400 text-uppercase small">Connexion au Neural Core...</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import api from '@/services/api';
import { useAuthStore } from '@/stores/auth';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';

const authStore = useAuthStore();

// États
const loading = ref(true);
const saving = ref(false);
const role = ref(localStorage.getItem('role') || 'AdminEntreprise');
const activeTab = ref('profile');

// État du sélecteur de fichier
const photoInput = ref(null);

// Formulaires (Correspondent aux DTOs Backend)
const userForm = ref({ nom: '', prenom: '', email: '', photoUrl: '', joinDate: '' });
const securityForm = ref({ currentPassword: '', newPassword: '', confirmPassword: '' });
const brandForm = ref({ companyName: '', color: '#eab308' });

const profileDisplayUrl = computed(() => {
  if (userForm.value.photoUrl) {
    return `http://localhost:5172${userForm.value.photoUrl}`;
  }
  return `https://ui-avatars.com/api/?name=${userForm.value.prenom}+${userForm.value.nom}&background=0f172a&color=eab308&size=128`;
});

// Navigation
const allTabs = [
  { id: 'profile', label: 'Profil', icon: 'fa-solid fa-user-gear', roles: ['SuperAdmin', 'AdminEntreprise', 'Formateur', 'Candidat'] },
  { id: 'security', label: 'Sécurité', icon: 'fa-solid fa-shield-halved', roles: ['SuperAdmin', 'AdminEntreprise', 'Formateur', 'Candidat'] },
  { id: 'branding', label: 'Branding', icon: 'fa-solid fa-palette', roles: ['AdminEntreprise'] },
];

const filteredTabs = computed(() => allTabs.filter(tab => tab.roles.includes(role.value)));
const roleDisplay = computed(() => {
  const map = { 'SuperAdmin': 'Master', 'AdminEntreprise': 'Organisation', 'Formateur': 'Evaluateur', 'Candidat': 'Candidat' };
  return map[role.value] || 'User';
});

// --- ACTIONS BACKEND ---

const fetchInitialData = async () => {
  loading.value = true;
  try {
    // 1. Charger le profil
    const resUser = await api.get('/Settings/me');
    userForm.value = resUser.data;

    // 2. Charger le branding si Admin
    if (role.value === 'AdminEntreprise') {
      const resBrand = await api.get('/Settings/branding');
      brandForm.value = resBrand.data;
    }
  } catch (error) {
    console.error("Erreur chargement :", error);
  } finally {
    loading.value = false;
  }
};

const saveChanges = async () => {
  saving.value = true;
  try {
    let endpoint = "";
    let payload = {};

    if (activeTab.value === 'profile') {
      endpoint = `/Settings/update-profile`;
      payload = userForm.value;
    } 
    else if (activeTab.value === 'security') {
      if (securityForm.value.newPassword !== securityForm.value.confirmPassword) {
        alert("Les mots de passe ne correspondent pas.");
        saving.value = false;
        return;
      }
      endpoint = `/Settings/change-password`;
      payload = securityForm.value;
    } 
    else if (activeTab.value === 'branding') {
      endpoint = `/Settings/update-branding`;
      payload = brandForm.value;
    }

    await api.post(endpoint, payload);
    alert("Paramètres synchronisés avec succès !");

    if (activeTab.value === 'profile') {
      authStore.user.name = `${userForm.value.prenom} ${userForm.value.nom}`;
      localStorage.setItem('user', JSON.stringify(authStore.user));
    }
    
    if (activeTab.value === 'security') {
      securityForm.value = { currentPassword: '', newPassword: '', confirmPassword: '' };
    }
  } catch (error) {
    console.error("Erreur sauvegarde :", error);
    alert(error.response?.data?.message || error.response?.data || "Échec de la synchronisation.");
  } finally {
    saving.value = false;
  }
};

const resetForm = () => fetchInitialData();

const triggerPhotoUpload = () => photoInput.value.click();

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
    
    // Synchro avec authStore (si on stocke la photo là aussi)
    authStore.user.photoUrl = res.data.photoUrl;
    localStorage.setItem('user', JSON.stringify(authStore.user));
    
    alert("Photo de profil mise à jour !");
  } catch (error) {
    console.error("Erreur upload :", error);
    alert("Échec de l'upload de l'image.");
  } finally {
    saving.value = false;
  }
};

onMounted(fetchInitialData);
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;600;700;800&display=swap');

.admin-dashboard-container {
  font-family: 'Plus Jakarta Sans', sans-serif;
  min-height: 100vh; background-color: #f8fafc;
  position: relative; overflow-x: hidden;
}

/* --- BACKGROUND DESIGN --- */
.background-overlay { position: absolute; inset: 0; background: radial-gradient(circle at 30% 30%, #ffffff 0%, #f1f5f9 100%); z-index: 0; }
.tech-grid-subtle { position: absolute; inset: 0; background-image: linear-gradient(rgba(148, 163, 184, 0.08) 1px, transparent 1px), linear-gradient(90deg, rgba(148, 163, 184, 0.08) 1px, transparent 1px); background-size: 45px 45px; z-index: 1; }
.glow-orb { position: absolute; border-radius: 50%; filter: blur(120px); z-index: 1; opacity: 0.15; }
.orb-amber { width: 400px; height: 400px; background: #fbbf24; top: -10%; right: -5%; }
.orb-indigo { width: 500px; height: 500px; background: #4f46e5; bottom: -10%; left: -5%; }
.z-index-10 { z-index: 10; }

/* --- TYPOGRAPHY --- */
.display-title { font-weight: 800; color: #0f172a; font-size: 38px; margin: 0; letter-spacing: -1.5px; }
.display-title span { color: #eab308; }
.brand-subtitle { color: #94a3b8; font-size: 11px; font-weight: 700; letter-spacing: 1.5px; }

/* --- SURFACES --- */
.glass-surface {
  background: rgba(255, 255, 255, 0.75);
  backdrop-filter: blur(15px);
  border: 1px solid rgba(226, 232, 240, 0.8);
  border-radius: 24px;
}
.border-amber-soft { border-top: 4px solid #eab308; }

/* --- NAVIGATION ONGLETS --- */
.settings-nav-matrix { display: flex; flex-direction: column; gap: 8px; }
.nav-matrix-btn {
  display: flex; align-items: center; gap: 15px; padding: 14px 20px;
  background: transparent; border: none; border-radius: 16px;
  color: #64748b; font-weight: 700; font-size: 12px; transition: 0.3s;
}
.icon-shell {
  width: 34px; height: 34px; background: #f1f5f9; border-radius: 10px;
  display: flex; align-items: center; justify-content: center;
}
.nav-matrix-btn:hover { background: #fff; color: #0f172a; }
.nav-matrix-btn.active { background: #0f172a; color: #fff; }
.nav-matrix-btn.active .icon-shell { background: rgba(255,255,255,0.1); color: #eab308; }

/* --- INPUTS --- */
.cyber-label { font-size: 10px; font-weight: 800; color: #94a3b8; text-transform: uppercase; margin-bottom: 8px; display: block; }
.cyber-input {
  width: 100%; padding: 12px 18px; border-radius: 12px;
  border: 1px solid #e2e8f0; background: #f8fafc; outline: none; transition: 0.3s;
  font-weight: 600; color: #1e293b;
}
.cyber-input:focus { border-color: #eab308; background: white; }

/* --- AVATAR --- */
.avatar-vessel { position: relative; width: 90px; height: 90px; }
.avatar-vessel img { width: 100%; height: 100%; border-radius: 22px; object-fit: cover; }
.btn-edit-neural {
  position: absolute; bottom: -5px; right: -5px; width: 30px; height: 30px;
  background: #0f172a; color: #eab308; border-radius: 10px; border: 2px solid #fff;
  display: flex; align-items: center; justify-content: center; font-size: 12px;
}

/* --- BUTTONS --- */
.btn-primary-gradient {
  background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%);
  color: #fff; border: none; border-radius: 14px;
  font-weight: 800; font-size: 12px; transition: 0.3s;
}
.btn-primary-gradient:hover:not(:disabled) { transform: translateY(-2px); box-shadow: 0 10px 20px rgba(0,0,0,0.1); }
.btn-primary-gradient:disabled { opacity: 0.6; cursor: not-allowed; }

.color-picker-neural { border: none; width: 45px; height: 45px; border-radius: 10px; cursor: pointer; padding: 0; background: none; }
.badge-tech { display: inline-flex; align-items: center; gap: 8px; background: white; padding: 4px 12px; border-radius: 100px; font-size: 9px; font-weight: 800; color: #64748b; border: 1px solid #e2e8f0; }
.pulse-dot { width: 6px; height: 6px; background: #eab308; border-radius: 50%; animation: blink 1s infinite; }
@keyframes blink { 0%, 100% { opacity: 1; } 50% { opacity: 0.3; } }
.btn-link-tech { background: none; border: none; color: #94a3b8; font-weight: 700; font-size: 11px; cursor: pointer; }
</style>