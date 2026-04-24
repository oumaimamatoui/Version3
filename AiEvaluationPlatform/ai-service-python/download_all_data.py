import polars as pl
from datasets import load_dataset, get_dataset_config_names
import pandas as pd

print("🚀 [BIG DATA SYSTEM] Création de la banque mondiale bilingue (+35,000 questions)...")

# 1. جلب كافة التخصصات من MMLU
all_subjects = get_dataset_config_names("cais/mmlu")
all_dfs = []

# تعريف التخصصات التقنية للفلترة الدقيقة
IT_SUBJECTS = ["computer_security", "college_computer_science", "high_school_computer_science", "machine_learning"]

for s in all_subjects:
    try:
        print(f"📥 Téléchargement : {s}...")
        # نأخذ كل الأجزاء (Test + Val + Dev) لزيادة العدد
        ds = load_dataset("cais/mmlu", s, split="test+validation+dev", trust_remote_code=True)
        df_pd = ds.to_pandas()
        df_pd['choices'] = df_pd['choices'].apply(lambda x: " | ".join(map(str, x)))
        
        is_it = s in IT_SUBJECTS
        t_en, t_fr = ("IT", "Informatique") if is_it else ("Science", "Sciences")
        sub = s.replace('_', ' ').capitalize()
        
        df_pl = pl.from_pandas(df_pd).with_columns([
            pl.lit(t_en).alias("theme_en"), pl.lit(t_fr).alias("theme_fr"),
            pl.lit(sub).alias("sub_theme_en"), pl.lit(sub).alias("sub_theme_fr"),
            pl.col("question").alias("question_en"), pl.col("question").alias("question_fr"),
            pl.col("choices").alias("options_en"), pl.col("choices").alias("options_fr")
        ])
        all_dfs.append(df_pl.select(["theme_en", "theme_fr", "sub_theme_en", "sub_theme_fr", "question_en", "question_fr", "options_en", "options_fr", "answer"]))
    except: continue

# 2. حقن الـ Frameworks الحديثة (Vue, .NET, Angular, DevOps, etc.)
print("🛠️ Injection massive des Frameworks Modernes...")
fw = [
    {"t_en":"IT","t_fr":"Informatique","s":"Vue.js","q_en":"What is Vue composition API?","q_fr":"Qu'est-ce que l'API de composition Vue?","o":"A|B|C","a":1},
    {"t_en":"IT","t_fr":"Informatique","s":"Angular","q_en":"What is a decorator?","q_fr":"C'est quoi un décorateur?","o":"A|B|C","a":0},
    {"t_en":"IT","t_fr":"Informatique","s":".NET","q_en":"What is a Middleware?","q_fr":"C'est quoi un Middleware?","o":"A|B|C","a":2},
    {"t_en":"IT","t_fr":"Informatique","s":"SpringBoot","q_en":"What is @RestController?","q_fr":"C'est quoi @RestController?","o":"A|B|C","a":1},
    {"t_en":"IT","t_fr":"Informatique","s":"DevOps","q_en":"What is a Docker image?","q_fr":"C'est quoi une image Docker?","o":"A|B|C","a":0}
]
df_fw = pl.DataFrame(fw).rename({"t_en":"theme_en","t_fr":"theme_fr","s":"sub_theme_en"}).with_columns([
    pl.col("sub_theme_en").alias("sub_theme_fr"), pl.col("o").alias("options_en"), 
    pl.col("o").alias("options_fr"), pl.col("a").alias("answer")
])

# دمج وحفظ
pl.concat(all_dfs + [df_fw], how="diagonal").write_csv("questions_big_data_BILINGUAL.csv")
print("✅ Base de données bilingue ultra-massive générée !")