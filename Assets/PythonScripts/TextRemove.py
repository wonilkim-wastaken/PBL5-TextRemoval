import cv2
import numpy as np
import os
import pickle

class TextRemover:
    def __init__(self, image_path):
        self.image_path = image_path
        self.image = cv2.imread(image_path)
        self.ocr_result = None
        self.square_mask = None
        self.binary = None
        self.precise_mask = None

    def load_ocr(self):
        grand_path = os.path.dirname(os.path.dirname(os.path.abspath(self.image_path)))
        ocr_path = os.path.join(
            grand_path,
            "ocr_out",
            os.path.basename(self.image_path) + ".pkl"
        )
                                                                                               
        with open(ocr_path, "rb") as f:
            self.ocr_result = pickle.load(f)

    def mask_binary_gen(self, height):
        gray = cv2.cvtColor(self.image, cv2.COLOR_BGR2GRAY)
        blur = cv2.GaussianBlur(gray, (5, 5), 0)
        _, self.binary = cv2.threshold(blur, 0, 255, cv2.THRESH_BINARY + cv2.THRESH_OTSU)
        self.square_mask = np.zeros_like(gray)

        filtered_ocr = [row for row in self.ocr_result if int(row['height']) >= height]

        for row in filtered_ocr:
            x, y, w, h = int(row['left']), int(row['top']), int(row['width']), int(row['height'])
            cv2.rectangle(self.square_mask, (x, y), (x + w, y + h), 255, thickness=-1)
    
    def precise_mask_gen(self):
        char_mask = cv2.bitwise_and(self.binary, self.square_mask)
        final_mask = np.zeros_like(self.square_mask)

        for row in self.ocr_result:
            x, y = int(row['left']), int(row['top'])
            w, h = int(row['width']), int(row['height'])

            roi = char_mask[y:y+h, x:x+w]

            corners = [
                roi[0, 0],
                roi[0, -1],
                roi[-1, 0],
                roi[-1, -1]
            ]
            
            if any(c != 0 for c in corners):
                roi_processed = 255 - roi
            else:
                roi_processed = roi
            
            final_mask[y:y+h, x:x+w] = roi_processed
        
        kernel = cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (5,5))
        self.precise_mask = cv2.dilate(final_mask, kernel, iterations=3)

    def text_remove(self):
        dst = cv2.inpaint(self.image, self.precise_mask, 3, cv2.INPAINT_TELEA)
        return dst