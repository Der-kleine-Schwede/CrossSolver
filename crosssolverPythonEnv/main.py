import numpy as np
import cv2 as cv
from matplotlib import pyplot as plt
import random
from cell import Cell
import pytesseract
import cv2

pytesseract.pytesseract.tesseract_cmd = "C:\ProgramData\Tesseract-OCR\\tesseract.exe"

file = open('recognized.txt', 'w+')
file.write("")
file.close()
img = cv2.imread('crossword.png')
assert img is not None, "file could not be read, check with os.path.exists()"
gray = cv2.cvtColor(img,cv.COLOR_BGR2GRAY)
# Add Gaussian blur before Canny
cv.imwrite('gray.jpg', gray)
edges = cv2.Canny(gray, 100, 200, apertureSize=3)
# Optional: Add dilation to connect nearby edges
kernel = np.ones((3,3), np.uint8)
edges = cv2.dilate(edges, kernel, iterations=1)
cv.imwrite('edges.jpg', edges)

vertical_lines = []
horizontal_lines = []

def getText(t_img):

    # Convert the image to gray scale
    gray = cv2.cvtColor(t_img, cv.COLOR_BGR2GRAY)

    # Performing OTSU threshold
    ret, thresh1 = cv2.threshold(gray, 0, 255, cv.THRESH_OTSU | cv.THRESH_BINARY_INV)

    # Specify structure shape and kernel size. 
    # Kernel size increases or decreases the area 
    # of the rectangle to be detected.
    # A smaller value like (10, 10) will detect 
    # each word instead of a sentence.
    rect_kernel = cv2.getStructuringElement(cv.MORPH_RECT, (10, 10))

    # Applying dilation on the threshold image
    dilation = cv2.dilate(thresh1, rect_kernel, iterations = 1)

    # Finding contours
    contours, hierarchy = cv2.findContours(dilation, cv.RETR_EXTERNAL, 
                                                     cv2.CHAIN_APPROX_NONE)

    # Creating a copy of image
    im2 = t_img.copy()

    # Looping through the identified contours
    # Then rectangular part is cropped and passed on
    # to pytesseract for extracting text from it
    # Extracted text is then written into the text file
    for cnt in contours:
        x, y, w, h = cv2.boundingRect(cnt)
        
        # Drawing a rectangle on copied image
        rect = cv2.rectangle(im2, (x, y), (x + w, y + h), (0, 255, 0), 2)
        
        # Cropping the text block for giving input to OCR
        cropped = im2[y:y + h, x:x + w]
        
        # Open the file in append mode
        file = open("recognized.txt", "a")
        
        # Apply OCR on the cropped image
        text = pytesseract.image_to_string(cropped, lang="deu")
        
        # Appending the text into file
        file.write(text)
        file.write("\n")
        
        # Close the file
        file.close()
def merge_lines(lines, threshold=10, is_vertical=True):
    if not lines:
        return []
    
    merged_lines = []
    current_line = lines[0]
    
    for line in lines[1:]:
        if is_vertical:
            # For vertical lines, compare x-coordinates
            if abs(line[0] - current_line[0]) < threshold:
                current_line = (
                    current_line[0],  # x1
                    min(current_line[1], line[1]),  # y1
                    current_line[2],  # x2
                    max(current_line[3], line[3])   # y2
                )
            else:
                merged_lines.append(current_line)
                current_line = line
        else:
            # For horizontal lines, compare y-coordinates
            if abs(line[1] - current_line[1]) < threshold:
                current_line = (
                    min(current_line[0], line[0]),  # x1
                    current_line[1],  # y1
                    max(current_line[2], line[2]),  # x2
                    current_line[3]   # y2
                )
            else:
                merged_lines.append(current_line)
                current_line = line
    
    merged_lines.append(current_line)
    return merged_lines

# Your existing initialization code...

# Modify HoughLinesP parameters
lines = cv.HoughLinesP(
    edges,
    rho=1,
    theta=np.pi/180,
    threshold=250,  # Increased threshold
    minLineLength=500,
    maxLineGap=20
)

vertical_lines = []
horizontal_lines = []

for line in lines:
    x1,y1,x2,y2 = line[0]
    
    if abs(x1-x2) > abs(y1-y2):
        horizontal_lines.append((x1,y1,x2,y2))
    else:
        vertical_lines.append((x1,y1,x2,y2))

# Sort and merge vertical lines
vertical_lines_sorted = sorted(vertical_lines, key=lambda line: line[0])
horizontal_lines_sorted = sorted(horizontal_lines, key=lambda line: line[1])
horizontal_lines_merged = merge_lines(horizontal_lines_sorted, threshold=15, is_vertical=False)
vertical_lines_merged = merge_lines(vertical_lines_sorted, threshold=15, is_vertical=True)
# Draw merged lines
for x1,y1,x2,y2 in vertical_lines_merged:
    cv.line(img,(x1,y1),(x2,y2),(0,255,0),2)
for x1,y1,x2,y2 in horizontal_lines_merged:
    cv.line(img,(x1,y1),(x2,y2),(0,0,255),2)
# Sorting horizontal lines by y1
def extract_cells(img, vertical_lines_merged, horizontal_lines_merged):
    cells = []
    # Add padding to avoid cutting text
    padding = 0
    
    for i in range(len(vertical_lines_merged) - 1):
        for j in range(len(horizontal_lines_merged) - 1):
            # Get coordinates for current cell
            x1 = vertical_lines_merged[i][0]
            y1 = horizontal_lines_merged[j][1]
            x2 = vertical_lines_merged[i + 1][0]
            y2 = horizontal_lines_merged[j + 1][1]
            
            # Extract cell with padding
            if (x2 - x1) > 10 and (y2 - y1) > 10:  # Minimum size check
                cell_img = img[y1+padding:y2-padding, x1+padding:x2-padding]
                cell = Cell(cell_img,x1,x2,y1,y2)
                cells.append(cell)
                cv2.imwrite(f'cell_{i}_{j}.jpg', cell_img)
                getText(cell_img)
    
    return cells

# After line detection and merging, call:
cells = extract_cells(img, vertical_lines_merged, horizontal_lines_merged)
cv.imwrite('houghlines3.jpg',img)