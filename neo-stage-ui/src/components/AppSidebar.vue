<template>
  <div>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css">
    <link href="https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:ital,wght@0,400;0,500;0,600;0,700;0,800;1,700&family=DM+Sans:wght@300;400;500;600&display=swap" rel="stylesheet">

    <!-- Bouton mobile -->
    <button @click="basculerSidebar" class="declencheur-mobile d-lg-none" :class="{ 'est-ouvert': sidebarActive }">
      <div class="lignes-mt">
        <span class="ligne"></span>
        <span class="ligne ligne--milieu"></span>
        <span class="ligne"></span>
      </div>
    </button>

    <!-- ════════════════ SIDEBAR ════════════════ -->
    <aside class="sidebar" :class="{ 'sidebar--ouverte': sidebarActive }" ref="sidebarRef">

      <!-- Couches de fond -->
      <div class="sb-grille-fond"></div>
      <div class="sb-orbe sb-orbe--haut"></div>
      <div class="sb-orbe sb-orbe--bas"></div>
      <div class="sb-barre-top"></div>

      <!-- ── EN-TÊTE ── -->
      <div class="sb-entete">
        <div class="sb-marque" @mouseenter="logoHover = true" @mouseleave="logoHover = false">
          <div class="sb-logo" :class="{ 'sb-logo--actif': logoHover }">
            <svg width="42" height="42" viewBox="0 0 42 42" fill="none">
              <rect width="42" height="42" rx="12" fill="#0f172a"/>
              <rect x="8" y="10" width="16" height="3" rx="1.5" fill="#eab308"/>
              <rect x="8" y="16" width="11" height="3" rx="1.5" fill="#eab308" opacity="0.7"/>
              <rect x="8" y="22" width="14" height="3" rx="1.5" fill="#eab308" opacity="0.5"/>
              <circle cx="30" cy="30" r="7" fill="#eab308"/>
              <path d="M27 30l2 2 4-4" stroke="#0f172a" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
          </div>
          <div class="sb-nom-marque">
            <div class="sb-nom">Evalua<em>Tech</em></div>
            <div class="sb-sous-titre">{{ t('sidebar.subBrand') }}</div>
          </div>
        </div>

        <div class="sb-puce-role">
          <span class="point-role"></span>
          {{ libelleRole }}
        </div>
      </div>

      <!-- ── PROFIL ── -->
      <div class="sb-profil-wrap">
        <div class="sb-profil">
          <div class="profil-anneau-av">
            <div class="profil-av">{{ authStore.user?.name?.charAt(0)?.toUpperCase() || 'U' }}</div>
            <span class="av-statut"></span>
          </div>
          <div class="profil-infos">
            <div class="profil-nom">{{ authStore.user?.name || t('roles.User') }}</div>
            <div class="profil-enligne">
              <span class="enligne-cligno"></span>
              {{ t('profile.online') }}
            </div>
          </div>
          <button class="profil-btn-menu" title="Options">
            <i class="fa-solid fa-ellipsis-vertical"></i>
          </button>
        </div>
      </div>

      <!-- ── RECHERCHE ── -->
      <div class="sb-recherche-wrap" style="position:relative">
        <div class="sb-recherche" :class="{ 'sb-recherche--active': rechercheQuery }">
          <i class="fa-solid fa-magnifying-glass sb-icone-recherche"></i>
          <input
            type="text"
            :placeholder="t('sidebar.search')"
            class="sb-input-recherche"
            v-model="rechercheQuery"
          />
          <kbd v-if="!rechercheQuery" class="sb-raccourci">⌘K</kbd>
          <i
            v-else
            class="fa-solid fa-xmark sb-icone-recherche"
            style="cursor:pointer; opacity:0.6; transition:opacity 0.2s"
            @mouseenter="$event.target.style.opacity=1"
            @mouseleave="$event.target.style.opacity=0.6"
            @click="rechercheQuery = ''"
          ></i>
        </div>

        <!-- Dropdown résultats -->
        <div v-if="liensFiltrés.length > 0" class="sb-dropdown-recherche">
          <div class="sb-dropdown-titre">
            <i class="fa-solid fa-magnifying-glass"></i>
            {{ liensFiltrés.length }} résultat{{ liensFiltrés.length > 1 ? 's' : '' }}
          </div>
          <router-link
            v-for="lien in liensFiltrés"
            :key="lien.vers"
            :to="lien.vers"
            class="sb-resultat-recherche"
            @click="rechercheQuery = ''"
          >
            <span class="resultat-icone" :class="lien.iconeClasse">
              <i :class="lien.icone"></i>
            </span>
            <span class="resultat-label">{{ lien.label }}</span>
            <i class="fa-solid fa-arrow-right resultat-fleche"></i>
          </router-link>
        </div>

        <div v-if="rechercheQuery && liensFiltrés.length === 0" class="sb-dropdown-recherche">
          <div class="sb-recherche-vide">
            <i class="fa-solid fa-face-sad-tear"></i>
            Aucun résultat pour "<strong>{{ rechercheQuery }}</strong>"
          </div>
        </div>
      </div>

      <!-- ── NAVIGATION ── -->
      <nav class="sb-nav">

        <router-link to="/dashboard" class="sb-lien-hero">
          <div class="hero-icone">
            <i class="fa-solid fa-house-chimney"></i>
          </div>
          <span>{{ t('sidebar.dashboard') }}</span>
          <div class="hero-fleche"><i class="fa-solid fa-arrow-right"></i></div>
        </router-link>

        <!-- ════ ENTREPRISE ════ -->
        <div v-if="roleUtilisateur !== 'SuperAdmin' && roleUtilisateur !== 'Candidat'" class="sb-sections">

          <!-- Général -->
          <div class="sb-groupe">
            <div class="sb-groupe-label">
              <span class="point-label" style="background:#6366F1"></span>Général
            </div>
            <router-link to="/dashboard" class="sb-lien">
              <span class="sb-lien-icone ic-indigo"><i class="fa-solid fa-gauge-high"></i></span>
              <span class="sb-lien-texte">{{ t('sidebar.links.overview') }}</span>
            </router-link>
          </div>

          <!-- Recrutement -->
          <template v-if="authStore.hasPermission('view_can') || authStore.hasPermission('inv_can')">
            <div class="sb-groupe">
              <div class="sb-groupe-label">
                <span class="point-label" style="background:#0EA5E9"></span>Recrutement
              </div>
              <router-link v-if="authStore.hasPermission('view_can')" to="/candidates-list" class="sb-lien">
                <span class="sb-lien-icone ic-sky"><i class="fa-solid fa-users-viewfinder"></i></span>
                <span class="sb-lien-texte">{{ t('sidebar.links.candidates') }}</span>
              </router-link>
              <router-link v-if="authStore.hasPermission('inv_can')" to="/invite" class="sb-lien">
                <span class="sb-lien-icone ic-cyan"><i class="fa-solid fa-envelope-open-text"></i></span>
                <span class="sb-lien-texte">{{ t('sidebar.links.invitations') }}</span>
              </router-link>
              <router-link v-if="authStore.hasPermission('view_can')" to="/groups" class="sb-lien">
                <span class="sb-lien-icone ic-blue"><i class="fa-solid fa-layer-group"></i></span>
                <span class="sb-lien-texte">{{ t('sidebar.links.groups') }}</span>
              </router-link>
            </div>
          </template>

          <!-- Évaluations -->
          <template v-if="authStore.hasPermission('view_tests') || authStore.hasPermission('edit_bank')">
            <div class="sb-groupe">
              <div class="sb-groupe-label">
                <span class="point-label" style="background:#F59E0B"></span>Évaluations
              </div>
              <router-link v-if="authStore.hasPermission('view_tests') || authStore.hasPermission('inv_can')" to="/campaigns" class="sb-lien">
                <span class="sb-lien-icone ic-amber"><i class="fa-solid fa-clipboard-list"></i></span>
                <span class="sb-lien-texte">{{ t('sidebar.links.campaigns') }}</span>
              </router-link>
              <router-link v-if="authStore.hasPermission('edit_bank')" to="/questions" class="sb-lien">
                <span class="sb-lien-icone ic-gold"><i class="fa-solid fa-vault"></i></span>
                <span class="sb-lien-texte">{{ t('sidebar.links.bank') }}</span>
              </router-link>
              <router-link v-if="authStore.hasPermission('edit_bank')" to="/ai-generator" class="sb-lien">
                <span class="sb-lien-icone ic-violet"><i class="fa-solid fa-wand-magic-sparkles"></i></span>
                <span class="sb-lien-texte">{{ t('sidebar.links.ai') }}</span>
              </router-link>
            </div>
          </template>

          <!-- Analytique -->
          <template v-if="authStore.hasPermission('view_tests')">
            <div class="sb-groupe">
              <div class="sb-groupe-label">
                <span class="point-label" style="background:#8B5CF6"></span>Analytique
              </div>
              <router-link to="/analyse-comportementale" class="sb-lien">
                <span class="sb-lien-icone ic-purple"><i class="fa-solid fa-brain"></i></span>
                <span class="sb-lien-texte">{{ t('sidebar.links.smart') }}</span>
              </router-link>
              <router-link to="/stats" class="sb-lien">
                <span class="sb-lien-icone ic-fuchsia"><i class="fa-solid fa-chart-pie"></i></span>
                <span class="sb-lien-texte">{{ t('sidebar.links.stats') }}</span>
              </router-link>
            </div>
          </template>

          <!-- Organisation -->
          <template v-if="authStore.hasPermission('view_staff') || authStore.hasPermission('view_rol') || roleUtilisateur === 'AdminEntreprise'">
            <div class="sb-groupe">
              <div class="sb-groupe-label">
                <span class="point-label" style="background:#10B981"></span>Organisation
              </div>
              <router-link v-if="authStore.hasPermission('view_staff')" to="/staff-members" class="sb-lien">
                <span class="sb-lien-icone ic-emerald"><i class="fa-solid fa-user-tie"></i></span>
                <span class="sb-lien-texte">{{ t('sidebar.links.staff') }}</span>
              </router-link>
              <router-link v-if="authStore.hasPermission('view_rol')" to="/roles" class="sb-lien">
                <span class="sb-lien-icone ic-teal"><i class="fa-solid fa-shield-halved"></i></span>
                <span class="sb-lien-texte">{{ t('sidebar.links.roles') }}</span>
              </router-link>
              <router-link v-if="roleUtilisateur === 'AdminEntreprise'" to="/settings" class="sb-lien">
                <span class="sb-lien-icone ic-slate"><i class="fa-solid fa-gear"></i></span>
                <span class="sb-lien-texte">{{ t('sidebar.links.settings') }}</span>
              </router-link>
            </div>
          </template>
        </div>

        <!-- ════ SUPER ADMIN ════ -->
        <!-- NOTE: Section Évaluations (Campagnes, Banque questions, Générateur IA) supprimée pour SuperAdmin -->
        <div v-if="roleUtilisateur === 'SuperAdmin'" class="sb-sections">
          <div class="sb-groupe">
            <div class="sb-groupe-label sb-groupe-label--maitre">
              <i class="fa-solid fa-crown" style="color:#F59E0B;font-size:9px"></i>Administration Générale
            </div>
            <router-link to="/super-admin" class="sb-lien">
              <span class="sb-lien-icone ic-orange"><i class="fa-solid fa-building-columns"></i></span>
              <span class="sb-lien-texte">{{ t('sidebar.links.organizations') }}</span>
            </router-link>
            <router-link to="/platform-users" class="sb-lien">
              <span class="sb-lien-icone ic-amber"><i class="fa-solid fa-users-gear"></i></span>
              <span class="sb-lien-texte">{{ t('sidebar.links.manageUsers') }}</span>
            </router-link>
            <router-link to="/super-admin/statistiques" class="sb-lien">
              <span class="sb-lien-icone ic-gold"><i class="fa-solid fa-earth-europe"></i></span>
              <span class="sb-lien-texte">{{ t('sidebar.links.globalStats') }}</span>
            </router-link>
          </div>
        </div>

        <!-- ════ CANDIDAT ════ -->
        <div v-if="roleUtilisateur === 'Candidat'" class="sb-sections">
          <div class="sb-groupe">
            <div class="sb-groupe-label">
              <span class="point-label" style="background:#22C55E"></span>Mon Parcours
            </div>
            <router-link to="/my-tests" class="sb-lien">
              <span class="sb-lien-icone ic-green"><i class="fa-solid fa-circle-play"></i></span>
              <span class="sb-lien-texte">{{ t('sidebar.links.takeTest') }}</span>
              <span class="sb-badge sb-badge--go">GO</span>
            </router-link>
              <router-link to="/history" class="sb-lien">
              <span class="sb-lien-icone ic-slate"><i class="fa-solid fa-clock-rotate-left"></i></span>
              <span class="sb-lien-texte">{{ t('sidebar.links.history') }}</span>
            </router-link>
            <router-link to="/results" class="sb-lien">
              <span class="sb-lien-icone ic-gold"><i class="fa-solid fa-trophy"></i></span>
              <span class="sb-lien-texte">{{ t('sidebar.links.results') }}</span>
            </router-link>
          
          </div>
        </div>

        <div class="sb-nav-espacement" style="height: 100px;"></div>
      </nav>

      <!-- ── MINI STATISTIQUES ── -->
      <div class="sb-barre-stats">
        <div class="stat-pilule stat-pilule--amber">
          <i class="fa-solid fa-bolt"></i>
          <span>{{ t('sidebar.stats.active', { count: nombreCampagnes }) }}</span>
        </div>
        <div class="stat-pilule stat-pilule--green">
          <i class="fa-solid fa-circle-check"></i>
          <span>Synchronisé</span>
        </div>
        <div class="stat-pilule stat-pilule--blue">
          <i class="fa-solid fa-signal"></i>
          <span>24ms</span>
        </div>
      </div>

      <!-- ── PIED DE PAGE ── -->
      <div class="sb-pied">
        <div class="sb-separateur"></div>
        <div class="sb-pied-interieur">
          <button @click="deconnexion" class="sb-btn-deconnexion">
            <span class="deconnexion-icone-wrap">
              <i class="fa-solid fa-arrow-right-from-bracket"></i>
            </span>
            <span class="deconnexion-label">{{ t('profile.logout') }}</span>
          </button>
          <div class="sb-version">v2.4.1</div>
        </div>
      </div>

    </aside>

    <Transition name="sb-overlay-anim">
      <div v-if="sidebarActive" class="sb-overlay-mobile" @click="basculerSidebar"></div>
    </Transition>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import { useI18n } from 'vue-i18n';
