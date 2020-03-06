def sudoku(puzzle):
    solve(0, 0, puzzle)
    return puzzle

def solve(row, col, puzzle):
    if col == len(puzzle[row]): #Go row by row til last element
        col = 0
        row += 1
        if row == 9:
            return True

    if puzzle[row][col] != 0:  #Check for given, skip if given.
        return solve(row, col + 1, puzzle)

    for x in range(1, 10):   #Check if a digit is valid to place in row/col/subbox
        if canPlace(puzzle, row, col, x):
            puzzle[row][col] = x
            if solve(row, col + 1, puzzle):
                return True
        puzzle[row][col] = 0        #Reset invalid squares



def canPlace(puzzle, row, col, digit):
    for value in puzzle[row]:
        if digit == value:
            return False

    for x in range(0, 9):
        test = puzzle[x][col]
        if digit == test:
            return False

    regionsize = 3  #Sqrt of puzzle length for non fixed length?
    rowindex = int(row/regionsize)
    colindex = int(col/regionsize)
    topleftrow = int(regionsize*rowindex)
    topleftcol = int(regionsize*colindex)

    for i in range(0, regionsize):
        for j in range(0, regionsize):
            if digit == puzzle[topleftrow + i][topleftcol + j]:
                return False

    return True
