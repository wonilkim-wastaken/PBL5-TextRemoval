from TextDetection import TextDetector
from peak_height_util import global_peak_height
import os
import pickle
import easyocr
import time
import argparse

parser = argparse.ArgumentParser()
parser.add_argument("--set", type=str, required=True)
args = parser.parse_args()

if args.set == "1":
    input_dir = 'C:/Users/is0646ep/Desktop/vr_env/Assets/ImageTextures/1'

if args.set == "2":
    input_dir = 'C:/Users/is0646ep/Desktop/vr_env/Assets/ImageTextures/2'

if args.set == "3":
    input_dir = 'C:/Users/is0646ep/Desktop/vr_env/Assets/ImageTextures/3'

output_dir = 'C:/Users/is0646ep/Desktop/vr_env/Assets/ocr_out'
os.makedirs(output_dir, exist_ok=True)

print("[START] Initializing OCR reader...", flush=True)
start_time = time.time()
reader = easyocr.Reader(['en', 'ja'], gpu=True)
print(f"[READY] OCR reader initialized in {time.time() - start_time:.2f} seconds.\n", flush=True)

print("[INFO] Starting OCR on all images...", flush=True)
total_start = time.time()

for fname in os.listdir(input_dir):
    if not fname.lower().endswith(".png"):
        continue

    input_path = os.path.join(input_dir, fname)
    output_path = os.path.join(output_dir, fname + ".pkl")

    print(f"[PROCESSING] {fname}", flush=True)
    file_start = time.time()

    try:
        detector = TextDetector(input_path, reader)
        detector.extract_text_roi()

        with open(output_path, 'wb') as f:
            pickle.dump(detector.ocr_result, f)

        elapsed = time.time() - file_start
        print(f"[SAVED] OCR result - {output_path} ({elapsed: .2f} sec)\n", flush=True)

    except Exception as e:
        print(f"[ERROR] Failed to process {fname}: {e}\n", flush=True)

total_elapsed = time.time() - total_start
print(f"[FINISHED] All images processed. Total time: {total_elapsed:.2f} sec", flush=True)

peak = global_peak_height(output_dir)
int_peak = int(peak)
with open("C:/Users/is0646ep/Desktop/vr_env/Assets/ocr_out/peak.txt", "w") as f:
    f.write(f"{int_peak}")

