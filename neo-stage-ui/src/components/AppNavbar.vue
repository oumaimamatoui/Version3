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

        <!-- ══════════════════════════════════════════════
             LANGUE — affiche uniquement les langues
             activées par le Super Admin
        ══════════════════════════════════════════════ -->
        <div class="menu-deroulant" ref="menuDeroulantLang">
          <button
            class="btn-action btn-langue"
            @click="basculerMenu('lang')"
            :title="t('toggleTheme')"
          >
            <Globe :size="16" />
            <span class="libelle-langue">
              {{ localeActive }}
            </span>
            <span class="drapeau-actif">{{ localeActiveMeta?.flag }}</span>
            <ChevronDown :size="11" class="chevron" :class="{ pivote: menuOuvert === 'lang' }" />
          </button>

          <Transition name="menu-fondu">
            <div v-if="menuOuvert === 'lang'" class="panneau-deroulant panneau-langue">

              <!-- En-tête du panneau langue -->
              <div class="lang-panel-header">
                <Globe :size="12" class="lang-panel-ico" />
                <span>{{ t('notifications.title') === 'Notifications' ? 'Language' : 'Langue' }}</span>
              </div>

              <!-- Liste des langues disponibles (filtrée par Super Admin) -->
              <div class="lang-list">
                <button
                  v-for="locale in localesDisponibles"
                  :key="locale.code"
                  class="item-deroulant item-langue"
                  :class="{ actif: localeActive === locale.code }"
                  @click="definirLocale(locale.code)"
                >
                  <span class="drapeau">{{ locale.flag }}</span>
                  <div class="lang-info">
                    <span class="lang-libelle">{{ locale.label }}</span>
                    <span class="lang-natif">{{ locale.nativeName }}</span>
                  </div>
                  <span
                    v-if="locale.code === langConfigDefault"
                    class="lang-default-chip"
                    title="Langue par défaut"
                  >DEF</span>
                  <Check v-if="localeActive === locale.code" :size="13" class="ico-coche" />
                </button>

                <!-- Aucune langue disponible (cas impossible mais défensif) -->
                <div v-if="localesDisponibles.length === 0" class="lang-vide">
                  <span>Aucune langue configurée</span>
                </div>
              </div>

              <!-- Pied : mention Super Admin -->
              <div class="lang-panel-footer">
                <Shield :size="10" class="me-1" />
                Géré par le Super Admin
              </div>
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
                <span class="titre-panneau">{{ t('notifications.title') }}</span>
                <span class="puce-compteur">
                  {{ t('notifications.new').replace('{count}', notifStore.unreadCount) }}
                </span>
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
                  <span>{{ t('notifications.empty') }}</span>
                </div>
              </div>
              <div class="pied-panneau">
                <button class="btn-tout-lu" @click="notifStore.markAllAsRead()">
                  {{ t('notifications.markRead') }}
                </button>
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
                <UserCircle :size="14" class="ico-item" /><span>{{ t('profile.myProfile') }}</span>
              </router-link>
              <router-link to="/settings" class="item-deroulant" @click="fermerMenu">
                <Settings :size="14" class="ico-item" /><span>{{ t('settings.tabs.profile') }}</span>
              </router-link>
              <div class="separateur-menu"></div>
              <button class="item-deroulant danger" @click="deconnexion">
                <LogOut :size="14" class="ico-item" /><span>{{ t('profile.logout') }}</span>
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
import { ref, computed, onMounted, onUnmounted, watch } from 'vue';
import { useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import { useNotificationStore } from '@/stores/notification';
import { useAuthStore }         from '@/stores/auth';
import api from '@/services/api';

import {
  Search, Sun, Moon, Globe, Bell, BellOff, ChevronDown,
  Check, UserCircle, Settings, LogOut, Shield,
} from 'lucide-vue-next';

// ── Importation des helpers i18n (ajustez le chemin selon votre projet) ──
import {
  ALL_LOCALES,
  getLangConfig,
  setUserLocale,
  resolveUserLocale,
  LANG_CONFIG_KEY,
} from '@/i18n';   // ← même chemin que votre fichier i18n.js

// ════════════════════════════════════════════════════════════
//  STORES & ROUTER
// ════════════════════════════════════════════════════════════
const router     = useRouter();
const notifStore = useNotificationStore();
const authStore  = useAuthStore();
const { t, locale } = useI18n();

// ════════════════════════════════════════════════════════════
//  REFS UI
// ════════════════════════════════════════════════════════════
const champRecherche     = ref(null);
const rechercheFocalisee = ref(false);
const menuOuvert         = ref(null);
const estSombre          = ref(false);

// ════════════════════════════════════════════════════════════
//  LANGUE — Système Super Admin
// ════════════════════════════════════════════════════════════

/**
 * Config Super Admin lue depuis localStorage.
 * Re-lue à chaque ouverture du menu pour rester synchronisée
 * si le Super Admin a modifié la config dans un autre onglet.
 */
const langConfig = ref(getLangConfig());

/** Code de la locale actuellement sélectionnée par l'utilisateur */
const localeActive = ref(resolveUserLocale());

/** Langue par défaut configurée par le Super Admin */
const langConfigDefault = computed(() => langConfig.value.default);

/**
 * Liste des locales disponibles pour l'utilisateur =
 * intersection entre ALL_LOCALES (ordre fixe) et langConfig.available
 */
const localesDisponibles = computed(() =>
  ALL_LOCALES.filter(l => langConfig.value.available.includes(l.code))
);

/** Métadonnées de la locale active (flag, label, dir…) */
const localeActiveMeta = computed(() =>
  ALL_LOCALES.find(l => l.code === localeActive.value) ?? ALL_LOCALES[0]
);

/**
 * Changer la langue utilisateur.
 * - Vérifie que la langue est dans la liste autorisée.
 * - Appelle setUserLocale() qui écrit en localStorage.
 * - Met à jour la locale vue-i18n pour réactiver les traductions.
 * - Applique la direction RTL/LTR sur <html>.
 */
const definirLocale = (code) => {
  if (!langConfig.value.available.includes(code)) return;
  const ok = setUserLocale(code);   // écrit dans localStorage
  if (!ok) return;
  localeActive.value = code;
  locale.value       = code;        // vue-i18n réactif
  appliquerDirection(code);
  fermerMenu();
};

/** Applique l'attribut dir="rtl|ltr" sur <html> */
const appliquerDirection = (code) => {
  const meta = ALL_LOCALES.find(l => l.code === code);
  document.documentElement.setAttribute('dir', meta?.dir ?? 'ltr');
  document.documentElement.setAttribute('lang', code.toLowerCase());
};

/**
 * Re-synchronise la config Super Admin depuis localStorage.
 * Appelé à l'ouverture du menu langue pour rester à jour
 * sans recharger la page.
 */
const syncLangConfig = () => {
  langConfig.value = getLangConfig();
  // Si la locale active a été désactivée par le Super Admin → retomber sur le défaut
  if (!langConfig.value.available.includes(localeActive.value)) {
    definirLocale(langConfig.value.default);
  }
};

/**
 * Écouter les changements de config Super Admin depuis d'autres onglets
 * (storage event = cross-tab communication).
 */
const onStorageChange = (e) => {
  if (e.key === LANG_CONFIG_KEY) syncLangConfig();
};

// ════════════════════════════════════════════════════════════
//  MENUS DÉROULANTS
// ════════════════════════════════════════════════════════════
const basculerMenu = (nom) => {
  if (nom === 'lang') syncLangConfig();   // toujours à jour à l'ouverture
  menuOuvert.value = menuOuvert.value === nom ? null : nom;
};
const fermerMenu = () => { menuOuvert.value = null; };

// ════════════════════════════════════════════════════════════
//  THÈME
// ════════════════════════════════════════════════════════════
const obtenirCleTheme = () =>
  authStore.user?.id ? `theme_${authStore.user.id}` : 'theme_invite';

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
    } catch (err) {
      console.error('Erreur sauvegarde thème :', err);
    }
  }
};

