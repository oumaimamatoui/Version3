<template>
  <div class="quiz-screen">
    <div v-if="!quizFinished" class="container p-5">
      <div class="progress mb-4 shadow-sm" style="height: 10px; border-radius: 50px;">
        <div class="progress-bar bg-primary progress-bar-striped progress-bar-animated" :style="{width: (currentIndex + 1) * (100 / questions.length) + '%'}"></div>
      </div>

      <div class="bg-white p-5 rounded-4 shadow border border-light">
        <div class="d-flex justify-content-between align-items-center mb-4">
          <span class="badge bg-light text-primary border px-3 py-2">Question {{ currentIndex + 1 }} / {{ questions.length }}</span>
          <div class="timer text-danger fw-bold"><i class="fa fa-clock me-1"></i> 14:55</div>
        </div>

        <h3 class="fw-bold text-slate-800 mb-4">{{ questions[currentIndex].text }}</h3>

        <div class="options-list">
          <div 
            v-for="(option, idx) in questions[currentIndex].options" 
            :key="idx"
            @click="selectOption(idx)"
            :class="['option-card p-3 mb-3 rounded-3 border', selectedOption === idx ? 'selected' : '']"
          >
            <div class="d-flex align-items-center">
              <div class="option-indicator me-3">{{ String.fromCharCode(65 + idx) }}</div>
              {{ option }}
            </div>
          </div>
        </div>

        <div class="d-flex justify-content-between mt-5">
          <button class="btn btn-light px-4 shadow-sm" @click="currentIndex--" :disabled="currentIndex === 0">Précédent</button>
          <button @click="nextQuestion" class="btn btn-primary px-5 py-2 shadow" :disabled="selectedOption === null">
            {{ currentIndex === questions.length - 1 ? 'Terminer l\'examen' : 'Suivant' }}
          </button>
        </div>
      </div>
    </div>

  
    <div v-else class="container text-center p-5">
      <div class="bg-white p-5 rounded-4 shadow-lg border border-light animate__animated animate__zoomIn">
        <i class="fa fa-check-circle fa-5x text-success mb-4"></i>
        <h1 class="fw-bold">Examen Soumis !</h1>
        <p class="lead text-muted">Vos réponses sont en cours d'analyse par l'IA.</p>
        <div class="alert alert-warning border-0 d-inline-block px-4">
          <i class="fa fa-shield-alt me-2"></i> Intégrité du test : <strong>98% (Aucune triche détectée)</strong>
        </div>
        <br>
        <router-link to="/ResultsView" class="btn btn-primary btn-lg px-5 mt-4 rounded-pill shadow">Voir mes résultats IA</router-link>
      </div>
    </div>
  </div>
</template>
<script setup>
import { ref } from 'vue';
import axios from 'axios';

const reponsesCandidat = ref([]); // Tableau des choix

const finaliserExamen = async () => {
  try {
    const payload = {
      candidatId: localStorage.getItem('userId'),
      campagneId: route.params.id,
      reponses: reponsesCandidat.value 
    };

    // Cet appel déclenche le calcul IA dans le backend .NET
    const res = await axios.post('http://localhost:5172/api/Candidatures/postuler', payload);
    
    alert("Examen terminé ! Votre score IA est de : " + res.data.score + "%");
    router.push('/resultats');
  } catch (e) {
    alert("Erreur lors de la soumission");
  }
};
</script>

<style scoped>
.quiz-screen { min-height: 100vh; background: #f0f4f8; display: flex; align-items: center; justify-content: center; }
.option-card { cursor: pointer; transition: 0.2s; border-color: #e2e8f0; background: #f8fafc; font-weight: 500; }
.option-card:hover { border-color: #4f46e5; background: #f5f3ff; }
.option-card.selected { border-color: #4f46e5; background: #4f46e5; color: white; }
.option-indicator { width: 30px; height: 30px; border-radius: 50%; background: rgba(0,0,0,0.05); display: flex; align-items: center; justify-content: center; font-size: 12px; }
.selected .option-indicator { background: rgba(255,255,255,0.2); }
</style>