<template>
  <div class="d-flex admin-body">
    <!-- COMPOSANTS DE STRUCTURE -->
    <AppSidebar />
    
    <div class="content flex-grow-1 p-4">
      <AppNavbar />

      <div class="mt-4 animate__animated animate__fadeIn">
        
        <!-- EN-TÊTE -->
        <div class="d-flex justify-content-between align-items-end mb-5">
          <div>
            <nav aria-label="breadcrumb" class="mb-2">
              <span class="tiny fw-800 text-amber uppercase ls-1">Administration</span>
            </nav>
            <h2 class="fw-800 m-0 text-slate-900">Rôles & Permissions</h2>
            <p class="text-slate-500 small mt-1">Définissez les niveaux d'accès et la sécurité de votre organisation.</p>
          </div>
          <div class="d-flex gap-3">
            <button class="btn-secondary-pro" @click="$router.go(-1)">
              <i class="fa-solid fa-arrow-left-long me-2"></i> Retour
            </button>
            <button @click="openAddModal" class="btn-primary-gradient shadow-premium">
              <i class="fa-solid fa-plus me-2"></i> Créer un rôle
            </button>
          </div>
        </div>

        <!-- STATISTIQUES -->
        <div class="row g-4 mb-5">
          <div class="col-md-3" v-for="stat in stats" :key="stat.label">
            <div class="stat-card-premium">
              <div class="d-flex align-items-center">
                <div class="icon-stat-box" :style="{ color: stat.color, background: stat.bg }">
                  <i :class="stat.icon"></i>
                </div>
                <div class="ms-3">
                  <div class="tiny text-slate-400 uppercase fw-800 ls-1">{{ stat.label }}</div>
                  <div class="fw-800 fs-3 text-slate-800">{{ stat.value }}</div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- FILTRES -->
        <div class="glass-panel p-3 mb-4 d-flex align-items-center gap-3">
          <div class="search-vessel flex-grow-1 pe-4">
            <i class="fa-solid fa-magnifying-glass text-slate-400"></i>
            <input type="text" v-model="searchQuery" class="input-search-pro ms-2" placeholder="Filtrer par nom de rôle...">
          </div>
        </div>

        <!-- CHARGEMENT / ERREUR RÉSEAU -->
        <div v-if="loading" class="text-center py-5">
           <div class="loader-ripple"><div></div><div></div></div>
           <p class="mt-3 text-slate-400 small">Connexion au serveur .NET (Port 5172)...</p>
        </div>

        <!-- ÉTAT VIDE -->
        <div v-else-if="roles.length === 0" class="text-center py-5">
          <i class="fa-solid fa-database fa-3x text-slate-200 mb-3"></i>
          <p class="text-slate-400">Aucun rôle trouvé ou serveur hors ligne.</p>
        </div>

        <!-- GRILLE DES RÔLES -->
        <div v-else class="row g-4">
          <div class="col-md-4" v-for="role in filteredRoles" :key="role.id">
            <div class="role-card-premium">
              <div class="card-header-inner p-4">
                <div class="d-flex justify-content-between align-items-start">
                  <div class="role-avatar shadow-sm">
                    <i class="fa-solid fa-shield-halved"></i>
                  </div>
                  <div class="dropdown">
                    <button class="btn-dot" data-bs-toggle="dropdown"><i class="fa-solid fa-ellipsis"></i></button>
                    <ul class="dropdown-menu dropdown-menu-end shadow-pro border-0">
                      <li><a class="dropdown-item py-2 fw-bold" @click.prevent="openEditModal(role)" href="#"><i class="fa-solid fa-pen me-2"></i> Modifier</a></li>
                      <li><hr class="dropdown-divider opacity-50"></li>
                      <li><a class="dropdown-item py-2 fw-bold text-danger" @click.prevent="deleteRole(role.id)" href="#"><i class="fa-solid fa-trash me-2"></i> Supprimer</a></li>
                    </ul>
                  </div>
                </div>
                <div class="mt-3">
                  <h5 class="fw-800 text-slate-800 mb-1">{{ role.nom }}</h5>
                  <span class="badge-members">{{ role.nombreMembres }} Membres actifs</span>
                </div>
              </div>

              <div class="card-body-inner px-4 pb-2">
                <p class="small text-slate-500 line-clamp-2">{{ role.description || 'Description non spécifiée' }}</p>
                <div class="permission-tags-container mt-3">
                  <span v-for="p in role.permissions?.slice(0, 3)" :key="p" class="badge-perm-tag">{{ p }}</span>
                  <span v-if="role.permissions?.length > 3" class="badge-perm-tag more">+{{ role.permissions.length - 3 }}</span>
                </div>
              </div>

              <div class="card-footer-inner p-4 pt-0">
                <div class="d-flex justify-content-between align-items-center mt-3 pt-3 border-top">
                  <span class="tiny text-slate-400 fw-bold">{{ formatDate(role.creeLe) }}</span>
                  <button @click="openViewModal(role)" class="btn-text-action">Détails <i class="fa-solid fa-arrow-right ms-1"></i></button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- MODALE : AJOUTER / MODIFIER / VOIR -->
    <Transition name="fade">
      <div v-if="showModal" class="modal-overlay" @click.self="closeModal">
        <div class="modal-card-pro animate__animated animate__zoomIn animate__faster">
          
          <div class="modal-header-pro">
            <div class="d-flex align-items-center">
              <div class="header-icon-box me-3">
                <i class="fa-solid fa-sliders text-amber"></i>
              </div>
              <div>
                <h5 class="fw-800 m-0 text-slate-900">{{ isViewOnly ? 'Consultation' : (isEditing ? 'Édition du rôle' : 'Nouveau Rôle') }}</h5>
                <p class="tiny text-slate-400 m-0 fw-bold uppercase ls-1">Configuration des privilèges système</p>
              </div>
            </div>
            <button @click="closeModal" class="btn-close-modal"><i class="fa-solid fa-xmark"></i></button>
          </div>

          <div class="modal-body-pro custom-scrollbar">
            <!-- SECTION 1 : CONFIGURATION GÉNÉRALE -->
            <div class="form-section mb-5">
              <div class="section-title-pro mb-4">
                <span class="num">01</span>
                <h6>Paramètres de base</h6>
              </div>
              
              <div class="row g-4">
                <div class="col-md-12">
                  <label class="form-label-pro" for="template">Modèle de rôle (Template)</label>
                  <select id="template" v-model="selectedTemplate" class="input-pro select-pro" @change="applyTemplate" :disabled="isViewOnly">
                    <option value="">Rôle personnalisé</option>
                    <option value="manager">Manager - Accès complet</option>
                    <option value="hr">RH - Recrutement & Staff</option>
                    <option value="viewer">Lecteur - Lecture seule</option>
                  </select>
                </div>

                <div class="col-md-12">
                  <label class="form-label-pro" for="roleName">Nom public du rôle *</label>
                  <input type="text" id="roleName" v-model="form.nom" class="input-pro" placeholder="ex. Manager RH" :disabled="isViewOnly">
                </div>

                <div class="col-md-6">
                  <label class="form-label-pro" for="firstName">Prénom du responsable</label>
                  <input type="text" id="firstName" v-model="form.prenom" class="input-pro" placeholder="Jean" :disabled="isViewOnly">
                </div>
                <div class="col-md-6">
                  <label class="form-label-pro" for="lastName">Nom du responsable</label>
                  <input type="text" id="lastName" v-model="form.nomFamille" class="input-pro" placeholder="Dupont" :disabled="isViewOnly">
                </div>

                <div class="col-md-12">
                  <label class="form-label-pro" for="email">Adresse Email de contact</label>
                  <input type="email" id="email" v-model="form.email" class="input-pro" placeholder="admin@entreprise.com" :disabled="isViewOnly">
                </div>

                <div class="col-12">
                  <label class="form-label-pro" for="desc">Description des responsabilités</label>
                  <textarea id="desc" v-model="form.description" class="input-pro" rows="3" placeholder="Décrivez l'objectif de ce rôle..." :disabled="isViewOnly"></textarea>
                </div>
              </div>
            </div>

            <!-- SECTION 2 : MATRICE DES PERMISSIONS -->
            <div class="form-section">
              <div class="d-flex justify-content-between align-items-center mb-4">
                <div class="section-title-pro m-0">
                  <span class="num">02</span>
                  <h6>Droits & Permissions</h6>
                </div>
                <div class="d-flex gap-3" v-if="!isViewOnly">
                  <button @click="selectAll" class="btn-link-action">Tout autoriser</button>
                  <button @click="clearAll" class="btn-link-action text-slate-400">Réinitialiser</button>
                </div>
              </div>

              <div v-for="cat in permissionGroups" :key="cat.title" class="category-card mb-4">
                <div class="category-header d-flex justify-content-between align-items-center p-3">
                  <div class="fw-800 text-slate-700 small uppercase ls-1">
                    <i :class="cat.icon + ' me-2 text-amber'"></i> {{ cat.title }}
                  </div>
                  <span class="count-pill">{{ getSelectedCount(cat.items) }} / {{ cat.items.length }}</span>
                </div>

                <div class="category-body p-3">
                  <div class="permission-matrix">
                    <div v-for="item in cat.items" :key="item.id" 
                         class="permission-tile" 
                         :class="{ active: form.permissions.includes(item.id), 'disabled': isViewOnly }"
                         @click="!isViewOnly && togglePermission(item.id)">
                      <div class="tile-content">
                        <div class="tile-checkbox" :class="{ checked: form.permissions.includes(item.id) }">
                          <i v-if="form.permissions.includes(item.id)" class="fa-solid fa-check"></i>
                        </div>
                        <div class="ms-3">
                          <div class="fw-bold text-slate-800 small">{{ item.label }}</div>
                          <div class="tiny text-slate-400">{{ item.desc }}</div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="modal-footer-pro">
            <button @click="closeModal" class="btn-outline-tech px-4">{{ isViewOnly ? 'Fermer' : 'Annuler' }}</button>
            <button v-if="!isViewOnly" @click="saveRole" class="btn-primary-gradient px-5" :disabled="saving">
              <span v-if="saving" class="spinner-border spinner-border-sm me-2"></span>
              {{ isEditing ? 'Sauvegarder' : 'Confirmer la création' }}
            </button>
          </div>
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, computed, onUnmounted } from 'vue';
import axios from 'axios';

