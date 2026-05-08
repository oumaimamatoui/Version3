<template>
  <nav class="barre-top" role="banner">
    <div class="barre-top-interieur">

      <!-- ── GAUCHE : RECHERCHE ── -->
      <div class="zone-recherche">
        <div class="recherche-wrap">
          <Search :size="14" class="recherche-ico" />
          <input
            ref="champRecherche"
            type="text"
            :placeholder="t('search')"
            class="champ-recherche"
            @focus="rechercheFocalisee = true"
            @blur="rechercheFocalisee = false"
          />
          <kbd class="recherche-raccourci">⌘K</kbd>
        </div>
      </div>

      <!-- ── DROITE : ACTIONS ── -->
      <div class="barre-top-actions">

        <!-- THÈME -->
        <button class="btn-action" @click="basculerTheme" :title="t('toggleTheme')">
          <Transition name="echange-icone" mode="out-in">
            <Sun v-if="estSombre" :size="16" key="soleil" />
            <Moon v-else :size="16" key="lune" />
          </Transition>
        </button>

        <!-- LANGUE -->
        <div class="menu-deroulant" ref="menuDeroulantLang">
          <button class="btn-action btn-langue" @click="basculerMenu('lang')">
            <Globe :size="16" />
            <span class="libelle-langue">{{ langueActive }}</span>
            <ChevronDown :size="11" class="chevron" />
          </button>
          <Transition name="menu-fondu">
            <div v-if="menuOuvert === 'lang'" class="panneau-deroulant panneau-langue">
              <button
                v-for="l in langues"
                :key="l.code"
                class="item-deroulant"
                :class="{ actif: langueActive === l.code }"
                @click="definirLangue(l.code)"
              >
                <span class="drapeau">{{ l.drapeau }}</span>
                <span>{{ l.libelle }}</span>
                <Check v-if="langueActive === l.code" :size="12" class="ico-coche" />
              </button>
            </div>
          </Transition>
        </div>

        <!-- NOTIFICATIONS -->
        <div class="menu-deroulant" ref="menuDeroulantNotif">
          <button
            class="btn-action btn-notif"
            @click="basculerMenu('notif'); notifStore.markAllAsRead()"
          >
            <Bell :size="16" />
            <span v-if="notifStore.unreadCount > 0" class="point-notif">
              {{ notifStore.unreadCount > 9 ? '9+' : notifStore.unreadCount }}
            </span>
          </button>
          <Transition name="menu-fondu">
            <div v-if="menuOuvert === 'notif'" class="panneau-deroulant panneau-notif">
              <div class="entete-panneau">
                <span class="titre-panneau">Notifications</span>
                <span class="puce-compteur">{{ notifStore.unreadCount }} nouvelles</span>
              </div>
              <div class="defilement-notif">
                <div
                  v-for="n in notifStore.notifications"
                  :key="n.id"
                  class="ligne-notif"
                >
                  <div class="icone-notif-wrap">
                    <Bell :size="12" />
                  </div>
                  <div class="corps-notif">
                    <p class="texte-notif">{{ n.text }}</p>
                    <span class="heure-notif">{{ n.time }}</span>
                  </div>
                </div>
                <div v-if="!notifStore.notifications?.length" class="notif-vide">
                  <BellOff :size="28" class="ico-vide" />
                  <span>Aucune notification</span>
                </div>
              </div>
              <div class="pied-panneau">
                <button class="btn-tout-lu">Tout marquer comme lu</button>
              </div>
            </div>
          </Transition>
        </div>

        <!-- SÉPARATEUR -->
        <span class="separateur"></span>

        <!-- PROFIL -->
        <div class="menu-deroulant" ref="menuDeroulantProfil">
          <button class="declencheur-profil" @click="basculerMenu('profil')">
            <div class="avatar-wrap">
              <img :src="urlPhotoProfil" :alt="authStore.user?.name" class="avatar-img" />
              <span class="point-statut"></span>
            </div>
            <div class="infos-utilisateur">
              <span class="nom-utilisateur">{{ authStore.user?.name || 'Utilisateur' }}</span>
              <span class="role-utilisateur">{{ affichageRole }}</span>
            </div>
            <ChevronDown :size="12" class="chevron" :class="{ pivote: menuOuvert === 'profil' }" />
          </button>
          <Transition name="menu-fondu">
            <div v-if="menuOuvert === 'profil'" class="panneau-deroulant panneau-profil">
              <div class="entete-profil">
                <img :src="urlPhotoProfil" :alt="authStore.user?.name" class="ep-avatar" />
                <div>
                  <div class="ep-nom">{{ authStore.user?.name || 'Utilisateur' }}</div>
                  <div class="ep-role">{{ affichageRole }}</div>
                </div>
              </div>
              <div class="separateur-menu"></div>
              <router-link to="/profile" class="item-deroulant" @click="fermerMenu">
                <UserCircle :size="14" class="ico-item" /><span>Mon profil</span>
              </router-link>
              <router-link to="/settings" class="item-deroulant" @click="fermerMenu">
                <Settings :size="14" class="ico-item" /><span>Paramètres</span>
              </router-link>
              <div class="separateur-menu"></div>
              <button class="item-deroulant danger" @click="deconnexion">
                <LogOut :size="14" class="ico-item" /><span>Déconnexion</span>
              </button>
            </div>
          </Transition>
        </div>

      </div>
    </div>

    <!-- Overlay fermeture au clic extérieur -->
    <div v-if="menuOuvert" class="clic-exterieur" @click="fermerMenu"></div>
  </nav>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue';
