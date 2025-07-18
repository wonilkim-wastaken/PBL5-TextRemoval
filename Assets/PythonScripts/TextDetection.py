import cv2
import numpy as np
import easyocr

class TextDetector: 
    def __init__(self, image_path, reader):
        self.image_path = image_path
        self.reader = reader
        self.ocr_result = None

    def extract_text_roi(self):
        results = self.reader.readtext(self.image_path)

        data = []

        for bbox, text, confidence in results:
            x_coords = [point[0] for point in bbox]
            y_coords = [point[1] for point in bbox]

            left = int(min(x_coords))
            top = int(min(y_coords))
            width = int(max(x_coords)) - left
            height = int(max(y_coords)) - top

            data.append({
                'text': text,
                'left': left,
                'top': top,
                'width': width,
                'height': height
            })
        
        self.ocr_result = data