import api from '@/services/api';

const { t } = useI18n();
const router          = useRouter();
const authStore       = useAuthStore();
const roleUtilisateur = computed(() => authStore.role);
const sidebarActive   = ref(false);
const nombreCampagnes = ref(0);
const logoHover       = ref(false);
const sidebarRef      = ref(null);
const rechercheQuery  = ref('');

const libelleRole = computed(() => {
  const cleRole = roleUtilisateur.value;
  return t(`roles.${cleRole}`, cleRole || 'Utilisateur');
});

// ── Tous les liens disponibles avec métadonnées ET restriction par rôle ──
const tousLesLiens = computed(() => {
  const role = roleUtilisateur.value;

  // Liens accessibles uniquement aux rôles Entreprise (non SuperAdmin, non Candidat)
  const estEntreprise = role !== 'SuperAdmin' && role !== 'Candidat';

  return [
    // ── Entreprise : Général ──
    {
      label: t('sidebar.links.overview'),
      vers: '/dashboard',
      icone: 'fa-solid fa-gauge-high',
      iconeClasse: 'ic-indigo',
      visible: estEntreprise,
    },
    // ── Entreprise : Recrutement ──
    {
      label: t('sidebar.links.candidates'),
      vers: '/candidates-list',
      icone: 'fa-solid fa-users-viewfinder',
      iconeClasse: 'ic-sky',
      visible: estEntreprise && authStore.hasPermission('view_can'),
    },
    {
      label: t('sidebar.links.invitations'),
      vers: '/invite',
      icone: 'fa-solid fa-envelope-open-text',
      iconeClasse: 'ic-cyan',
      visible: estEntreprise && authStore.hasPermission('inv_can'),
    },
    {
      label: t('sidebar.links.groups'),
      vers: '/groups',
      icone: 'fa-solid fa-layer-group',
      iconeClasse: 'ic-blue',
      visible: estEntreprise && authStore.hasPermission('view_can'),
    },
    // ── Entreprise : Évaluations ──
    {
      label: t('sidebar.links.campaigns'),
      vers: '/campaigns',
      icone: 'fa-solid fa-clipboard-list',
      iconeClasse: 'ic-amber',
      visible: estEntreprise && (authStore.hasPermission('view_tests') || authStore.hasPermission('inv_can')),
    },
    {
      label: t('sidebar.links.bank'),
      vers: '/questions',
      icone: 'fa-solid fa-vault',
      iconeClasse: 'ic-gold',
      visible: estEntreprise && authStore.hasPermission('edit_bank'),
    },
    {
      label: t('sidebar.links.ai'),
      vers: '/ai-generator',
      icone: 'fa-solid fa-wand-magic-sparkles',
      iconeClasse: 'ic-violet',
      visible: estEntreprise && authStore.hasPermission('edit_bank'),
    },
    // ── Entreprise : Analytique ──
    {
      label: t('sidebar.links.smart'),
      vers: '/analyse-comportementale',
      icone: 'fa-solid fa-brain',
      iconeClasse: 'ic-purple',
      visible: estEntreprise && authStore.hasPermission('view_tests'),
    },
    {
      label: t('sidebar.links.stats'),
      vers: '/stats',
      icone: 'fa-solid fa-chart-pie',
      iconeClasse: 'ic-fuchsia',
      visible: estEntreprise && authStore.hasPermission('view_tests'),
    },
    // ── Entreprise : Organisation ──
    {
      label: t('sidebar.links.staff'),
      vers: '/staff-members',
      icone: 'fa-solid fa-user-tie',
      iconeClasse: 'ic-emerald',
      visible: estEntreprise && authStore.hasPermission('view_staff'),
    },
    {
      label: t('sidebar.links.roles'),
      vers: '/roles',
      icone: 'fa-solid fa-shield-halved',
      iconeClasse: 'ic-teal',
      visible: estEntreprise && authStore.hasPermission('view_rol'),
    },
    {
      label: t('sidebar.links.settings'),
      vers: '/settings',
      icone: 'fa-solid fa-gear',
      iconeClasse: 'ic-slate',
      visible: estEntreprise && role === 'AdminEntreprise',
    },
    // ── SuperAdmin uniquement : Administration Générale ──
    // NOTE : Campagnes, Banque questions, Générateur IA sont EXCLUS pour SuperAdmin
    {
      label: t('sidebar.links.organizations'),
      vers: '/super-admin',
      icone: 'fa-solid fa-building-columns',
      iconeClasse: 'ic-orange',
      visible: role === 'SuperAdmin',
    },
    {
      label: t('sidebar.links.manageUsers'),
      vers: '/platform-users',
      icone: 'fa-solid fa-users-gear',
      iconeClasse: 'ic-amber',
      visible: role === 'SuperAdmin',
    },
    {
      label: t('sidebar.links.globalStats'),
      vers: '/super-admin/statistiques',
      icone: 'fa-solid fa-earth-europe',
      iconeClasse: 'ic-gold',
      visible: role === 'SuperAdmin',
    },
    // ── Candidat uniquement : Mon Parcours ──
    {
      label: t('sidebar.links.takeTest'),
      vers: '/my-tests',
      icone: 'fa-solid fa-circle-play',
      iconeClasse: 'ic-green',
      visible: role === 'Candidat',
    },
    {
      label: t('sidebar.links.results'),
      vers: '/results',
      icone: 'fa-solid fa-trophy',
      iconeClasse: 'ic-gold',
      visible: role === 'Candidat',
    },
    {
      label: t('sidebar.links.history'),
      vers: '/history',
      icone: 'fa-solid fa-clock-rotate-left',
      iconeClasse: 'ic-slate',
      visible: role === 'Candidat',
    },
  ];
});

