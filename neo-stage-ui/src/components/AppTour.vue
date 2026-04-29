<template>
  <div v-if="isVisible" class="onboarding-overlay">
    <div class="onboarding-card bg-primary text-white p-4 shadow-2xl animate__animated animate__bounceIn">
      <h5>🚀 Bienvenue sur NeoStage !</h5>
      <p class="small mt-2">{{ steps[currentStep].text }}</p>
      <div class="d-flex justify-content-between align-items-center mt-4">
        <span class="small">{{ currentStep + 1 }} / {{ steps.length }}</span>
        <div>
          <button @click="closeTour" class="btn btn-sm btn-link text-white text-decoration-none me-2">Passer</button>
          <button @click="nextStep" class="btn btn-sm btn-light text-primary fw-bold">
            {{ currentStep === steps.length - 1 ? 'Terminer' : 'Suivant' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';

const isVisible = ref(true);
const currentStep = ref(0);

const steps = [
  { text: "Commencez par configurer votre banque de questions assistée par l'IA." },
  { text: "Créez une campagne de recrutement pour générer des liens d'invitation." },
  { text: "Consultez les analyses IA pour prendre les meilleures décisions." }
];

const nextStep = () => {
  if (currentStep.value < steps.length - 1) currentStep.value++;
  else closeTour();
};

const closeTour = () => isVisible.value = false;
</script>

<style scoped>
.onboarding-overlay {
  position: fixed; top: 0; left: 0; width: 100%; height: 100%;
  background: rgba(0,0,0,0.7); z-index: 10000;
  display: flex; align-items: center; justify-content: center;
}
.onboarding-card { width: 350px; border-radius: 15px; }
</style>