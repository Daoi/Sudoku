using System;
using System.Text;
using System.Windows.Forms;

namespace Sudoku
{
    internal class SudokuBoard
    {
        int[,] matrix;
        int N; //Board Size(NxN)
        int regionSize;
        int K; //Missing numbers
        Random rng = new Random();

        public SudokuBoard(int N, int K)
        {
            this.N = N;
            this.K = K;
            regionSize = (int)Math.Sqrt(N);
            matrix = new int[N, N];
            diagValues();
            placeRemaining(0, regionSize);
            removeValues();
            

        }

        private void addValues() { }

        private void diagValues()
        {
            for (int i = 0; i < N; i = i + regionSize)
                createRegions(i, i);
        }

        private void createRegions(int row, int col)
        {
            int val;
            for(int i = 0; i< regionSize; i++)
            {
                for (int j = 0; j < regionSize; j++) {
                    do
                    {
                        val = getNumber(N);
                    }
                    while (!validForRegion(row, col, val));
                        matrix[row + i, col + j] = val;
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
                    matrix[i, j] = val;
                    if (placeRemaining(i, j + 1))
                    {
                        return true;
                    }
                    matrix[i, j] = 0;
                }
            }
            return false;
        }

        private void removeValues()
        {
            int count = K;
            while(count != 0)
            {
                int row = rng.Next(0,N);
                int col = rng.Next(0, N);
                if(matrix[row,col] != 0)
                {
                    count--;
                    matrix[row, col] = 0;
                }
            }
        }



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
                    if (matrix[rowStart + i, colStart + j] == val)
                        return false;
                }
            }
            return true;
        }

        //Check row
        private Boolean validForRow(int i, int val)
        {
            for(int j = 0; j < N; j++)
            {
                if(matrix[i,j] == val)
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
                if (matrix[i, j] == val)
                {
                    return false;
                }
            }
            return true;
        }

        private int getNumber(int N)
        {
            return rng.Next(1, N+1);
        }

        public void printSudoku()
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < N; i++)
            {
                for(int j = 0; j < N; j++)
                {
                    sb.Append(matrix[i, j] + " ");
                }
                sb.Append("\r\n");
            }

            MessageBox.Show(sb.ToString(), "board");
        }

        public int getCellValue(int row, int col)
        {
            return matrix[row, col];
        }
        

    }
}