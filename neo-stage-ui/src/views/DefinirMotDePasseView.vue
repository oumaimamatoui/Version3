<template>
  <div class="login-container">
    <!-- COUCHES D'ARRIÈRE-PLAN (Style Login) -->
    <div class="background-overlay"></div>
    <div class="glow-orb orb-amber"></div>
    <div class="glow-orb orb-blue"></div>
    <div class="tech-grid-subtle"></div>

    <!-- ASSISTANT ROBOT IA -->
    <div class="ai-robot-wrapper">
      <div class="robot-float">
        <svg class="modern-robot" viewBox="0 0 200 200" width="180">
          <defs>
            <linearGradient id="botGrad" x1="0%" y1="0%" x2="100%" y2="100%">
              <stop offset="0%" style="stop-color:#ffffff" />
              <stop offset="100%" style="stop-color:#f1f5f9" />
            </linearGradient>
            <filter id="glowGold">
              <feGaussianBlur stdDeviation="3" result="blur"/>
              <feMerge><feMergeNode in="blur"/><feMergeNode in="SourceGraphic"/></feMerge>
            </filter>
          </defs>
          <rect x="55" y="50" width="90" height="85" rx="28" fill="url(#botGrad)" stroke="#e2e8f0" stroke-width="2"/>
          <rect x="65" y="65" width="70" height="32" rx="16" fill="#1e293b" />
          <g filter="url(#glowGold)">
            <circle cx="85" cy="81" r="5" fill="#eab308">
              <animate attributeName="opacity" values="1;0.3;1" dur="2s" repeatCount="indefinite" />
            </circle>
            <circle cx="115" cy="81" r="5" fill="#eab308">
              <animate attributeName="opacity" values="1;0.3;1" dur="2s" repeatCount="indefinite" />
            </circle>
          </g>
          <path d="M85 110 Q100 118 115 110" stroke="#cbd5e1" stroke-width="2" fill="none" stroke-linecap="round" />
        </svg>
      </div>
      <div class="robot-shadow"></div>
    </div>

    <!-- CARTE D'ACTIVATION -->
    <div class="login-box animate__animated animate__fadeInUp">
      
      <!-- Logo & Titre -->
      <div class="text-center mb-5">
        <div class="brand-link">
          <h3 class="brand-title">Evalua<span>Tech</span></h3>
          <p class="brand-subtitle">FINALISATION DU COMPTE</p>
        </div>
      </div>

      <div class="text-center mb-4">
        <h5 class="fw-bold text-slate-800">Sécurisez votre accès</h5>
        <p class="text-muted small">Définissez un mot de passe pour activer votre terminal.</p>
      </div>

      <!-- Alertes d'erreurs -->
      <div v-if="errorMessage" class="alert-cyber mb-4">
        <i class="fa fa-exclamation-circle me-2"></i> {{ errorMessage }}
      </div>

      <form @submit.prevent="finalize" class="auth-form">
        
        <!-- Nouveau Mot de passe -->
        <div class="mb-4 text-start">
          <label class="cyber-label">Nouveau mot de passe</label>
          <div class="input-group-cyber">
            <div class="input-icon"><i class="fa fa-key"></i></div>
            <input 
              :type="showPass ? 'text' : 'password'" 
              v-model="pass" 
              class="cyber-input" 
              placeholder="••••••••" 
              required
            >
            <div class="eye-toggle" @click="showPass = !showPass">
              <i :class="['fa', showPass ? 'fa-eye-slash' : 'fa-eye']"></i>
            </div>
          </div>

          <!-- Strength Meter (Amélioré) -->
          <div class="strength-meter mt-2" v-if="pass">
            <div class="d-flex gap-1 mb-1">
              <div v-for="n in 3" :key="n" class="strength-bar" :class="getStrengthClass(n)"></div>
            </div>
            <span class="tiny-text fw-bold text-uppercase" :style="{ color: strengthColor }">{{ strengthText }}</span>
          </div>
        </div>

        <!-- Confirmation -->
        <div class="mb-5 text-start">
          <label class="cyber-label">Confirmer le mot de passe</label>
          <div class="input-group-cyber" :class="{ 'error-border': conf && pass !== conf }">
            <div class="input-icon"><i class="fa fa-shield-check"></i></div>
            <input 
              :type="showPass ? 'text' : 'password'" 
              v-model="conf" 
              class="cyber-input" 
              placeholder="••••••••" 
              required
            >
          </div>
          
          <!-- Indicateur de correspondance -->
          <div class="mt-2 d-flex align-items-center tiny-text" v-if="conf">
            <i :class="pass === conf ? 'fa fa-circle-check text-success' : 'fa fa-circle-xmark text-danger'" class="me-1"></i>
            <span :class="pass === conf ? 'text-success' : 'text-danger'" class="fw-bold">
              {{ pass === conf ? 'Validation conforme' : 'Mots de passe différents' }}
            </span>
          </div>
        </div>

        <!-- Bouton Submit -->
        <button type="submit" class="btn-primary-gradient w-100" :disabled="isLoading || !isFormValid">
          <span v-if="isLoading" class="spinner-border spinner-border-sm me-2"></span>
          <span v-else>ACTIVER MON COMPTE <i class="fa fa-rocket ms-2"></i></span>
        </button>
      </form>

      <div class="mt-5 text-center">
        <router-link to="/login" class="btn-back">
          <i class="fa fa-chevron-left me-2"></i> Retour à la connexion
        </router-link>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import axios from 'axios';

