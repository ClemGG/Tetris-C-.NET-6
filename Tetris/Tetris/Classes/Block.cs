using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.Classes
{
    internal abstract class Block
    {
        /// <summary>
        /// The first dimension holds the arrays of each rotation state.
        /// </summary>
        protected abstract Position[][] Tiles { get; }
        protected abstract Position StartOffset { get; }
        public abstract int ID { get; }

        private int _rotationState;
        private Position _offset;

        public Block()
        {
            _offset = new Position(StartOffset.Row, StartOffset.Column);
        }

        /// <summary>
        /// Returns all Cells occupied by this Block.
        /// </summary>
        public IEnumerable<Position> TilePositions()
        {
            foreach (Position p in Tiles[_rotationState])
            {
                yield return new Position(p.Row + _offset.Row, p.Column + _offset.Column);
            }
        }

        public void RotateCW()
        {
            //The % warps it back to 0 if we overflow.
            _rotationState = (_rotationState + 1) % Tiles.Length;
        }

        public void RotateCCW()
        {
            if(_rotationState == 0)
            {
                _rotationState = Tiles.Length - 1;
            }
            else
            {
                _rotationState--;
            }
        }

        public void Move(int r, int c)
        {
            _offset.Row += r;
            _offset.Column += c;
        }

        public void Reset()
        {
            _rotationState = 0;
            _offset.Row = StartOffset.Row;
            _offset.Column = StartOffset.Column;
        }
    }
}
