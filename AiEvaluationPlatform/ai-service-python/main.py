import os
import io
import re
import json
import random
import joblib
import numpy as np
import polars as pl
import spacy
from typing import List, Dict, Optional
from contextlib import asynccontextmanager

# FastAPI & Pydantic
from fastapi import FastAPI, HTTPException, UploadFile, File, Form
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
from pypdf import PdfReader
from docx import Document

# IA Google Gemini
from google import genai
from dotenv import load_dotenv

# Charger les variables d'environnement (Clé API)
load_dotenv()
GOOGLE_API_KEY = os.getenv("GEMINI_API_KEY", "VOTRE_CLE_PAR_DEFAUT_SI_NON_ENV")
client_gemini = genai.Client(api_key=GOOGLE_API_KEY)

# --- CHARGEMENT DU MODÈLE NLP LOCAL ---
try:
    # Utilisation du modèle léger pour les performances
    nlp = spacy.load("fr_core_news_sm")
except:
    nlp = None

# --- COFFRE-FORT DE MÉMOIRE IA (Optimisation Haute Performance) ---
AI_HUB = {}

@asynccontextmanager
async def lifespan(app: FastAPI):
    """ 
    Initialisation du Hub IA : Chargement des modèles et calcul des 
    benchmarks mondiaux au démarrage pour éviter les latences.
    """
    print("🚀 [DÉMARRAGE] Initialisation du moteur IA...")
    try:
        # 1. Chargement des modèles prédictifs (XGBoost / Scikit-Learn)
        # Assurez-vous que ces fichiers existent dans votre dossier
        AI_HUB['hiring_model'] = joblib.load('hiring_brain.pkl')
        AI_HUB['salary_model'] = joblib.load('salary_brain.pkl')
        
        # 2. Chargement de la banque de questions (+30 000 questions)
        AI_HUB['questions_db'] = pl.read_csv("questions_big_data_BILINGUAL.csv")

        # 3. Analyse comportementale Big Data (Scan sans charger tout en RAM)
        q_behavior = pl.scan_csv("data-final.csv", separator="\t", ignore_errors=True, null_values=["NULL", "nan"])
        stats = q_behavior.select([
            pl.col("testelapse").mean().alias("moy"),
            pl.col("testelapse").std().alias("std"),
            pl.len().alias("count")
        ]).collect()
        
        AI_HUB['global_avg_time'] = stats["moy"][0]
        AI_HUB['global_std_time'] = stats["std"][0]
        AI_HUB['total_profiles'] = stats["count"][0]
        
        print(f"✅ IA opérationnelle : {AI_HUB['total_profiles']} profils et {AI_HUB['questions_db'].height} questions chargés.")
    except Exception as e:
        print(f"⚠️ Alerte : Certains fichiers de données sont manquants. Mode dégradé activé. Erreur: {e}")
        # Initialisation minimale pour éviter le crash
        AI_HUB['questions_db'] = pl.DataFrame()
        AI_HUB['total_profiles'] = 0

    yield
    print("🔌 Fermeture du moteur IA.")

app = FastAPI(title="Evaluatech Assistant IA Ultime", lifespan=lifespan)

# --- CONFIGURATION CORS (Pour permettre au Frontend d'appeler l'IA) ---
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# --- MODÈLES DE DONNÉES ---

class RequeteGeneration(BaseModel):
    theme: str 
    sous_theme: Optional[str] = None 
    langue: str = "fr" 
    nombre_questions: int

class RequeteRapport(BaseModel):
    nom: str
    scores_tech: List[float] 
    experience: int 
    scores_ocean: Dict[str, float]
    temps_chrono: float
    langue: str = "fr"

class CandidatClassement(BaseModel):
    id: str
    score: float

class RequeteClassement(BaseModel):
    candidats: List[CandidatClassement]

# --- UTILITAIRES ---

def lire_contenu_fichier(file: UploadFile):
    """ Extrait le texte d'un PDF ou DOCX """
    extension = file.filename.split('.')[-1].lower()
    content = file.file.read()
    text = ""
    try:
        if extension == "pdf":
            reader = PdfReader(io.BytesIO(content))
            text = "".join([p.extract_text() for p in reader.pages[:10]]) # Limite 10 pages
        elif extension in ["docx", "doc"]:
            doc = Document(io.BytesIO(content))
            text = "\n".join([p.text for p in doc.paragraphs])
    except Exception as e:
        print(f"Erreur lecture fichier: {e}")
    return text

def extraire_concepts_nlp(texte: str):
    """ Analyse le texte pour extraire les mots-clés techniques """
    if not nlp or not texte: return []
    doc = nlp(texte.lower())
    concepts = [t.text for t in doc if t.pos_ in ["NOUN", "PROPN"] and len(t.text) > 3]
    return list(set(concepts))[:12]

# --- ENDPOINTS ---

@app.get("/")
def home():
    return {"status": "Online", "service": "AI-Evaluation-Service", "version": "2.0.0"}

@app.get("/ia/list-all-categories")
def lister_categories(langue: str = "fr"):
    col_t = "theme_fr" if langue == "fr" else "theme_en"
    col_st = "sub_theme_fr" if langue == "fr" else "sub_theme_en"
    df = AI_HUB['questions_db']
    if df.is_empty(): return {"categories": []}
    structure = df.group_by(col_t).agg(pl.col(col_st).unique().sort()).to_dicts()
    return {"langue": langue, "categories": structure}