// ── Liens filtrés : uniquement ceux visibles pour le rôle courant + correspondant à la recherche ──
const liensFiltrés = computed(() => {
  if (!rechercheQuery.value.trim()) return [];
  const q = rechercheQuery.value.toLowerCase();
  return tousLesLiens.value.filter(l => l.visible && l.label.toLowerCase().includes(q));
});

const chargerNombres = async () => {
  try {
    const res = await api.get('/Campagnes');
    nombreCampagnes.value = res.data.length;
  } catch (err) {
    console.error('Erreur chargement sidebar :', err);
  }
};

const basculerSidebar = () => { sidebarActive.value = !sidebarActive.value; };
const deconnexion     = () => { authStore.logout(); router.push('/login'); };

onMounted(chargerNombres);
</script>

<style scoped>
/* ════════════════════════════════════════════════════
   TOKENS DE DESIGN — MODE CLAIR
════════════════════════════════════════════════════ */
.sidebar {
  --sb-largeur:   272px;
  --fond-sidebar: #ffffff;
  --fond-carte:   #f8fafc;
  --fond-survol:  #f1f5f9;
  --fond-actif:   #fefce8;
  --bord:         #e2e8f0;
  --bord-or:      rgba(234,179,8,0.35);

  --texte-principal:  #0f172a;
  --texte-secondaire: #475569;
  --texte-discret:    #94a3b8;

  --amber:        #eab308;
  --amber-clair:  #fefce8;
  --amber-bord:   #fde68a;
  --amber-fonce:  #a16207;
  --amber-moyen:  #ca8a04;

  --vert:  #22c55e;
  --bleu:  #3b82f6;
  --rouge: #ef4444;

  --police-titre: 'Plus Jakarta Sans', sans-serif;
  --police-corps: 'DM Sans', sans-serif;

  --r-xs: 6px; --r-sm: 8px; --r-md: 12px; --r-lg: 16px; --r-xl: 20px;

  --ombre-douce: 0 1px 3px rgba(0,0,0,0.06), 0 4px 16px rgba(0,0,0,0.04);
  --ombre-or:    0 4px 20px rgba(234,179,8,0.18);
  --ombre-carte: 0 1px 4px rgba(0,0,0,0.05);
  --sb-logo-bg:   #0f172a;

  --ease-out:    cubic-bezier(0.16, 1, 0.3, 1);
  --ease-spring: cubic-bezier(0.34, 1.56, 0.64, 1);
  --ease-std:    cubic-bezier(0.4, 0, 0.2, 1);
}

