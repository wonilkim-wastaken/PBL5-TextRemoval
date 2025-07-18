import os
import pickle
import numpy as np
from scipy.stats import gaussian_kde

'''
def calculate_tail_start(ocr_dir: str, threshold_ratio: float = 0.01) -> float:
    all_heights = []

    for fname in os.listdir(ocr_dir):
        if fname.endswith(".pkl"):
            with open(os.path.join(ocr_dir, fname), "rb") as f:
                ocr_result = pickle.load(f)
                heights = [int(row['height']) for row in ocr_result]
                all_heights.extend(heights)

    if not all_heights:
        raise ValueError("No OCR data exists.")

    kde = gaussian_kde(all_heights)
    x_vals = np.linspace(min(all_heights), max(all_heights), 1000)
    density = kde(x_vals)

    max_density = np.max(density)
    threshold_density = threshold_ratio * max_density
    tail_start_idx = np.argmax(density < threshold_density)
    tail_start_x = x_vals[tail_start_idx]

    print("Tail starts at", tail_start_x)

    return tail_start_x
'''

# peak_height_all.py

def global_peak_height(ocr_dir: str) -> float:
    all_heights = []

    for fname in os.listdir(ocr_dir):
        if fname.endswith(".pkl"):
            file_path = os.path.join(ocr_dir, fname)
            with open(file_path, "rb") as f:
                ocr_result = pickle.load(f)
                heights = [int(row['height']) for row in ocr_result if 'height' in row]
                all_heights.extend(heights)

    if not all_heights:
        raise ValueError("No OCR data.")

    kde = gaussian_kde(all_heights)
    x_vals = np.linspace(min(all_heights), max(all_heights), 1000)
    density = kde(x_vals)

    peak_idx = np.argmax(density)
    peak_height = x_vals[peak_idx]

    print(f"[Global] Peak height across all files: {peak_height:.2f}")
    return peak_height
