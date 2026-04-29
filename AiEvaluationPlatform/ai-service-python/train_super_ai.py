import polars as pl
from datasets import load_dataset
import os

def build_mega_dataset():
    print("🚀 [BIG DATA] Downloading Technical Bank...")
    subjects = ["college_computer_science", "machine_learning", "computer_security"]
    all_dfs = []

    for s in subjects:
        try:
            ds = load_dataset("cais/mmlu", s, split="test")
            df = pl.from_pandas(ds.to_pandas()).with_columns([
                pl.lit(s).alias("sub_theme"),
                pl.col("question").alias("question_en"),
                pl.col("choices").map_elements(lambda x: "|".join(map(str, x)), return_dtype=pl.String).alias("options")
            ])
            all_dfs.append(df.select(["sub_theme", "question_en", "options", "answer"]))
        except: continue

    if all_dfs:
        pl.concat(all_dfs).write_parquet("questions_bank.parquet")
        print(f"✅ Success: questions_bank.parquet created!")

if __name__ == "__main__":
    build_mega_dataset()