// ════════════════════════════════════════════════════════════
//  PROFIL
// ════════════════════════════════════════════════════════════
const affichageRole = computed(() => ({
  SuperAdmin:      'SuperAdmin',
  AdminEntreprise: 'Administrateur',
  Evaluateur:      'Évaluateur',
  Candidat:        'Candidat',
  Recruteur:       'RH / Recruteur',
}[authStore.role] || authStore.role || 'Utilisateur'));

const urlPhotoProfil = computed(() => {
  if (authStore.user?.photoUrl)
    return `http://localhost:5172${authStore.user.photoUrl}`;
  return `https://ui-avatars.com/api/?name=${encodeURIComponent(
    authStore.user?.name || 'User'
  )}&background=0f172a&color=fff&bold=true`;
});

// ════════════════════════════════════════════════════════════
//  DÉCONNEXION
// ════════════════════════════════════════════════════════════
const deconnexion = () => {
  const themeActuel = estSombre.value ? 'dark' : 'light';
  localStorage.setItem('theme_invite', themeActuel);
  authStore.logout();
  router.push('/login');
};

// ════════════════════════════════════════════════════════════
//  RACCOURCIS CLAVIER
// ════════════════════════════════════════════════════════════
const gererToucheClavier = (e) => {
  if ((e.metaKey || e.ctrlKey) && e.key === 'k') {
    e.preventDefault();
    champRecherche.value?.focus();
  }
  if (e.key === 'Escape') fermerMenu();
};

