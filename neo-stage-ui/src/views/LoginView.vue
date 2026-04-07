<template>
  <div class="login-container">
    <!-- EFFETS DE FOND CYBER -->
    <div class="background-overlay"></div>
    <div class="glow-orb orb-amber"></div>
    <div class="glow-orb orb-blue"></div>
    <div class="tech-grid-subtle"></div>

    <!-- ROBOT IA FLOTTANT -->
    <div class="ai-robot-wrapper">
      <div class="robot-float">
        <svg class="modern-robot" viewBox="0 0 200 200" width="160">
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

    <!-- CARTE DE CONNEXION -->
    <div class="login-box animate__animated animate__fadeInUp">
      
      <div class="text-center mb-5">
        <h3 class="brand-title">Evalua<span>Tech</span></h3>
        <p class="brand-subtitle">SYSTÈME D'ACCÈS INTELLIGENT</p>
      </div>

      <Transition name="fade-slide" mode="out-in">
        <!-- ÉTAT SUCCÈS -->
        <div v-if="requestSent" class="text-center py-5" key="success">
          <div class="success-icon"><i class="fa fa-check-circle"></i></div>
          <h4 class="fw-bold">Accès Autorisé</h4>
          <p class="text-muted">Initialisation de votre terminal...</p>
        </div>

        <!-- FORMULAIRE -->
        <div v-else key="form">
          <div class="google-wrapper mb-4 d-flex justify-content-center">
            <div id="googleButtonPlaceholder"></div>
          </div>

          <div class="divider"><span>OU IDENTIFICATION MANUELLE</span></div>

          <form @submit.prevent="handleClassicLogin">
            <div v-if="errorMessage" class="alert-cyber mb-4">
              <i class="fa fa-exclamation-circle me-2"></i> {{ errorMessage }}
            </div>

            <div class="input-group-cyber mb-3">
              <div class="input-icon"><i class="fa fa-envelope"></i></div>
              <input v-model="loginForm.email" type="email" class="cyber-input" placeholder="Email professionnel" required>
            </div>
            
            <div class="input-group-cyber mb-3">
              <div class="input-icon"><i class="fa fa-lock"></i></div>
              <input :type="showPassword ? 'text' : 'password'" v-model="loginForm.password" class="cyber-input" placeholder="Mot de passe" required>
              <div class="eye-toggle" @click="showPassword = !showPassword">
                <i :class="['fa', showPassword ? 'fa-eye-slash' : 'fa-eye']"></i>
              </div>
            </div>

            <div class="d-flex justify-content-between align-items-center mb-4">
              <label class="remember-me">
                <input type="checkbox" v-model="loginForm.remember">
                <span class="checkmark"></span> Se souvenir
              </label>
              <router-link to="/forgot-password" class="link-amber">Oublié ?</router-link>
            </div>

            <button type="submit" class="btn-primary-gradient" :disabled="isLoading">
              <span v-if="isLoading"><i class="fa fa-spinner fa-spin me-2"></i></span>
              <span v-else>SE CONNECTER</span>
            </button>
          </form>

          <!-- LIEN VERS REGISTER (CORRIGÉ ICI) -->
          <div class="text-center mt-4">
            <p class="text-footer">
              Pas encore de compte ? 
              <router-link to="/register" class="link-amber fw-bold">S'enregistrer</router-link>
            </p>
          </div>
        </div>
      </Transition>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import axios from 'axios';

const router = useRouter();
const authStore = useAuthStore();

const isLoading = ref(false);
const errorMessage = ref("");
const requestSent = ref(false);
const showPassword = ref(false);
const loginForm = reactive({ email: '', password: '', remember: false });

onMounted(() => {
  if (window.google) {
    google.accounts.id.initialize({
      client_id: 'VOTRE_CLIENT_ID.apps.googleusercontent.com',
      callback: handleGoogleLogin,
    });
    google.accounts.id.renderButton(document.getElementById("googleButtonPlaceholder"), { theme: "outline", size: "large", shape: "pill" });
  }
});

