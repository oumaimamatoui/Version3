import api from './api';

export default {
  async lister() {
    const res = await api.get('/Questions');
    return res.data;
  },

  // Fix Error 400 : Le QuestionnaireId est obligatoire dans ton modèle BDD
  async ajouter(q) {
    const payload = {
      texte: q.text,               // "text" dans Vue -> "Texte" en C#
      type: 0,                     // 0 = QCM
      poids: 1.0,                  // Obligatoire en double
      bonneReponse: "",            // Champ Required en C#
      questionnaireId: "00000000-0000-0000-0000-000000000000" // Guid Obligatoire
    };
    return await api.post('/Questions', payload);
  }
};