<template>
  <div class="d-flex admin-layout">
    <AppSidebar />
    
    <div class="main-content flex-grow-1">
      <AppNavbar />

      <main class="p-4 p-lg-5 animate-in">
        
        <!-- HEADER & QUICK ACTIONS -->
        <header class="d-flex justify-content-between align-items-end mb-5">
          <div>
            <h2 class="brand-title">Gestion des <span>Candidats</span></h2>
            <p class="brand-subtitle">RÉPERTOIRE GLOBAL ET SUIVI DES TALENTS</p>
          </div>
          
          <div class="d-flex gap-3">
            <button @click="$router.push('/invite')" class="btn-action-outline">
              <i class="fa-solid fa-user-plus me-2"></i> Invite Unique
            </button>
            <button @click="$router.push('/groups')" class="btn-action-gold">
              <i class="fa-solid fa-users-rectangle me-2"></i> Invite par Groupe
            </button>
          </div>
        </header>

        <!-- KPI STATS CARDS (يبقى كما هو) -->
        <div class="row g-4 mb-5">
          <div class="col-md-4" v-for="stat in kpiStats" :key="stat.label">
            <div class="stat-card shadow-sm">
              <div class="stat-icon" :style="{ background: stat.bg, color: stat.color }">
                <i :class="stat.icon"></i>
              </div>
              <div class="ms-3">
                <div class="stat-value">{{ stat.value }}</div>
                <div class="stat-label">{{ stat.label }}</div>
              </div>
            </div>
          </div>
        </div>

        <!-- FILTERS BAR (يبقى كما هو) -->
        <div class="filter-bar premium-card mb-4 shadow-sm">
          <div class="row g-3 align-items-center">
            <div class="col-md-5">
              <div class="search-input-group">
                <i class="fa-solid fa-magnifying-glass"></i>
                <input v-model="search" type="text" placeholder="Rechercher..." class="filter-input">
              </div>
            </div>
            <div class="col-md-4">
              <select v-model="selectedFilter" class="filter-select">
                <option value="">Toutes les Campagnes</option>
                <option v-for="camp in campaigns" :key="camp.id" :value="camp.nom || camp.titre">
                  {{ camp.nom || camp.titre }}
                </option>
              </select>
            </div>
            <div class="col-md-3 text-end">
              <button @click="fetchCandidates" class="btn-refresh">
                <i class="fa-solid fa-rotate"></i> Actualiser
              </button>
            </div>
          </div>
        </div>

        <!-- CANDIDATES TABLE -->
        <div class="premium-card p-0 overflow-hidden shadow-lg border-0">
          <div class="table-responsive">
            <table class="table table-hover align-middle mb-0">
              <thead class="bg-navy text-white">
                <tr>
                  <th class="ps-4 py-3">Candidat</th>
                  <th>E-mail</th>
                  <th>Campagne / Groupe</th>
                  <th>Statut d'Invitation</th>
                  <th class="text-end pe-4">Actions</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="c in filteredCandidates" :key="c.id" class="candidate-row">
                  <td class="ps-4">
                    <div class="d-flex align-items-center">
                      <div class="avatar-initials me-3">
                        {{ c.name ? c.name.charAt(0).toUpperCase() : '?' }}
                      </div>
                      <span class="fw-800 text-navy">{{ c.name || 'Nom non renseigné' }}</span>
                    </div>
                  </td>
                  <td class="text-slate-500 fw-600">{{ c.email }}</td>
                  <td>
                    <span class="group-tag">
                      <i class="fa-solid fa-tag me-1"></i> {{ c.group || 'Aucun groupe' }}
                    </span>
                  </td>
                  <td>
                    <span class="status-pill-modern" :class="getStatusClass(c.status)">
                      {{ c.status || 'Invité' }}
                    </span>
                  </td>
                  <td class="text-end pe-4">
                    <!-- ✅ التحديث هنا: عند الضغط يتم الانتقال لصفحة التفاصيل باستخدام الـ ID -->
                    <button 
                        @click="$router.push({ name: 'details-candidat', params: { id: c.id } })" 
                        class="btn-mini-action" 
                        title="Voir détails"
                    >
                      <i class="fa-solid fa-eye"></i>
                    </button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import api from '@/services/api';
import AppSidebar from '@/components/AppSidebar.vue';
import AppNavbar from '@/components/AppNavbar.vue';

const candidates = ref([]);
const campaigns = ref([]);
const search = ref('');
const selectedFilter = ref('');

