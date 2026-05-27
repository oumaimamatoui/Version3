import api from './api';

export default {
  // Envoyer les réponses pour calcul IA
  soumettreExamen(candidatId, campagneId, reponsesChoisies) {
    return api.post('/Candidatures/postuler', {
      candidatId: candidatId,
      campagneId: campagneId,
      reponses: reponsesChoisies
    });
  },

  // Récupérer les résultats IA
  getResultats(idCandidature) {
    return api.get(`/Candidatures/${idCandidature}`);
  }
};