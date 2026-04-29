from fastapi import FastAPI, Form, File, UploadFile, HTTPException
from fastapi.middleware.cors import CORSMiddleware
import google.generativeai as genai
import json
import re
import logging
import io
import PyPDF2
from docx import Document

# --- CONFIGURATION DES LOGS ---
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

app = FastAPI(title="Evaluatech Unified AI Engine v2.0")

# --- CONFIGURATION CORS ---
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_methods=["*"],
    allow_headers=["*"],
)

# --- CONFIGURATION GEMINI ---
API_KEY = "AIzaSyDRatPj0MltmjRjfKBTtPAH32XjcJpfWvo"
genai.configure(api_key=API_KEY)

# --- FONCTIONS UTILITAIRES ---

def extract_text_from_file(file: UploadFile):
    content = ""
    try:
        ext = file.filename.split('.')[-1].lower()
        fb = file.file.read()
        if ext == "pdf":
            reader = PyPDF2.PdfReader(io.BytesIO(fb))
            for p in reader.pages: content += p.extract_text() or ""
        elif ext == "docx":
            doc = Document(io.BytesIO(fb))
            content = "\n".join([p.text for p in doc.paragraphs])
        return content[:4000]
    except Exception as e:
        logger.error(f"Extraction error: {e}")
        return ""

def get_working_model():
    try:
        available = [m.name for m in genai.list_models() if 'generateContent' in m.supported_generation_methods]
        for t in ['models/gemini-1.5-flash', 'models/gemini-pro']:
            if t in available: return t
        return available[0]
    except:
        return 'gemini-1.5-flash'

def clean_and_parse_json(raw_text):
    try:
        json_match = re.search(r'\{.*\}', raw_text, re.DOTALL)
        if json_match:
            return json.loads(json_match.group())
        return json.loads(raw_text)
    except Exception as e:
        logger.error(f"JSON Parsing failed: {e}")
        raise Exception("L'IA n'a pas renvoyé un format JSON valide.")

def handle_error(e):
    error_str = str(e)
    if "429" in error_str or "RESOURCE_EXHAUSTED" in error_str:
        logger.error("🛑 QUOTA IA ÉPUISÉ")
        raise HTTPException(status_code=429, detail="Quota Gemini épuisé. Attendez 60s.")
    logger.error(f"🔥 ERREUR SERVEUR : {error_str}")
    raise HTTPException(status_code=500, detail=error_str)

# --- ROUTES API ---

@app.get("/")
async def health_check():
    return {"status": "AI Server is running"}

# 1. Génération Vault (Actifs)
@app.post("/ia/generate-ultra")
async def generate_ultra(theme: str = Form(...), sousTheme: str = Form(...), n: int = Form(5)):
    try:
        model = genai.GenerativeModel(get_working_model())
        prompt = f"Génère {n} questions QCM techniques sur {theme}/{sousTheme}. Retourne JSON pur: {{'questions': [{{'question':'','options':[],'answer':0}}]}}"
        response = model.generate_content(prompt)
        return clean_and_parse_json(response.text)
    except Exception as e: handle_error(e)

# 2. Génération Recrutement (Pro / CV)
@app.post("/ia/generate-pro")
async def generate_pro(nombre: int = Form(...), themetique: str = Form(...), difficulte: str = Form(...), file: UploadFile = File(None)):
    try:
        context = extract_text_from_file(file) if file else ""
        model = genai.GenerativeModel(get_working_model())
        prompt = f"Expert RH. Génère {nombre} questions QCM ({difficulte}) sur {themetique}. Contexte: {context}. Retourne JSON: {{'status': 'IA_SUCCESS', 'questions': []}}"
        response = model.generate_content(prompt)
        return clean_and_parse_json(response.text)
    except Exception as e: handle_error(e)

# 3. Analyse Cognitive / Comportementale (Neural Scan)
@app.post("/ia/analyze-candidate")
async def analyze_candidate(nom: str = Form(...), scores_techniques: str = Form(...)):
    try:
        model = genai.GenerativeModel(get_working_model())
        prompt = f"Analyse cognitive de {nom} (Scores: {scores_techniques}). Génère JSON: global_score, neural_tier, profile_type, radar_data (Rigueur, Adaptabilité, Communication, Critique, Leadership), traits (name, val, icon, color), terminal_insights (time, type, text)."
        response = model.generate_content(prompt)
        return clean_and_parse_json(response.text)
    except Exception as e:
        return {"id": "SIM-2026", "global_score": 75, "neural_tier": "AVANCÉ", "profile_type": "DÉVELOPPEUR", "radar_data": [{"label":"Rigueur","val":80},{"label":"Adapt.","val":70},{"label":"Comm.","val":60},{"label":"Critique","val":75},{"label":"Lead.","val":50}], "traits": [], "terminal_insights": [{"time":"00:00","type":"amber","text":"MODE DÉMO"}]}

# 4. Rapport de Performance (ResultsView)
@app.post("/ia/performance-report")
async def generate_performance_report(global_score: int = Form(...), themes_json: str = Form(...)):
    try:
        model = genai.GenerativeModel(get_working_model())
        prompt = f"Analyse résultats techniques. Score: {global_score}/100. Thèmes: {themes_json}. Génère JSON: synthese (string), forces (array), faiblesses (array), roadmap ({{'objectif':'','certification':''}})"
        response = model.generate_content(prompt)
        return clean_and_parse_json(response.text)
    except Exception as e:
        return {"synthese": "Analyse technique standard.", "forces": ["Rigueur"], "faiblesses": ["Optimisation"], "roadmap": {"objectif": "Senior", "certification": "Cloud Architect"}}

# 5. Analyse de l'Historique (HistoryView)
@app.post("/ia/analyze-history")
async def analyze_history(history_json: str = Form(...)):
    try:
        model = genai.GenerativeModel(get_working_model())
        prompt = f"Analyse progression utilisateur. Historique JSON: {history_json}. Génère JSON: progression_status (CROISSANTE/STABLE/ALERTE), global_summary (string), top_skill (string), advice (string), cyber_log (string)."
        response = model.generate_content(prompt)
        return clean_and_parse_json(response.text)
    except Exception as e:
        return {"progression_status": "STABLE", "global_summary": "Données insuffisantes.", "top_skill": "N/A", "advice": "Continuez vos tests.", "cyber_log": "LOG_IA: Standby."}

# --- LANCEMENT ---
if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="127.0.0.1", port=8000)