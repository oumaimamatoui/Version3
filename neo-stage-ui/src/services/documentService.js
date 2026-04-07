import api from './api';

export default {
  // Upload CV du candidat
  uploadCV(candidatId, file) {
    const formData = new FormData();
    formData.append('fichier', file);
    formData.append('type', 'CV');
    
    return api.post(`/Documents/candidat/${candidatId}`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    });
  },

  // Télécharger un fichier
  getLienFichier(nomFichier) {
    return `http://localhost:5172/api/Documents/telecharger/${nomFichier}`;
  }
};