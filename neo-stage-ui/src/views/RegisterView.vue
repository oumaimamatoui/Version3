<template>
  <div class="elite-register-root">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.2/css/all.min.css">

    <div class="luxury-bg">
      <div class="aura-sphere sphere-amber"></div>
      <div class="aura-sphere sphere-blue"></div>
      <div class="aura-sphere sphere-rose"></div>
      <div class="mesh-grain"></div>
    </div>

    <div class="register-master-container">
      <div class="bento-register-card animate__animated animate__fadeInUp">

        <!-- ROBOT EN HAUT -->
        <div class="bot-anchor">
          <div class="robot-float">
            <svg class="master-bot-register" viewBox="0 0 200 200" xmlns="http://www.w3.org/2000/svg">
              <circle cx="100" cy="40" r="10" fill="none" stroke="#fbbf24" stroke-width="1.5" class="signal-ping"/>
              <ellipse cx="100" cy="183" rx="38" ry="9" fill="rgba(15,23,42,0.05)" class="bot-shadow"/>
              <rect x="55" y="55" width="90" height="90" rx="42" fill="white" stroke="#f1f5f9" stroke-width="1"/>
              <rect x="65" y="75" width="70" height="42" rx="18" fill="#0f172a"/>
              <circle cx="85" cy="96" r="4.5" fill="#fbbf24" class="led-blink"/>
              <circle cx="115" cy="96" r="4.5" fill="#fbbf24" class="led-blink"/>
              <line x1="100" y1="40" x2="100" y2="55" stroke="#0f172a" stroke-width="3" stroke-linecap="round"/>
              <circle cx="100" cy="40" r="6" fill="#fbbf24" class="antenna-core"/>
            </svg>
          </div>
        </div>

        <!-- HEADER -->
        <div class="form-header">
          <h1 class="brand-logo">Evalua<span>Tech</span></h1>
          <p class="brand-tagline">CRÉATION DE COMPTE PROFESSIONNEL</p>
          <div class="header-line"></div>
        </div>

        <!-- SUCCÈS -->
        <Transition name="fade-slide" mode="out-in">
          <div v-if="requestSent" class="success-portal" key="success">
            <div class="check-circle-anim">
              <i class="fa-solid fa-shield-check"></i>
            </div>
            <h3>Demande Envoyée</h3>
            <p>Un email de confirmation vous sera envoyé très prochainement.</p>
            <button @click="goToLogin" class="btn-return">Retour à l'accueil</button>
          </div>

          <!-- FORMULAIRE -->
          <div v-else key="form" class="form-content">
            <form @submit.prevent="handleRegister" class="portal-form">
              <div v-if="errorMessage" class="error-banner">
                <i class="fa-solid fa-circle-exclamation"></i> {{ errorMessage }}
              </div>

              <div class="grid-inputs">
                <div class="field-premium">
                  <label>Organisation</label>
                  <div class="input-glow-wrapper">
                    <i class="fa-solid fa-building"></i>
                    <input v-model="form.nomEntreprise" type="text" placeholder="Nom de l'entreprise" required>
                  </div>
                </div>

                <div class="field-premium">
                  <label>Responsable</label>
                  <div class="input-glow-wrapper">
                    <i class="fa-solid fa-user-tie"></i>
                    <input v-model="form.nomResponsable" type="text" placeholder="Prénom Nom" required>
                  </div>
                </div>
              </div>

              <div class="field-premium">
                <label>Email Professionnel</label>
                <div class="input-glow-wrapper">
                  <i class="fa-solid fa-envelope-open-text"></i>
                  <input v-model="form.emailResponsable" type="email" placeholder="contact@domaine.pro" required>
                </div>
              </div>

              <div class="field-premium">
                <label>Matricule Fiscale <small class="opt-tag">(Optionnel)</small></label>
                <div class="input-glow-wrapper">
                  <i class="fa-solid fa-id-card"></i>
                  <input v-model="form.matriculeFiscale" type="text" placeholder="Référence légale">
                </div>
              </div>

              <button type="submit" class="btn-sunburst" :disabled="isLoading">
                <div class="btn-label" v-if="!isLoading">
                  <span>DÉPLOYER L'ACCÈS</span>
                  <i class="fa-solid fa-arrow-right"></i>
                </div>
                <div v-else class="btn-loader"><span></span><span></span><span></span></div>
                <div class="shine-sweep"></div>
              </button>
            </form>

            <footer class="portal-footer">
              <p>Déjà membre ? <router-link to="/login" class="register-link">Identifiez-vous</router-link></p>
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
import axios from 'axios';

