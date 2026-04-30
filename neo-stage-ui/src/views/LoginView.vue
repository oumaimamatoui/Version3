<template>
  <div class="elite-login-root">
    <!-- Iconographie Pro -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.2/css/all.min.css">

    <!-- Fond Immersif Elite -->
    <div class="luxury-bg">
      <div class="aura-sphere sphere-amber"></div>
      <div class="aura-sphere sphere-blue"></div>
      <div class="aura-sphere sphere-rose"></div>
      <div class="mesh-grain"></div>
    </div>

    <div class="login-master-container">
      
      <!-- CARTE BENTO DE CONNEXION -->
      <div class="bento-login-card animate__animated animate__fadeInUp">
        
        <!-- ROBOT COMPAGNON -->
        <div class="bot-anchor">
          <div class="robot-float">
            <svg class="master-bot-login" viewBox="0 0 200 200" xmlns="http://www.w3.org/2000/svg">
              <circle cx="100" cy="40" r="10" fill="none" stroke="#fbbf24" stroke-width="1.5" class="signal-ping" />
              <rect x="55" y="55" width="90" height="90" rx="42" fill="white" stroke="#f1f5f9" stroke-width="1"/>
              <rect x="65" y="75" width="70" height="42" rx="18" fill="#0f172a" />
              <circle cx="85" cy="95" r="4.5" fill="#fbbf24" class="led-blink" />
              <circle cx="115" cy="95" r="4.5" fill="#fbbf24" class="led-blink" />
              <!-- Cœur supprimé ici -->
              <line x1="100" y1="40" x2="100" y2="55" stroke="#0f172a" stroke-width="3" stroke-linecap="round"/>
              <circle cx="100" cy="40" r="6" fill="#fbbf24" class="antenna-core" />
            </svg>
          </div>
        </div>

        <div class="form-header">
          <h1 class="brand-logo">Evalua<span>Tech</span></h1>
          <p class="brand-tagline">AUTHENTIFICATION INTELLIGENTE</p>
          <div class="header-line"></div>
        </div>

        <Transition name="fade-slide" mode="out-in">
          <!-- ÉTAT SUCCÈS -->
          <div v-if="requestSent" class="success-portal" key="success">
            <div class="check-circle-anim">
              <i class="fa-solid fa-shield-check"></i>
            </div>
            <h3>Accès Autorisé</h3>
            <p>Initialisation de votre terminal personnel...</p>
          </div>

          <!-- FORMULAIRE -->
          <div v-else key="form" class="form-content">
            
            <div class="social-login-area">
              <GoogleLogin :callback="handleGoogleLogin" class="google-btn-custom" />
            </div>

            <div class="auth-divider">
              <span>OU IDENTIFICATION MANUELLE</span>
            </div>

            <form @submit.prevent="handleClassicLogin" class="portal-form">
              <div v-if="errorMessage" class="error-banner">
                <i class="fa-solid fa-circle-exclamation"></i> {{ errorMessage }}
              </div>

              <div class="field-premium">
                <label>Email Professionnel</label>
                <div class="input-glow-wrapper">
                  <i class="fa-solid fa-envelope-open-text"></i>
                  <input v-model="loginForm.email" type="email" placeholder="nom@entreprise.pro" required>
                </div>
              </div>
              
              <div class="field-premium">
                <label>Mot de Passe</label>
                <div class="input-glow-wrapper">
                  <i class="fa-solid fa-key-skeleton"></i>
                  <input :type="showPassword ? 'text' : 'password'" v-model="loginForm.password" placeholder="Votre clé d'accès" required>
                  <button type="button" class="eye-toggle-btn" @click="showPassword = !showPassword">
                    <i :class="['fa-solid', showPassword ? 'fa-eye-slash' : 'fa-eye']"></i>
                  </button>
                </div>
              </div>

              <div class="form-options">
                <label class="custom-check">
                  <input type="checkbox" v-model="loginForm.remember">
                  <span class="checkmark"></span> Se souvenir
                </label>
                <router-link to="/forgot-password" class="forgot-link">Clé oubliée ?</router-link>
              </div>

              <button type="submit" class="btn-sunburst" :disabled="isLoading">
                <div class="btn-label" v-if="!isLoading">
                  <span>DÉVERROUILLER L'ACCÈS</span>
                  <i class="fa-solid fa-unlock-keyhole"></i>
                </div>
                <div v-else class="btn-loader"><span></span><span></span><span></span></div>
                <div class="shine-sweep"></div>
              </button>
            </form>

            <footer class="portal-footer">
              <p>Nouveau membre ? <router-link to="/register" class="register-link">Créer un compte</router-link></p>
              <router-link to="/" class="back-home-link">
                <i class="fa-solid fa-arrow-left-long"></i> Retour à l'accueil
              </router-link>
            </footer>
          </div>
        </Transition>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import { GoogleLogin } from 'vue3-google-login';
