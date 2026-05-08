import api from './api';

export default {
  // Liste des campagnes (Fix Error 405 : vérifie bien que c'est du GET)
  async lister() {
    const res = await api.get('/Campagnes');
    return res.data;
  },

  // Création (Fix Error 400 : Mappage des noms français)
  async creer(formData) {
    const payload = {
      nom: formData.title,         // "title" dans Vue -> "Nom" en C#
      position: formData.position,
      statut: 0,                   // 0 = Brouillon (Enum)
      entrepriseId: localStorage.getItem('entrepriseId') || "00000000-0000-0000-0000-000000000000"
    };
    return await api.post('/Campagnes', payload);
  },

  async supprimer(id) {
    return await api.delete(`/Campagnes/${id}`);
  }
};