// 1. IMPORTATION DES COMPOSANTS (Utilisez @ pour src/)
import AppSidebar from '@/components/AppSidebar.vue';
import AppNavbar from '@/components/AppNavbar.vue';

const API_URL = 'http://localhost:5172/api/Roles'; 
const abortController = new AbortController();

const roles = ref([]);
const stats = ref([]);
const showModal = ref(false);
const isEditing = ref(false);
const isViewOnly = ref(false);
const loading = ref(true);
const saving = ref(false);
const searchQuery = ref('');
const selectedTemplate = ref('');

// Structure réactive du formulaire (synchronisée avec Role.cs)
const form = reactive({ 
  id: null, 
  nom: '', 
  prenom: '', 
  nomFamille: '', 
  email: '', 
  description: '', 
  permissions: [] 
});

const fetchData = async () => {
  loading.value = true;
  try {
    const [rolesRes, statsRes] = await Promise.all([
      axios.get(API_URL, { signal: abortController.signal }),
      axios.get(`${API_URL}/stats`, { signal: abortController.signal })
    ]);
    roles.value = rolesRes.data;
    stats.value = statsRes.data;
  } catch (err) {
    if (axios.isCancel(err)) return;
    console.error("Erreur API : Vérifiez que le serveur .NET tourne sur le port 5172.");
    roles.value = [];
  } finally {
    loading.value = false;
  }
};

