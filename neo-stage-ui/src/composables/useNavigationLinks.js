import { computed } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useI18n } from 'vue-i18n';

export function useNavigationLinks() {
  const authStore = useAuthStore();
  const { t } = useI18n();

  const role = computed(() => authStore.role);

  const tousLesLiens = computed(() => {
    const r = role.value;
    const estEntreprise = r !== 'SuperAdmin' && r !== 'Candidat';

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
        visible: estEntreprise && (authStore.hasPermission('view_tests') || authStore.hasPermission('inv_can')) && r !== 'AdminEntreprise',
      },
      {
        label: t('sidebar.links.bank'),
        vers: '/questions',
        icone: 'fa-solid fa-vault',
        iconeClasse: 'ic-gold',
        visible: estEntreprise && authStore.hasPermission('edit_bank') && r !== 'AdminEntreprise',
      },
      {
        label: t('sidebar.links.ai'),
        vers: '/ai-generator',
        icone: 'fa-solid fa-wand-magic-sparkles',
        iconeClasse: 'ic-violet',
        visible: estEntreprise && authStore.hasPermission('edit_bank') && r !== 'AdminEntreprise',
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
        visible: estEntreprise && r === 'AdminEntreprise',
      },
      // ── SuperAdmin ──
      {
        label: t('sidebar.links.organizations'),
        vers: '/super-admin',
        icone: 'fa-solid fa-building-columns',
        iconeClasse: 'ic-orange',
        visible: r === 'SuperAdmin',
      },
      {
        label: t('sidebar.links.manageUsers'),
        vers: '/platform-users',
        icone: 'fa-solid fa-users-gear',
        iconeClasse: 'ic-amber',
        visible: r === 'SuperAdmin',
      },
      {
        label: t('sidebar.links.globalStats'),
        vers: '/super-admin/statistiques',
        icone: 'fa-solid fa-earth-europe',
        iconeClasse: 'ic-gold',
        visible: r === 'SuperAdmin',
      },
      {
        label: t('sidebar.links.analyticsView'),
        vers: '/super-admin-analytics',
        icone: 'fa-solid fa-chart-simple',
        iconeClasse: 'ic-violet',
        visible: r === 'SuperAdmin',
      },
      // ── Candidat ──
      {
        label: t('sidebar.links.takeTest'),
        vers: '/my-tests',
        icone: 'fa-solid fa-circle-play',
        iconeClasse: 'ic-green',
        visible: r === 'Candidat',
      },
      {
        label: t('sidebar.links.results'),
        vers: '/results',
        icone: 'fa-solid fa-trophy',
        iconeClasse: 'ic-gold',
        visible: r === 'Candidat',
      },
      {
        label: t('sidebar.links.history'),
        vers: '/history',
        icone: 'fa-solid fa-clock-rotate-left',
        iconeClasse: 'ic-slate',
        visible: r === 'Candidat',
      },
    ];
  });

  return { tousLesLiens, role };
}