/* ════════════════════════════════════════════════════
   TOKENS — MODE SOMBRE
════════════════════════════════════════════════════ */
[data-theme="dark"] .sidebar {
  --fond-sidebar: #0d1117;
  --fond-carte:   #161b22;
  --fond-survol:  #1e2430;
  --fond-actif:   rgba(234,179,8,0.1);
  --bord:         rgba(255,255,255,0.07);
  --bord-or:      rgba(234,179,8,0.28);

  --texte-principal:  #f0f6fc;
  --texte-secondaire: #8b949e;
  --texte-discret:    #4d5566;

  --amber-clair: rgba(234,179,8,0.1);
  --amber-bord:  rgba(234,179,8,0.2);
  --amber-fonce: #fbbf24;

  --ombre-douce: 0 1px 3px rgba(0,0,0,0.3), 0 4px 16px rgba(0,0,0,0.2);
  --ombre-or:    0 4px 20px rgba(234,179,8,0.1);
  --ombre-carte: 0 1px 4px rgba(0,0,0,0.2);
  --sb-logo-bg:   #0d1117;
}

/* ════════════════════════════════════════════════════
   PALETTE D'ICÔNES — MODE CLAIR
════════════════════════════════════════════════════ */
.ic-indigo { background:#EEF2FF; color:#6366F1; border-color:rgba(99,102,241,0.18); }
.ic-sky    { background:#E0F2FE; color:#0EA5E9; border-color:rgba(14,165,233,0.18); }
.ic-cyan   { background:#ECFEFF; color:#06B6D4; border-color:rgba(6,182,212,0.18); }
.ic-blue   { background:#EFF6FF; color:#3B82F6; border-color:rgba(59,130,246,0.18); }
.ic-amber  { background:#FEF3C7; color:#F59E0B; border-color:rgba(245,158,11,0.2); }
.ic-gold   { background:#FEF9C3; color:#EAB308; border-color:rgba(234,179,8,0.2); }
.ic-orange { background:#FFF7ED; color:#F97316; border-color:rgba(249,115,22,0.18); }
.ic-violet { background:#EEF2FF; color:#818CF8; border-color:rgba(129,140,248,0.2); }
.ic-purple { background:#F5F3FF; color:#8B5CF6; border-color:rgba(139,92,246,0.18); }
.ic-fuchsia{ background:#FDF4FF; color:#D946EF; border-color:rgba(217,70,239,0.16); }
.ic-emerald{ background:#ECFDF5; color:#10B981; border-color:rgba(16,185,129,0.18); }
.ic-teal   { background:#F0FDFA; color:#14B8A6; border-color:rgba(20,184,166,0.18); }
.ic-green  { background:#F0FDF4; color:#22C55E; border-color:rgba(34,197,94,0.18); }
.ic-slate  { background:#F8FAFC; color:#64748B; border-color:rgba(100,116,139,0.14); }

/* Mode sombre — icônes */
[data-theme="dark"] .ic-indigo { background:rgba(99,102,241,0.15); color:#818CF8; border-color:rgba(99,102,241,0.25); }
[data-theme="dark"] .ic-sky    { background:rgba(14,165,233,0.15); color:#38BDF8; border-color:rgba(14,165,233,0.25); }
[data-theme="dark"] .ic-cyan   { background:rgba(6,182,212,0.15); color:#22D3EE; border-color:rgba(6,182,212,0.25); }
[data-theme="dark"] .ic-blue   { background:rgba(59,130,246,0.15); color:#60A5FA; border-color:rgba(59,130,246,0.25); }
[data-theme="dark"] .ic-amber  { background:rgba(245,158,11,0.15); color:#FCD34D; border-color:rgba(245,158,11,0.25); }
[data-theme="dark"] .ic-gold   { background:rgba(234,179,8,0.15); color:#FDE047; border-color:rgba(234,179,8,0.25); }
[data-theme="dark"] .ic-orange { background:rgba(249,115,22,0.15); color:#FB923C; border-color:rgba(249,115,22,0.25); }
[data-theme="dark"] .ic-violet { background:rgba(129,140,248,0.15); color:#A5B4FC; border-color:rgba(129,140,248,0.25); }
[data-theme="dark"] .ic-purple { background:rgba(139,92,246,0.15); color:#A78BFA; border-color:rgba(139,92,246,0.25); }
[data-theme="dark"] .ic-fuchsia{ background:rgba(217,70,239,0.12); color:#E879F9; border-color:rgba(217,70,239,0.2); }
[data-theme="dark"] .ic-emerald{ background:rgba(16,185,129,0.15); color:#34D399; border-color:rgba(16,185,129,0.25); }
[data-theme="dark"] .ic-teal   { background:rgba(20,184,166,0.15); color:#2DD4BF; border-color:rgba(20,184,166,0.25); }
[data-theme="dark"] .ic-green  { background:rgba(34,197,94,0.15); color:#4ADE80; border-color:rgba(34,197,94,0.25); }
[data-theme="dark"] .ic-slate  { background:rgba(100,116,139,0.15); color:#94A3B8; border-color:rgba(100,116,139,0.2); }

.sb-lien:hover .sb-lien-icone,
.sb-lien.router-link-active .sb-lien-icone { transform: scale(1.12) rotate(-4deg); }

/* ════════════════════════════════════════════════════
   BOUTON MOBILE
════════════════════════════════════════════════════ */
.declencheur-mobile {
  position: fixed; top: 14px; left: 14px; z-index: 2500;
  width: 42px; height: 42px;
  background: var(--fond-sidebar); border: 1.5px solid var(--bord);
  border-radius: var(--r-md); box-shadow: var(--ombre-douce);
  display: flex; align-items: center; justify-content: center;
  cursor: pointer; padding: 0;
  transition: all 0.3s var(--ease-spring);
}
.declencheur-mobile:hover { border-color: var(--amber); box-shadow: var(--ombre-or); }
.lignes-mt { display: flex; flex-direction: column; gap: 4.5px; }
.ligne { display: block; width: 18px; height: 1.8px; background: var(--texte-principal); border-radius: 2px; transition: all 0.36s var(--ease-spring); transform-origin: center; }
.ligne--milieu { width: 11px; background: var(--amber); }
.declencheur-mobile.est-ouvert .ligne:first-child { transform: translateY(6.3px) rotate(45deg); }
.declencheur-mobile.est-ouvert .ligne--milieu     { opacity: 0; transform: scaleX(0); }
.declencheur-mobile.est-ouvert .ligne:last-child  { transform: translateY(-6.3px) rotate(-45deg); }

/* ════════════════════════════════════════════════════
   COQUILLE SIDEBAR
════════════════════════════════════════════════════ */
.sidebar {
  width: var(--sb-largeur); min-width: var(--sb-largeur);
  height: 100vh; position: sticky; top: 0;
  display: flex; flex-direction: column;
  background: var(--fond-sidebar);
  border-right: 1px solid var(--bord);
  font-family: var(--police-corps);
  z-index: 1200; overflow: hidden;
  transition: transform 0.4s var(--ease-out), box-shadow 0.4s var(--ease-std), background 0.3s ease, border-color 0.3s ease;
}

.sb-barre-top {
  position: absolute; top: 0; left: 0; right: 0; height: 3px;
  background: linear-gradient(90deg, #eab308 0%, #f59e0b 45%, rgba(234,179,8,0.15) 100%);
  z-index: 30; box-shadow: 0 1px 10px rgba(234,179,8,0.25);
}

.sb-grille-fond {
  position: absolute; inset: 0; pointer-events: none; z-index: 0;
  background-image:
    linear-gradient(rgba(234,179,8,0.025) 1px, transparent 1px),
    linear-gradient(90deg, rgba(234,179,8,0.025) 1px, transparent 1px);
  background-size: 32px 32px;
  transition: opacity 0.3s;
}
[data-theme="dark"] .sb-grille-fond { opacity: 0.5; }

.sb-orbe { position: absolute; border-radius: 50%; pointer-events: none; z-index: 0; filter: blur(70px); }
.sb-orbe--haut { width:220px;height:220px; background:radial-gradient(circle,rgba(234,179,8,0.18),transparent); top:-80px;left:-60px; animation:orbe-derive 20s ease-in-out infinite alternate; }
.sb-orbe--bas  { width:180px;height:180px; background:radial-gradient(circle,rgba(16,185,129,0.1),transparent); bottom:0;right:-50px; animation:orbe-derive 26s ease-in-out infinite alternate-reverse; }
@keyframes orbe-derive { from{transform:translate(0,0)} to{transform:translate(14px,18px)} }

/* ════════════════════════════════════════════════════
   EN-TÊTE
════════════════════════════════════════════════════ */
.sb-entete {
  position: relative; z-index: 5;
  padding: 22px 18px 14px;
  flex-shrink: 0;
  border-bottom: 1px solid var(--bord);
  transition: border-color 0.3s ease;
}

.sb-marque { display: flex; align-items: center; gap: 12px; margin-bottom: 14px; cursor: default; user-select: none; }

.sb-logo {
  width: 42px; height: 42px; flex-shrink: 0;
  border-radius: 13px; overflow: hidden;
  box-shadow: 0 4px 16px rgba(0,0,0,0.12), 0 1px 4px rgba(0,0,0,0.08);
  transition: all 0.36s var(--ease-spring);
  background: var(--sb-logo-bg);
}
.sb-logo--actif { transform: rotate(-6deg) scale(1.08); box-shadow: 0 8px 28px rgba(234,179,8,0.3); }
.sb-logo svg { display: block; width: 100%; height: 100%; }

.sb-nom-marque { display: flex; flex-direction: column; gap: 2px; }
.sb-nom { font-family: var(--police-titre); font-size: 19px; font-weight: 800; letter-spacing: -0.7px; line-height: 1.1; color: var(--texte-principal); transition: color 0.3s ease; }
.sb-nom em { font-style: italic; color: var(--amber); }
.sb-sous-titre { font-size: 9px; font-weight: 700; color: var(--texte-discret); letter-spacing: 0.18em; text-transform: uppercase; font-family: var(--police-titre); transition: color 0.3s ease; }

.sb-puce-role {
  display: inline-flex; align-items: center; gap: 6px;
  background: var(--fond-carte); border: 1px solid var(--bord); border-radius: 999px;
  padding: 5px 12px 5px 8px; font-size: 10px; font-weight: 700;
  color: var(--texte-secondaire); font-family: var(--police-titre); letter-spacing: 0.04em;
  box-shadow: var(--ombre-carte);
  transition: background 0.3s ease, border-color 0.3s ease, color 0.3s ease;
}
.point-role { width: 6px; height: 6px; background: var(--vert); border-radius: 50%; animation: pulse-point 2.2s ease infinite; }
@keyframes pulse-point { 0%,100%{box-shadow:0 0 0 0 rgba(34,197,94,0.45)} 50%{box-shadow:0 0 0 5px rgba(34,197,94,0)} }

/* ════════════════════════════════════════════════════
   PROFIL
════════════════════════════════════════════════════ */
.sb-profil-wrap { position: relative; z-index: 5; padding: 10px 12px 6px; flex-shrink: 0; }
.sb-profil {
  display: flex; align-items: center; gap: 10px;
  background: var(--fond-carte); border: 1px solid var(--bord);
  border-radius: var(--r-xl); padding: 10px 12px; cursor: pointer;
  box-shadow: var(--ombre-carte);
  transition: all 0.28s var(--ease-spring);
  position: relative; overflow: hidden;
}
.sb-profil::before {
  content: ''; position: absolute; top: 0; left: -80%; width: 50%; height: 100%;
  background: linear-gradient(90deg, transparent, rgba(234,179,8,0.1), transparent);
  transition: left 0.6s var(--ease-std);
}
.sb-profil:hover::before { left: 130%; }
.sb-profil:hover { border-color: var(--bord-or); box-shadow: var(--ombre-or); transform: translateY(-1px); }

.profil-anneau-av { position: relative; flex-shrink: 0; }
.profil-av {
  width: 36px; height: 36px; border-radius: 10px;
  background: linear-gradient(135deg, #eab308, #ca8a04);
  display: flex; align-items: center; justify-content: center;
  font-family: var(--police-titre); font-size: 16px; font-weight: 800; color: #1C1917;
  box-shadow: 0 3px 12px rgba(234,179,8,0.3);
  user-select: none; transition: transform 0.3s var(--ease-spring);
}
.sb-profil:hover .profil-av { transform: scale(1.05) rotate(-3deg); }
.av-statut {
  position: absolute; bottom: -2px; right: -2px;
  width: 9px; height: 9px; background: var(--vert);
  border: 2px solid var(--fond-sidebar); border-radius: 50%;
  transition: border-color 0.3s ease;
}

.profil-infos { flex: 1; min-width: 0; }
.profil-nom { font-size: 13px; font-weight: 700; color: var(--texte-principal); letter-spacing: -0.2px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; font-family: var(--police-titre); transition: color 0.3s ease; }
.profil-enligne { display: flex; align-items: center; gap: 5px; font-size: 11px; color: var(--texte-discret); margin-top: 1px; transition: color 0.3s ease; }
.enligne-cligno { width: 5px; height: 5px; background: var(--vert); border-radius: 50%; flex-shrink: 0; animation: cligno-enligne 2s ease-in-out infinite alternate; }
@keyframes cligno-enligne { from{opacity:0.45} to{opacity:1;box-shadow:0 0 5px rgba(34,197,94,0.5)} }

.profil-btn-menu {
  width: 26px; height: 26px; display: flex; align-items: center; justify-content: center;
  border: none; background: transparent; border-radius: var(--r-xs);
  cursor: pointer; color: var(--texte-discret); font-size: 11px;
  transition: all 0.2s var(--ease-std); flex-shrink: 0;
}
.sb-profil:hover .profil-btn-menu { color: var(--amber); background: var(--amber-clair); }

/* ════════════════════════════════════════════════════
   RECHERCHE
════════════════════════════════════════════════════ */
.sb-recherche-wrap { position: relative; z-index: 100; padding: 4px 12px 8px; flex-shrink: 0; }
.sb-recherche {
  display: flex; align-items: center; gap: 8px;
  background: var(--fond-carte); border: 1px solid var(--bord);
  border-radius: var(--r-md); padding: 8px 10px;
  transition: all 0.22s var(--ease-std); box-shadow: var(--ombre-carte);
}
.sb-recherche:focus-within,
.sb-recherche--active { border-color: var(--amber); box-shadow: 0 0 0 3px rgba(234,179,8,0.12); }
.sb-icone-recherche { color: var(--texte-discret); font-size: 11px; flex-shrink: 0; transition: color 0.3s ease; }
.sb-input-recherche {
  flex: 1; border: none; background: transparent;
  font-size: 12.5px; color: var(--texte-principal); font-family: var(--police-corps);
  outline: none; min-width: 0; transition: color 0.3s ease;
}
.sb-input-recherche::placeholder { color: var(--texte-discret); }
.sb-raccourci {
  background: var(--fond-survol); border: 1px solid var(--bord); border-radius: 5px;
  padding: 1px 6px; font-size: 9px; font-weight: 700; color: var(--texte-discret);
  font-family: var(--police-corps); flex-shrink: 0;
  transition: background 0.3s ease, border-color 0.3s ease, color 0.3s ease;
}

/* Dropdown résultats */
.sb-dropdown-recherche {
  position: absolute;
  top: calc(100% - 4px);
  left: 12px; right: 12px;
  background: var(--fond-sidebar);
  border: 1px solid var(--bord-or);
  border-radius: var(--r-md);
  box-shadow: var(--ombre-or), 0 8px 32px rgba(0,0,0,0.08);
  z-index: 200;
  overflow: hidden;
  animation: dropdown-entre 0.18s var(--ease-out) both;
}
@keyframes dropdown-entre { from{opacity:0;transform:translateY(-6px)} to{opacity:1;transform:translateY(0)} }

.sb-dropdown-titre {
  padding: 8px 12px 6px;
  font-size: 9.5px; font-weight: 800; color: var(--texte-discret);
  text-transform: uppercase; letter-spacing: 0.14em;
  font-family: var(--police-titre);
  display: flex; align-items: center; gap: 6px;
  border-bottom: 1px solid var(--bord);
}
.sb-dropdown-titre i { color: var(--amber); font-size: 9px; }

.sb-resultat-recherche {
  display: flex; align-items: center; gap: 10px;
  padding: 9px 12px;
  font-size: 13px; font-weight: 600;
  color: var(--texte-principal);
  text-decoration: none;
  font-family: var(--police-titre);
  transition: background 0.15s, color 0.15s;
  border-bottom: 1px solid var(--bord);
}
.sb-resultat-recherche:last-child { border-bottom: none; }
.sb-resultat-recherche:hover { background: var(--fond-actif); color: var(--amber-fonce); }

.resultat-icone {
  width: 28px; height: 28px;
  display: flex; align-items: center; justify-content: center;
  border-radius: var(--r-sm); font-size: 11px; flex-shrink: 0;
  border: 1px solid transparent;
}
.resultat-label { flex: 1; }
.resultat-fleche { font-size: 9px; color: var(--amber); opacity: 0; transition: opacity 0.15s, transform 0.15s; }
.sb-resultat-recherche:hover .resultat-fleche { opacity: 1; transform: translateX(2px); }

.sb-recherche-vide {
  padding: 14px 12px;
  font-size: 12px; color: var(--texte-discret);
  font-family: var(--police-titre);
  display: flex; align-items: center; gap: 8px;
}
.sb-recherche-vide strong { color: var(--texte-secondaire); }

/* ════════════════════════════════════════════════════
   NAVIGATION
════════════════════════════════════════════════════ */
.sb-nav {
  position: relative; z-index: 5;
  flex: 1; overflow-y: auto; overflow-x: hidden;
  padding: 4px 10px 0;
  scrollbar-width: thin;
  scrollbar-color: rgba(234,179,8,0.2) transparent;
}
.sb-nav::-webkit-scrollbar { width: 3px; }
.sb-nav::-webkit-scrollbar-thumb { background: rgba(234,179,8,0.2); border-radius: 4px; }

/* Lien héro */
.sb-lien-hero {
  display: flex; align-items: center; gap: 10px;
  padding: 11px 12px; border-radius: var(--r-lg);
  font-size: 13.5px; font-weight: 800; color: var(--texte-principal);
  text-decoration: none; margin-bottom: 8px; margin-top: 4px;
  background: var(--fond-carte); border: 1.5px solid var(--bord);
  position: relative; overflow: hidden;
  box-shadow: var(--ombre-douce);
  font-family: var(--police-titre);
  transition: all 0.28s var(--ease-spring);
}
.sb-lien-hero:hover,
.sb-lien-hero.router-link-active {
  border-color: var(--bord-or); background: var(--fond-actif);
  box-shadow: var(--ombre-or); transform: translateX(2px);
  color: var(--texte-principal);
}

.hero-icone {
  width: 32px; height: 32px;
  display: flex; align-items: center; justify-content: center;
  background: linear-gradient(135deg, #eab308, #ca8a04);
  border-radius: var(--r-sm); font-size: 13px; color: #0f172a;
  flex-shrink: 0; box-shadow: 0 3px 10px rgba(234,179,8,0.3);
  transition: all 0.25s var(--ease-spring);
}
.sb-lien-hero:hover .hero-icone { transform: scale(1.1) rotate(-4deg); }
.hero-fleche { margin-left: auto; font-size: 10px; color: var(--texte-discret); opacity: 0; flex-shrink: 0; transition: all 0.22s; }
.sb-lien-hero:hover .hero-fleche { opacity: 1; transform: translateX(3px); color: var(--amber); }

/* Groupe */
.sb-groupe { margin-bottom: 4px; }
.sb-groupe-label {
  display: flex; align-items: center; gap: 7px;
  padding: 14px 4px 5px;
  font-size: 9.5px; font-weight: 800; color: var(--texte-discret);
  text-transform: uppercase; letter-spacing: 0.18em;
  font-family: var(--police-titre); user-select: none;
  transition: color 0.3s ease;
}
.sb-groupe-label--maitre { color: #ca8a04; gap: 6px; }
[data-theme="dark"] .sb-groupe-label--maitre { color: #fbbf24; }
.sb-groupe-label::after { content: ''; flex: 1; height: 1px; background: linear-gradient(90deg, var(--bord), transparent); }
.point-label { width: 5px; height: 5px; border-radius: 50%; flex-shrink: 0; }

/* Lien nav */
.sb-lien {
  display: flex; align-items: center; gap: 10px;
  padding: 7px 10px; border-radius: var(--r-md);
  font-size: 13px; font-weight: 500; color: var(--texte-secondaire);
  text-decoration: none; margin-bottom: 1px;
  border: 1px solid transparent;
  position: relative; overflow: hidden;
  transition: all 0.2s var(--ease-spring);
}
.sb-lien:hover {
  color: var(--texte-principal); background: var(--fond-survol);
  border-color: var(--bord); transform: translateX(3px);
}
.sb-lien.router-link-active,
.sb-lien.router-link-exact-active {
  color: var(--amber-fonce); font-weight: 700;
  background: var(--fond-actif);
  border-color: var(--bord-or);
}
[data-theme="dark"] .sb-lien.router-link-active,
[data-theme="dark"] .sb-lien.router-link-exact-active { color: #fde047; }
.sb-lien.router-link-active::before {
  content: ''; position: absolute; left: 0; top: 18%; bottom: 18%;
  width: 3px;
  background: linear-gradient(180deg, #eab308, #ca8a04);
  border-radius: 0 4px 4px 0;
  box-shadow: 2px 0 10px rgba(234,179,8,0.4);
}

/* Icône de lien */
.sb-lien-icone {
  width: 30px; height: 30px;
  display: flex; align-items: center; justify-content: center;
  border-radius: var(--r-sm); font-size: 12px; flex-shrink: 0;
  border: 1px solid transparent;
  transition: all 0.26s var(--ease-spring);
}
.sb-lien-texte { flex: 1; }

/* Badges */
.sb-badge {
  margin-left: auto; flex-shrink: 0; border-radius: 999px;
  font-size: 9px; font-weight: 800; padding: 2px 8px;
  font-family: var(--police-titre); letter-spacing: 0.04em;
}
.sb-badge--or {
  background: var(--amber-clair); color: var(--amber-fonce);
  border: 1px solid var(--amber-bord);
}
.sb-badge--nouveau {
  background: #0f172a; color: #eab308; font-size: 8px; letter-spacing: 0.12em;
}
[data-theme="dark"] .sb-badge--nouveau { background: rgba(234,179,8,0.15); color: #fde047; }
.sb-badge--go {
  background: rgba(34,197,94,0.1); color: #166534; border: 1px solid rgba(34,197,94,0.22);
}
[data-theme="dark"] .sb-badge--go { background: rgba(34,197,94,0.12); color: #4ade80; border-color: rgba(34,197,94,0.2); }

.sb-nav-espacement { height: 14px; }

/* ════════════════════════════════════════════════════
   BARRE DE STATISTIQUES
════════════════════════════════════════════════════ */
.sb-barre-stats {
  position: relative; z-index: 5;
  padding: 6px 12px 2px;
  display: flex; gap: 5px; flex-shrink: 0;
}
.stat-pilule {
  flex: 1; display: flex; align-items: center; justify-content: center; gap: 5px;
  background: var(--fond-carte); border: 1px solid var(--bord);
  border-radius: var(--r-sm); padding: 6px;
  font-size: 9.5px; font-weight: 700; color: var(--texte-secondaire);
  font-family: var(--police-titre); box-shadow: var(--ombre-carte);
  transition: background 0.3s ease, border-color 0.3s ease, color 0.3s ease;
}
.stat-pilule i { font-size: 9px; }
.stat-pilule--amber i { color: var(--amber); }
.stat-pilule--green i { color: var(--vert); }
.stat-pilule--blue  i { color: var(--bleu); }

/* ════════════════════════════════════════════════════
   PIED DE PAGE
════════════════════════════════════════════════════ */
.sb-pied { position: relative; z-index: 5; flex-shrink: 0; padding: 6px 12px 18px; }
.sb-separateur {
  height: 1px; margin-bottom: 8px;
  background: linear-gradient(90deg, transparent, var(--bord), transparent);
  transition: background 0.3s ease;
}
.sb-pied-interieur { display: flex; align-items: center; gap: 8px; }
.sb-btn-deconnexion {
  flex: 1; display: flex; align-items: center; gap: 8px;
  background: rgba(239,68,68,0.05); border: 1px solid rgba(239,68,68,0.1);
  border-radius: var(--r-md); padding: 9px 12px;
  color: var(--rouge); font-size: 13px; font-weight: 700;
  font-family: var(--police-titre); cursor: pointer;
  transition: all 0.25s var(--ease-spring);
}
.sb-btn-deconnexion:hover { background: rgba(239,68,68,0.08); border-color: rgba(239,68,68,0.2); transform: translateY(-1px); box-shadow: 0 4px 14px rgba(239,68,68,0.08); }
.deconnexion-icone-wrap {
  width: 28px; height: 28px; display: flex; align-items: center; justify-content: center;
  background: rgba(239,68,68,0.08); border-radius: var(--r-xs); font-size: 11px; flex-shrink: 0;
  transition: transform 0.22s var(--ease-spring);
}
.sb-btn-deconnexion:hover .deconnexion-icone-wrap { transform: translateX(2px); }
.deconnexion-label { flex: 1; text-align: left; }
.sb-version { font-size: 9.5px; font-weight: 700; color: var(--texte-discret); letter-spacing: 0.06em; font-family: var(--police-titre); flex-shrink: 0; transition: color 0.3s ease; }

/* ════════════════════════════════════════════════════
   OVERLAY & RESPONSIVE
════════════════════════════════════════════════════ */
.sb-overlay-mobile { position: fixed; inset: 0; background: rgba(15,23,42,0.4); backdrop-filter: blur(4px); z-index: 1100; }
.sb-overlay-anim-enter-active { transition: opacity 0.3s; }
.sb-overlay-anim-leave-active { transition: opacity 0.22s; }
.sb-overlay-anim-enter-from, .sb-overlay-anim-leave-to { opacity: 0; }

@media (max-width: 991px) {
  .sidebar { position: fixed; left: 0; top: 0; transform: translateX(-100%); box-shadow: none; }
  .sidebar.sidebar--ouverte { transform: translateX(0); box-shadow: 24px 0 60px rgba(0,0,0,0.15); }
}

/* ════════════════════════════════════════════════════
   ANIMATIONS D'ENTRÉE EN CASCADE
════════════════════════════════════════════════════ */
.sb-lien-hero    { animation: sb-entree 0.5s var(--ease-out) 0.04s both; }
.sb-profil-wrap  { animation: sb-haut 0.5s var(--ease-out) 0.02s both; }
.sb-recherche-wrap { animation: sb-haut 0.5s var(--ease-out) 0.05s both; }
.sb-groupe:nth-child(1) .sb-lien { animation: sb-entree 0.45s var(--ease-out) 0.08s both; }
.sb-groupe:nth-child(2) .sb-lien { animation: sb-entree 0.45s var(--ease-out) 0.12s both; }
.sb-groupe:nth-child(3) .sb-lien { animation: sb-entree 0.45s var(--ease-out) 0.16s both; }
.sb-groupe:nth-child(4) .sb-lien { animation: sb-entree 0.45s var(--ease-out) 0.20s both; }
.sb-groupe:nth-child(5) .sb-lien { animation: sb-entree 0.45s var(--ease-out) 0.24s both; }
.sb-groupe:nth-child(6) .sb-lien { animation: sb-entree 0.45s var(--ease-out) 0.28s both; }
@keyframes sb-entree { from{opacity:0;transform:translateX(-12px)} to{opacity:1;transform:translateX(0)} }
@keyframes sb-haut   { from{opacity:0;transform:translateY(8px)}   to{opacity:1;transform:translateY(0)} }
</style>