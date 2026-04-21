<template>
  <div class="activation-container">
    <!-- EFFETS DE FOND CYBER (Identiques au Login) -->
    <div class="background-overlay"></div>
    <div class="glow-orb orb-amber"></div>
    <div class="glow-orb orb-blue"></div>
    <div class="tech-grid-subtle"></div>

    <!-- ROBOT IA FLOTTANT -->
    <div class="ai-robot-wrapper">
      <div class="robot-float">
        <svg class="modern-robot" viewBox="0 0 200 200" width="140">
          <defs>
            <linearGradient id="botGrad" x1="0%" y1="0%" x2="100%" y2="100%">
              <stop offset="0%" style="stop-color:#ffffff" />
              <stop offset="100%" style="stop-color:#f1f5f9" />
            </linearGradient>
          </defs>
          <rect x="55" y="50" width="90" height="85" rx="28" fill="url(#botGrad)" stroke="#e2e8f0" stroke-width="2"/>
          <rect x="65" y="65" width="70" height="32" rx="16" fill="#1e293b" />
          <g>
            <circle cx="85" cy="81" r="5" fill="#eab308">
              <animate attributeName="opacity" values="1;0.3;1" dur="2s" repeatCount="indefinite" />
            </circle>
            <circle cx="115" cy="81" r="5" fill="#eab308">
              <animate attributeName="opacity" values="1;0.3;1" dur="2s" repeatCount="indefinite" />
            </circle>
          </g>
        </svg>
      </div>
      <div class="robot-shadow"></div>
    </div>

    <!-- CARTE D'ACTIVATION -->
    <div class="login-box animate__animated animate__fadeInUp">
      
      <div class="text-center mb-5">
        <h3 class="brand-title">Evalua<span>Tech</span></h3>
        <p class="brand-subtitle">ACTIVATION DU TERMINAL</p>
      </div>

      <Transition name="fade-slide" mode="out-in">
        
        <!-- ÉTAT 1 : CHARGEMENT / VÉRIFICATION -->
        <div v-if="loading" class="text-center py-5" key="loading">
          <div class="loader-cyber mb-3">
            <i class="fa fa-atom fa-spin text-amber"></i>
          </div>
          <p class="text-muted fw-bold small uppercase ls-1">Vérification du protocole...</p>
        </div>

        <!-- ÉTAT 2 : SUCCÈS -->
        <div v-else-if="success" class="text-center py-4" key="success">
          <div class="success-icon mb-4"><i class="fa fa-shield-check"></i></div>
          <h4 class="fw-bold text-navy">Accès Activé</h4>
          <p class="text-muted small mb-4">Votre identité numérique est désormais sécurisée.</p>
          <button @click="router.push('/login')" class="btn-primary-gradient w-100">
            DÉMARRER LA SESSION <i class="fa fa-arrow-right ms-2"></i>
          </button>
        </div>

        <!-- ÉTAT 3 : ERREUR (Token invalide) -->
        <div v-else-if="error" class="text-center py-4" key="error">
          <div class="error-icon-cyber mb-4"><i class="fa fa-triangle-exclamation"></i></div>
          <h4 class="fw-bold text-navy">Échec d'Authentification</h4>
          <p class="text-muted small mb-4">{{ errorMessage }}</p>
          <router-link to="/login" class="link-amber fw-bold uppercase small">Retour à l'accueil</router-link>
        </div>

        <!-- ÉTAT 4 : FORMULAIRE DE MOT DE PASSE -->
        <div v-else key="form">
          <p class="text-center text-muted small mb-4">Initialisez votre mot de passe pour finaliser l'activation de votre compte.</p>
          
          <form @submit.prevent="handleActivation">
            <!-- NOUVEAU : PRÉNOM & NOM -->
            <div class="row g-3 mb-3">
              <div class="col-md-6">
                <div class="input-group-cyber">
                  <div class="input-icon"><i class="fa fa-user"></i></div>
                  <input 
                    v-model="form.prenom" 
                    type="text" 
                    class="cyber-input" 
                    placeholder="Prénom" 
                    required
                  >
                </div>
              </div>
              <div class="col-md-6">
                <div class="input-group-cyber">
                  <div class="input-icon"><i class="fa fa-id-card"></i></div>
                  <input 
                    v-model="form.nom" 
                    type="text" 
                    class="cyber-input" 
                    placeholder="Nom"
                    required
                  >
                </div>
              </div>
            </div>

            <div class="input-group-cyber mb-3">
              <div class="input-icon"><i class="fa fa-lock"></i></div>
              <input 
                v-model="form.password" 
                type="password" 
                class="cyber-input" 
                placeholder="Nouveau mot de passe" 
                required
              >
            </div>
            
            <div class="input-group-cyber mb-4">
              <div class="input-icon"><i class="fa fa-shield-halved"></i></div>
              <input 
                v-model="form.confirmPassword" 
                type="password" 
                class="cyber-input" 
                placeholder="Confirmer le mot de passe" 
                required
              >
            </div>

            <button type="submit" class="btn-primary-gradient w-100" :disabled="isSubmitting">
              <span v-if="isSubmitting"><i class="fa fa-sync fa-spin me-2"></i> TRAITEMENT...</span>
              <span v-else>ACTIVER MON COMPTE</span>
            </button>
          </form>
        </div>

      </Transition>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import api from '@/services/api';

const route = useRoute();
const router = useRouter();

const loading = ref(true);
const success = ref(false);
const error = ref(false);
const errorMessage = ref('');
const isSubmitting = ref(false);