const kpiStats = computed(() => [
  { label: 'Total Candidats', value: candidates.value.length, icon: 'fa-solid fa-users', bg: '#fff9e6', color: '#f59e0b' },
  { label: 'Invitations Actives', value: candidates.value.length, icon: 'fa-solid fa-paper-plane', bg: '#eef2ff', color: '#6366f1' },
  { label: 'Groupes Engagés', value: campaigns.value.length, icon: 'fa-solid fa-layer-group', bg: '#f0fff4', color: '#10b981' }
]);

const fetchCandidates = async () => {
  try {
    const res = await api.get('/Candidates');
    candidates.value = Array.isArray(res.data) ? res.data : [];
  } catch (e) { console.error(e); }
};

const fetchCampaigns = async () => {
  try {
    const res = await api.get('/Invitations/campagnes');
    campaigns.value = res.data;
  } catch (e) { console.error(e); }
};

onMounted(() => {
  fetchCandidates();
  fetchCampaigns();
});

const filteredCandidates = computed(() => {
  return candidates.value.filter(c => {
    const matchesSearch = (c.name?.toLowerCase().includes(search.value.toLowerCase()) || 
                          c.email?.toLowerCase().includes(search.value.toLowerCase()) ||
                          c.group?.toLowerCase().includes(search.value.toLowerCase()));
    const matchesGroup = selectedFilter.value === '' || c.group === selectedFilter.value;
    return matchesSearch && matchesGroup;
  });
});

const getStatusClass = (status) => {
  if (!status) return 'status-invited';
  return status.toLowerCase() === 'terminé' ? 'status-done' : 'status-invited';
};
</script>

<style scoped>
/* التنسيق (CSS) يبقى كما هو تماماً */
.admin-layout { background: #f8fafc; min-height: 100vh; font-family: 'Plus Jakarta Sans', sans-serif; }
.brand-title { font-weight: 800; font-size: 2.3rem; letter-spacing: -1px; color: #0f172a; }
.brand-title span { color: #f59e0b; }
.brand-subtitle { font-size: 10px; font-weight: 800; color: #94a3b8; letter-spacing: 2px; }
.btn-action-gold { background: #f59e0b; color: #0f172a; border: none; padding: 12px 25px; border-radius: 14px; font-weight: 800; transition: 0.3s; }
.btn-action-gold:hover { transform: translateY(-3px); box-shadow: 0 10px 20px rgba(245, 158, 11, 0.2); }
.btn-action-outline { background: white; color: #0f172a; border: 2px solid #f1f5f9; padding: 10px 25px; border-radius: 14px; font-weight: 800; transition: 0.3s; }
.btn-action-outline:hover { background: #f1f5f9; }
.stat-card { background: white; padding: 25px; border-radius: 24px; display: flex; align-items: center; border: 1px solid #f1f5f9; }
.stat-icon { width: 50px; height: 50px; border-radius: 14px; display: flex; align-items: center; justify-content: center; font-size: 20px; }
.stat-value { font-size: 24px; font-weight: 800; color: #0f172a; }
.stat-label { font-size: 11px; font-weight: 700; color: #94a3b8; text-transform: uppercase; }
.premium-card { background: white; border-radius: 20px; padding: 20px; }
.search-input-group { position: relative; }
.search-input-group i { position: absolute; left: 15px; top: 50%; transform: translateY(-50%); color: #94a3b8; }
.filter-input, .filter-select { width: 100%; padding: 12px 15px 12px 40px; border-radius: 12px; border: 1.5px solid #f1f5f9; background: #f8fafc; font-weight: 600; outline: none; }
.filter-select { padding-left: 15px; }
.btn-refresh { background: transparent; border: none; color: #64748b; font-weight: 800; font-size: 13px; }
.bg-navy { background: #0f172a; }
.candidate-row:hover { background-color: #fffcf5 !important; }
.avatar-initials { width: 35px; height: 35px; background: #fef3c7; color: #d97706; border-radius: 10px; display: flex; align-items: center; justify-content: center; font-weight: 800; font-size: 14px; }
.group-tag { background: #f1f5f9; color: #475569; padding: 5px 12px; border-radius: 8px; font-size: 11px; font-weight: 700; }
.status-pill-modern { font-size: 10px; font-weight: 800; padding: 5px 12px; border-radius: 8px; text-transform: uppercase; }
.status-invited { background: #eef2ff; color: #6366f1; }
.status-done { background: #ecfdf5; color: #10b981; }
.btn-mini-action { background: white; border: 1px solid #f1f5f9; color: #94a3b8; padding: 5px 10px; border-radius: 8px; transition: 0.2s; cursor: pointer; }
.btn-mini-action:hover { background: #0f172a; color: #f59e0b; }
.animate-in { animation: fadeInRight 0.6s ease-out; }
@keyframes fadeInRight { from { opacity: 0; transform: translateX(20px); } to { opacity: 1; transform: translateX(0); } }
</style>