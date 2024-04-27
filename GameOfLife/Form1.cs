using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TP14_JeudelaVie
{

    
    public partial class Form1 : Form
    {
        const int squarePerLine = 10;
        const int squarePerColumn = 10;
        static int positionOffsetX = 50;
        static int positionOffsetY = 50;
        private Barrier barrier=new (10*10);
        Mutex mutexObj = new();
        PictureBox[,] squares = new PictureBox[squarePerLine, squarePerColumn];
        static bool[,] squaresState = new bool[squarePerLine, squarePerColumn];
        static bool[,] squaresFutureState = new bool[squarePerLine, squarePerColumn];

        static Cell[,] squaresCells=new Cell[squarePerLine, squarePerColumn];
        static Cell[,] squaresFutureCells = new Cell[squarePerLine, squarePerColumn];

        static int squareSize = 20;

        static bool red = true;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            squareModel.Visible = false;
            squareModelAlive.Visible = false;

            myTimer.Enabled = false;
            myTimer.Interval = 550;

            timer1.Enabled = false;
            timer1.Interval = 200;
            redToolStripMenuItem.Checked = true;

            stopToolStripMenuItem.Checked = true;
            middleSpeedToolStripMenuItem.Checked = true;

            this.Size = new Size(positionOffsetX + squarePerLine * (squareSize + squareSize/4) + positionOffsetX, positionOffsetY + squarePerColumn * (squareSize + squareSize / 4) + positionOffsetY);

            for (int i = 0; i < squarePerLine; i++)
            {
                for (int j = 0; j < squarePerColumn; j++)
                {
                    squares[i, j] = new PictureBox();                  
                    squares[i, j].Width = squareSize;
                    squares[i, j].Height = squareSize;
                    squares[i, j].Location = new Point(positionOffsetX + i* (squareSize + squareSize / 4) , positionOffsetY + j * (squareSize + squareSize / 4));

                    squares[i, j].BackColor = squareModel.BackColor;
                    squares[i, j].Click += new EventHandler(Cell_Click);
                    squaresCells[i, j] = new DeadCell();
                    Controls.Add(this.squares[i, j]);
                }
            }
        }

        void Cell_Click(object sender, EventArgs e)
        {
            bool @true = false;
            int i;
            int j = 0;

            for (i = 0; i < squarePerLine; i++)
            {
                for (j = 0; j < squarePerColumn; j++)
                {
                    if (sender == squares[i, j])
                    {
                        @true = true;
                        break;
                    }
                }
                if (@true == true) break;
            }

            squaresState[i, j] = !squaresState[i, j];

            if (squaresState[i, j] == true)
                if (red == false)
                {
                    squares[i, j].BackColor = squareModelAlive.BackColor;
                    squaresCells[i, j] = new GreenCell();
                }
                else
                {
                    squares[i, j].BackColor = pictureBox1.BackColor;
                    squaresCells[i, j] = new RedCell();

                }
            else
            {
                squares[i, j].BackColor = squareModel.BackColor;
                squaresCells[i, j] = new Cell();
            }
        }
        public void CalculateNextCellState(int x, int y)
        {
            int redNeighborsStrength = 0;
            int greenNeighborsStrength = 0;
            int neighborPosX;
            int neighborPosY;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    neighborPosX = x + i;

                    if (neighborPosX >= squarePerLine)
                    {
                        neighborPosX = 0;
                    }

                    if (neighborPosX < 0)
                    {
                        neighborPosX = squarePerLine - 1;
                    }


                    neighborPosY = y + j;

                    if (neighborPosY >= squarePerColumn)
                    {
                        neighborPosY = 0;
                    }

                    if (neighborPosY < 0)
                    {
                        neighborPosY = squarePerColumn - 1;
                    }

                    if (squaresCells[neighborPosX, neighborPosY] is GreenCell)
                    {
                        greenNeighborsStrength += 1;
                    }
                    if (squaresCells[neighborPosX, neighborPosY] is RedCell)
                    {
                        redNeighborsStrength += 3;
                    }
                }
            }
            if (squaresCells[x, y] is GreenCell)
            {
                greenNeighborsStrength -= 1;
            }
            if (squaresCells[x, y] is RedCell)
            {
                redNeighborsStrength -= 3;
            }

            if (greenNeighborsStrength >= redNeighborsStrength)
            {
                if (greenNeighborsStrength == 0)
                {
                    squaresFutureCells[x, y] = new DeadCell();
                    return;
                }
                if (greenNeighborsStrength == 1)
                {
                    squaresFutureCells[x, y] = new DeadCell();
                    return;

                }
                else if (greenNeighborsStrength == 2)
                {
                    squaresFutureCells[x, y] = squaresCells[x, y];
                    return;

                }
                else if (greenNeighborsStrength == 3)
                {
                    squaresFutureCells[x, y] = new GreenCell();
                    return;

                }
                else if (greenNeighborsStrength >= 4)
                {
                    squaresFutureCells[x, y] = new DeadCell();
                    return;

                }
            }
            if (greenNeighborsStrength < redNeighborsStrength)
            {
                if (redNeighborsStrength == 3||(redNeighborsStrength == 3&& greenNeighborsStrength == 0))
                {
                    squaresFutureCells[x, y] = new DeadCell();
                    return;

                }
                else if (redNeighborsStrength == 6)
                {
                    squaresFutureCells[x, y] = squaresCells[x, y];
                    return;

                }
                else if (redNeighborsStrength == 9)
                {
                    squaresFutureCells[x, y] = new RedCell();
                    return;

                }
                else if (redNeighborsStrength >= 12)
                {
                    squaresFutureCells[x, y] = new DeadCell();
                    return;

                }
            }
            

        }
        private void tmrClock_Tick(object sender, EventArgs e)
        {
            Thread[] threads = new Thread[squarePerLine * squarePerColumn];

            for (int i = 0; i < squarePerLine; i++)
            {
                for (int j = 0; j < squarePerColumn; j++)
                {
                    int x = i;
                    int y = j;
                    threads[i * squarePerColumn + j] = new Thread(() =>
                    {
                        int numNeigh = calculateNeighbors(x, y, squaresState);
                        if (numNeigh <= 1)
                        {
                            squaresFutureState[x, y] = false;
                        }
                        else if (numNeigh == 2)
                        {
                            squaresFutureState[x, y] = squaresState[x, y];
                        }
                        else if (numNeigh == 3)
                        {
                            squaresFutureState[x, y] = true;
                        }
                        else
                        {
                            squaresFutureState[x, y] = false;
                        }
                    });
                    threads[i * squarePerColumn + j].Start();
                }
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }


            for (int i = 0; i < squarePerLine; i++)
            {
                for (int j = 0; j < squarePerColumn; j++)
                {
                    squares[i, j].BackColor = squaresState[i, j] ? pictureBox1.BackColor : squareModel.BackColor;
                }
            }
        }

        void transferBoolArray(Cell [,]Table1, Cell[,] Table2)
        {
            for (int i = 0; i < squarePerLine; i++)
            {
                for (int j = 0; j < squarePerColumn; j++)
                {
                    Table1[i, j] = Table2[i, j];
                }
            }
        }

        static int calculateNeighbors(int x, int y, bool [,] Table1)
        {
            int NeighborsCount = 0;
            int neighborPosX;
            int neighborPosY;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    neighborPosX = x + i;

                    if (neighborPosX>= squarePerLine)
                    {
                        neighborPosX = 0;
                    }

                    if(neighborPosX<0)
                    {
                        neighborPosX = squarePerLine - 1;
                    }


                    neighborPosY = y + j;

                    if (neighborPosY >= squarePerColumn)
                    {
                        neighborPosY = 0;
                    }

                    if (neighborPosY < 0)
                    {
                        neighborPosY = squarePerColumn - 1;
                    }


                    if (Table1[neighborPosX, neighborPosY] == true)
                        NeighborsCount = NeighborsCount + 1;
                }
            }

            if (Table1[x, y] == true)
                NeighborsCount = NeighborsCount - 1;

            return NeighborsCount;
        }

        private void slowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myTimer.Interval = 1000;
            lentToolStripMenuItem.Checked = true;
            middleSpeedToolStripMenuItem.Checked = false;
            rapideToolStripMenuItem.Checked = false;
        }

        private void normalSpeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myTimer.Interval = 550;
            lentToolStripMenuItem.Checked = false;
            middleSpeedToolStripMenuItem.Checked = true;
            rapideToolStripMenuItem.Checked = false;
        }

        private void quickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myTimer.Interval = 100;
            lentToolStripMenuItem.Checked = false;
            middleSpeedToolStripMenuItem.Checked = false;
            rapideToolStripMenuItem.Checked = true;
        }
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < squarePerLine; i++)
            {
                for (int j = 0; j < squarePerColumn; j++)
                {
                    squares[i, j].BackColor = squareModel.BackColor;
                    squaresState[i, j] = false;
                }
            }
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myTimer.Enabled = false;
            timer1.Enabled= true;
            newToolStripMenuItem.Enabled = false;
            runToolStripMenuItem.Checked = true;
            stopToolStripMenuItem.Checked = false;
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myTimer.Enabled = false;
            timer1.Enabled = false;
            newToolStripMenuItem.Enabled = true;
            runToolStripMenuItem.Checked = false;
            stopToolStripMenuItem.Checked = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void redToolStripMenuItem_Click(object sender, EventArgs e)
        {
            red = true;
            redToolStripMenuItem.Checked= true;
            greanToolStripMenuItem.Checked= false;
        }

        private void greanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            red = false;
            redToolStripMenuItem.Checked = false;
            greanToolStripMenuItem.Checked = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Thread[] threads = new Thread[squarePerLine * squarePerColumn];
            for (int i = 0; i < squarePerLine; i++)
            {
                for (int j = 0; j < squarePerColumn; j++)
                {
                    int x = i;
                    int y = j;
                    threads[i * squarePerColumn + j] = new Thread(() =>
                    {
                        CalculateNextCellState(x, y);
                    });
                    threads[i * squarePerColumn + j].Start();
                    
                }
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            transferBoolArray(squaresCells, squaresFutureCells);

            for (int i = 0; i < squarePerLine; i++)
            {
                for (int j = 0; j < squarePerColumn; j++)
                {
                   
                    if (squaresCells[i, j] is RedCell)
                    {
                        squares[i, j].BackColor = pictureBox1.BackColor;
                    }
                    if (squaresCells[i, j] is GreenCell)
                    {
                        squares[i, j].BackColor = squareModelAlive.BackColor;
                    }
                    if (squaresCells[i, j] is DeadCell)
                    {
                        squares[i, j].BackColor = squareModel.BackColor;
                    }

                }
            }
            bool allRedCells = true;

            for (int i = 0; i < squarePerLine; i++)
            {
                for (int j = 0; j < squarePerColumn; j++)
                {
                    if (squaresCells[i, j] is GreenCell)
                    {
                        allRedCells = false;
                        break;
                    }
                }
            }

            bool allGreenCells = true;

            for (int i = 0; i < squarePerLine; i++)
            {
                for (int j = 0; j < squarePerColumn; j++)
                {
                    if (squaresCells[i, j] is RedCell)
                    {
                        allGreenCells = false;
                        break;
                    }
                }
            }

            if(allRedCells)
            {
                timer1.Enabled = false;
                newToolStripMenuItem_Click(sender, e);
                MessageBox.Show("Red Allert");
                

            }

            else if(allGreenCells)
            {
                timer1.Enabled = false;
                newToolStripMenuItem_Click(sender, e);

                MessageBox.Show("Green day");
                
            }

        }
    }
    public class Cell
    {
        public int Strength { get; protected set; }

        public Cell(int strength)
        {
            Strength = strength;
        }
        public Cell()
        {

        }

    }

    public class RedCell : Cell
    {
        public RedCell() : base(3)
        {
        }
    }

    public class GreenCell : Cell
    {
        public GreenCell() : base(1)
        {
        }
    }

    public class DeadCell:Cell
    {
        public DeadCell() : base(0)
        {
        }
    }
}