const router = useRouter();
const isLoading = ref(false);
const requestSent = ref(false);
const errorMessage = ref("");

const form = reactive({
  nomEntreprise: '',
  nomResponsable: '',
  emailResponsable: '',
  matriculeFiscale: ''
});

const goToLogin = () => router.push('/login');

const handleRegister = async () => {
  errorMessage.value = "";
  isLoading.value = true;
  try {
    await new Promise(r => setTimeout(r, 1500));
    await axios.post('http://localhost:5172/api/Registration', form);
    requestSent.value = true;
  } catch (err) {
    errorMessage.value = err.response?.data || "Une erreur technique est survenue.";
  } finally {
    isLoading.value = false;
  }
};
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;500;600;700;800&display=swap');

.elite-register-root {
  font-family: 'Plus Jakarta Sans', sans-serif;
  min-height: 100vh; display: flex; justify-content: center; align-items: center;
  background: #fdfdfd; position: relative; overflow: hidden; padding: 20px;
}

.luxury-bg { position: absolute; inset: 0; z-index: 1; }
.aura-sphere { position: absolute; border-radius: 50%; filter: blur(140px); opacity: 0.25; animation: orbit 25s infinite linear; }
.sphere-amber { width: 600px; height: 600px; background: #fbbf24; top: -10%; left: -10%; }
.sphere-blue  { width: 500px; height: 500px; background: #60a5fa; bottom: -10%; right: -5%; }
.sphere-rose  { width: 400px; height: 400px; background: #fda4af; top: 35%; right: 15%; opacity: 0.1; }
.mesh-grain { position: absolute; inset: 0; opacity: 0.03; background-image: url("data:image/svg+xml,%3Csvg viewBox='0 0 200 200' xmlns='http://www.w3.org/2000/svg'%3e%3cfilter id='n'%3e%3cfeTurbulence type='fractalNoise' baseFrequency='0.65' numOctaves='3'/%3e%3c/filter%3e%3crect width='100%25' height='100%25' filter='url(%23n)'/%3e%3c/svg%3e"); }
@keyframes orbit { from { transform: rotate(0) translate(40px) rotate(0); } to { transform: rotate(360deg) translate(40px) rotate(-360deg); } }

.register-master-container { z-index: 10; width: 100%; max-width: 510px; margin-top: 50px; }

.bento-register-card {
  background: rgba(255, 255, 255, 0.98); backdrop-filter: blur(40px);
  border-radius: 50px; padding: 75px 50px 50px; border: 1px solid white;
  box-shadow: 0 40px 100px -20px rgba(0,0,0,0.06); position: relative; overflow: visible;
}

/* ROBOT */
.bot-anchor { position: absolute; top: -90px; left: 50%; transform: translateX(-50%); }
.robot-float { animation: floatBot 4s ease-in-out infinite; }
.master-bot-register { width: 155px; filter: drop-shadow(0 20px 40px rgba(0,0,0,0.08)); }
.signal-ping { animation: ping 3s infinite; transform-origin: center; }
.bot-shadow { animation: shadowEase 4s ease-in-out infinite; transform-origin: center; }
.led-blink { animation: blink 4s infinite; transform-origin: center; }

@keyframes floatBot { 0%, 100% { transform: translateY(0); } 50% { transform: translateY(-15px); } }
@keyframes ping { 0% { r: 5; opacity: 0.8; } 100% { r: 38; opacity: 0; } }
@keyframes blink { 0%, 90%, 100% { transform: scaleY(1); } 95% { transform: scaleY(0.1); } }
@keyframes shadowEase { 0%, 100% { transform: scale(1); opacity: 0.05; } 50% { transform: scale(0.88); opacity: 0.02; } }

/* HEADER */
.brand-logo { font-size: 36px; font-weight: 900; color: #0f172a; margin: 0; text-align: center; letter-spacing: -2px; }
.brand-logo span { color: #fbbf24; }
.brand-tagline { color: #94a3b8; font-size: 11px; font-weight: 800; text-align: center; letter-spacing: 2px; margin-top: 5px; }
.header-line { width: 45px; height: 5px; background: #fbbf24; border-radius: 5px; margin: 18px auto 32px; }

/* INPUTS */
.grid-inputs { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; }
.field-premium { margin-bottom: 20px; }
.field-premium label { display: block; font-size: 11px; font-weight: 800; color: #475569; margin-bottom: 9px; text-transform: uppercase; letter-spacing: 0.5px; }
.opt-tag { color: #cbd5e1; font-weight: 400; text-transform: none; }

.input-glow-wrapper { position: relative; display: flex; align-items: center; }
.input-glow-wrapper i { position: absolute; left: 17px; color: #fbbf24; font-size: 15px; transition: 0.3s; }
.input-glow-wrapper input {
  width: 100%; padding: 16px 18px 16px 50px; border-radius: 18px; border: 1.5px solid #f1f5f9;
  background: #f8fafc; font-size: 14px; color: #1e293b; font-family: inherit; transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
}
.input-glow-wrapper input:focus {
  outline: none; border-color: #fbbf24; background: white;
  box-shadow: 0 10px 22px rgba(251, 191, 36, 0.1); transform: translateY(-2px);
}

/* BUTTON */
.btn-sunburst {
  width: 100%; padding: 19px; border-radius: 22px; border: none;
  background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
  color: #0f172a; font-weight: 800; cursor: pointer; position: relative;
  overflow: hidden; font-family: inherit; transition: all 0.4s;
  box-shadow: 0 15px 35px rgba(251, 191, 36, 0.3); margin-top: 6px;
}
.btn-sunburst:hover { transform: translateY(-4px); box-shadow: 0 20px 45px rgba(251, 191, 36, 0.45); }
.shine-sweep {
  position: absolute; top: 0; left: -100%; width: 60%; height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255,255,255,0.4), transparent);
  animation: sweep 4s infinite;
}
@keyframes sweep { 0% { left: -120%; } 30% { left: 150%; } 100% { left: 150%; } }
.btn-label { display: flex; justify-content: center; align-items: center; gap: 12px; }

/* SUCCESS */
.success-portal { text-align: center; padding: 30px 0; }
.check-circle-anim { width: 80px; height: 80px; background: #fef3c7; color: #fbbf24; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-size: 38px; margin: 0 auto 20px; animation: zoomIn 0.5s; }
.btn-return { margin-top: 20px; background: #0f172a; color: white; padding: 13px 28px; border-radius: 14px; border: none; cursor: pointer; font-weight: 700; font-family: inherit; }

/* FOOTER */
.portal-footer { margin-top: 36px; text-align: center; }
.portal-footer p { font-size: 14px; color: #94a3b8; margin-bottom: 18px; }
.register-link { color: #0f172a; font-weight: 800; text-decoration: none; border-bottom: 2px solid #fbbf24; padding-bottom: 2px; }
.back-home-link { font-size: 13px; color: #94a3b8; text-decoration: none; display: flex; align-items: center; justify-content: center; gap: 10px; transition: 0.3s; }
.back-home-link:hover { color: #fbbf24; transform: translateX(-5px); }

/* MISC */
.error-banner { background: #fff1f2; color: #e11d48; padding: 14px; border-radius: 15px; margin-bottom: 22px; font-size: 13px; font-weight: 600; text-align: center; border: 1px solid #ffe4e6; }
.btn-loader { display: flex; gap: 6px; justify-content: center; }
.btn-loader span { width: 8px; height: 8px; background: #0f172a; border-radius: 50%; animation: loaderPulse 0.6s infinite alternate; }
.btn-loader span:nth-child(2) { animation-delay: 0.2s; }
.btn-loader span:nth-child(3) { animation-delay: 0.4s; }
@keyframes loaderPulse { from { transform: scale(1); opacity: 1; } to { transform: scale(1.5); opacity: 0.3; } }

.fade-slide-enter-active, .fade-slide-leave-active { transition: all 0.4s ease; }
.fade-slide-enter-from { opacity: 0; transform: translateY(10px); }
.fade-slide-leave-to { opacity: 0; transform: translateY(-10px); }

@media (max-width: 600px) {
  .bento-register-card { padding: 75px 28px 40px; border-radius: 36px; }
  .grid-inputs { grid-template-columns: 1fr; }
}
</style>