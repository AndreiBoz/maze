/**************************************************************************
 *                                                                        *
 *  Website:     https://github.com/florinleon/ActressMas                 *
 *  Description: The reactive architecture using the ActressMas framework *
 *  Copyright:   (c) 2018, Florin Leon                                    *
 *                                                                        *
 *  This program is free software; you can redistribute it and/or modify  *
 *  it under the terms of the GNU General Public License as published by  *
 *  the Free Software Foundation. This program is distributed in the      *
 *  hope that it will be useful, but WITHOUT ANY WARRANTY; without even   *
 *  the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR   *
 *  PURPOSE. See the GNU General Public License for more details.         *
 *                                                                        *
 **************************************************************************/

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Timers;

namespace Reactive
{
    public partial class MazeForm : Form
    {
        private MazeAgent _ownerAgent;
        private Bitmap _doubleBufferImage;

        private MazeGenerator _mazeGenerator;

        public MazeForm()
        {
            _mazeGenerator = new MazeGenerator();
            InitializeComponent();  
        }

        public void SetOwner(MazeAgent a)
        {
            _ownerAgent = a;
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            DrawMaze();
        }

        public void UpdateMazeGUI()
        {
            DrawMaze();
        }

        private void pictureBox_Resize(object sender, EventArgs e)
        {
            DrawMaze();
        }

        private void DrawMaze()
        {
            int w = pictureBox.Width;
            int h = pictureBox.Height;

            if (_doubleBufferImage != null)
            {
                _doubleBufferImage.Dispose();
                GC.Collect(); // prevents memory leaks
            }

            _doubleBufferImage = new Bitmap(w, h);
            Graphics g = Graphics.FromImage(_doubleBufferImage);
            g.Clear(Color.GhostWhite);

            int minXY = Math.Min(w, h);
            int maxMN = Math.Max(_mazeGenerator.Columns, _mazeGenerator.Lines);
            int cellSize = (minXY - 30) / maxMN;
            
            // Draw complete maze
            for (int i = 0; i < _mazeGenerator.Lines; i++)
            {
                for (int j = 0; j < _mazeGenerator.Columns; j++)
                {
                    //HighlightCells(g, i, j, cellSize);  // Highlight Cells
                    DrawMaze(_mazeGenerator.Maze[i,j], g, i, j, cellSize); // Draw generated maze
                    FillMazeCells(_mazeGenerator.Maze[i, j], g, i, j, cellSize); // Fill cells surrounded by walls
                    FillBetweenMazeCells(_mazeGenerator.Maze[i, j], g, i, j, cellSize); // Fill space between cells
                    DrawMazeBorders(_mazeGenerator.Maze[i, j], g, i, j, cellSize); // Draw maze borders
                }
            }

            if (_ownerAgent != null)
            {
                foreach (string v in _ownerAgent.ExplorerPositions.Values)
                {
                    string[] t = v.Split();
                    int x = Convert.ToInt32(t[0]);
                    int y = Convert.ToInt32(t[1]);

                    g.FillEllipse(Brushes.Black, 20 + x * cellSize + 6, 20 + y * cellSize + 6, cellSize - 12, cellSize - 12);
                }
            }

            Graphics pbg = pictureBox.CreateGraphics();
            pbg.DrawImage(_doubleBufferImage, 0, 0);
        }

        private void DrawMaze(Directions cell, Graphics g, int i, int j, int cellSize)
        {
            if (cell.up)
                g.FillRectangle(Brushes.Brown, 20 + j * cellSize + 10, 10 + i * cellSize + 10, cellSize - 20, 10);
            if (cell.down)
                g.FillRectangle(Brushes.Brown, 20 + j * cellSize + 10, cellSize + i * cellSize + 10, cellSize - 20, 10);
            if (cell.left)
                g.FillRectangle(Brushes.Brown, 20 + j * cellSize, 30 + i * cellSize, 10, cellSize - 20);
            if (cell.right)
                g.FillRectangle(Brushes.Brown, cellSize + j * cellSize + 10, 20 + i * cellSize + 10, 10, cellSize - 20);
        }

        private void FillMazeCells(Directions cell, Graphics g, int i, int j, int cellSize)
        {
            if (cell.up && cell.down && cell.left && cell.right)
            {
                g.FillRectangle(Brushes.Brown, 20 + j * cellSize + 10, 20 + i * cellSize + 10, cellSize - 20, cellSize - 20);
            }
        }

        private void FillBetweenMazeCells(Directions cell, Graphics g, int i, int j, int cellSize)
        {   // Do not exceed boundary
            if (i < _mazeGenerator.Lines - 1 && j < _mazeGenerator.Columns - 1)
            {
                Directions cellDiagonal = _mazeGenerator.Maze[i + 1, j + 1];
                if (cell.right && cell.down && cellDiagonal.left && cellDiagonal.up)
                {
                    g.FillRectangle(Brushes.Brown, 20 + j * cellSize + 10, cellSize + i * cellSize + 10, cellSize - 0, 20); // |_| full
                }
                //if (cell.down && cellDiagonal.left && cellDiagonal.up) // |_|
                //{
                    //g.FillRectangle(Brushes.Brown, 20 + j * cellSize + 10, cellSize + i * cellSize + 10, cellSize - 0, 20);
                //}
                //if (cell.right && cellDiagonal.left)
                //{
                  //  g.FillRectangle(Brushes.Brown, 20 + j * cellSize + 10, cellSize + i * cellSize + 10, cellSize - 0, 20);
                //}
            }
        }

        private void DrawMazeBorders(Directions cell, Graphics g, int i, int j, int cellSize)
        {   
            if (i == 0 && cell.up)
                g.FillRectangle(Brushes.Brown, 10 + j * cellSize + 10, i * cellSize, cellSize, 30);// draw upper border
            if (j == 0 && cell.left)
                g.FillRectangle(Brushes.Brown, 0 + j * cellSize, 30 + i * cellSize, 30, cellSize); // draw left border
            if (i == _mazeGenerator.Lines - 1 && cell.down)
                g.FillRectangle(Brushes.Brown, 10 + j * cellSize + 10, cellSize + i * cellSize + 10, cellSize, 30); // draw bottom border
            if (j == _mazeGenerator.Columns - 1 && cell.right)
                g.FillRectangle(Brushes.Brown, cellSize + j * cellSize + 10, 20 + i * cellSize + 10, 30, cellSize); // draw right border

        }

        private void HighlightCells(Graphics g, int i, int j, int cellSize)
        {
            g.FillRectangle(Brushes.LightGreen, 20 + j * cellSize + 10, 20 + i * cellSize + 10, cellSize - 20, cellSize - 20);
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
 
        }
    }
}