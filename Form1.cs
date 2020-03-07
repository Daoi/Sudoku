using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Sudoku
{
    public partial class frmMain : Form
    {

        SudokuBoard game;
        Random rng = new Random();
        int difficulty = 43;
        int cardCellWidth;
        int cardCellHeight;
        int barWidth = 3;  // Width or thickness of horizontal and vertical bars
        int xcardUpperLeft = 0;
        int ycardUpperLeft = 0;
        int padding = 2;
        int cardSize;
        int[,] solvedBoard;
        private Button[,] boardCell;

        public frmMain()
        {
            InitializeComponent();
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            btnPlay.Enabled = false;
            btnPlay.Visible = false;
            btnNewBoard.Enabled = true;
            btnNewBoard.Visible = true;
            btnCheck.Enabled = true;
            btnCheck.Visible = true;
            btnSolve.Enabled = true;
            btnSolve.Visible = true;
            btnReset.Enabled = true;
            btnReset.Visible = true;
            lblInstructions.Visible = true;

            game = new SudokuBoard(9, difficulty);
            cardSize = 9;
            boardCell = new Button[cardSize, cardSize];
            solvedBoard = new int[cardSize, cardSize];
            game.printSudoku();
            generateBoard();
            game.getCells(ref boardCell);
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
                    int value = game.getCellValue(row, col);
                    boardCell[row, col].Text = value.ToString();
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
            btnSolve.Enabled = false;
            btnNewBoard.Enabled = false;
            btnCheck.Enabled = false;
            btnReset.Enabled = false;
            pnlBoard.Controls.OfType<Button>().ToList().ForEach(btn => btn.Enabled = false);

            DialogResult dr = MessageBox.Show("Would you like the solve process to be visualized?",
                    "Solve", MessageBoxButtons.YesNo);
            if (dr == DialogResult.No)
            {
                game.setTimers();
            }

            solvedBoard = game.solve();

            btnReset.Enabled = true;
            btnNewBoard.Enabled = true;
            btnCheck.Enabled = true;
        }

        private void btnNewBoard_Click(object sender, EventArgs e)
        {
            game = new SudokuBoard(9, difficulty);
            pnlBoard.Controls.OfType<Button>().ToList().ForEach(btn => btn.Dispose()); //Get rid of all current buttons
            cardSize = 9;
            boardCell = new Button[cardSize, cardSize];
            solvedBoard = new int[cardSize, cardSize];
            generateBoard();
            game.getCells(ref boardCell);
            btnSolve.Enabled = true;
            btnReset.Enabled = true;

        }

        private void rbtnEasy_Click(object sender, EventArgs e)
        {
            rbtnMedium.Checked = false;
            rbtnHard.Checked = false;
            difficulty = rng.Next(30, 45);
        }

        private void rbtnMedium_Click(object sender, EventArgs e)
        {
            rbtnEasy.Checked = false;
            rbtnHard.Checked = false;
            difficulty = rng.Next(46, 61);
        }

        private void rbtnHard_Click(object sender, EventArgs e)
        {
            rbtnEasy.Checked = false;
            rbtnMedium.Checked = false;
            difficulty = rng.Next(62, 72);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            btnSolve.Enabled = true;
            int[,] originalBoard = game.getOriginal();
            for(int i = 0; i < cardSize; i++)
            {
                for(int j = 0; j < cardSize; j++)
                {
                    boardCell[i, j].Text = originalBoard[i, j].ToString();
                }
            }
            pnlBoard.Controls.OfType<Button>().ToList().ForEach(btn => reenable(btn));
        }

        private void reenable(Button btn)
        {
            if(btn.Text == "0"){ btn.Enabled = true;}
            else { btn.Enabled = false; }
        }
    }
}

