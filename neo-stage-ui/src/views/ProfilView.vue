<template>
  <div class="d-flex admin-body">
    <AppSidebar />
    <div class="content flex-grow-1">
      <AppNavbar />
      <div class="p-4 pt-0 text-slate-800">
        <div class="row g-4">
          
        
          <div class="col-lg-4">
            <div class="glass-card p-4 rounded-5 shadow-sm text-center border-0 animate__animated animate__fadeInLeft">
              
          
              <div class="profile-avatar-wrapper mx-auto mb-4" @click="triggerFileInput">
                <img :src="profileDisplayUrl" class="profile-img shadow-lg" alt="Photo de profil">
                <div class="photo-overlay">
                  <i class="fa fa-camera"></i>
                  <span>Changer</span>
                </div>
                <div class="status-indicator online"></div>
              
                <input type="file" ref="fileInput" @change="onFileChange" hidden accept="image/*">
              </div>

              <h4 class="fw-bold text-slate-800 mb-1">{{ user.prenom }} {{ user.nom }}</h4>
              <p class="text-indigo small fw-bold mb-4">{{ roleDisplay }}</p>
              
              <div class="d-flex justify-content-center gap-2">
                <button class="btn btn-light-pro btn-sm shadow-sm px-3">
                  <i class="fa fa-share-alt me-1"></i> Partager
                </button>
                <button @click="goToSettings" class="btn btn-indigo btn-sm shadow-sm px-3">
                  <i class="fa fa-edit me-1"></i> Editer
                </button>
              </div>
            </div>
          </div>

       
          <div class="col-lg-8">
            <div class="glass-card p-4 rounded-5 shadow-sm border-0 animate__animated animate__fadeInRight">
              <h6 class="fw-bold text-slate-800 mb-4">
                <i class="fa fa-award text-warning me-2"></i>Compétences validées par EvaluaTech IA
              </h6>
              
              <div class="d-flex flex-wrap gap-3 mb-5">
                <div v-for="badge in badges" :key="badge.name" class="badge-card text-center p-3 rounded-4 border border-light">
                  <div class="icon-badge mb-2"><i :class="badge.icon"></i></div>
                  <div class="fw-bold tiny text-slate-700">{{ badge.name }}</div>
                  <div class="text-success tiny fw-bold mt-1">
                    <i class="fa fa-check-circle small"></i> Validé
                  </div>
                </div>
              </div>

              <h6 class="fw-bold text-slate-800 mb-3 border-bottom pb-2">Informations Générales</h6>
              <div class="row g-4">
                <div class="col-md-6">
                  <label class="tiny text-muted uppercase d-block mb-1">Email Professionnel</label>
                  <span class="fw-semibold">{{ user.email }}</span>
                </div>
                <div class="col-md-6">
                  <label class="tiny text-muted uppercase d-block mb-1">Inscrit depuis</label>
                  <span class="fw-semibold">{{ user.joinDate }}</span>
                </div>
                <div class="col-md-12">
                  <label class="tiny text-muted uppercase d-block mb-1">Bio Technique (Analyse IA)</label>
                  <p class="small text-muted mt-1 leading-relaxed">
                    Passionnée par l'écosystème .NET 10 et Vue.js 3. Expertise démontrée lors des tests en architecture micro-services et gestion d'état Pinia. 
                    L'IA suggère un fort potentiel pour des rôles de <strong>Lead Developer</strong>.
                  </p>
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
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import api from '@/services/api';
import { useAuthStore } from '@/stores/auth';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';

const router = useRouter();
const authStore = useAuthStore();

const user = ref({ nom: '', prenom: '', email: '', photoUrl: '', joinDate: '' });
const loading = ref(true);

const profileDisplayUrl = computed(() => {
  if (user.value.photoUrl) {
    return `http://localhost:5172${user.value.photoUrl}`;
  }
  return 'https://images.unsplash.com/photo-1494790108377-be9c29b29330?ixlib=rb-1.2.1&auto=format&fit=crop&w=256&q=80';
});

const roleDisplay = computed(() => {
  const map = { 
    'SuperAdmin': 'Master Platform Admin', 
    'AdminEntreprise': 'Administrateur Organisation', 
    'Formateur': 'Évaluateur Expert', 
    'Candidat': 'Candidat' 
  };
  return map[authStore.role] || 'Chargement...';
});

const fetchProfile = async () => {
  try {
    const res = await api.get('/Settings/me');
    user.value = res.data;
  } catch (error) {
    console.error("Erreur profil:", error);
  } finally {
    loading.value = false;
  }
};

onMounted(fetchProfile);


const triggerFileInput = () => {
  fileInput.value.click();
};

const onFileChange = (e) => {
  const file = e.target.files[0];
  if (file) {
    profileImage.value = URL.createObjectURL(file);
    alert("Photo mise à jour localement pour la démo !");
  }
};

const goToSettings = () => {
  router.push('/settings');
};
</script>

<style scoped>
.admin-body { background: #f8fafc; min-height: 100vh; font-family: 'Plus Jakarta Sans', sans-serif; }
.glass-card { background: white; border: 1px solid #e2e8f0; }


.profile-avatar-wrapper { 
  position: relative; 
  width: 130px; 
  height: 130px; 
  cursor: pointer;
}

.profile-img { 
  width: 100%; 
  height: 100%; 
  object-fit: cover; 
  border-radius: 24px; 
  border: 4px solid white;
}

.photo-overlay {
  position: absolute;
  top: 0; left: 0; width: 100%; height: 100%;
  background: rgba(79, 70, 229, 0.6);
  color: white;
  border-radius: 24px;
  display: flex; flex-direction: column;
  align-items: center; justify-content: center;
  opacity: 0;
  transition: 0.3s;
}

.profile-avatar-wrapper:hover .photo-overlay {
  opacity: 1;
}

.photo-overlay i { font-size: 24px; margin-bottom: 5px; }
.photo-overlay span { font-size: 10px; font-weight: 700; text-transform: uppercase; }

.status-indicator { 
  position: absolute; bottom: -5px; right: -5px; 
  width: 22px; height: 22px; 
  border-radius: 50%; border: 4px solid white; 
}
.status-indicator.online { background: #22c55e; }


.badge-card { background: #f8fafc; min-width: 110px; transition: 0.3s; border-color: #f1f5f9 !important; }
.badge-card:hover { transform: translateY(-5px); border-color: #4f46e5 !important; box-shadow: 0 10px 20px rgba(0,0,0,0.05); }
.icon-badge { font-size: 28px; }


.btn-indigo { background: #4f46e5; color: white; border: none; font-weight: 700; border-radius: 10px; }
.btn-indigo:hover { background: #3730a3; }
.btn-light-pro { background: white; border: 1px solid #e2e8f0; color: #64748b; font-weight: 700; border-radius: 10px; }

.text-indigo { color: #4f46e5 !important; }
.tiny { font-size: 11px; }
.uppercase { text-transform: uppercase; letter-spacing: 1px; }
.leading-relaxed { line-height: 1.6; }
</style>