import { useRouter } from 'vue-router';
import { useNotificationStore } from '@/stores/notification';
import { useAuthStore } from '@/stores/auth';
import api from '@/services/api';

import {
  Search, Sun, Moon, Globe, Bell, BellOff, ChevronDown,
  Check, UserCircle, Settings, LogOut,
} from 'lucide-vue-next';

const router     = useRouter();
const notifStore = useNotificationStore();
const authStore  = useAuthStore();

const champRecherche      = ref(null);
const rechercheFocalisee  = ref(false);
const menuOuvert          = ref(null);
const estSombre           = ref(false);
const langueActive        = ref(localStorage.getItem('lang') || 'FR');

const langues = [
  { code: 'FR', libelle: 'Français', drapeau: '🇫🇷' },
  { code: 'EN', libelle: 'English',  drapeau: '🇺🇸' },
];

const affichageRole = computed(() => ({
  SuperAdmin:      'SuperAdmin',
  AdminEntreprise: 'Administrateur',
  Evaluateur:      'Évaluateur',
  Candidat:        'Candidat',
  Recruteur:       'RH / Recruteur',
}[authStore.role] || authStore.role || 'Utilisateur'));

const urlPhotoProfil = computed(() => {
  if (authStore.user?.photoUrl) return `http://localhost:5172${authStore.user.photoUrl}`;
  return `https://ui-avatars.com/api/?name=${encodeURIComponent(authStore.user?.name || 'User')}&background=0f172a&color=fff&bold=true`;
});

// ── Menus déroulants ──
const basculerMenu = (nom) => {
  menuOuvert.value = menuOuvert.value === nom ? null : nom;
};
const fermerMenu = () => { menuOuvert.value = null; };

// ── Thème ──
const obtenirCleTheme = () => authStore.user?.id ? `theme_${authStore.user.id}` : 'theme_invite';

const appliquerTheme = (theme) => {
  document.documentElement.setAttribute('data-theme', theme);
  document.documentElement.classList.toggle('mode-sombre', theme === 'dark');
};

const basculerTheme = async () => {
  estSombre.value = !estSombre.value;
  const theme = estSombre.value ? 'dark' : 'light';
  appliquerTheme(theme);
  localStorage.setItem(obtenirCleTheme(), theme);
  if (authStore.user) {
    try {
      await api.post('/Settings/theme', JSON.stringify(theme), {
        headers: { 'Content-Type': 'application/json' },
      });
      authStore.user.themePreference = theme;
      localStorage.setItem('user', JSON.stringify(authStore.user));
    } catch (erreur) {
      console.error('Erreur sauvegarde thème :', erreur);
    }
  }
};

// ── Langue ──
const definirLangue = (lang) => {
  langueActive.value = lang;
  localStorage.setItem('lang', lang);
  fermerMenu();
};

const t = (cle) => ({
  FR: { search: 'Rechercher…  ⌘K', toggleTheme: 'Changer de thème' },
  EN: { search: 'Search…  ⌘K',     toggleTheme: 'Toggle theme' },
}[langueActive.value]?.[cle] || cle);

// ── Déconnexion ──
const deconnexion = () => {
  const themeActuel = estSombre.value ? 'dark' : 'light';
  localStorage.setItem('theme_invite', themeActuel);
  authStore.logout();
  router.push('/login');
};

// ── Raccourci clavier ⌘K ──
const gererToucheClavier = (e) => {
  if ((e.metaKey || e.ctrlKey) && e.key === 'k') {
    e.preventDefault();
    champRecherche.value?.focus();
  }
  if (e.key === 'Escape') fermerMenu();
};

