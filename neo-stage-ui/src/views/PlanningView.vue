<template>
  <div class="d-flex admin-body">
    <AppSidebar />
    <div class="content flex-grow-1 bg-main">
      <AppNavbar />
      <div class="p-4 pt-2">
        <header class="d-flex justify-content-between align-items-center mb-4">
          <div>
            <h2 class="page-title fw-800 text-slate-800">Planification des Évaluations</h2>
            <p class="text-muted small">Associez vos questionnaires aux dates de passage (Logique UML)</p>
          </div>
          <div class="badge-tech">
            <i class="fa-solid fa-server me-2"></i> Synchro BDD Active
          </div>
        </header>

        <div class="row g-4">
          <!-- CALENDRIER DYNAMIQUE -->
          <div class="col-lg-8">
            <div class="glass-card p-4 rounded-4 shadow-sm h-100 bg-white border-0">
              <div class="d-flex justify-content-between align-items-center mb-4">
                <h6 class="fw-bold m-0 text-uppercase ls-1">Calendrier Mars 2026</h6>
              </div>
              
              <div class="calendar-grid">
                <div v-for="day in ['Lun','Mar','Mer','Jeu','Ven','Sam','Dim']" :key="day" class="calendar-header">{{ day }}</div>
                
                <div v-for="n in 31" :key="n" class="calendar-day">
                  <span class="day-num">{{ n }}</span>
                  <!-- Affichage des événements selon le Backend -->
                  <div v-for="sess in eventsOnCalendar" :key="sess.id">
                     <div v-if="new Date(sess.debut).getDate() === n" class="event-pill">
                       {{ sess.titre }}
                     </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- FORMULAIRE (SYNC UML CAMPAGNE -> EVALUATION) -->
          <div class="col-lg-4">
            <div class="glass-card p-4 rounded-4 shadow-sm border-0 bg-white">
              <div class="d-flex align-items-center gap-2 mb-4">
                <div class="icon-sq bg-indigo"><i class="fa-solid fa-plus-circle"></i></div>
                <h6 class="fw-bold m-0">Nouvelle Session</h6>
              </div>
              
              <div class="form-wrapper">
                <!-- Choix de la campagne -->
                <div class="mb-3">
                  <label class="q-label">Choisir Campagne (UML)</label>
                  <select v-model="form.campagneId" class="q-select">
                    <option disabled value="">Sélectionnez un projet...</option>
                    <option v-for="c in listCampagnes" :key="c.id" :value="c.id">{{ c.nom }}</option>
                  </select>
                </div>

                <!-- Durée mapt m3a limiteTemps fil UML -->
                <div class="mb-3">
                  <label class="q-label">Durée Evaluation (min)</label>
                  <input type="number" v-model.number="form.duree" class="q-input" placeholder="ex: 60">
                </div>

                <!-- dateDebut fil UML -->
                <div class="mb-3">
                  <label class="q-label">Date d'Ouverture</label>
                  <input type="datetime-local" v-model="form.dateOuverture" class="q-input">
                </div>

                <!-- IA Scoring Logic -->
                <div class="mb-4">
                  <label class="q-label">Notation Intelligence Artificielle</label>
                  <select v-model="form.modeNotation" class="q-select fw-bold text-indigo">
                    <option value="IA">IA Scoring Automatique (Standard)</option>
                    <option value="Manuel">Validation RH Manuelle</option>
                  </select>
                </div>

                <button @click="validerPlanning" class="btn-primary-gradient w-100 py-3" :disabled="loading">
                  <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
                  ACTIVER LA SESSION
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';

const API_BASE = "http://localhost:5172/api";

// États
const listCampagnes = ref([]);
const eventsOnCalendar = ref([]);
const loading = ref(false);

const form = ref({
  campagneId: '',
  duree: 60,
  dateOuverture: '',
  modeNotation: 'IA'
});

onMounted(() => {
  chargerDonnees();
});

const chargerDonnees = async () => {
  try {
    // 1. Recupération des campagnes déjà créées dans le Builder
    const resCamp = await axios.get(`${API_BASE}/Campagnes`);
    listCampagnes.value = resCamp.data;

    // 2. Recupération du planning pour le calendrier
    const resPlan = await axios.get(`${API_BASE}/Plannings/calendrier`);
    eventsOnCalendar.value = resPlan.data; // Formats attendus: [{id, titre, debut}]
  } catch (err) {
    console.error("Problème API :", err);
  }
};

const validerPlanning = async () => {
  if (!form.value.campagneId || !form.value.dateOuverture) {
    alert("Veuillez remplir tout le formulaire de planification.");
    return;
  }

  loading.value = true;
  try {
    const payload = {
      campagneId: form.value.campagneId,
      dureeMinutes: form.value.duree,
      dateOuverture: form.value.dateOuverture,
      modeNotation: form.value.modeNotation
    };

    await axios.post(`${API_BASE}/Plannings`, payload);
    alert("La planification a été enregistrée avec succès.");
    
    // Refresh
    form.value.campagneId = '';
    chargerDonnees();
  } catch (err) {
    alert("Erreur système. Veuillez vérifier la connexion au serveur.");
  } finally {
    loading.value = false;
  }
};
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@700;800&display=swap');

.bg-main { background-color: #f8fafc; font-family: 'Plus Jakarta Sans', sans-serif; min-height: 100vh; }
.page-title { letter-spacing: -1px; }

/* CALENDRIER UI */
.calendar-grid { display: grid; grid-template-columns: repeat(7, 1fr); gap: 8px; }
.calendar-header { padding: 15px; text-align: center; font-weight: 800; font-size: 11px; color: #94a3b8; text-transform: uppercase; }
.calendar-day { background: #fcfcfd; border: 1px solid #f1f5f9; min-height: 110px; padding: 10px; border-radius: 12px; }
.day-num { font-size: 13px; font-weight: 800; color: #cbd5e1; }

.event-pill {
  background: #4f46e5; color: white; padding: 4px 8px; 
  border-radius: 6px; font-size: 9px; font-weight: 700; margin-top: 5px;
  white-space: nowrap; overflow: hidden; text-overflow: ellipsis;
}

/* FORM STYLE */
.q-label { display: block; font-size: 11px; font-weight: 800; color: #64748b; text-transform: uppercase; margin-bottom: 6px; }
.q-input, .q-select { 
  width: 100%; padding: 12px 15px; border: 1.5px solid #eef2f6; 
  border-radius: 12px; background: #f9fafb; font-size: 14px; font-weight: 600; outline: none; transition: 0.3s;
}
.q-input:focus { border-color: #4f46e5; background: white; }

.btn-primary-gradient {
  background: linear-gradient(135deg, #1e293b, #0f172a); color: white;
  border: none; border-radius: 12px; font-weight: 800; transition: 0.3s;
}
.btn-primary-gradient:hover { transform: translateY(-2px); box-shadow: 0 10px 20px rgba(0,0,0,0.1); }

.icon-sq { width: 36px; height: 36px; border-radius: 10px; display: flex; align-items: center; justify-content: center; color: white; }
.bg-indigo { background: #4f46e5; }
.ls-1 { letter-spacing: 1px; }
.badge-tech { background: #ecfdf5; color: #059669; padding: 8px 15px; border-radius: 20px; font-size: 12px; font-weight: 700; }
</style>