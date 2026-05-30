import axios from 'axios';

const AI_BASE = import.meta.env.VITE_AI_URL || import.meta.env.VITE_API_BASE || 'http://127.0.0.1:8000';
const AI_INTERNAL_KEY = import.meta.env.VITE_AI_INTERNAL_API_KEY || 'fallback_secret_for_demo';

const aiApi = axios.create({
  baseURL: AI_BASE,
  headers: {
    'X-API-KEY': AI_INTERNAL_KEY
  }
});

export const aiService = {
  async getSuggestions(role, lang) {
    const response = await aiApi.get(`/ia/chat/suggestions`, {
      params: { role, lang }
    });
    return response.data;
  },

  async sendMessage(message, role, sessionId, userName = '', enterpriseName = '', enterprisePlan = '') {
    const fd = new FormData();
    fd.append('message', message);
    fd.append('role', role);
    fd.append('lang', 'auto');
    fd.append('session_id', sessionId);
    fd.append('user_name', userName);
    fd.append('enterprise_name', enterpriseName);
    fd.append('enterprise_plan', enterprisePlan);
    
    const response = await aiApi.post(`/ia/chat`, fd, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    });
    return response.data;
  },

  async resetSession(sessionId) {
    const fd = new FormData();
    fd.append('session_id', sessionId);
    
    const response = await aiApi.post(`/ia/chat/reset`, fd, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    });
    return response.data;
  },

  async transcribeAudio(audioBlob, langue = 'fr') {
    const fd = new FormData();
    fd.append('file', audioBlob, 'recording.webm');
    fd.append('langue', langue);
    const response = await aiApi.post('/ia/transcribe-audio', fd, {
      headers: { 'Content-Type': 'multipart/form-data' },
      timeout: 30000
    });
    return response.data;
  },

  async analyzeInterview(question, response, type, langue = 'fr') {
    const res = await aiApi.post('/ia/interview/analyze', {
      question,
      response,
      type,
      langue
    }, { timeout: 30000 });
    return res.data;
  }
};

export default aiService;
