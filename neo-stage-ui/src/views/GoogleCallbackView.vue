<template>
  <div class="callback-container d-flex align-items-center justify-content-center vh-100">
    <div class="text-center animate__animated animate__fadeIn">
      <div v-if="status === 'loading'">
        <div class="spinner-border text-primary mb-3" role="status"></div>
        <h4 class="fw-bold">Liaison de votre compte Google...</h4>
        <p class="text-muted">Veuillez ne pas fermer cette fenêtre.</p>
      </div>
      
      <div v-else-if="status === 'success'">
        <div class="success-icon mb-3">
          <i class="fa-solid fa-circle-check text-success fa-4x"></i>
        </div>
        <h4 class="fw-bold">Succès !</h4>
        <p class="text-muted">Votre compte Gmail est désormais lié à EvaluaTech.</p>
        <button @click="finish" class="btn btn-primary mt-3 px-5">RETOURNER AUX PARAMÈTRES</button>
      </div>

      <div v-else-if="status === 'error'">
        <div class="error-icon mb-3">
          <i class="fa-solid fa-circle-xmark text-danger fa-4x"></i>
        </div>
        <h4 class="fw-bold">Erreur d'enrôlement</h4>
        <p class="text-muted">{{ errorMessage }}</p>
        <button @click="finish" class="btn btn-outline-secondary mt-3">RÉESSAYER</button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import api from '@/services/api';

const route = useRoute();
const router = useRouter();
const status = ref('loading');
const errorMessage = ref('');

const finalizeConnection = async () => {
    const code = route.query.code;
    
    if (!code) {
        status.value = 'error';
        errorMessage.value = "Aucun code d'autorisation n'a été reçu de Google.";
        return;
    }

    try {
        await api.post('/GoogleAuth/callback', { code });
        status.value = 'success';
    } catch (err) {
        status.value = 'error';
        errorMessage.value = err.response?.data?.message || "Une erreur est survenue lors de l'échange des tokens.";
    }
};

const finish = () => {
    router.push('/settings');
};

onMounted(finalizeConnection);
</script>

<style scoped>
.callback-container {
  background-color: #f8fafc;
}
</style>
