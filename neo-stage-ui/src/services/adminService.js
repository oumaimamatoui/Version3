import api from './api';

export default {
  // Liste des demandes en attente
  getDemandes() {
    return api.get('/Admin/pending');
  },

  // Approuver une entreprise
  approuverEntreprise(id) {
    return api.post(`/Admin/approve/${id}`);
  },

  // Refuser une entreprise
  refuserEntreprise(id, raison) {
    return api.post(`/Admin/reject/${id}`, { reason: raison });
  }
};