const handleClassicLogin = async () => {
  isLoading.value = true;
  errorMessage.value = "";
  try {
    const res = await axios.post('http://localhost:5172/api/Auth/login', loginForm);
    handleAuthSuccess(res.data);
  } catch (err) {
    errorMessage.value = "Identifiants invalides.";
  } finally { isLoading.value = false; }
};

const handleAuthSuccess = (data) => {
  const { token, role, nom } = data;
  // Stockage store
  authStore.setUser({ email: loginForm.email, name: nom }, role, token);
  requestSent.value = true;
  
  setTimeout(() => {
    // Redirection basée sur votre router.js
    if (role?.toUpperCase() === 'SUPERADMIN') {
      router.push('/super-admin');
    } else {
      router.push('/dashboard');
    }
  }, 1500);
};

const handleGoogleLogin = async (response) => { /* Votre logique google */ };
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;600;700;800&display=swap');

.login-container {
  font-family: 'Plus Jakarta Sans', sans-serif;
  min-height: 100vh; display: flex; justify-content: center; align-items: center;
  background-color: #f8fafc; position: relative; overflow: hidden;
}

.background-overlay { position: absolute; inset: 0; background: radial-gradient(circle at 30% 30%, #ffffff 0%, #f1f5f9 100%); z-index: 0; }
.glow-orb { position: absolute; border-radius: 50%; filter: blur(120px); z-index: 1; opacity: 0.2; }
.orb-amber { width: 500px; height: 500px; background: #fbbf24; top: -10%; left: -10%; }
.orb-blue { width: 400px; height: 400px; background: #60a5fa; bottom: -5%; right: -5%; }
.tech-grid-subtle { position: absolute; inset: 0; background-image: linear-gradient(rgba(148, 163, 184, 0.08) 1px, transparent 1px), linear-gradient(90deg, rgba(148, 163, 184, 0.08) 1px, transparent 1px); background-size: 45px 45px; z-index: 1; }

.login-box {
  width: 100%; max-width: 420px; background: rgba(255, 255, 255, 0.96);
  backdrop-filter: blur(20px); border-radius: 32px; padding: 40px; z-index: 5;
  box-shadow: 0 25px 60px -15px rgba(0, 0, 0, 0.08); border: 1px solid white;
}

.brand-title { font-weight: 800; color: #0f172a; font-size: 30px; margin: 0; }
.brand-title span { color: #eab308; }
.brand-subtitle { color: #94a3b8; font-size: 11px; font-weight: 700; letter-spacing: 1px; }

.input-group-cyber { position: relative; display: flex; align-items: center; background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 16px; }
.input-group-cyber:focus-within { border-color: #eab308; background: #fff; box-shadow: 0 0 0 4px rgba(234, 179, 8, 0.08); }
.input-icon { padding-left: 15px; color: #94a3b8; }
.cyber-input { width: 100%; background: transparent; border: none; padding: 12px; outline: none; font-size: 14px; }

.btn-primary-gradient {
  width: 100%; padding: 15px; background: linear-gradient(135deg, #eab308 0%, #facc15 100%);
  color: #fff; border: none; border-radius: 16px; font-weight: 800; cursor: pointer; transition: 0.3s;
}
.btn-primary-gradient:hover { transform: translateY(-2px); box-shadow: 0 10px 20px rgba(234, 179, 8, 0.3); }

.divider { display: flex; align-items: center; margin: 25px 0; color: #cbd5e1; font-size: 10px; font-weight: 800; }
.divider::before, .divider::after { content: ""; flex: 1; height: 1px; background: #e2e8f0; }
.divider span { padding: 0 10px; }

.link-amber { color: #eab308; text-decoration: none; font-size: 13px; font-weight: 600; }
.text-footer { font-size: 14px; color: #64748b; }

.alert-cyber { background: #fef2f2; border: 1px solid #fecaca; color: #dc2626; padding: 10px; border-radius: 12px; font-size: 13px; }
.success-icon { font-size: 3rem; color: #eab308; margin-bottom: 10px; }

.ai-robot-wrapper { position: absolute; top: 5%; z-index: 10; display: flex; flex-direction: column; align-items: center; }
.robot-float { animation: robotFloat 4s ease-in-out infinite; }
@keyframes robotFloat { 0%, 100% { transform: translateY(0); } 50% { transform: translateY(-10px); } }
</style>