import axios from 'axios';

const router = useRouter();
const authStore = useAuthStore();

const isLoading = ref(false);
const errorMessage = ref("");
const requestSent = ref(false);
const showPassword = ref(false);
const loginForm = reactive({ email: '', password: '', remember: false });

const handleClassicLogin = async () => {
  isLoading.value = true;
  errorMessage.value = "";
  try {
    const res = await axios.post('http://localhost:5172/api/Auth/login', loginForm);
    handleAuthSuccess(res.data);
  } catch (err) {
    errorMessage.value = err.response?.data?.message || "Identifiants invalides.";
  } finally { isLoading.value = false; }
};

const handleAuthSuccess = (data) => {
  const { token, role, nom, email, photo, themePreference, privileges } = data;
  authStore.setUser(
    { email: email || loginForm.email, name: nom, photoUrl: photo, themePreference: themePreference }, 
    role, 
    token, 
    privileges || []
  );
  requestSent.value = true;
  setTimeout(() => {
    if (role?.toUpperCase() === 'SUPERADMIN') router.push('/super-admin');
    else router.push('/dashboard');
  }, 1500);
};

const handleGoogleLogin = async (response) => {
  isLoading.value = true;
  errorMessage.value = "";
  try {
    const res = await axios.post('http://localhost:5172/api/Auth/google-login', { idToken: response.credential });
    handleAuthSuccess(res.data);
  } catch (err) {
    errorMessage.value = "Échec de l'identification Google.";
  } finally { isLoading.value = false; }
};
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;500;600;700;800&display=swap');

/* --- FOND --- */
.elite-login-root {
  font-family: 'Plus Jakarta Sans', sans-serif;
  min-height: 100vh; display: flex; justify-content: center; align-items: center;
  background: #fdfdfd; position: relative; overflow: hidden; padding: 20px;
}

