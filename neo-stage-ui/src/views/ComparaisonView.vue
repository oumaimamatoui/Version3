<template>
  <div class="d-flex admin-body">
    <AppSidebar />
    <div class="content flex-grow-1 p-4 text-white">
      <h3 class="mb-4"><i class="fa fa-columns text-primary me-2"></i> Comparateur de Profils</h3>

      <div class="row g-4">
        <div class="col-md-6" v-for="c in candidatesToCompare" :key="c.id">
          <div class="bg-secondary p-4 rounded shadow border-top border-4" :class="c.color">
            <div class="text-center mb-4">
              <img :src="c.avatar" class="rounded-circle mb-2" width="80">
              <h5>{{ c.name }}</h5>
              <span class="badge bg-dark border border-secondary">{{ c.title }}</span>
            </div>
            
            <div class="comparison-stats">
              <div class="mb-3">
                <label class="small text-muted d-block">Score Technique</label>
                <div class="d-flex align-items-center">
                  <div class="progress flex-grow-1" style="height: 8px;">
                    <div class="progress-bar" :class="c.bg" :style="{width: c.score + '%'}"></div>
                  </div>
                  <span class="ms-2 fw-bold">{{ c.score }}%</span>
                </div>
              </div>
              <div class="mb-3">
                <label class="small text-muted d-block">Rapidité d'exécution</label>
                <p>{{ c.speed }}</p>
              </div>
              <div class="mb-3">
                <label class="small text-muted d-block">Points Forts IA</label>
                <p class="small">{{ c.strengths }}</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import AppSidebar from '../components/AppSidebar.vue';

const candidatesToCompare = [
  { id: 1, name: "Ali Salah", title: "Fullstack JS", score: 88, speed: "Rapide (12min)", strengths: "Excellente gestion de l'état Vuex/Pinia.", color: "border-primary", bg: "bg-primary", avatar: "https://i.pravatar.cc/150?u=1" },
  { id: 2, name: "Sonia M.", title: "Frontend Vue", score: 82, speed: "Moyenne (18min)", strengths: "Design System & CSS ultra précis.", color: "border-info", bg: "bg-info", avatar: "https://i.pravatar.cc/150?u=2" }
];
</script>