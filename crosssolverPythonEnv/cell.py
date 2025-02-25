class Cell:
  def __init__(self, img, x1, x2, y1, y2, cellType="default"):
    self.img = img
    self.x1 = x1
    self.y1 = y1
    self.x2 = x2
    self.y2 = y2
    self.type = cellType
    self.cells = []