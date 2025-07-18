import os
import random
import argparse

def shuffle_filenames(target_path):
    files = [f for f in os.listdir(target_path) if f.startswith("flyer") and f.endswith(".png")]
    full_paths = [os.path.join(target_path, f) for f in files]

    if len(files) < 2:
        print("You need at least to fiels to randomize")
        return

    shuffled = files[:]
    random.shuffle(shuffled)

    temp_names = []
    for i, old_path in enumerate(full_paths):
        temp_path = os.path.join(target_path, f"temp_{i}.tmp")
        os.rename(old_path, temp_path)
        temp_names.append(temp_path)


    for temp_path, new_name in zip(temp_names, shuffled):
        new_path = os.path.join(target_path, new_name)
        os.rename(temp_path, new_path)

    print("✅ Done!")
    print(f"🎲 Random Num: {random.randint(1, 6)}")

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Flyer randomizer")
    parser.add_argument("-d", "--dir", type=int, required=True, help="set name (1, 2, 3)")
    args = parser.parse_args()

    current_dir = os.getcwd()
    folder_name = str(args.dir)
    target_path = os.path.join(current_dir, folder_name)

    if not os.path.isdir(target_path):
        print(f"❌ Directory does not exist: {target_path}")
    else:
        shuffle_filenames(target_path)