onMounted(() => {
  const themeSauvegarde = authStore.user?.themePreference || localStorage.getItem(obtenirCleTheme()) || 'light';
  estSombre.value = themeSauvegarde === 'dark';
  appliquerTheme(themeSauvegarde);
  window.addEventListener('keydown', gererToucheClavier);
});

onUnmounted(() => window.removeEventListener('keydown', gererToucheClavier));
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=DM+Sans:wght@400;500;600;700&display=swap');

/* ════════════════════════════════
   BARRE DE NAVIGATION SUPÉRIEURE
════════════════════════════════ */
.barre-top {
  position: sticky;
  top: 0;
  z-index: 1050;
  height: 64px;
  background: rgba(255, 255, 255, 0.88);
  backdrop-filter: blur(16px);
  -webkit-backdrop-filter: blur(16px);
  border-bottom: 1px solid #edf0f4;
  font-family: 'DM Sans', sans-serif;
}

[data-theme="dark"] .barre-top {
  background: rgba(13, 17, 23, 0.88);
  border-bottom-color: rgba(255, 255, 255, 0.06);
}

.barre-top-interieur {
  height: 100%;
  max-width: 100%;
  padding: 0 24px;
  display: flex;
  align-items: center;
  gap: 16px;
}

/* ════════════════════════════════
   ZONE DE RECHERCHE
════════════════════════════════ */
.zone-recherche {
  flex: 1;
  max-width: 420px;
}

.recherche-wrap {
  position: relative;
  display: flex;
  align-items: center;
}

.recherche-ico {
  position: absolute;
  left: 13px;
  color: #9ca3af;
  pointer-events: none;
  flex-shrink: 0;
}

.champ-recherche {
  width: 100%;
  height: 38px;
  background: #f6f8fa;
  border: 1px solid #e2e8f0;
  border-radius: 10px;
  padding: 0 40px 0 38px;
  font-size: 13px;
  font-weight: 500;
  color: #0d1117;
  font-family: 'DM Sans', sans-serif;
  transition: border-color 0.2s, box-shadow 0.2s, background 0.2s;
  outline: none;
}

