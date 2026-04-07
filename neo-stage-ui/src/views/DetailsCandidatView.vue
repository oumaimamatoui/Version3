<template>
  <div class="d-flex admin-layout">
    <AppSidebar />
    <div class="main-content flex-grow-1">
      <AppNavbar />
      
      <!-- LOADING STATE -->
      <div v-if="loading" class="text-center py-5">
        <div class="spinner-cyber"></div>
        <p class="mt-3 fw-800 text-slate-400">ANALYSE DU PROFIL EN COURS...</p>
      </div>

      <!-- ERROR STATE -->
      <div v-else-if="error" class="text-center py-5 animate-in">
        <i class="fa-solid fa-circle-exclamation fa-4x text-danger mb-3"></i>
        <h3 class="fw-800">Candidat introuvable</h3>
        <button @click="$router.push('/candidates-list')" class="btn-back mt-3">Retour à la liste</button>
      </div>

      <!-- CANDIDATE PROFILE -->
      <div v-else-if="candidate" class="p-4 p-lg-5 animate-in">
        <div class="d-flex justify-content-between align-items-center mb-5">
           <button @click="$router.back()" class="btn-back">
             <i class="fa-solid fa-arrow-left me-2"></i> Retour au répertoire
           </button>
           <div class="status-badge-live"><span class="dot"></span> Dossier Actif</div>
        </div>

        <div class="row g-4">
          <!-- LEFT: IDENTITY CARD -->
          <div class="col-lg-4">
            <div class="premium-card text-center p-4 h-100">
              <div class="avatar-container mb-4">
                <img :src="candidate.photoUrl || 'https://via.placeholder.com/150'" class="avatar-img shadow">
                <div class="score-ring shadow-sm">{{ candidate.scoreGlobal }}%</div>
              </div>
              <h4 class="fw-900 text-navy m-0">{{ candidate.fullName }}</h4>
              <p class="text-muted small uppercase fw-bold mb-4">{{ candidate.campaignName }}</p>
              
              <div class="info-grid text-start">
                <div class="info-item mb-3">
                  <label>EMAIL PROFESSIONNEL</label>
                  <span>{{ candidate.email }}</span>
                </div>
                <div class="info-item">
                  <label>ID CANDIDAT</label>
                  <span class="text-slate-400">#{{ candidate.id.split('-')[0] }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- RIGHT: AI VERDICT & SKILLS -->
          <div class="col-lg-8">
            <!-- AI BOX -->
            <div class="ai-verdict-card p-4 mb-4 shadow-sm">
               <div class="d-flex align-items-center mb-3">
                 <div class="ai-icon-box"><i class="fa-solid fa-robot"></i></div>
                 <h5 class="fw-800 m-0 ms-3 text-white">Verdict de l'IA Evaluatech</h5>
               </div>
               <p class="ai-text m-0">{{ candidate.iaVerdict || "L'IA analyse les réponses pour générer un verdict comportemental et technique précis." }}</p>
            </div>

            <!-- SKILLS ANALYSIS -->
            <div class="premium-card p-4 shadow-sm">
              <h6 class="fw-800 mb-4 text-navy"><i class="fa-solid fa-chart-simple me-2"></i>Compétences Techniques</h6>
              <div v-for="s in candidate.skills" :key="s.name" class="skill-item mb-4">
                <div class="d-flex justify-content-between align-items-end mb-2">
                  <span class="skill-name">{{ s.name }}</span>
                  <span class="skill-val">{{ s.val }}%</span>
                </div>
                <div class="modern-progress-bg">
                  <div class="modern-progress-fill" :style="{ width: s.val + '%' }"></div>
                </div>
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
import { useRoute } from 'vue-router';
import axios from 'axios';
import AppSidebar from '@/components/AppSidebar.vue';
import AppNavbar from '@/components/AppNavbar.vue';

const route = useRoute();
const candidate = ref(null);
const loading = ref(true);
const error = ref(false);

const fetchCandidateDetails = async () => {
  loading.value = true;
  try {
    const res = await axios.get(`http://localhost:5172/api/Candidates/${route.params.id}`);
    candidate.value = res.data;
  } catch (err) {
    error.value = true;
  } finally {
    loading.value = false;
  }
};

onMounted(fetchCandidateDetails);
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;600;800&display=swap');

.admin-layout { background: #f8fafc; min-height: 100vh; font-family: 'Plus Jakarta Sans', sans-serif; }

/* AVATAR & SCORE */
.avatar-container { position: relative; display: inline-block; }
.avatar-img { width: 140px; height: 140px; border-radius: 40px; object-fit: cover; border: 5px solid #fff; }
.score-ring { 
  position: absolute; bottom: -10px; right: -10px; 
  background: #f59e0b; color: #fff; width: 50px; height: 50px; 
  border-radius: 50%; display: flex; align-items: center; justify-content: center; 
  font-weight: 900; font-size: 14px; border: 3px solid #fff;
}

/* PREMIUM CARDS */
.premium-card { background: white; border-radius: 30px; border: 1px solid #f1f5f9; box-shadow: 0 10px 30px rgba(0,0,0,0.03); }
.info-item label { display: block; font-size: 9px; font-weight: 800; color: #94a3b8; letter-spacing: 1px; }
.info-item span { font-weight: 700; color: #0f172a; }

/* AI VERDICT */
.ai-verdict-card { background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%); border-radius: 25px; border: 1px solid #334155; }
.ai-icon-box { background: rgba(245, 158, 11, 0.2); color: #f59e0b; width: 40px; height: 40px; border-radius: 12px; display: flex; align-items: center; justify-content: center; font-size: 18px; }
.ai-text { color: #cbd5e1; font-size: 14px; line-height: 1.7; font-weight: 500; }

/* SKILLS */
.skill-name { font-weight: 800; color: #1e293b; font-size: 13px; }
.skill-val { font-weight: 800; color: #f59e0b; font-size: 12px; }
.modern-progress-bg { height: 10px; background: #f1f5f9; border-radius: 20px; overflow: hidden; }
.modern-progress-fill { height: 100%; background: #f59e0b; border-radius: 20px; transition: 1.5s cubic-bezier(0.4, 0, 0.2, 1); }

/* MISC */
.btn-back { background: #fff; border: 1px solid #e2e8f0; padding: 10px 20px; border-radius: 14px; font-weight: 800; color: #0f172a; transition: 0.3s; }
.btn-back:hover { background: #f8fafc; transform: translateX(-5px); }

.spinner-cyber { width: 40px; height: 40px; border: 4px solid #f1f5f9; border-top: 4px solid #f59e0b; border-radius: 50%; animation: spin 1s linear infinite; margin: auto; }
@keyframes spin { 100% { transform: rotate(360deg); } }

.status-badge-live { background: #ecfdf5; color: #10b981; padding: 6px 15px; border-radius: 50px; font-weight: 800; font-size: 11px; display: flex; align-items: center; }
.status-badge-live .dot { width: 8px; height: 8px; background: #10b981; border-radius: 50%; margin-right: 8px; animation: pulse 2s infinite; }

@keyframes pulse { 0% { opacity: 1; } 50% { opacity: 0.4; } 100% { opacity: 1; } }
.animate-in { animation: fadeInUp 0.5s ease-out; }
@keyframes fadeInUp { from { opacity: 0; transform: translateY(20px); } to { opacity: 1; transform: translateY(0); } }
</style>