const form = ref({
  prenom: '',
  nom: '',
  password: '',
  confirmPassword: ''
});

const API_URL = '/Activation';

onMounted(async () => {
  const t = route.query.token;
  const token = Array.isArray(t) ? t[0] : t;

  if (!token) {
    error.value = true;
    errorMessage.value = "Le jeton de sécurité est manquant.";
    loading.value = false;
    return;
  }
  
  console.log("[DEBUG] Activation Token detected:", token);
  
  try {
    const res = await api.get(`${API_URL}/check/${token}`);
    if (!res.data.valide) {
      error.value = true;
      errorMessage.value = "Ce lien a expiré ou est invalide.";
    }
  } catch (err) {
    error.value = true;
    errorMessage.value = "Connexion au serveur de sécurité impossible.";
  } finally {
    setTimeout(() => { loading.value = false; }, 800); // Petit délai pour l'effet visuel
  }
});

const handleActivation = async () => {
  if (form.value.password.length < 6) return alert("Minimum 6 caractères requis.");
  if (form.value.password !== form.value.confirmPassword) return alert("Les mots de passe divergent.");

  isSubmitting.value = true;
  const t = route.query.token;
  const cleanToken = Array.isArray(t) ? t[0] : t;
  
  try {
    await api.post(`${API_URL}/complete`, {
      token: cleanToken,
      password: form.value.password,
      prenom: form.value.prenom,
      nom: form.value.nom
    });
    success.value = true;
  } catch (err) {
    alert(err.response?.data?.message || "Erreur critique d'activation.");
  } finally {
    isSubmitting.value = false;
  }
};
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;600;700;800&display=swap');

.activation-container {
  font-family: 'Plus Jakarta Sans', sans-serif;
  min-height: 100vh; display: flex; justify-content: center; align-items: center;
  background-color: #f8fafc; position: relative; overflow: hidden;
}

/* BACKGROUND EFFECTS */
.background-overlay { position: absolute; inset: 0; background: radial-gradient(circle at 30% 30%, #ffffff 0%, #f1f5f9 100%); z-index: 0; }
.glow-orb { position: absolute; border-radius: 50%; filter: blur(120px); z-index: 1; opacity: 0.2; }
.orb-amber { width: 500px; height: 500px; background: #fbbf24; top: -10%; left: -10%; }
.orb-blue { width: 400px; height: 400px; background: #60a5fa; bottom: -5%; right: -5%; }
.tech-grid-subtle { position: absolute; inset: 0; background-image: linear-gradient(rgba(148, 163, 184, 0.08) 1px, transparent 1px), linear-gradient(90deg, rgba(148, 163, 184, 0.08) 1px, transparent 1px); background-size: 45px 45px; z-index: 1; }

/* BOX STYLE */
.login-box {
  width: 100%; max-width: 420px; background: rgba(255, 255, 255, 0.96);
  backdrop-filter: blur(20px); border-radius: 32px; padding: 40px; z-index: 5;
  box-shadow: 0 25px 60px -15px rgba(0, 0, 0, 0.08); border: 1px solid white;
}

.brand-title { font-weight: 800; color: #0f172a; font-size: 30px; margin: 0; }
.brand-title span { color: #eab308; }
.brand-subtitle { color: #94a3b8; font-size: 11px; font-weight: 700; letter-spacing: 2px; }

/* INPUTS & BUTTONS */
.input-group-cyber { position: relative; display: flex; align-items: center; background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 16px; }
.input-group-cyber:focus-within { border-color: #eab308; background: #fff; box-shadow: 0 0 0 4px rgba(234, 179, 8, 0.08); }
.input-icon { width: 45px; text-align: center; color: #94a3b8; }
.cyber-input { width: 100%; background: transparent; border: none; padding: 14px 14px 14px 0; outline: none; font-size: 14px; font-weight: 600; }

.btn-primary-gradient {
  width: 100%; padding: 16px; background: linear-gradient(135deg, #eab308 0%, #facc15 100%);
  color: #fff; border: none; border-radius: 16px; font-weight: 800; cursor: pointer; transition: 0.3s;
  letter-spacing: 0.5px; font-size: 13px;
}
.btn-primary-gradient:hover:not(:disabled) { transform: translateY(-2px); box-shadow: 0 10px 20px rgba(234, 179, 8, 0.3); }
.btn-primary-gradient:disabled { opacity: 0.7; cursor: not-allowed; }

/* ROBOT */
.ai-robot-wrapper { position: absolute; top: 8%; z-index: 10; display: flex; flex-direction: column; align-items: center; }
.robot-float { animation: robotFloat 4s ease-in-out infinite; }
@keyframes robotFloat { 0%, 100% { transform: translateY(0); } 50% { transform: translateY(-12px); } }

/* ICONS & STATES */
.success-icon { font-size: 4rem; color: #eab308; text-shadow: 0 10px 20px rgba(234, 179, 8, 0.2); }
.error-icon-cyber { font-size: 4rem; color: #ef4444; }
.loader-cyber { font-size: 2.5rem; }
.text-amber { color: #eab308; }
.link-amber { color: #eab308; text-decoration: none; }
.ls-1 { letter-spacing: 1px; }
.uppercase { text-transform: uppercase; }

/* TRANSITIONS */
.fade-slide-enter-active, .fade-slide-leave-active { transition: all 0.4s ease; }
.fade-slide-enter-from { opacity: 0; transform: translateX(20px); }
.fade-slide-leave-to { opacity: 0; transform: translateX(-20px); }
</style>