onMounted(fetchData);
onUnmounted(() => abortController.abort());

const filteredRoles = computed(() => {
  return roles.value.filter(r => 
    r.nom !== 'SuperAdmin' && 
    r.nom.toLowerCase().includes(searchQuery.value.toLowerCase())
  );
});

const applyTemplate = () => {
  if (!selectedTemplate.value) return;
  const templates = {
    manager: { nom: 'Manager Général', perms: ['view_can', 'inv_can', 'del_can', 'add_rol', 'view_rol', 'add_staff', 'view_staff'] },
    hr: { nom: 'Responsable RH', perms: ['view_can', 'inv_can', 'view_staff'] },
    viewer: { nom: 'Lecteur Système', perms: ['view_can', 'view_staff'] }
  };
  const t = templates[selectedTemplate.value];
  form.nom = t.nom;
  form.permissions = [...t.perms];
};

const saveRole = async () => {
  if (!form.nom) return alert("Le nom du rôle est obligatoire.");
  saving.value = true;
  try {
    if (isEditing.value) await axios.put(`${API_URL}/${form.id}`, form);
    else await axios.post(API_URL, form);
    await fetchData();
    closeModal();
  } catch (err) {
    alert("Erreur lors de l'enregistrement.");
  } finally {
    saving.value = false;
  }
};

