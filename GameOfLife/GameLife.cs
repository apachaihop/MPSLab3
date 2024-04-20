using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameOfLife
{
public class GameOfLife
    {
        private bool[,] currentGeneration;
        private bool[,] nextGeneration;
        private int rows;
        private int cols;
        private bool running;
        private Barrier barrier;

        public GameOfLife(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            currentGeneration = new bool[rows, cols];
            nextGeneration = new bool[rows, cols];
            running = false;
            barrier = new Barrier(1); // Initialize barrier with initial count 1
        }

        public void StartSimulation()
        {
            running = true;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    int row = i;
                    int col = j;

                    if (currentGeneration[row, col])
                    {
                        Thread cellThread = new Thread(() =>
                        {
                            while (running)
                            {
                                UpdateCellState(row, col);
                                barrier.SignalAndWait(); // Signal that the cell has reached the synchronization point
                                Thread.Sleep(1000); // Global clock interval
                            }
                        });

                        cellThread.Start();
                    }
                }
            }
        }

        private void UpdateCellState(int row, int col)
        {
            int aliveNeighbors = CalculateAliveNeighbors(row, col);

            if (currentGeneration[row, col])
            {
                nextGeneration[row, col] = aliveNeighbors == 2 || aliveNeighbors == 3;
            }
            else
            {
                nextGeneration[row, col] = aliveNeighbors == 3;
            }
        }

        private int CalculateAliveNeighbors(int row, int col)
        {
            int aliveNeighbors = 0;

            int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
            int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };

            for (int i = 0; i < 8; i++)
            {
                int newRow = row + dx[i];
                int newCol = col + dy[i];

                if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols)
                {
                    if (currentGeneration[newRow, newCol])
                    {
                        aliveNeighbors++;
                    }
                }
            }

            return aliveNeighbors;
        }

        public void StopSimulation()
        {
            running = false;
        }
    }

}