// ════════════════════════════════════════════════════════════
//  LIFECYCLE
// ════════════════════════════════════════════════════════════
onMounted(() => {
  // Thème
  const themeSauvegarde =
    authStore.user?.themePreference ||
    localStorage.getItem(obtenirCleTheme()) ||
    'light';
  estSombre.value = themeSauvegarde === 'dark';
  appliquerTheme(themeSauvegarde);

  // Langue initiale
  localeActive.value = resolveUserLocale();
  locale.value       = localeActive.value;
  appliquerDirection(localeActive.value);

  // Listeners
  window.addEventListener('keydown', gererToucheClavier);
  window.addEventListener('storage', onStorageChange);   // cross-tab sync
});

onUnmounted(() => {
  window.removeEventListener('keydown', gererToucheClavier);
  window.removeEventListener('storage', onStorageChange);
});
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=DM+Sans:wght@400;500;600;700&family=JetBrains+Mono:wght@700&display=swap');

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
   RECHERCHE
════════════════════════════════ */
.zone-recherche { flex: 1; max-width: 420px; }
.recherche-wrap { position: relative; display: flex; align-items: center; }
.recherche-ico  { position: absolute; left: 13px; color: #9ca3af; pointer-events: none; flex-shrink: 0; }

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
  font-family: 'JetBrains Mono', monospace;
  pointer-events: none;
}
[data-theme="dark"] .recherche-raccourci { background: rgba(255,255,255,0.08); color: #6b7280; }

/* ════════════════════════════════
   ACTIONS
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
[data-theme="dark"] .btn-action:hover {
  background: rgba(255,255,255,0.07);
  color: #f0f6fc;
  border-color: rgba(255,255,255,0.08);
}

/* ════════════════════════════════
   BOUTON LANGUE
════════════════════════════════ */
.btn-langue {
  width: auto;
  padding: 0 10px;
  gap: 5px;
}
.libelle-langue {
  font-size: 11px;
  font-weight: 800;
  letter-spacing: 0.8px;
  font-family: 'JetBrains Mono', monospace;
}
.drapeau-actif {
  font-size: 14px;
  line-height: 1;
}
.chevron {
  color: #9ca3af;
  transition: transform 0.2s;
  flex-shrink: 0;
}
.chevron.pivote { transform: rotate(180deg); }

/* ════════════════════════════════
   PANNEAU LANGUE — AMÉLIORÉ
════════════════════════════════ */
.panneau-langue {
  min-width: 220px;
  padding: 0;
  overflow: hidden;
}

.lang-panel-header {
  display: flex;
  align-items: center;
  gap: 7px;
  padding: 10px 14px 8px;
  font-size: 10px;
  font-weight: 800;
  color: #94a3b8;
  text-transform: uppercase;
  letter-spacing: 1px;
  border-bottom: 1px solid #edf0f4;
  font-family: 'JetBrains Mono', monospace;
}
[data-theme="dark"] .lang-panel-header {
  color: #4b5563;
  border-bottom-color: rgba(255,255,255,0.06);
}
.lang-panel-ico { opacity: 0.6; }

.lang-list { padding: 6px; }

/* Item langue enrichi */
.item-langue {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 10px 12px;
  border-radius: 9px;
}
.lang-info {
  display: flex;
  flex-direction: column;
  text-align: left;
  flex: 1;
  min-width: 0;
}
.lang-libelle {
  font-size: 13px;
  font-weight: 600;
  color: #0d1117;
  line-height: 1.2;
}
[data-theme="dark"] .lang-libelle { color: #e2e8f0; }
.lang-natif {
  font-size: 10px;
  color: #94a3b8;
  font-weight: 500;
  margin-top: 1px;
}

/* Chip "DEF" pour la langue par défaut */
.lang-default-chip {
  font-size: 9px;
  font-weight: 800;
  font-family: 'JetBrains Mono', monospace;
  letter-spacing: 0.5px;
  color: #d97706;
  background: #fef3c7;
  border: 1px solid #fde68a;
  padding: 1px 6px;
  border-radius: 5px;
  flex-shrink: 0;
}
[data-theme="dark"] .lang-default-chip {
  background: rgba(245,158,11,0.12);
  border-color: rgba(245,158,11,0.25);
}

/* Pied de panneau langue */
.lang-panel-footer {
  display: flex;
  align-items: center;
  padding: 8px 14px;
  font-size: 10px;
  font-weight: 700;
  color: #94a3b8;
  border-top: 1px solid #edf0f4;
  background: #f8fafc;
  font-family: 'JetBrains Mono', monospace;
  letter-spacing: 0.3px;
}
[data-theme="dark"] .lang-panel-footer {
  background: rgba(255,255,255,0.02);
  border-top-color: rgba(255,255,255,0.05);
  color: #4b5563;
}
.me-1 { margin-right: 4px; }

/* Vide */
.lang-vide {
  padding: 16px;
  text-align: center;
  font-size: 12px;
  color: #94a3b8;
}

/* ════════════════════════════════
   NOTIFICATIONS
════════════════════════════════ */
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

.separateur {
  width: 1px;
  height: 24px;
  background: #e2e8f0;
  margin: 0 6px;
  flex-shrink: 0;
}
[data-theme="dark"] .separateur { background: rgba(255,255,255,0.08); }

/* ════════════════════════════════
   PROFIL
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

.avatar-wrap { position: relative; flex-shrink: 0; }
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
   PANNEAUX DÉROULANTS (base)
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

/* Notifications */
.panneau-notif { width: 340px; padding: 0; }

.entete-panneau {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 14px 16px 12px;
  border-bottom: 1px solid #edf0f4;
}
[data-theme="dark"] .entete-panneau { border-bottom-color: rgba(255,255,255,0.06); }

.titre-panneau { font-size: 14px; font-weight: 700; color: #0d1117; }
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

.defilement-notif { max-height: 300px; overflow-y: auto; }
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

.heure-notif { font-size: 11px; color: #9ca3af; }

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

/* Profil */
.panneau-profil { min-width: 220px; padding: 6px; right: 0; }

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

.ep-avatar { width: 36px; height: 36px; border-radius: 8px; object-fit: cover; border: 1px solid #e2e8f0; }
.ep-nom { font-size: 13px; font-weight: 700; color: #0d1117; line-height: 1.2; }
[data-theme="dark"] .ep-nom { color: #f0f6fc; }
.ep-role { font-size: 10px; font-weight: 700; color: #d97706; text-transform: uppercase; letter-spacing: 0.5px; }

/* Items déroulants */
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
.item-deroulant.actif { color: #d97706; font-weight: 600; background: #fffbeb; }
[data-theme="dark"] .item-deroulant.actif { background: rgba(245,158,11,0.08); }
.item-deroulant.danger { color: #e11d48; }
.item-deroulant.danger:hover { background: #fff1f2; color: #be123c; }
[data-theme="dark"] .item-deroulant.danger:hover { background: rgba(225,29,72,0.08); }

.ico-item { color: #d97706; flex-shrink: 0; }
.item-deroulant.danger .ico-item { color: inherit; }
.ico-coche { margin-left: auto; color: #d97706; flex-shrink: 0; }
.drapeau { font-size: 18px; flex-shrink: 0; }

.separateur-menu {
  height: 1px;
  background: #edf0f4;
  margin: 4px 0;
}
[data-theme="dark"] .separateur-menu { background: rgba(255,255,255,0.06); }

/* ════════════════════════════════
   OVERLAY
════════════════════════════════ */
.clic-exterieur { position: fixed; inset: 0; z-index: 1099; }

/* ════════════════════════════════
   TRANSITIONS
════════════════════════════════ */
.menu-fondu-enter-active { transition: opacity 0.15s ease, transform 0.15s ease; }
.menu-fondu-leave-active { transition: opacity 0.1s ease, transform 0.1s ease; }
.menu-fondu-enter-from   { opacity: 0; transform: translateY(-6px); }
.menu-fondu-leave-to     { opacity: 0; transform: translateY(-4px); }

.echange-icone-enter-active,
.echange-icone-leave-active { transition: all 0.2s ease; }
.echange-icone-enter-from,
.echange-icone-leave-to     { opacity: 0; transform: scale(0.6) rotate(30deg); }
</style>