<template>
  <div class="ai-editor p-3 bg-dark rounded border border-secondary">
    <label class="form-label text-white">Description de l'examen (Assitée par IA)</label>
    <textarea v-model="content" class="form-control bg-secondary text-white border-dark mb-2" rows="4"></textarea>
    
    <div class="d-flex justify-content-between">
      <button @click="improveText" class="btn btn-sm btn-info" :disabled="isImproving">
        <i v-if="isImproving" class="spinner-border spinner-border-sm me-1"></i>
        <i v-else class="fa fa-magic me-1"></i> Améliorer avec l'IA
      </button>
      <small class="text-muted">{{ content.length }} caractères</small>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';

const content = ref("Bienvenue à ce test de recrutement. Bonne chance.");
const isImproving = ref(false);

const improveText = () => {
  isImproving.value = true;
  setTimeout(() => {
    content.value = "Bienvenue à l'évaluation technique NeoStage. Ce test a été conçu par nos experts IA pour évaluer vos compétences réelles. Assurez-vous d'être dans un endroit calme. Bonne chance pour votre carrière !";
    isImproving.value = false;
  }, 1500);
};
</script>