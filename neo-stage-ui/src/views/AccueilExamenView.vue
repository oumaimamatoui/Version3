<template>
  <div class="premium-secure-viewport">
    <div class="background-overlay"></div>
    <div class="glow-orb orb-amber"></div>
    <div class="glow-orb orb-blue"></div>

    <div v-if="loading" class="centering-layout">
        <div class="spinner-pro-premium"></div>
        <h2 class="ms-3 fw-800">CHARGEMENT DE LA SESSION...</h2>
    </div>

    <div v-else-if="errorMsg" class="centering-layout">
        <div class="card-glass-setup text-center">
            <i class="fa-solid fa-circle-xmark fa-4x text-danger mb-4"></i>
            <h1 class="title-display">Accès <span>Refusé</span></h1>
            <p class="text-muted fw-bold">{{ errorMsg }}</p>
            <button @click="router.push('/dashboard')" class="btn-img btn-dark mt-4 w-100">RETOUR AU TERMINAL</button>
        </div>
    </div>

    <template v-else>
        <main class="main-terminal">
            <div v-if="!examStarted" class="centering-layout">
                <div class="card-glass-setup animate__animated animate__zoomIn">
                    <h1 class="title-display">Session <span>{{ campaignName }}</span></h1>
                    <p class="text-muted mt-3">Prêt à commencer l'évaluation ?</p>
                    <button @click="initiateSession" class="btn-img btn-amber px-5 mt-5">DÉMARRER LE TEST</button>
                </div>
            </div>
            
            <div v-else class="centering-layout">
                 <div class="surface-card-q">
                    <h2 class="fw-800">{{ questions[currentIndex]?.texte }}</h2>
                    <!-- ... Votre logique de Quiz ... -->
                    <button @click="router.push('/dashboard')" class="btn-img btn-gray mt-5">QUITTER</button>
                 </div>
            </div>
        </main>
    </template>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'; // Ajoutez computed
import { useRoute, useRouter } from 'vue-router';
import axios from 'axios';

const route = useRoute();
const router = useRouter();

// --- ETATS ---
const loading = ref(true);
const errorMsg = ref(null);
const examStarted = ref(false);
const quizFinished = ref(false);
const campaignName = ref("");
const questions = ref([]);
const currentIndex = ref(0);
const evaluationId = ref(null);

// ✅ FIX : Ajoutez ces variables pour stopper l'erreur "Property securityBreach not defined"
const isFocused = ref(true);
const isFullscreen = ref(false);
const securityBreach = computed(() => !isFocused.value || !isFullscreen.value);

const loadData = async () => {
    const cid = route.params.id; 

    // ✅ FIX : Empêche l'appel si l'ID est "undefined"
    if (!cid || cid === 'undefined') {
        errorMsg.value = "Identifiant de session invalide. Veuillez repasser par le Dashboard.";
        loading.value = false;
        return;
    }

    try {
        const res = await axios.get(`http://localhost:5172/api/Examen/setup/${cid}`);
        evaluationId.value = res.data.evaluationId;
        questions.value = res.data.questions;
        campaignName.value = res.data.campagneNom;
        loading.value = false;
    } catch (err) {
        // ✅ FIX : Affiche le message d'erreur du backend
        errorMsg.value = err.response?.data?.message || "Erreur de connexion au serveur.";
        loading.value = false;
    }
};

onMounted(loadData);
</script>
<style scoped>
/* Mix de vos styles précédents */
.premium-secure-viewport { height: 100vh; background: #f8fafc; overflow: hidden; position: relative; font-family: 'Plus Jakarta Sans'; }
.background-overlay { position: absolute; inset: 0; background: radial-gradient(circle at 30% 30%, #fff 0%, #f1f5f9 100%); }
.glow-orb { position: absolute; border-radius: 50%; filter: blur(130px); opacity: 0.15; }
.orb-amber { width: 600px; height: 600px; background: #fbbf24; top: -100px; right: -100px; }
.orb-blue { width: 500px; height: 500px; background: #60a5fa; bottom: -100px; left: -100px; }
.centering-layout { height: 100%; display: flex; align-items: center; justify-content: center; z-index: 10; position: relative; }
.card-glass-setup, .surface-card-q { background: white; border-radius: 35px; box-shadow: 0 30px 60px rgba(0,0,0,0.05); padding: 50px; }
.surface-card-q { width: 850px; }
.option-card { padding: 20px; background: #f8fafc; border: 2px solid #f1f5f9; border-radius: 20px; margin-bottom: 10px; cursor: pointer; transition: 0.2s; font-weight: 700; }
.option-card.selected { background: #0f172a; color: white; border-color: #0f172a; }
.btn-img { padding: 15px 30px; border-radius: 15px; border: none; font-weight: 800; cursor: pointer; }
.btn-amber { background: #eab308; }
.btn-gray { background: #e2e8f0; }
.btn-dark { background: #0f172a; color: white; }
.timer-box { background: #0f172a; color: white; padding: 10px 20px; border-radius: 10px; font-weight: 800; }
</style>