const pass = ref('');
const conf = ref('');
const showPass = ref(false);
const token = ref('');
const isLoading = ref(false);
const errorMessage = ref('');

const router = useRouter();
const route = useRoute();

// --- LOGIQUE SÉCURITÉ ---
const strengthScore = computed(() => {
  let score = 0;
  if (pass.value.length >= 8) score++;
  if (/[A-Z]/.test(pass.value)) score++;
  if (/[0-9]/.test(pass.value)) score++;
  return score;
});

const strengthText = computed(() => ["Trop court", "Faible", "Moyen", "Robuste"][strengthScore.value]);
const strengthColor = computed(() => ["#94a3b8", "#ef4444", "#f59e0b", "#10b981"][strengthScore.value]);

const getStrengthClass = (n) => {
  if (strengthScore.value >= n) {
    if (strengthScore.value === 1) return 'bg-danger';
    if (strengthScore.value === 2) return 'bg-warning';
    return 'bg-success';
  }
  return '';
};

const isFormValid = computed(() => pass.value.length >= 6 && pass.value === conf.value);

onMounted(() => {
  token.value = route.query.token;
  if (!token.value) errorMessage.value = "Lien d'activation manquant.";
});

const finalize = async () => {
  try {
    isLoading.value = true;
    errorMessage.value = "";
    await axios.post('http://localhost:5172/api/Activation/complete', {
        token: token.value,
        password: pass.value
    });
    router.push({ path: '/login', query: { activated: 'true' } });
  } catch (error) {
    errorMessage.value = error.response?.data || "Erreur de finalisation.";
  } finally {
    isLoading.value = false;
  }
};
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;600;700;800&display=swap');

.login-container {
  font-family: 'Plus Jakarta Sans', sans-serif;
  min-height: 100vh;
  display: flex; justify-content: center; align-items: center;
  background-color: #f8fafc; position: relative; overflow: hidden;
}