.luxury-bg { position: absolute; inset: 0; z-index: 1; }
.aura-sphere { position: absolute; border-radius: 50%; filter: blur(140px); opacity: 0.25; animation: orbit 25s infinite linear; }
.sphere-amber { width: 600px; height: 600px; background: #fbbf24; top: -10%; left: -10%; }
.sphere-blue { width: 500px; height: 500px; background: #60a5fa; bottom: -10%; right: -5%; }
.sphere-rose { width: 400px; height: 400px; background: #fda4af; top: 35%; right: 15%; opacity: 0.1; }
.mesh-grain { position: absolute; inset: 0; opacity: 0.03; background-image: url("data:image/svg+xml,%3Csvg viewBox='0 0 200 200' xmlns='http://www.w3.org/2000/svg'%3e%3cfilter id='n'%3e%3cfeTurbulence type='fractalNoise' baseFrequency='0.65' numOctaves='3'/%3e%3c/filter%3e%3crect width='100%25' height='100%25' filter='url(%23n)'/%3e%3c/svg%3e"); }

@keyframes orbit { from { transform: rotate(0) translate(40px) rotate(0); } to { transform: rotate(360deg) translate(40px) rotate(-360deg); } }

/* --- CARD --- */
.login-master-container { z-index: 10; width: 100%; max-width: 480px; margin-top: 40px; }

.bento-login-card {
  background: rgba(255, 255, 255, 0.98);
  backdrop-filter: blur(40px);
  border-radius: 50px; padding: 70px 50px 50px; border: 1px solid white;
  box-shadow: 0 40px 100px -20px rgba(0,0,0,0.06); position: relative;
  overflow: visible;
}

/* --- ROBOT --- */
.bot-anchor { position: absolute; top: -95px; left: 50%; transform: translateX(-50%); }
.robot-float { animation: floatBot 4s ease-in-out infinite; }
.master-bot-login { width: 170px; filter: drop-shadow(0 20px 40px rgba(0,0,0,0.08)); }
.signal-ping { animation: ping 3s infinite; transform-origin: center; }

@keyframes floatBot { 0%, 100% { transform: translateY(0); } 50% { transform: translateY(-15px); } }
@keyframes ping { 0% { r: 5; opacity: 0.8; } 100% { r: 40; opacity: 0; } }
@keyframes blink { 0%, 90%, 100% { transform: scaleY(1); } 95% { transform: scaleY(0.1); } }
.led-blink { animation: blink 4s infinite; transform-origin: center; }

/* --- HEADER --- */
.brand-logo { font-size: 38px; font-weight: 900; color: #0f172a; margin: 0; text-align: center; letter-spacing: -2px; }
.brand-logo span { color: #fbbf24; }
.brand-tagline { color: #94a3b8; font-size: 11px; font-weight: 800; text-align: center; letter-spacing: 2px; margin-top: 5px; }
.header-line { width: 45px; height: 5px; background: #fbbf24; border-radius: 5px; margin: 20px auto 40px; }

/* --- DIVIDER --- */
.auth-divider { display: flex; align-items: center; margin: 30px 0; color: #cbd5e1; font-size: 11px; font-weight: 800; letter-spacing: 1px; }
.auth-divider::before, .auth-divider::after { content: ""; flex: 1; height: 1px; background: #f1f5f9; }
.auth-divider span { padding: 0 15px; }

.social-login-area { display: flex; justify-content: center; margin-bottom: 30px; }

/* --- INPUTS --- */
.field-premium { margin-bottom: 22px; }
.field-premium label { display: block; font-size: 11px; font-weight: 800; color: #475569; margin-bottom: 10px; text-transform: uppercase; letter-spacing: 0.5px; }

.input-glow-wrapper { position: relative; display: flex; align-items: center; }
.input-glow-wrapper i { position: absolute; left: 18px; color: #fbbf24; font-size: 16px; transition: 0.3s; }
.input-glow-wrapper input {
  width: 100%; padding: 17px 20px 17px 52px; border-radius: 20px; border: 1.5px solid #f1f5f9;
  background: #f8fafc; font-size: 15px; transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
}
.input-glow-wrapper input:focus {
  outline: none; border-color: #fbbf24; background: white;
  box-shadow: 0 12px 25px rgba(251, 191, 36, 0.1); transform: translateY(-3px);
}
.input-glow-wrapper input:focus + i { transform: scale(1.25); color: #0f172a; }

.eye-toggle-btn { position: absolute; right: 18px; background: none; border: none; color: #cbd5e1; cursor: pointer; font-size: 16px; }

.form-options { display: flex; justify-content: space-between; align-items: center; margin-bottom: 30px; font-size: 13px; font-weight: 600; }
.forgot-link { color: #fbbf24; text-decoration: none; font-weight: 700; }
.custom-check { display: flex; align-items: center; gap: 8px; color: #64748b; cursor: pointer; }

/* --- BUTTON SUNBURST --- */
.btn-sunburst {
  width: 100%; padding: 20px; border-radius: 22px; border: none;
  background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
  color: #0f172a; font-weight: 800; cursor: pointer; position: relative;
  overflow: hidden; transition: all 0.4s; box-shadow: 0 15px 35px rgba(251, 191, 36, 0.3);
}
.btn-sunburst:hover { transform: translateY(-4px); box-shadow: 0 20px 45px rgba(251, 191, 36, 0.45); }
.shine-sweep {
  position: absolute; top: 0; left: -100%; width: 60%; height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255,255,255,0.4), transparent);
  animation: sweep 4s infinite;
}
@keyframes sweep { 0% { left: -120%; } 30% { left: 150%; } 100% { left: 150%; } }

.btn-label { display: flex; justify-content: center; align-items: center; gap: 12px; }

/* --- FOOTER --- */
.portal-footer { margin-top: 40px; text-align: center; }
.portal-footer p { font-size: 14px; color: #94a3b8; margin-bottom: 20px; }
.register-link { color: #0f172a; font-weight: 800; text-decoration: none; border-bottom: 2px solid #fbbf24; padding-bottom: 2px; }

.back-home-link { font-size: 13px; color: #94a3b8; text-decoration: none; display: flex; align-items: center; justify-content: center; gap: 10px; transition: 0.3s; margin-top: 15px; }
.back-home-link:hover { color: #fbbf24; transform: translateX(-5px); }

/* --- STATUS --- */
.error-banner { background: #fff1f2; color: #e11d48; padding: 15px; border-radius: 15px; margin-bottom: 25px; font-size: 13px; font-weight: 600; text-align: center; border: 1px solid #ffe4e6; }
.success-portal { text-align: center; padding: 40px 0; }
.check-circle-anim { width: 85px; height: 85px; background: #fef3c7; color: #fbbf24; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-size: 40px; margin: 0 auto 25px; animation: zoomIn 0.5s; }

/* --- LOADERS --- */
.btn-loader { display: flex; gap: 6px; justify-content: center; }
.btn-loader span { width: 8px; height: 8px; background: #0f172a; border-radius: 50%; animation: loaderPulse 0.6s infinite alternate; }
.btn-loader span:nth-child(2) { animation-delay: 0.2s; }
.btn-loader span:nth-child(3) { animation-delay: 0.4s; }
@keyframes loaderPulse { from { transform: scale(1); opacity: 1; } to { transform: scale(1.5); opacity: 0.3; } }
</style>