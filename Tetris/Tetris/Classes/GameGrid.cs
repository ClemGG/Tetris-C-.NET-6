using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.Classes
{
    internal class GameGrid
    {
        /// <summary>
        /// (0, 0) starts at the top of the grid.
        /// </summary>
        private readonly int[,] _grid;

        public int Rows { get; }
        public int Columns { get; }

        public int this[int r, int c]
        {
            get => _grid[r, c];
            set => _grid[r, c] = value;
        }

        public GameGrid(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            _grid = new int[rows, columns];
        }

        public bool IsCellInside(int r, int c)
        {
            return r >= 0 && r < Rows && c >= 0 && c < Columns;
        }

        public bool IsCellEmpty(int r, int c)
        {
            return IsCellInside(r, c) && _grid[r, c] == 0;
        }

        public bool IsRowFull(int r)
        {
            for (int c = 0; c < Columns; c++)
            {
                if (_grid[r, c] == 0)
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsRowEmpty(int r)
        {
            for (int c = 0; c < Columns; c++)
            {
                if (_grid[r, c] != 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Called when a row is full. We empty it and we move down all rows above it.
        /// </summary>
        public void ClearRow(int r)
        {
            for (int c = 0; c < Columns; c++)
            {
                _grid[r, c] = 0;
            }
        }

        /// <param name="numRows">Number of times we should move the row down.</param>
        public void MoveRowDown(int r, int numRows)
        {
            for (int c = 0; c < Columns; c++)
            {
                _grid[r + numRows, c] = _grid[r, c];
                _grid[r, c] = 0;
            }
        }

        public int ClearFulllRows()
        {
            int cleared = 0;

            //Since (0, 0) starts at the top of the grid, we count from the bottom
            for (int r = Rows - 1; r >= 0; r--)
            {
                if (IsRowFull(r))
                {
                    ClearRow(r);
                    cleared++;
                }
                else if(cleared > 0)
                {
                    MoveRowDown(r, cleared);
                }
            }

            return cleared;
        }
    }
}
