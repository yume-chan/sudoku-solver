using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SudokuSolver
{
    class Grid : ICloneable
    {
        public static IntList Numbers = new IntList();

        int[,] values = new int[9, 9];
        void setValue(int x, int y, int value)
        {
            values[x, y] = value;

            for (int i = 0; i < 9; i++)
            {
                if (i != y)
                    Possibilities[x, i].Remove(value);
                if (i != x)
                    Possibilities[i, y].Remove(value);
            }
            int rectX = x / 3 * 3, rectY = y / 3 * 3;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    Possibilities[rectX + i, rectY + j].Remove(value);

            SolvedCount++;
        }
        public void UpdateValues()
        {
            bool changed;
            do
            {
                changed = false;
                for (int i = 0; i < 9; i++)
                    for (int j = 0; j < 9; j++)
                        if (values[i, j] == 0 && Possibilities[i, j].Count == 1)
                        {
                            setValue(i, j, Possibilities[i, j][0]);
                            changed = true;
                        }
            }
            while (changed);
        }
        public int this[int x, int y]
        {
            get { return values[x, y]; }
            set { setValue(x, y, value); }
        }

        public class IntList
        {
            int[] items = new int[9];
            public int Count = 0;

            public int this[int index]
            {
                get { return items[index]; }
                set { items[index] = value; }
            }

            public void Add(int value)
            {
                items[Count++] = value;
            }

            public void Remove(int value)
            {
                int index = Array.IndexOf<int>(items, value);
                if (index != -1 && index < Count)
                {
                    Count--;
                    if (index < Count)
                        Array.Copy(items, index + 1, items, index, Count - index);
                }
            }

            public IntList() { }
            public IntList(IntList another)
            {
                Array.Copy(another.items, items, another.Count);
                Count = another.Count;
            }
        }

        public IntList[,] Possibilities = new IntList[9, 9];

        public class Position
        {
            public int X, Y;
            public override string ToString()
            {
                return string.Format("({0}, {1})", X, Y);
            }
        }

        public Position GetNextPosition()
        {
            int x = 0, y = 0;
            int nextCount = 10;
            int currentCount;
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                {
                    if (values[i, j] == 0)
                    {
                        currentCount = Possibilities[i, j].Count;
                        if (currentCount == 0)
                            return null;
                        if (currentCount < nextCount)
                        {
                            nextCount = currentCount;
                            x = i;
                            y = j;
                        }
                    }
                }
            return new Position() { X = x, Y = y };
        }

        class StateObject
        {
            public int X, Y, Value;
        }

        public void Resolve()
        {
            Position position = GetNextPosition();
            int x = position.X, y = position.Y;
            IntList possibilities = Possibilities[x, y];
            for (int i = 0; i < possibilities.Count; i++)
                new Thread(new ParameterizedThreadStart(MultithreadResolve)).Start(new StateObject() { X = x, Y = y, Value = possibilities[i] });
            // Resolve(GetNextPosition(), this);
        }

        public bool Resolve(Position position, Grid state)
        {
            int x = position.X, y = position.Y;
            Grid stateCopy = new Grid(state);
            IntList possibilities = stateCopy.Possibilities[x, y];
            for (int i = 0; i < possibilities.Count; i++)
            {
                state[x, y] = possibilities[i];
                state.UpdateValues();

                if (state.SolvedCount == 81)
                {
                    Console.Write(state.ToString());

                    Console.WriteLine();
                    lock (typeof(Grid))
                        Console.Write("Elapsed time: {0}ms", Environment.TickCount - Program.StartTime);
                    return true;

                    // All answers.
                    /*
                    Sudoku = (Grid)stateCopy.Clone();
                    zone = Sudoku[zone.X, zone.Y];
                    continue;
                    */
                }

                Grid.Position nextPosition = state.GetNextPosition();
                if (nextPosition != null && Resolve(nextPosition, state))
                    return true;
                state = new Grid(stateCopy);
            }
            return false;
        }

        public void MultithreadResolve(object param)
        {
            StateObject state = (StateObject)param;
            Grid stateCopy = new Grid(this);
            stateCopy[state.X, state.Y] = state.Value;
            stateCopy.UpdateValues();
            if (stateCopy.SolvedCount == 81)
            {
                Console.Write(state.ToString());
                return;

                // All answers.
                /*
                Sudoku = (Grid)stateCopy.Clone();
                zone = Sudoku[zone.X, zone.Y];
                continue;
                */
            }
            Grid.Position nextPosition = stateCopy.GetNextPosition();
            if (nextPosition != null)
                Resolve(nextPosition, stateCopy);
        }

        public int SolvedCount = 0;

        public Grid()
        {
            Numbers = new IntList();
            for (int i = 1; i < 10; i++)
                Numbers.Add(i);

            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    Possibilities[i, j] = new IntList(Numbers);
        }
        public Grid(Grid another)
        {
            values = (int[,])another.values.Clone();

            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    Possibilities[i, j] = new IntList(another.Possibilities[i, j]);

            SolvedCount = another.SolvedCount;
        }

        public object Clone()
        {
            return new Grid(this);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < 9; i++)
            {
                builder.AppendLine();
                for (int j = 0; j < 9; j++)
                {
                    builder.Append(values[i, j]);
                    builder.Append(" ");
                }
            }
            builder.AppendLine();
            builder.Append(SolvedCount);
            return builder.ToString();
        }
    }
}
