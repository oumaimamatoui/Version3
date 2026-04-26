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
import MyTestsView from '../views/MyTestsView.vue'         // "Passer un test"
import HistoryView from '../views/HistoryView.vue'         // "Historique"
import ResultsView from '../views/ResultsView.vue'         // "Analyse IA / Rapport détaillé"
import AccueilExamenView from '../views/AccueilExamenView.vue'
import QuizView from '../views/QuizView.vue'

// Espace Évaluateur / RH / Recruteur
import BanqueQuestionsView from '../views/BanqueQuestionsView.vue'
import CampagnesView from '../views/CampagnesView.vue'
import AIGeneratorView from '../views/AIGeneratorView.vue'
import EvaluateurAssessments from '../views/EvaluateurAssessments.vue'
import PlanningView from '../views/PlanningView.vue'
import AnalyseComportementaleView from '../views/AnalyseComportementaleView.vue'
import StatsView from '../views/StatsView.vue'
import SuiviPerformanceView from '../views/SuiviPerformanceView.vue'
import ComparaisonView from '../views/ComparaisonView.vue'

// Espace Admin Entreprise
import GroupsView from '../views/GroupsView.vue'
import RolesManagementView from '../views/RolesManagementView.vue'
import StaffMembersView from '../views/StaffMembersView.vue'
import ListeCandidatsView from '../views/ListeCandidatsView.vue'
import DetailsCandidatView from '../views/DetailsCandidatView.vue'
import InviteView from '../views/InviteView.vue'
import RapportsView from '../views/RapportsView.vue'
import GestionGroupesView from '../views/GestionGroupesView.vue'
import GestionStaffView from '../views/GestionStaffView.vue'

// Espace Super Admin
import SuperAdminView from '../views/SuperAdminView.vue'
import GestionAbonnementsView from '../views/GestionAbonnementsView.vue'
import GestionEntreprisesView from '../views/GestionEntreprisesView.vue'
import PlatformUsersView from '../views/PlatformUsersView.vue'
import AnalyticsView from '../views/AnalyticsView.vue'
import SuperAdminAnalytics from '../views/SuperAdminAnalytics.vue'