.champ-recherche::placeholder { color: #9ca3af; }

.champ-recherche:focus {
  background: #fff;
  border-color: #d97706;
  box-shadow: 0 0 0 3px rgba(217, 119, 6, 0.12);
}

[data-theme="dark"] .champ-recherche {
  background: rgba(255, 255, 255, 0.05);
  border-color: rgba(255, 255, 255, 0.08);
  color: #f0f6fc;
}
[data-theme="dark"] .champ-recherche:focus {
  background: rgba(255, 255, 255, 0.08);
  border-color: #d97706;
}

.recherche-raccourci {
  position: absolute;
  right: 10px;
  background: #edf0f4;
  color: #6b7280;
  font-size: 10px;
  font-weight: 700;
  padding: 2px 6px;
  border-radius: 5px;
  font-family: 'DM Sans', sans-serif;
  pointer-events: none;
}
[data-theme="dark"] .recherche-raccourci { background: rgba(255,255,255,0.08); color: #6b7280; }

/* ════════════════════════════════
   ACTIONS DE LA BARRE
════════════════════════════════ */
.barre-top-actions {
  display: flex;
  align-items: center;
  gap: 4px;
  margin-left: auto;
}

.btn-action {
  width: 38px;
  height: 38px;
  border-radius: 9px;
  border: 1px solid transparent;
  background: transparent;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  color: #57606a;
  transition: background 0.15s, color 0.15s, border-color 0.15s;
  position: relative;
  gap: 5px;
}

.btn-action:hover {
  background: #f6f8fa;
  color: #0d1117;
  border-color: #e2e8f0;
}

[data-theme="dark"] .btn-action { color: #8b949e; }
[data-theme="dark"] .btn-action:hover { background: rgba(255,255,255,0.07); color: #f0f6fc; border-color: rgba(255,255,255,0.08); }

/* Bouton langue */
.btn-langue {
  width: auto;
  padding: 0 10px;
  gap: 6px;
}
.libelle-langue {
  font-size: 12px;
  font-weight: 700;
  letter-spacing: 0.5px;
}
.chevron {
  color: #9ca3af;
  transition: transform 0.2s;
  flex-shrink: 0;
}
.chevron.pivote { transform: rotate(180deg); }

/* Point de notification */
.btn-notif { position: relative; }
.point-notif {
  position: absolute;
  top: 5px;
  right: 5px;
  min-width: 16px;
  height: 16px;
  padding: 0 4px;
  background: #d97706;
  color: white;
  font-size: 9px;
  font-weight: 800;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 2px solid white;
  line-height: 1;
}
[data-theme="dark"] .point-notif { border-color: #0d1117; }

/* Séparateur vertical */
.separateur {
  width: 1px;
  height: 24px;
  background: #e2e8f0;
  margin: 0 6px;
  flex-shrink: 0;
}
[data-theme="dark"] .separateur { background: rgba(255,255,255,0.08); }

/* ════════════════════════════════
   DÉCLENCHEUR PROFIL
════════════════════════════════ */
.declencheur-profil {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 4px 10px 4px 4px;
  border-radius: 10px;
  border: 1px solid transparent;
  background: transparent;
  cursor: pointer;
  transition: background 0.15s, border-color 0.15s;
}

.declencheur-profil:hover {
  background: #f6f8fa;
  border-color: #e2e8f0;
}
[data-theme="dark"] .declencheur-profil:hover {
  background: rgba(255,255,255,0.06);
  border-color: rgba(255,255,255,0.08);
}

.avatar-wrap {
  position: relative;
  flex-shrink: 0;
}

.avatar-img {
  width: 34px;
  height: 34px;
  border-radius: 8px;
  object-fit: cover;
  border: 1px solid #e2e8f0;
  display: block;
}
[data-theme="dark"] .avatar-img { border-color: rgba(255,255,255,0.1); }

.point-statut {
  position: absolute;
  bottom: -2px;
  right: -2px;
  width: 9px;
  height: 9px;
  background: #10b981;
  border: 2px solid white;
  border-radius: 50%;
}
[data-theme="dark"] .point-statut { border-color: #0d1117; }

.infos-utilisateur {
  display: none;
  flex-direction: column;
  text-align: left;
}

@media (min-width: 1024px) {
  .infos-utilisateur { display: flex; }
}

.nom-utilisateur {
  font-size: 13px;
  font-weight: 600;
  color: #0d1117;
  line-height: 1.2;
  white-space: nowrap;
}
[data-theme="dark"] .nom-utilisateur { color: #f0f6fc; }

.role-utilisateur {
  font-size: 10px;
  font-weight: 700;
  color: #d97706;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

/* ════════════════════════════════
   PANNEAUX DÉROULANTS
════════════════════════════════ */
.menu-deroulant { position: relative; }

.panneau-deroulant {
  position: absolute;
  top: calc(100% + 10px);
  right: 0;
  background: #ffffff;
  border: 1px solid #edf0f4;
  border-radius: 12px;
  box-shadow: 0 8px 32px rgba(13, 17, 23, 0.1), 0 2px 8px rgba(13, 17, 23, 0.06);
  z-index: 1100;
  min-width: 180px;
  overflow: hidden;
}

[data-theme="dark"] .panneau-deroulant {
  background: #161b22;
  border-color: rgba(255, 255, 255, 0.08);
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.4);
}

/* Panneau langue */
.panneau-langue {
  min-width: 160px;
  padding: 6px;
}

/* Panneau notifications */
.panneau-notif { width: 340px; padding: 0; }

.entete-panneau {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 14px 16px 12px;
  border-bottom: 1px solid #edf0f4;
}
[data-theme="dark"] .entete-panneau { border-bottom-color: rgba(255,255,255,0.06); }

.titre-panneau {
  font-size: 14px;
  font-weight: 700;
  color: #0d1117;
}
[data-theme="dark"] .titre-panneau { color: #f0f6fc; }

.puce-compteur {
  background: #fef3c7;
  color: #92400e;
  font-size: 10px;
  font-weight: 700;
  padding: 2px 8px;
  border-radius: 20px;
}
[data-theme="dark"] .puce-compteur { background: rgba(245,158,11,0.15); color: #fbbf24; }

.defilement-notif {
  max-height: 300px;
  overflow-y: auto;
}
.defilement-notif::-webkit-scrollbar { width: 3px; }
.defilement-notif::-webkit-scrollbar-thumb { background: #e2e8f0; border-radius: 3px; }

.ligne-notif {
  display: flex;
  align-items: flex-start;
  gap: 10px;
  padding: 12px 16px;
  border-bottom: 1px solid #f6f8fa;
  transition: background 0.15s;
  cursor: pointer;
}
.ligne-notif:hover { background: #fafafa; }
[data-theme="dark"] .ligne-notif { border-bottom-color: rgba(255,255,255,0.04); }
[data-theme="dark"] .ligne-notif:hover { background: rgba(255,255,255,0.03); }

.icone-notif-wrap {
  width: 28px;
  height: 28px;
  border-radius: 7px;
  background: #fef3c7;
  color: #d97706;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}
[data-theme="dark"] .icone-notif-wrap { background: rgba(245,158,11,0.12); }

.texte-notif {
  font-size: 12px;
  font-weight: 600;
  color: #0d1117;
  margin: 0 0 3px;
  line-height: 1.4;
}
[data-theme="dark"] .texte-notif { color: #e2e8f0; }

.heure-notif {
  font-size: 11px;
  color: #9ca3af;
}

.notif-vide {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
  padding: 32px 16px;
  color: #9ca3af;
  font-size: 13px;
  font-weight: 500;
}
.ico-vide { opacity: 0.4; }

.pied-panneau {
  padding: 10px 16px;
  border-top: 1px solid #edf0f4;
  text-align: center;
}
[data-theme="dark"] .pied-panneau { border-top-color: rgba(255,255,255,0.06); }

.btn-tout-lu {
  background: none;
  border: none;
  font-size: 12px;
  font-weight: 600;
  color: #d97706;
  cursor: pointer;
  font-family: 'DM Sans', sans-serif;
}
.btn-tout-lu:hover { color: #b45309; }

/* Panneau profil */
.panneau-profil {
  min-width: 220px;
  padding: 6px;
  right: 0;
}

.entete-profil {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 10px 12px 12px;
  background: #f6f8fa;
  border-radius: 8px;
  margin-bottom: 4px;
}
[data-theme="dark"] .entete-profil { background: rgba(255,255,255,0.04); }

.ep-avatar {
  width: 36px;
  height: 36px;
  border-radius: 8px;
  object-fit: cover;
  border: 1px solid #e2e8f0;
}

.ep-nom {
  font-size: 13px;
  font-weight: 700;
  color: #0d1117;
  line-height: 1.2;
}
[data-theme="dark"] .ep-nom { color: #f0f6fc; }

.ep-role {
  font-size: 10px;
  font-weight: 700;
  color: #d97706;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

/* Éléments déroulants */
.item-deroulant {
  display: flex;
  align-items: center;
  gap: 9px;
  padding: 9px 12px;
  border-radius: 8px;
  font-size: 13px;
  font-weight: 500;
  color: #374151;
  text-decoration: none;
  cursor: pointer;
  background: none;
  border: none;
  width: 100%;
  text-align: left;
  font-family: 'DM Sans', sans-serif;
  transition: background 0.13s, color 0.13s;
}
.item-deroulant:hover { background: #f6f8fa; color: #0d1117; }
[data-theme="dark"] .item-deroulant { color: #8b949e; }
[data-theme="dark"] .item-deroulant:hover { background: rgba(255,255,255,0.06); color: #f0f6fc; }

.item-deroulant.actif { color: #d97706; font-weight: 600; }
.item-deroulant.danger { color: #e11d48; }
.item-deroulant.danger:hover { background: #fff1f2; color: #be123c; }
[data-theme="dark"] .item-deroulant.danger:hover { background: rgba(225,29,72,0.08); }

.ico-item { color: #d97706; flex-shrink: 0; }
.item-deroulant.danger .ico-item { color: inherit; }

.ico-coche { margin-left: auto; color: #d97706; }
.drapeau { font-size: 16px; }

.separateur-menu {
  height: 1px;
  background: #edf0f4;
  margin: 4px 0;
}
[data-theme="dark"] .separateur-menu { background: rgba(255,255,255,0.06); }

/* ════════════════════════════════
   OVERLAY CLIC EXTÉRIEUR
════════════════════════════════ */
.clic-exterieur {
  position: fixed;
  inset: 0;
  z-index: 1099;
}

/* ════════════════════════════════
   TRANSITIONS
════════════════════════════════ */
.menu-fondu-enter-active { transition: opacity 0.15s ease, transform 0.15s ease; }
.menu-fondu-leave-active { transition: opacity 0.1s ease, transform 0.1s ease; }
.menu-fondu-enter-from   { opacity: 0; transform: translateY(-6px); }
.menu-fondu-leave-to     { opacity: 0; transform: translateY(-4px); }

.echange-icone-enter-active, .echange-icone-leave-active { transition: all 0.2s ease; }
.echange-icone-enter-from, .echange-icone-leave-to { opacity: 0; transform: scale(0.6) rotate(30deg); }
</style>