/* --- DÉCORATIONS Login Style --- */
.background-overlay { position: absolute; inset: 0; background: radial-gradient(circle at 30% 30%, #ffffff 0%, #f1f5f9 100%); z-index: 0; }
.glow-orb { position: absolute; border-radius: 50%; filter: blur(120px); z-index: 1; opacity: 0.2; }
.orb-amber { width: 500px; height: 500px; background: #fbbf24; top: -10%; left: -10%; }
.orb-blue { width: 400px; height: 400px; background: #60a5fa; bottom: -5%; right: -5%; }
.tech-grid-subtle { position: absolute; inset: 0; background-image: linear-gradient(rgba(148, 163, 184, 0.08) 1px, transparent 1px), linear-gradient(90deg, rgba(148, 163, 184, 0.08) 1px, transparent 1px); background-size: 45px 45px; z-index: 1; }

/* --- ROBOT IA --- */
.ai-robot-wrapper { position: absolute; top: 2%; z-index: 10; display: flex; flex-direction: column; align-items: center; }
.robot-float { animation: robotFloat 4s ease-in-out infinite; }
@keyframes robotFloat { 0%, 100% { transform: translateY(0); } 50% { transform: translateY(-15px); } }
.robot-shadow { width: 50px; height: 6px; background: rgba(0,0,0,0.04); border-radius: 50%; filter: blur(4px); margin-top: 5px; }

/* --- LOGIN BOX --- */
.login-box {
  width: 100%; max-width: 440px;
  background: rgba(255, 255, 255, 0.96);
  backdrop-filter: blur(20px);
  border: 1px solid rgba(255, 255, 255, 1);
  border-radius: 32px; padding: 50px 40px;
  z-index: 5; box-shadow: 0 25px 60px -15px rgba(0, 0, 0, 0.08);
}

.brand-title { font-weight: 800; color: #0f172a; font-size: 32px; margin: 0; letter-spacing: -1px; }
.brand-title span { color: #eab308; }
.brand-subtitle { color: #94a3b8; font-size: 11px; font-weight: 700; letter-spacing: 2px; margin-top: 5px; }

/* --- INPUTS Login Style --- */
.cyber-label { display: block; font-weight: 700; color: #475569; font-size: 12px; text-transform: uppercase; margin-bottom: 8px; margin-left: 5px; }
.input-group-cyber {
  position: relative; display: flex; align-items: center;
  background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 16px; transition: 0.3s;
}
.input-group-cyber:focus-within {
  border-color: #eab308; background: #fff; box-shadow: 0 0 0 4px rgba(234, 179, 8, 0.08);
}
.input-icon { padding-left: 18px; color: #94a3b8; font-size: 14px; }
.cyber-input { width: 100%; background: transparent; border: none; padding: 15px 15px; color: #1e293b; outline: none; font-size: 14px; font-weight: 600; }
.eye-toggle { padding-right: 18px; color: #94a3b8; cursor: pointer; }

/* --- STRENGTH METER --- */
.strength-bar { height: 4px; flex: 1; background: #e2e8f0; border-radius: 10px; transition: 0.3s; }
.tiny-text { font-size: 10px; letter-spacing: 0.5px; }

/* --- BUTTONS --- */
.btn-primary-gradient {
  width: 100%; padding: 18px; 
  background: linear-gradient(135deg, #eab308 0%, #facc15 100%);
  color: #fff; border: none; border-radius: 16px;
  font-weight: 800; font-size: 15px; cursor: pointer;
  box-shadow: 0 10px 20px -5px rgba(234, 179, 8, 0.3);
  transition: 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275);
}
.btn-primary-gradient:hover:not(:disabled) { transform: translateY(-2px); box-shadow: 0 15px 25px -5px rgba(234, 179, 8, 0.4); }
.btn-primary-gradient:disabled { opacity: 0.6; cursor: not-allowed; filter: grayscale(0.5); }

.btn-back { color: #94a3b8; text-decoration: none; font-size: 14px; font-weight: 600; transition: 0.3s; }
.btn-back:hover { color: #0f172a; }


.alert-cyber { background: #fff1f2; border: 1px solid #fecaca; color: #e11d48; padding: 12px; border-radius: 12px; font-size: 13px; font-weight: 600; }
.error-border { border-color: #ef4444 !important; }
</style>