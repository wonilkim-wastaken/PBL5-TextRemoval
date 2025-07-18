import os
import argparse
import pickle
import time
import cv2
from multiprocessing import Pool
from TextRemove import TextRemover

parser = argparse.ArgumentParser()
parser.add_argument("--height", type=int, default=0, help="Minimum height of characters to remove")
parser.add_argument("--set", type=str, required=True, help="Which image set to choose")
args = parser.parse_args()

height = args.height
selected_set = args.set

# Absolute path to your Assets folder
base_dir = ""
ImageTextures = os.path.join(base_dir, 'ImageTextures')

input_dir = os.path.join(ImageTextures, selected_set)
ocr_dir = os.path.join(base_dir, 'ocr_out')
output_dir = os.path.join(base_dir, 'inpaint_out')
os.makedirs(output_dir, exist_ok=True)

def process_image(tasks):
    fname, height = tasks
    
    input_path = os.path.join(input_dir, fname)
    ocr_path = os.path.join(ocr_dir, fname + ".pkl")
    output_path = os.path.join(output_dir, os.path.splitext(fname)[0] + "_out.png")

    start = time.time()

    try:
        remover = TextRemover(input_path)
        with open(ocr_path, 'rb') as f:
            remover.ocr_result = pickle.load(f)

        remover.mask_binary_gen(height)
        remover.precise_mask_gen()
        dst = remover.text_remove()
        cv2.imwrite(output_path, dst)

        elapsed = time.time() - start
        return f"[DONE] {fname} - {output_path} ({elapsed:.2f} sec)"

    except Exception as e:
        return f"[ERROR] {fname}: {e}"

if __name__ == "__main__":
    start_all = time.time()

    file_list = [f for f in os.listdir(input_dir) if f.lower().endswith(".png")]

    tasks = [(f, height) for f in file_list]

    with Pool(processes=os.cpu_count()) as pool:
        results = pool.map(process_image, tasks)

    for r in results:
        print(r)

    print(f"[FINISHED] Total time: {time.time() - start_all:.2f} sec")