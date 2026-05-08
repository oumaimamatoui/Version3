import { defineStore } from 'pinia';

export const useEvaluationStore = defineStore('evaluation', {
  state: () => ({
    // 1. Banque de données brute
    allQuestions: [
      { id: 1, text: "Expliquez le Virtual DOM dans Vue.js", theme: "Vue.js", difficulty: "Moyen", duration: 60 },
      { id: 2, text: "Différence entre let, const et var ?", theme: "JS", difficulty: "Facile", duration: 30 },
      { id: 3, text: "Comment fonctionne l'injection de dépendances en .NET ?", theme: ".NET", difficulty: "Difficile", duration: 90 },
      { id: 4, text: "Expliquez le pattern Repository", theme: "Architecture", difficulty: "Moyen", duration: 60 }
    ],
    // 2. Panier temporaire pour le constructeur
    selectedQuestions: [],
    // 3. Modèles de tests créés
    testTemplates: [
      { id: 101, name: "Test Standard Frontend", qCount: 3, duration: 15 }
    ],
    // 4. Campagnes actives
    campaigns: [
      { id: 501, title: "Recrutement Été 2024", testName: "Test Standard Frontend", candidates: 12 }
    ]
  }),
  actions: {
    addToBuilder(q) {
      if (!this.selectedQuestions.find(item => item.id === q.id)) {
        this.selectedQuestions.push(q);
      }
    },
    publishTemplate(template) {
      this.testTemplates.unshift(template);
      this.selectedQuestions = []; // On vide le panier après publication
    },
    addCampaign(camp) {
      this.campaigns.unshift(camp);
    }
  }
});