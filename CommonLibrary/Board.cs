
using CommonLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public class Board
    {
        public Cell[,] Cells { get; set; }
        public Board()
        {
            {
                // cell 24 is for white eaten and 25 is for black
                Cells = new Cell[3, 3];
                resetCells();
            }

        }
        private void resetCells()
        {
            for (int i = 0; i < Cells.GetLength(0); i++)
            {
                for (int j = 0; j < Cells.GetLength(1); j++)
                {
                    Cells[i, j] = new Cell() { Id = i, CellColor = PlayerCell.Empty };
                }
            }

        }
    }
}