@app.post("/ia/generate-questions-smart")
async def generer_questions(req: RequeteGeneration):
    c_t = "theme_fr" if req.langue == "fr" else "theme_en"
    c_st = "sub_theme_fr" if req.langue == "fr" else "sub_theme_en"
    c_q = "question_fr" if req.langue == "fr" else "question_en"
    c_o = "options_fr" if req.langue == "fr" else "options_en"
    
    pool = AI_HUB['questions_db'].filter(pl.col(c_t).str.to_lowercase() == req.theme.lower().strip())
    if req.sous_theme:
        pool = pool.filter(pl.col(c_st).str.to_lowercase() == req.sous_theme.lower().strip())
    
    if pool.height == 0:
        raise HTTPException(status_code=404, detail="Aucune question trouvée dans la base locale.")
    
    selection = pool.sample(min(req.nombre_questions, pool.height))
    return {"theme": req.theme, "questions": selection.select([c_q, c_o]).to_dicts()}

@app.post("/ia/generate-pro")
async def generer_questions_pro(
    file: Optional[UploadFile] = File(None),
    nombre: int = Form(10),
    themetique: str = Form("Auto-detect from content"),
    difficulte: str = Form("Medium"), 
    type_question: str = Form("MULTI_CHOICE"), 
    langue: str = Form("fr")
):
    """ Génération de questions par IA Gemini basée sur un document (CV/Fiche de poste) """
    texte_contexte = lire_contenu_fichier(file) if file else ""

    prompt = f"""
    En tant qu'expert RH et technique, génère {nombre} questions de niveau {difficulte}.
    Thème : {themetique}. Type : {type_question}. Langue : {langue}.
    Contexte supplémentaire (Document) : {texte_contexte[:3000]}
    
    Format de réponse attendu : JSON STRICT uniquement, sans texte avant ou après.
    Structure : [{{ "question": "...", "options": ["choix1", "choix2", "choix3", "choix4"], "answer": 0 }}]
    (L'index 'answer' correspond au bon choix dans la liste 'options').
    """

    try:
        response = client_gemini.models.generate_content(model="gemini-2.0-flash", contents=prompt)
        # Nettoyage et parsing du JSON
        json_match = re.search(r'\[.*\]', response.text, re.DOTALL)
        if json_match:
            questions_ia = json.loads(json_match.group(0))
            return {"status": "IA_SUCCESS", "questions": questions_ia}
        else:
            raise ValueError("Réponse IA non formatée correctement.")
    
    except Exception as e:
        print(f"Fallback CSV activé à cause de : {e}")
        # Retour à la base de données locale Polars en cas d'échec de l'API
        pool = AI_HUB['questions_db']
        selection = pool.sample(min(nombre, pool.height))
        return {"status": "FALLBACK_CSV", "questions": selection.to_dicts()}

@app.post("/ia/generate-smart-report")
async def generer_rapport(req: RequeteRapport):
    # Calcul prédictif simple (Normalisation basique des entrées)
    inputs = np.array([req.scores_tech + [req.experience]])
    
    try:
        prob = AI_HUB['hiring_model'].predict_proba(inputs)[0][1]
        salaire = AI_HUB['salary_model'].predict(inputs)[0]
    except:
        prob, salaire = 0.5, 0 # Fallback si modèles non chargés

    z_score = (req.temps_chrono - AI_HUB.get('global_avg_time', 0)) / (AI_HUB.get('global_std_time', 1))
    
    labels = {
        "fr": {"fiabilite": "ÉLEVÉE", "verdict": "RECOMMANDÉ", "al": "SUSPECT", "pos": "Top 15% mondial"},
        "en": {"fiabilite": "HIGH", "verdict": "RECOMMENDED", "al": "SUSPICIOUS", "pos": "Global Top 15%"}
    }.get(req.langue, "fr")

    fiabilite = labels["fiabilite"] if z_score > -1.5 else labels["al"]
    
    return {
        "candidat": req.nom,
        "analyse_ia": {
            "compatibilite": f"{round(prob * 100, 1)}%",
            "salaire_estime": f"${round(float(salaire), 2)}",
            "fiabilite_behaviorale": fiabilite,
            "positionnement": labels["pos"] if z_score < -1 else "Normal"
        },
        "verdict_final": labels["verdict"] if (prob > 0.7 and fiabilite == labels["fiabilite"]) else "A VÉRIFIER"
    }

@app.post("/ia/rank-candidates")
async def rank_candidates(req: RequeteClassement):
    try:
        df = pl.DataFrame([c.model_dump() for c in req.candidats]).sort("score", descending=True)
        return {"classement": df.to_dicts(), "meilleur_talent_id": df['id'][0]}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@app.get("/ia/platform-pulse")
def platform_status():
    return {
        "statut": "Optimal",
        "profils_mondiaux": AI_HUB.get('total_profiles', 0),
        "catalogue_questions": AI_HUB['questions_db'].height,
        "moteurs_actifs": ["Polars", "XGBoost", "Gemini 2.0 Flash"]
    }

@app.post("/ia/auto-detect-category")
def detect_category_pro(file: UploadFile = File(...)):
    texte = lire_contenu_fichier(file)
    keywords = extraire_concepts_nlp(texte)
    # Logique simple de détection
    tech_hits = any(k in ["code", "sql", "dev", "python", "data", "cloud"] for k in keywords)
    prediction = "Informatique / Tech" if tech_hits else "Management / RH"
    return {"categorie_detectee": prediction, "mots_cles": keywords[:5]}

if __name__ == "__main__":
    import uvicorn
    uvicorn.run(app, host="0.0.0.0", port=8000)