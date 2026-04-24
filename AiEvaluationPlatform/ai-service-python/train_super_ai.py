import polars as pl
import xgboost as xgb
import joblib
from sklearn.preprocessing import LabelEncoder

print("🧠 [ENTRAÎNEMENT IA] Fusion des 3 Datasets en cours...")

try:
    # --- 1. SOURCE : Placement_Data (Données de Recrutement) ---
    # On apprend à l'IA qui est "Recrutable" et quel est le "Juste Salaire"
    df_p = pl.read_csv('Placement_Data_Full_Class.csv').to_pandas()
    le = LabelEncoder()
    df_p['workex'] = le.fit_transform(df_p['workex'])
    df_p['status'] = le.fit_transform(df_p['status']) # Placed = 1

    features = ['ssc_p', 'hsc_p', 'degree_p', 'mba_p', 'workex']
    
    # Modèle 1 : Prédiction de recrutement (Classification)
    hiring_model = xgb.XGBClassifier(n_estimators=300).fit(df_p[features], df_p['status'])
    joblib.dump(hiring_model, 'hiring_brain.pkl')
    
    # Modèle 2 : Estimation du salaire (Regression)
    df_sal = df_p[df_p['status'] == 1].copy()
    salary_model = xgb.XGBRegressor().fit(df_sal[features], df_sal['salary'].fillna(df_p['salary'].median()))
    joblib.dump(salary_model, 'salary_brain.pkl')
    joblib.dump(features, 'feature_names.pkl')
    print("✅ Modèles de Recrutement et Salaire générés.")

    # --- 2. SOURCE : StudentsPerformance (Données Académiques) ---
    # On analyse les thèmes où les gens échouent pour suggérer des questions intelligentes
    df_s = pl.read_csv('StudentsPerformance.csv')
    stats_acad = {
        "mathématiques": df_s.select(pl.col("math score").mean()).item(),
        "lecture": df_s.select(pl.col("reading score").mean()).item(),
        "écriture": df_s.select(pl.col("writing score").mean()).item()
    }
    joblib.dump(stats_acad, 'subject_difficulty.pkl')
    print("✅ Analyse de difficulté académique terminée.")

    print("\n🚀 [SUCCÈS] Tous les modèles locaux sont prêts pour l'API !")

except Exception as e:
    print(f"❌ Erreur lors de l'entraînement : {e}")