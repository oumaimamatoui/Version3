<template>
  <div class="p-4">
    <h2 class="fw-bold mb-4">Analyse des Groupes</h2>
    <div class="row g-4">
      <div class="col-lg-8">
        <div class="card p-4 shadow-sm border-0 rounded-4">
          <h5>Classement Top Candidats</h5>
          <!-- Liste simplifiée pour le code -->
          <div class="mt-3">
            <div class="d-flex justify-content-between p-3 bg-light rounded-3 mb-2">
              <span>Jean Dupont (Groupe Info)</span>
              <span class="fw-bold text-success">98%</span>
            </div>
          </div>
        </div>
      </div>
      <div class="col-lg-4">
        <div class="card p-4 bg-primary text-white border-0 rounded-4">
          <h5>Smart Analysis IA</h5>
          <p class="small opacity-75">Le Groupe "Fintech" a une progression de 15% supérieure à la moyenne.</p>
        </div>
      </div>
    </div>
  </div>
</template>
<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';

const statsIA = ref(null);

onMounted(async () => {
  // On récupère les détails de l'évaluation calculée par le backend
  const res = await axios.get(`http://localhost:5172/api/Candidatures/${route.params.id}`);
  statsIA.value = res.data.evaluation;
  // Ici, tu peux injecter statsIA.value.score dans ton graphique Radar
});
</script>
<style>
.sidebar-tech { background: #0f172a; height: 100vh; min-width: 280px; }
.nav-link-tech { color: #94a3b8; padding: 12px; border-radius: 12px; display: flex; align-items: center; gap: 15px; text-decoration: none; }
.nav-link-tech:hover, .router-link-active { background: rgba(255,255,255,0.1); color: #fff; }
.icon-shell { width: 35px; height: 35px; display: flex; align-items: center; justify-content: center; border-radius: 8px; background: rgba(255,255,255,0.05); }
.role-pill { background: #eab308; color: #000; font-weight: bold; padding: 4px 12px; border-radius: 50px; font-size: 10px; }
.bg-primary-soft { background: #eef2ff; }
</style>