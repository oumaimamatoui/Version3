import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

/**
 * 1. IMPORTATIONS DES VUES
 */
// Authentification & Public
import HomeView from '../views/HomeView.vue'
import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue'
import TarificationView from '../views/TarificationView.vue'
import DefinirMotDePasseView from '../views/DefinirMotDePasseView.vue'
import ActivationView from '../views/Activation.vue'
import ForgotPasswordView from '../views/ForgotPasswordView.vue'
import ResetPasswordView from '../views/ResetPasswordView.vue'
import NotFoundView from '../views/NotFoundView.vue'

// Commun (Connecté)
import DashboardView from '../views/DashboardView.vue'
import ProfilView from '../views/ProfilView.vue'
import SettingsView from '../views/SettingsView.vue'
import AideSupportView from '../views/AideSupportView.vue'
import GoogleCallbackView from '../views/GoogleCallbackView.vue'

// Espace Candidat
import MyTestsView from '../views/MyTestsView.vue'
import HistoryView from '../views/HistoryView.vue'
import ResultsView from '../views/ResultsView.vue'
import AccueilExamenView from '../views/AccueilExamenView.vue'
import QuizView from '../views/QuizView.vue'

// Espace Évaluateur / RH / Recruteur
import BanqueQuestionsView from '../views/BanqueQuestionsView.vue'
import CampagnesView from '../views/CampagnesView.vue'
import AIGeneratorView from '../views/AIGeneratorView.vue'
import AnalyseComportementaleView from '../views/AnalyseComportementaleView.vue'
import StatsView from '../views/StatsView.vue'

// Espace Admin Entreprise
import GroupsView from '../views/GroupsView.vue'
import RolesManagementView from '../views/RolesManagementView.vue'
import StaffMembersView from '../views/StaffMembersView.vue'
import ListeCandidatsView from '../views/ListeCandidatsView.vue'
import DetailsCandidatView from '../views/DetailsCandidatView.vue'
import InviteView from '../views/InviteView.vue'

// Espace Super Admin
import SuperAdminView from '../views/SuperAdminView.vue'
import PlatformUsersView from '../views/PlatformUsersView.vue'
import AnalyticsView from '../views/AnalyticsView.vue'

const routes = [
  // --- PUBLIC ---
  { path: '/', name: 'home', component: HomeView },
  { path: '/login', name: 'login', component: LoginView },
  { path: '/register', name: 'register', component: RegisterView },
  { path: '/pricing', name: 'pricing', component: TarificationView },
  { path: '/activate-account', name: 'activation', component: ActivationView },
  { path: '/forgot-password', name: 'forgot-password', component: ForgotPasswordView },
  { path: '/reset-password', name: 'reset-password', component: ResetPasswordView },
  { path: '/definir-mot-de-passe', name: 'definir-mot-de-passe', component: DefinirMotDePasseView },

  // --- COMMON (AUTH) ---
  { path: '/dashboard', name: 'dashboard', component: DashboardView, meta: { requiresAuth: true } },
  { path: '/profile', name: 'profile', component: ProfilView, meta: { requiresAuth: true } },
  { path: '/settings', name: 'settings', component: SettingsView, meta: { requiresAuth: true } },
  { path: '/aide-support', name: 'aide-support', component: AideSupportView, meta: { requiresAuth: true } },
  { path: '/google-callback', name: 'google-callback', component: GoogleCallbackView, meta: { requiresAuth: true } },

  // --- CANDIDATE ---
  { path: '/my-tests', name: 'my-tests', component: MyTestsView, meta: { requiresAuth: true, role: 'Candidat' } },
  { path: '/results', name: 'results', component: ResultsView, meta: { requiresAuth: true, role: 'Candidat' } },
  { path: '/history', name: 'history', component: HistoryView, meta: { requiresAuth: true, role: 'Candidat' } },
  { path: '/exam-lobby/:id', name: 'exam-lobby', component: AccueilExamenView, meta: { requiresAuth: true, role: 'Candidat' } },
  { path: '/quiz/:id', name: 'quiz', component: QuizView, meta: { requiresAuth: true, role: 'Candidat' } },

  // --- RECRUITER / EVALUATOR ---
  { path: '/campaigns', name: 'campaigns', component: CampagnesView, meta: { requiresAuth: true, permission: ['view_tests', 'inv_can'] } },
  { path: '/questions', name: 'questions', component: BanqueQuestionsView, meta: { requiresAuth: true, permission: 'edit_bank' } },
  { path: '/ai-generator', name: 'ai-generator', component: AIGeneratorView, meta: { requiresAuth: true, permission: 'edit_bank' } },
  { path: '/analyse-comportementale', name: 'analyse-comportementale', component: AnalyseComportementaleView, meta: { requiresAuth: true, permission: 'view_tests' } },
  { path: '/stats', name: 'stats', component: StatsView, meta: { requiresAuth: true, permission: 'view_tests' } },

  // --- ADMIN ENTREPRISE ---
  { path: '/candidates-list', name: 'candidates-list', component: ListeCandidatsView, meta: { requiresAuth: true, permission: 'view_can' } },
  { path: '/details-candidat/:id', name: 'details-candidat', component: DetailsCandidatView, meta: { requiresAuth: true, permission: 'view_can' } },
  { path: '/invite', name: 'invite', component: InviteView, meta: { requiresAuth: true, permission: 'inv_can' } },
  { path: '/groups', name: 'groups', component: GroupsView, meta: { requiresAuth: true, permission: 'view_can' } },
  { path: '/staff-members', name: 'staff-members', component: StaffMembersView, meta: { requiresAuth: true, permission: 'view_staff' } },
  { path: '/roles', name: 'roles', component: RolesManagementView, meta: { requiresAuth: true, permission: 'view_rol' } },

  // --- SUPER ADMIN ---
  { path: '/super-admin', name: 'super-admin', component: SuperAdminView, meta: { requiresAuth: true, role: 'SuperAdmin' } },
  { path: '/platform-users', name: 'platform-users', component: PlatformUsersView, meta: { requiresAuth: true, role: 'SuperAdmin' } },
  { path: '/platform-analytics', name: 'platform-analytics', component: AnalyticsView, meta: { requiresAuth: true, role: 'SuperAdmin' } },

  // --- 404 ---
  { path: '/:pathMatch(.*)*', name: 'not-found', component: NotFoundView }
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
  scrollBehavior() { return { top: 0 } }
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  const isAuthenticated = authStore.isAuthenticated
  const userRole = authStore.role

  if (to.meta.requiresAuth && !isAuthenticated) {
    return next({ name: 'login' })
  }

  if (to.meta.role) {
    const rolesAutorises = Array.isArray(to.meta.role) ? to.meta.role : [to.meta.role]
    if (!rolesAutorises.includes(userRole)) {
      return next({ name: 'dashboard' }) 
    }
  }

  if (to.meta.permission) {
    const requiredPerms = Array.isArray(to.meta.permission) ? to.meta.permission : [to.meta.permission];
    const hasAccess = requiredPerms.some(p => authStore.hasPermission(p));
    if (!hasAccess) {
      return next({ name: 'dashboard' });
    }
  }

  next()
})

export default router