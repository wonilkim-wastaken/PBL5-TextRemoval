import os
import random
import argparse

def shuffle_filenames(target_path):
    # flyer_*.jpg 파일만 선택
    files = [f for f in os.listdir(target_path) if f.startswith("flyer") and f.endswith(".png")]
    full_paths = [os.path.join(target_path, f) for f in files]

    if len(files) < 2:
        print("⚠️ 파일이 2개 이상 있어야 무작위로 섞을 수 있습니다.")
        return

    # 파일 순서를 무작위로 섞기
    shuffled = files[:]
    random.shuffle(shuffled)

    # 임시 이름으로 우선 변경
    temp_names = []
    for i, old_path in enumerate(full_paths):
        temp_path = os.path.join(target_path, f"temp_{i}.tmp")
        os.rename(old_path, temp_path)
        temp_names.append(temp_path)

    # 섞은 이름으로 최종 변경
    for temp_path, new_name in zip(temp_names, shuffled):
        new_path = os.path.join(target_path, new_name)
        os.rename(temp_path, new_path)

    # 랜덤 숫자 출력
    print("✅ Done!")
    print(f"🎲 Random Num: {random.randint(1, 6)}")

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Flyer 파일 무작위 이름 변경기")
    parser.add_argument("-d", "--dir", type=int, required=True, help="작업할 하위 폴더 번호 (예: 1)")
    args = parser.parse_args()

    # 하위 폴더 경로 구성
    current_dir = os.getcwd()
    folder_name = str(args.dir)
    target_path = os.path.join(current_dir, folder_name)

    # 경로 유효성 확인 후 실행
    if not os.path.isdir(target_path):
        print(f"❌ 하위 폴더가 존재하지 않음: {target_path}")
    else:
        shuffle_filenames(target_path)
