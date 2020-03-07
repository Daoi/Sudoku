using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Sudoku
{
    public class SudokuBoard
    {

        int waitBadValue = 50;
        int waitValidValue = 100;
        int waitReset = 0;
        int[,] modifiedBoard;
        int[,] displayBoard;
        int[,] originalBoard;
        Button[,] cells;
        int N; //Board Size(NxN)
        int regionSize;
        int K; //Missing numbers
        Random rng = new Random();

        public SudokuBoard(int N, int K)
        {
            this.N = N;
            this.K = K;
            regionSize = (int)Math.Sqrt(N);
            displayBoard = new int[N, N];
            diagValues();
            placeRemaining(0, regionSize);
            modifiedBoard = displayBoard;
            removeValues();
            originalBoard = (int[,])displayBoard.Clone();
        }

        //Start creating board values
        private void addValues() { }

        private void diagValues()
        {
            for (int i = 0; i < N; i = i + regionSize)
                createRegions(i, i);
        }

        private void createRegions(int row, int col)
        {
            int val;
            for (int i = 0; i < regionSize; i++)
            {
                for (int j = 0; j < regionSize; j++)
                {
                    do
                    {
                        val = getNumber(N);
                    }
                    while (!validForRegion(row, col, val));
                    displayBoard[row + i, col + j] = val;
                }
            }
        }
        
        private Boolean placeRemaining(int i, int j)
        {
            if (j >= N && i < N - 1)
            {
                i++;
                j = 0;
            }
            if (i >= N && j >= N)
            {
                return true;
            }
            if (i < regionSize)
            {
                if (j < regionSize)
                {
                    j = regionSize;
                }
            }
            else if (i < N - regionSize)
            {
                if (j == (int)(i / regionSize) * regionSize)
                {
                    j = j + regionSize;
                }
            }
            else
            {
                if (j == N - regionSize)
                {
                    i++;
                    j = 0;
                    if (i >= N)
                    {
                        return true;
                    }
                }
            }
            for (int val = 1; val <= N; val++)
            {
                if (IsSafe(i, j, val))
                {
                    displayBoard[i, j] = val;
                    if (placeRemaining(i, j + 1))
                    {
                        return true;
                    }
                    displayBoard[i, j] = 0;
                }
            }
            return false;
        }
        //Remove values based on selected difficulty
        private void removeValues()
        {
            int count = K;
            while (count != 0)
            {
                int row = rng.Next(0, N);
                int col = rng.Next(0, N);
                if (displayBoard[row, col] != 0)
                {
                    count--;
                    displayBoard[row, col] = 0;
                }
            }
        }
        
        //Check if a value is safe to place
        private Boolean IsSafe(int i, int j, int val)
        {
            return (validForRow(i, val)
                    && validForCol(j, val)
                    && validForRegion(i - i % regionSize, j - j % regionSize, val));
        }

        //Check "Squares"(Regions the size of Sqrt of board dimensions)
        private Boolean validForRegion(int rowStart, int colStart, int val)
        {
            for (int i = 0; i < regionSize; i++)
            {
                for (int j = 0; j < regionSize; j++)
                {
                    if (displayBoard[rowStart + i, colStart + j] == val)
                        return false;
                }
            }
            return true;
        }
        
        //Check row
        private Boolean validForRow(int i, int val)
        {
            for (int j = 0; j < N; j++)
            {
                if (displayBoard[i, j] == val)
                {
                    return false;
                }
            }
            return true;
        }

        //Check col
        private Boolean validForCol(int j, int val)
        {
            for (int i = 0; i < N; i++)
            {
                if (displayBoard[i, j] == val)
                {
                    return false;
                }
            }
            return true;
        }
        
        
        private int getNumber(int N)
        {
            return rng.Next(1, N + 1);
        }
        //End creating board values


        //Utility
        public void printSudoku()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    sb.Append(displayBoard[i, j] + " ");
                }
                sb.Append("\r\n");
            }

            MessageBox.Show(sb.ToString(), "board");
        }
        
        public int getCellValue(int row, int col)
        {
            return displayBoard[row, col];
        }

        public void getCells(ref Button[,] cells)
        {
            this.cells = cells;
        }

        public int[,] getOriginal()
        {
            return originalBoard;
        }

        //Sudoku Puzzle Solving Start
        public int[,] solve()
        {
            solvePuzzle(0, 0);
            return modifiedBoard;
        }
        private Boolean solvePuzzle(int row, int col)
        {
            if (col == 9)
            {
                col = 0;
                row++;
                if (row == 9)
                {
                    return true;
                }
            }

            if (modifiedBoard[row, col] != 0)
            {
                return solvePuzzle(row, col + 1);
            }

            for(int i = 1; i < 10; i++)
            {
                if(canPlace(row, col, i))
                {
                    modifiedBoard[row, col] = i;
                    cells[row, col].BackColor = Color.Cyan;
                    cells[row, col].Text = modifiedBoard[row, col].ToString();
                    wait(waitValidValue);
                    cells[row, col].BackColor = SystemColors.Control;
                    if (solvePuzzle(row, col + 1))
                    {
                        return true;
                    }
                }
                modifiedBoard[row, col] = 0;
                cells[row, col].Text = modifiedBoard[row,col].ToString();
                cells[row, col].BackColor = Color.LightGreen;
                wait(waitReset);
                cells[row,col].BackColor = SystemColors.Control;
            }

            return false;

        }

        private Boolean canPlace(int row, int col, int digit)
        {
            //Row
            for(int j = 0; j < N; j++)
            {
                if(digit == modifiedBoard[row, j])
                {
                    cells[row, j].BackColor = Color.Red;
                    wait(waitBadValue);
                    cells[row, j].BackColor = SystemColors.Control;
                    return false;
                }
            }
            //Col
            for(int i = 0; i < N; i++)
            {
                if(digit == modifiedBoard[i, col])
                {
                    cells[i, col].BackColor = Color.Red;
                    wait(waitBadValue);
                    cells[i, col].BackColor = SystemColors.Control;
                    return false;
                }
            }
            //Subregions
            int rowIndex = row / regionSize;
            int colIndex = col / regionSize;
            int topLeftRow = regionSize * rowIndex;
            int topleftCol = regionSize * colIndex;

            for(int i = 0; i < regionSize; i++)
            {
                for(int j = 0; j < regionSize; j++)
                {
                    if(digit == modifiedBoard[topLeftRow + i,topleftCol + j])
                    {
                        cells[topLeftRow + i, topleftCol + j].BackColor = Color.Red;
                        wait(waitBadValue);
                        cells[topLeftRow + i, topleftCol + j].BackColor = SystemColors.Control;
                        return false;
                    }
                }
            }
            return true;
        }

        public void setTimers()
        {
            waitBadValue = 0;
            waitValidValue = 0;
            waitReset = 0;
        }

        public void wait(int milliseconds)
        {
            Timer timer1 = new Timer();
            if (milliseconds == 0 || milliseconds < 0) return;
            timer1.Interval = milliseconds;
            timer1.Enabled = true;
            timer1.Start();
            timer1.Tick += (s, e) =>
            {
                timer1.Enabled = false;
                timer1.Stop();
            };
            while (timer1.Enabled)
            {
                Application.DoEvents();
            }
        }
        //Puzzle Solving End

    }
}