const routes = [
  /**
   * ROUTES PUBLIQUES
   */
  { path: '/', name: 'home', component: HomeView },
  { path: '/login', name: 'login', component: LoginView },
  { path: '/register', name: 'register', component: RegisterView },
  { path: '/pricing', name: 'pricing', component: TarificationView },
  { path: '/activate-role', name: 'activation', component: ActivationView },
  { path: '/forgot-password', name: 'forgot-password', component: ForgotPasswordView },
  { path: '/reset-password', name: 'reset-password', component: ResetPasswordView },
  { path: '/definir-mot-de-passe', name: 'definir-mot-de-passe', component: DefinirMotDePasseView },

  /**
   * ROUTES COMMUNES (AUTHENTIFIÉES)
   */
  { path: '/dashboard', name: 'dashboard', component: DashboardView, meta: { requiresAuth: true } },
  { path: '/profile', name: 'profile', component: ProfilView, meta: { requiresAuth: true } },
  { path: '/settings', name: 'settings', component: SettingsView, meta: { requiresAuth: true } },
  { path: '/aide-support', name: 'aide-support', component: AideSupportView, meta: { requiresAuth: true } },
  { path: '/google-callback', name: 'google-callback', component: GoogleCallbackView, meta: { requiresAuth: true } },

  /**
   * ESPACE CANDIDAT (SÉCURISÉ)
   */
  // 1. "Passer un test"
{ path: '/my-tests', name: 'my-tests', component: MyTestsView, meta: { requiresAuth: true, role: 'Candidat' } },
  
  // 2. Mes résultats (La vue Performance IA que vous avez montrée)
  { path: '/results', name: 'results', component: ResultsView, meta: { requiresAuth: true, role: 'Candidat' } },
  { path: '/results/:id', name: 'results-detail', component: ResultsView, meta: { requiresAuth: true, role: 'Candidat' } },

  // 3. Historique
  { path: '/history', name: 'history', component: HistoryView, meta: { requiresAuth: true, role: 'Candidat' } },

  { path: '/quiz/:id', name: 'quiz', component: QuizView, meta: { requiresAuth: true, role: 'Candidat' } },
  { path: '/:pathMatch(.*)*', name: 'not-found', component: NotFoundView },
  // Processus d'examen
  { path: '/quiz', redirect: { name: 'my-tests' } },
  { path: '/quiz/:id', name: 'quiz', component: QuizView, meta: { requiresAuth: true, role: 'Candidat' } },
  { path: '/exam-lobby', redirect: { name: 'my-tests' } },
  { path: '/exam-lobby/:id', name: 'exam-lobby', component: AccueilExamenView, meta: { requiresAuth: true, role: 'Candidat' } },

  /**
   * ESPACE ÉVALUATEUR / RH / RECRUTEUR
   */
  { path: '/questions', name: 'questions', component: BanqueQuestionsView, meta: { requiresAuth: true, role: ['Evaluateur', 'AdminEntreprise', 'Recruteur'] } },
  { path: '/campaigns', name: 'campaigns', component: CampagnesView, meta: { requiresAuth: true, role: ['Evaluateur', 'AdminEntreprise', 'Recruteur'] } },
  { path: '/ai-generator', name: 'ai-generator', component: AIGeneratorView, meta: { requiresAuth: true, role: ['Evaluateur', 'AdminEntreprise', 'Recruteur'] } },
  { path: '/analyse-comportementale', name: 'analyse-comportementale', component: AnalyseComportementaleView, meta: { requiresAuth: true, role: ['Evaluateur', 'AdminEntreprise', 'Recruteur'] } },
  { path: '/stats', name: 'stats', component: StatsView, meta: { requiresAuth: true, role: ['Evaluateur', 'AdminEntreprise', 'Recruteur'] } },
  { path: '/comparaison', name: 'comparaison', component: ComparaisonView, meta: { requiresAuth: true, role: ['Evaluateur', 'AdminEntreprise', 'Recruteur'] } },
  { path: '/suivi-performance', name: 'suivi-performance', component: SuiviPerformanceView, meta: { requiresAuth: true, role: ['Evaluateur', 'AdminEntreprise', 'Recruteur'] } },
  { path: '/sessions', name: 'sessions', component: PlanningView, meta: { requiresAuth: true, role: 'Evaluateur' } },
  { path: '/test-builder', name: 'test-builder', component: EvaluateurAssessments, meta: { requiresAuth: true, role: 'Evaluateur' } },

  /**
   * ESPACE ADMINISTRATION ENTREPRISE
   */
  { path: '/roles', name: 'roles', component: RolesManagementView, meta: { requiresAuth: true, role: 'AdminEntreprise' } },
  { path: '/groups', name: 'groups', component: GroupsView, meta: { requiresAuth: true, role: ['AdminEntreprise', 'Recruteur'] } },
  { path: '/gestion-groupes', name: 'gestion-groupes', component: GestionGroupesView, meta: { requiresAuth: true, role: ['AdminEntreprise', 'Recruteur'] } },
  { path: '/candidates-list', name: 'candidates-list', component: ListeCandidatsView, meta: { requiresAuth: true, role: ['AdminEntreprise', 'Recruteur'] } },
  { path: '/details-candidat/:id', name: 'details-candidat', component: DetailsCandidatView, meta: { requiresAuth: true, role: ['AdminEntreprise', 'Recruteur'] } },
  { path: '/invite', name: 'invite', component: InviteView, meta: { requiresAuth: true, role: ['AdminEntreprise', 'Recruteur'] } },
  { path: '/reporting', name: 'reporting', component: RapportsView, meta: { requiresAuth: true, role: ['AdminEntreprise', 'Recruteur'] } },
  { path: '/staff-members', name: 'staff-members', component: StaffMembersView, meta: { requiresAuth: true, role: ['AdminEntreprise', 'Recruteur'] } },
  { path: '/gestion-staff', name: 'gestion-staff', component: GestionStaffView, meta: { requiresAuth: true, role: ['AdminEntreprise', 'Recruteur'] } },

  /**
   * ESPACE SUPER ADMIN
   */
  { path: '/super-admin', name: 'super-admin', component: SuperAdminView, meta: { requiresAuth: true, role: 'SuperAdmin' } },
  { path: '/gestion-abonnements', name: 'gestion-abonnements', component: GestionAbonnementsView, meta: { requiresAuth: true, role: 'SuperAdmin' } },
  { path: '/gestion-entreprises', name: 'gestion-entreprises', component: GestionEntreprisesView, meta: { requiresAuth: true, role: 'SuperAdmin' } },
  { path: '/platform-users', name: 'platform-users', component: PlatformUsersView, meta: { requiresAuth: true, role: 'SuperAdmin' } },
  { path: '/platform-analytics', name: 'platform-analytics', component: AnalyticsView, meta: { requiresAuth: true, role: 'SuperAdmin' } },
  { path: '/super-admin-analytics', name: 'super-admin-analytics', component: SuperAdminAnalytics, meta: { requiresAuth: true, role: 'SuperAdmin' } },

  /**
   * ERREUR 404
   */
  { path: '/:pathMatch(.*)*', name: 'not-found', component: NotFoundView }
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
  scrollBehavior() { return { top: 0 } }
})

/**
 * GUARD DE NAVIGATION (SÉCURITÉ RBAC)
 */
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  const isAuthenticated = authStore.isAuthenticated
  const userRole = authStore.role

  // 1. Redirection vers login si la route nécessite une auth et que l'user n'est pas connecté
  if (to.meta.requiresAuth && !isAuthenticated) {
    return next({ name: 'login' })
  }

  // 2. Vérification des droits d'accès par rôle
  if (to.meta.role) {
    const rolesAutorises = Array.isArray(to.meta.role) ? to.meta.role : [to.meta.role]
    if (!rolesAutorises.includes(userRole)) {
      return next({ name: 'dashboard' }) 
    }
  }

  next()
})

export default router