const deleteRole = async (id) => {
  if (confirm("Supprimer ce rôle définitivement ?")) {
    try {
      await axios.delete(`${API_URL}/${id}`);
      await fetchData();
    } catch (err) {
      alert("Action impossible : ce rôle est déjà attribué à des utilisateurs.");
    }
  }
};

const openAddModal = () => { 
  isEditing.value = false; 
  isViewOnly.value = false; 
  resetForm(); 
  selectedTemplate.value = '';
  showModal.value = true; 
};

const openEditModal = (role) => {
  isEditing.value = true; 
  isViewOnly.value = false;
  Object.assign(form, { 
    ...role, 
    prenom: role.prenom || '',
    nomFamille: role.nomFamille || '',
    email: role.email || '',
    permissions: [...role.permissions] 
  });
  selectedTemplate.value = '';
  showModal.value = true;
};

const openViewModal = (role) => { openEditModal(role); isViewOnly.value = true; };
const closeModal = () => showModal.value = false;
const resetForm = () => {
  Object.assign(form, { id: null, nom: '', prenom: '', nomFamille: '', email: '', description: '', permissions: [] });
};

const formatDate = (d) => d ? new Date(d).toLocaleDateString('fr-FR') : '-';

const togglePermission = (id) => {
  const i = form.permissions.indexOf(id);
  i > -1 ? form.permissions.splice(i, 1) : form.permissions.push(id);
};

const getSelectedCount = (items) => items.filter(i => form.permissions.includes(i.id)).length;
const selectAll = () => { form.permissions = permissionGroups.flatMap(g => g.items.map(i => i.id)); };
const clearAll = () => form.permissions = [];

const permissionGroups = [
  { title: 'Candidats & Recrutement', icon: 'fa-solid fa-user-tie', items: [{ id: 'view_can', label: 'Consultation', desc: 'Accès aux listes' }, { id: 'inv_can', label: 'Invitations', desc: 'Envoyer des tests' }, { id: 'del_can', label: 'Suppression', desc: 'Archiver les dossiers' }] },
  { title: 'Gestion du Système', icon: 'fa-solid fa-shield-halved', items: [{ id: 'add_rol', label: 'Création Rôles', desc: 'Ajouter des profils' }, { id: 'view_rol', label: 'Audit Droits', desc: 'Voir les logs' }] },
  { title: 'Administration Staff', icon: 'fa-solid fa-users-gear', items: [{ id: 'add_staff', label: 'Nouveau Staff', desc: 'Inscrire un employé' }, { id: 'view_staff', label: 'Voir Staff', desc: 'Accès annuaire' }] }
];
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&display=swap');

