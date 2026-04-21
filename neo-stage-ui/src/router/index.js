import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

/**
 * 1. IMPORTATIONS DES VUES
 */
import HomeView from '../views/HomeView.vue'
import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue'
import TarificationView from '../views/TarificationView.vue'
import DefinirMotDePasseView from '../views/DefinirMotDePasseView.vue'
import ActivationView from '../views/Activation.vue' // Votre vue avec le Robot AI
import ForgotPasswordView from '../views/ForgotPasswordView.vue'
import ResetPasswordView from '../views/ResetPasswordView.vue'
import NotFoundView from '../views/NotFoundView.vue'

import DashboardView from '../views/DashboardView.vue'
import ProfilView from '../views/ProfilView.vue'
import SettingsView from '../views/SettingsView.vue'
import AideSupportView from '../views/AideSupportView.vue'

import AccueilExamenView from '../views/AccueilExamenView.vue'
import QuizView from '../views/QuizView.vue'
import ResultsView from '../views/ResultsView.vue'
import HistoryView from '../views/HistoryView.vue'

import BanqueQuestionsView from '../views/BanqueQuestionsView.vue'
import CampagnesView from '../views/CampagnesView.vue'
import ListeCandidatsView from '../views/ListeCandidatsView.vue'
import DetailsCandidatView from '../views/DetailsCandidatView.vue'
import InviteView from '../views/InviteView.vue'
import RapportsView from '../views/RapportsView.vue'
import AIGeneratorView from '../views/AIGeneratorView.vue'
import EvaluateurAssessments from '../views/EvaluateurAssessments.vue'
import PlanningView from '../views/PlanningView.vue'

import AnalyseComportementaleView from '../views/AnalyseComportementaleView.vue'
import StatsView from '../views/StatsView.vue'

// Vues spécifiques Admin Entreprise
import GroupsView from '../views/GroupsView.vue'
import RolesManagementView from '../views/RolesManagementView.vue'
import StaffMembersView from '../views/StaffMembersView.vue'

import SuperAdminView from '../views/SuperAdminView.vue'
import GestionAbonnementsView from '../views/GestionAbonnementsView.vue'
import PlatformUsersView from '../views/PlatformUsersView.vue'
import AnalyticsView from '../views/AnalyticsView.vue'
import GoogleCallbackView from '../views/GoogleCallbackView.vue'

const routes = [
  /**
   * ROUTES PUBLIQUES
   */
  { path: '/', name: 'home', component: HomeView },
  { path: '/login', name: 'login', component: LoginView },
  { path: '/register', name: 'register', component: RegisterView },
  { path: '/pricing', name: 'pricing', component: TarificationView },

  //  SYNCHRONISATION : Cette route correspond maintenant au lien envoyé par l'email du backend
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

  /**
   * ESPACE CANDIDAT
   */
  { path: '/exam-lobby/:id', name: 'exam-lobby', component: AccueilExamenView, meta: { requiresAuth: true, role: 'Candidat' } },
  { path: '/quiz/:id', name: 'quiz', component: QuizView, meta: { requiresAuth: true, role: 'Candidat' } },
  { path: '/results/:id', name: 'results', component: ResultsView, meta: { requiresAuth: true, role: 'Candidat' } },
  { path: '/history', name: 'history', component: HistoryView, meta: { requiresAuth: true, role: 'Candidat' } },

  /**
   * ESPACE ÉVALUATEUR / RH / RECRUTEUR
   */
  { path: '/questions', name: 'questions', component: BanqueQuestionsView, meta: { requiresAuth: true, role: ['Evaluateur', 'AdminEntreprise', 'Recruteur'] } },

  { path: '/ai-generator', name: 'ai-generator', component: AIGeneratorView, meta: { requiresAuth: true, role: ['Evaluateur', 'AdminEntreprise', 'Recruteur'] } },
  { path: '/analyse-comportementale', name: 'analyse-comportementale', component: AnalyseComportementaleView, meta: { requiresAuth: true, role: ['Evaluateur', 'AdminEntreprise', 'Recruteur'] } },
  { path: '/sessions', name: 'sessions', component: PlanningView, meta: { requiresAuth: true, role: 'Evaluateur' } },
  { path: '/stats', name: 'stats', component: StatsView, meta: { requiresAuth: true, role: ['Evaluateur', 'AdminEntreprise', 'Recruteur'] } },
  { path: '/test-builder', name: 'test-builder', component: EvaluateurAssessments, meta: { requiresAuth: true, role: 'Evaluateur' } },

  /**
   * ESPACE ADMINISTRATION ENTREPRISE & RECRUTEMENT
   */
  { path: '/roles', name: 'roles', component: RolesManagementView, meta: { requiresAuth: true, role: 'AdminEntreprise' } },
  { path: '/groups', name: 'groups', component: GroupsView, meta: { requiresAuth: true, role: ['AdminEntreprise', 'Recruteur'] } },
  { path: '/campaigns', name: 'campaigns', component: CampagnesView, meta: { requiresAuth: true, role: ['Evaluateur', 'AdminEntreprise', 'Recruteur'] } },
  { path: '/candidates-list', name: 'candidates-list', component: ListeCandidatsView, meta: { requiresAuth: true, role: ['AdminEntreprise', 'Recruteur'] } },
  { path: '/details-candidat/:id', name: 'details-candidat', component: DetailsCandidatView, meta: { requiresAuth: true, role: ['AdminEntreprise', 'Recruteur'] } },
  { path: '/invite', name: 'invite', component: InviteView, meta: { requiresAuth: true, role: ['AdminEntreprise', 'Recruteur'] } },
  { path: '/reporting', name: 'reporting', component: RapportsView, meta: { requiresAuth: true, role: ['AdminEntreprise', 'Recruteur'] } },
  { path: '/staff-members', name: 'staff-members', component: StaffMembersView, meta: { requiresAuth: true, role: ['AdminEntreprise', 'Recruteur'] } },

  /**
   * ESPACE SUPER ADMIN (PLATEFORME)
   */
  { path: '/super-admin', name: 'super-admin', component: SuperAdminView, meta: { requiresAuth: true, role: 'SuperAdmin' } },
  { path: '/gestion-abonnements', name: 'gestion-abonnements', component: GestionAbonnementsView, meta: { requiresAuth: true, role: 'SuperAdmin' } },
  { path: '/platform-users', name: 'platform-users', component: PlatformUsersView, meta: { requiresAuth: true, role: 'SuperAdmin' } },
  { path: '/platform-analytics', name: 'platform-analytics', component: AnalyticsView, meta: { requiresAuth: true, role: 'SuperAdmin' } },
  { path: '/google-callback', name: 'google-callback', component: GoogleCallbackView, meta: { requiresAuth: true } },

  /**
   * ERREUR 404 (Toujours en dernier)
   */
  { path: '/:pathMatch(.*)*', name: 'not-found', component: NotFoundView }
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
  scrollBehavior() { return { top: 0 } }
})

/**
 * GUARD DE NAVIGATION (SÉCURITÉ)
 */
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  const isAuthenticated = authStore.isAuthenticated
  const userRole = authStore.role

  // 1. Vérification de l'authentification
  if (to.meta.requiresAuth && !isAuthenticated) {
    return next({ name: 'login' })
  }

  // 2. Vérification des rôles (RBAC)
  if (to.meta.role) {
    const rolesAutorises = Array.isArray(to.meta.role) ? to.meta.role : [to.meta.role]
    if (!rolesAutorises.includes(userRole)) {
      return next({ name: 'dashboard' }) // Redirection si le rôle n'a pas accès
    }
  }

  next()
})

export default router