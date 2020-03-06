using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Sudoku
{
    public partial class frmMain : Form
    {
        
        SudokuBoard a = new SudokuBoard(9, 43);
        int cardCellWidth;
        int cardCellHeight;
        int barWidth = 3;  // Width or thickness of horizontal and vertical bars
        int xcardUpperLeft = 0;
        int ycardUpperLeft = 0;
        int padding = 2;
        int cardSize;
        int[,] originalBoard;
        private Button[,] boardCell;
        public frmMain()
        {
            InitializeComponent();
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            btnPlay.Enabled = false;
            btnPlay.Visible = false;
            cardSize = 9;
            boardCell = new Button[cardSize, cardSize];
            originalBoard = new int[cardSize, cardSize];
            a.printSudoku();
            generateBoard();
        }

        private void Button_MouseDown(object sender, MouseEventArgs me)
        {
            Button button = ((Button)sender);
            int rowID = (int)Char.GetNumericValue(button.Name[3]);
            int colID = (int)Char.GetNumericValue(button.Name[4]);
            int cellID = rowID * 3 + colID;
            int val = int.Parse(button.Text);
            if (me.Button == MouseButtons.Left)
            {
                if (val < 9)
                {
                    button.Text = ((val + 1).ToString());
                }
                else
                {
                    button.Text = 1.ToString();
                }

            }
            if (me.Button == MouseButtons.Right)
            {
                if (val > 1)
                {
                    button.Text = ((val - 1).ToString());
                }
                else
                {
                    button.Text = 9.ToString();
                }

            }

        } // end button clickhandler 

        //Board Creation Start
        private void generateBoard()
        {
            cardCellWidth = (pnlBoard.Size.Width / cardSize) - (padding);
            cardCellHeight = (pnlBoard.Size.Height / cardSize) - (padding);
            Size size = new Size(cardCellWidth, cardCellHeight);
            Point loc = new Point(0, 0);
            int x;
            int y;

            x = xcardUpperLeft;
            y = ycardUpperLeft;

            //Top Line
            drawHorizBar(x, y, cardSize);
            y = y + barWidth;

            //Button Start
            drawVertBar(x, y);
            for (int row = 0; row < cardSize; row++)
            {
                loc.Y = row * (size.Height + padding);

                for (int col = 0; col < cardSize; col++)
                {
                    boardCell[row, col] = new Button
                    {
                        Location = new Point(col * (size.Width + padding) + barWidth, loc.Y),
                        Size = size,
                        Font = new Font("Arial", 24, FontStyle.Bold),
                        Enabled = true
                    };

                    boardCell[row, col].Font = new Font("Arial", 24, FontStyle.Bold);
                    int value = a.getCellValue(row, col);
                    boardCell[row, col].Text = value.ToString();
                    originalBoard[row, col] = value;
                    boardCell[row, col].Name = "btn" + row.ToString() + col.ToString();
                    if (boardCell[row, col].Text != "0")
                    {
                        boardCell[row, col].Enabled = false;
                    }

                    //Associates the same event handler with each of the buttons generated
                    boardCell[row, col].MouseDown += new MouseEventHandler(Button_MouseDown);

                    // Add button to the form
                    pnlBoard.Controls.Add(boardCell[row, col]);

                    // Draw vertical line                 
                    x += cardCellWidth + padding;
                    if (row == 0) drawVertBar(x, y);
                } // end for col
                // One row now complete

                // Draw bottom line
                x = xcardUpperLeft;
                y = y + cardCellHeight + padding;
                drawHorizBar(x, y - 6, cardSize);

            }
        }

        private void drawHorizBar(int x, int y, int cardSize)
        {
            Color backColor = Color.Black;
            if (y == 204 || y == 411)
            {
                backColor = Color.Red;
            }

            Label lblHorizBar = new Label
            {
                BackColor = backColor,
                Location = new System.Drawing.Point(x, y),
                Name = "lblHorizBar" + y.ToString(),
                Size = new System.Drawing.Size((cardCellWidth + padding) * cardSize, barWidth),
                TabIndex = 1000
            };



            pnlBoard.Controls.Add(lblHorizBar);
            lblHorizBar.Visible = true;
            lblHorizBar.CreateControl();
            lblHorizBar.Show();
            x = x + cardCellWidth;
        }

        private void drawVertBar(int x, int y)
        {
            Color backColor = Color.Black;
            if (x == 300 || x == 600)
            {
                backColor = Color.Red;
            }

            Label lblVertBar = new Label
            {
                BackColor = backColor,
                Location = new System.Drawing.Point(x, y),
                Name = "lblVertBar" + x.ToString(),
                Size = new System.Drawing.Size(barWidth, (cardCellHeight + padding) * cardSize),
                TabIndex = 1000
            };
            pnlBoard.Controls.Add(lblVertBar);
            lblVertBar.Visible = true;
            lblVertBar.CreateControl();
            lblVertBar.Show();
        }//Board Creation End

        private void btnSolve_Click(object sender, EventArgs e)
        {

            //Format for python script
            StringBuilder sb = new StringBuilder("[[");

            for (int i = 0; i < cardSize; i++)
            {
                for (int j = 0; j < cardSize; j++)
                {
                    sb.Append(originalBoard[i, j].ToString() + ",");
                }
                sb.Length--;
                sb.Append("]," + "\r\n");
            }
            //Remove newline character/','
            sb.Length--;
            sb.Length--;
            sb.Length--;
            sb.Append("]");
            //Finish formating
            File.GetPath
            runCmd(@"C:\Program Files(x86)\Microsoft Visual Studio\Shared\Python36_64\python.exe", "Solver.py " + sb.ToString());
        }

        private void runCmd(string cmd, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = cmd;//cmd is full path to python.exe
            start.Arguments = args;//args is path to .py file and any cmd line args
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.Write(result);
                }
            }
        }
    }
}

