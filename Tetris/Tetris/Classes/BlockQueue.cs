using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.Classes
{
    internal class BlockQueue
    {
        public readonly Block[] Blocks = new Block[]
        {
            new IBlock(),
            new JBlock(),
            new LBlock(),
            new OBlock(),
            new SBlock(),
            new TBlock(),
            new ZBlock()
        };

        private readonly Random _random = new Random();

        public Block NextBlock { get; private set; }

        public BlockQueue()
        {
            NextBlock = RandomBlock();
        }

        private Block RandomBlock()
        {
            return Blocks[_random.Next(Blocks.Length)];
        }

        public Block GetAndUpdate()
        {
            Block block = NextBlock;

            do
            {
                NextBlock = RandomBlock();
            }
            while (block.ID == NextBlock.ID);

            return block;
        } 
    }
}
