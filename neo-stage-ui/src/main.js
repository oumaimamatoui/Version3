import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { createPinia } from 'pinia'
import Toast from "vue-toastification"
import "vue-toastification/dist/index.css"
import vue3GoogleLogin from 'vue3-google-login' 

// Importation des composants (Vérifiez bien que ces fichiers existent !)
import AppSidebar from './components/AppSidebar.vue'
import AppNavbar from './components/AppNavbar.vue'


import i18n from './i18n' // Importe le fichier créé ci-dessus
// ... tes autres imports (Pinia, etc.)

const app = createApp(App)
app.use(i18n)
// ENREGISTREMENT GLOBAL
app.component('AppSidebar', AppSidebar)
app.component('AppNavbar', AppNavbar)

app.use(createPinia())
app.use(router)
app.use(Toast)

app.use(vue3GoogleLogin, {
  clientId: '900574679253-6srju48d84teupohd1op1c43g80hdoor.apps.googleusercontent.com'
})

app.mount('#app')