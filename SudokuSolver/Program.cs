using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SudokuSolver
{
    class Program
    {
        public static Grid Sudoku = new Grid();
        public static long StartTime = 0;

        static void Main(string[] args)
        {
            string[] Lines = new string[9]
            {
                "000001030",
                "402006000",
                "000000070",
                "980030000",
                "000108000",
                "000040026",
                "030000000",
                "000900104",
                "050700000"
            };

            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    if (Lines[i][j] != '0')
                        Sudoku[i, j] = int.Parse(Lines[i][j].ToString());

            StartTime = Environment.TickCount;

            // For simple sudoku.
            /*
            Sudoku.UpdateValues();

            if (Sudoku.SolvedCount == 81)
                Console.Write(Sudoku.ToString());
            else
            */
            Sudoku.Resolve();

            Console.ReadLine();
        }
    }
}