.admin-body { background: #f8fafc; font-family: 'Plus Jakarta Sans', sans-serif; min-height: 100vh; color: #1e293b; }
.ls-1 { letter-spacing: 1px; }
.tiny { font-size: 10px; }
.uppercase { text-transform: uppercase; }

.stat-card-premium { background: white; padding: 20px; border-radius: 20px; box-shadow: 0 4px 15px rgba(0,0,0,0.02); border: 1px solid #f1f5f9; }
.icon-stat-box { width: 50px; height: 50px; border-radius: 14px; display: flex; align-items: center; justify-content: center; font-size: 20px; }

.glass-panel { background: white; border-radius: 18px; border: 1px solid #e2e8f0; }
.input-search-pro { border: none; outline: none; width: 100%; font-weight: 600; color: #475569; background: transparent; }

.role-card-premium { background: white; border-radius: 24px; border: 1px solid #eef2f6; transition: 0.3s; height: 100%; display: flex; flex-direction: column; }
.role-card-premium:hover { transform: translateY(-8px); border-color: #eab308; }
.role-avatar { width: 45px; height: 45px; background: #fff7ed; color: #f59e0b; border-radius: 12px; display: flex; align-items: center; justify-content: center; }
.badge-members { font-size: 11px; color: #64748b; font-weight: 700; background: #f8fafc; padding: 4px 10px; border-radius: 8px; }
.badge-perm-tag { font-size: 10px; font-weight: 800; background: #eff6ff; color: #2563eb; padding: 4px 10px; border-radius: 8px; margin-right: 5px; }

.modal-overlay { position: fixed; inset: 0; background: rgba(15, 23, 42, 0.6); backdrop-filter: blur(8px); z-index: 2000; display: flex; align-items: center; justify-content: center; }
.modal-card-pro { background: white; width: 95%; max-width: 900px; height: 85vh; border-radius: 32px; display: flex; flex-direction: column; overflow: hidden; }
.modal-header-pro { padding: 25px 40px; border-bottom: 1px solid #f1f5f9; display: flex; justify-content: space-between; align-items: center; }
.header-icon-box { width: 42px; height: 42px; background: #fffbeb; border-radius: 10px; display: flex; align-items: center; justify-content: center; }
.modal-body-pro { padding: 40px; overflow-y: auto; flex-grow: 1; background: #fcfdfe; }
.modal-footer-pro { padding: 25px 40px; border-top: 1px solid #f1f5f9; display: flex; justify-content: flex-end; gap: 15px; }

.section-title-pro { display: flex; align-items: center; }
.section-title-pro .num { width: 28px; height: 28px; background: #0f172a; color: white; border-radius: 8px; font-size: 12px; font-weight: 800; display: flex; align-items: center; justify-content: center; margin-right: 12px; }

.input-pro { width: 100%; padding: 14px 18px; border-radius: 14px; border: 1.5px solid #e2e8f0; font-size: 14px; transition: 0.2s; background: white; }
.select-pro { appearance: none; background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 24 24' stroke='%2364748b'%3E%3Cpath stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='M19 9l-7 7-7-7'%3E%3C/path%3E%3C/svg%3E"); background-repeat: no-repeat; background-position: right 1rem center; background-size: 1.5em; padding-right: 2.5rem; }
.form-label-pro { font-size: 11px; font-weight: 800; color: #64748b; margin-bottom: 8px; display: block; text-transform: uppercase; }

.category-card { background: white; border: 1px solid #f1f5f9; border-radius: 20px; overflow: hidden; }
.category-header { background: #f8fafc; }
.permission-matrix { display: grid; grid-template-columns: repeat(auto-fill, minmax(250px, 1fr)); gap: 12px; }
.permission-tile { padding: 15px; border-radius: 16px; border: 1.5px solid #f1f5f9; cursor: pointer; transition: 0.2s; }
.permission-tile.active { border-color: #f59e0b; background: #fffbeb; }
.tile-checkbox { width: 20px; height: 20px; border: 2px solid #cbd5e1; border-radius: 6px; display: flex; align-items: center; justify-content: center; }
.tile-checkbox.checked { background: #f59e0b; border-color: #f59e0b; color: white; }

.btn-primary-gradient { background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%); color: white; border: none; padding: 12px 25px; border-radius: 14px; font-weight: 800; }
.btn-outline-tech { background: white; border: 1.5px solid #e2e8f0; color: #475569; padding: 12px 25px; border-radius: 14px; }
.btn-link-action { background: none; border: none; color: #2563eb; font-weight: 800; font-size: 12px; }

.loader-ripple { display: inline-block; position: relative; width: 60px; height: 60px; }
.loader-ripple div { position: absolute; border: 4px solid #f59e0b; opacity: 1; border-radius: 50%; animation: ripple 1s infinite; }
@keyframes ripple {
  0% { top: 30px; left: 30px; width: 0; height: 0; opacity: 1; }
  100% { top: 0; left: 0; width: 60px; height: 60px; opacity: 0; }
}
</style>