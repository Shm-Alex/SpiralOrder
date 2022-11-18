using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NSpiralOrder
{
    internal class Program
    {
        enum Directions
        {
            up, right, down, left, _max
        }
        private class Velocity
        {
            public Velocity(Directions direction) {
                _direction = direction;
            }

            Directions _direction;

            static Dictionary<Directions, (int verticalV, int horisontalV)> toV
               = new Dictionary<Directions, (int verticalV, int horisontalV)>()
               { { Directions.up,(-1,0)},
                  { Directions.right,(0,1)},
                  { Directions.down,(1,0)},
                  { Directions.left,(0,-1)},
               };

            public int horisontalV => toV[_direction].horisontalV;
            public int verticalV => toV[_direction].verticalV;
            public void RotateClockwise()
            {
                _direction = (Directions)((((int)_direction) + 1) % (int)Directions._max);
            }
        }
        public static bool InBorder<T>(T[][] matrix, int column, int row) => (row >= 0) && (column >= 0) &&
                                                                        (row < matrix.Length) &&
                                                                         (column < matrix[row].Length);
        public static bool OnLeftBorder<T>(T[][] matrix, int column, int row) => (column == 0) &&
                                                                         (row < matrix.Length) &&
                                                                          (row >=0);
        public static bool OnRightBorder<T>(T[][] matrix, int column, int row)
            => (row < matrix.Length) && (row >= 0)
            && (column==matrix[row].Length-1)                           ;
        public static bool OnTopBorder<T>(T[][] matrix, int column, int row)
            => (row == 0)
            && (column < matrix[0].Length) && (column >=0 );
        public static bool OnBottomBorder<T>(T[][] matrix, int column, int y)
            => (y == matrix.Length-1)
            && (column < matrix[y].Length) && (column >= 0);
        public static bool OnBorder<T>(T[][] matrix, int column, int row) => 
            OnLeftBorder(matrix, column, row)||
            OnRightBorder(matrix, column, row) ||
            OnBottomBorder(matrix, column, row) ||
            OnTopBorder(matrix, column, row) 
            ;
                                                                    
        public static IList<int> SpiralOrder(int[][] matrix)
        {
            int nmbrRows = matrix.Length;
            int nmbrColumns = matrix[0].Length;
            int retSize = (nmbrColumns) * (nmbrRows);
            var ret = new (int value,int origColumn,int origRow,bool handled)[retSize];
            int column = 0, row = 0;
            int delay = 1000;
            Velocity v = new Velocity(Directions.right);
            for (int i = 0; i < retSize; i++)
            {
                ret[i].value = matrix[row][column];
                ret[i].origRow = row;
                ret[i].origColumn=column;
                ret[i].handled = true;

                int nexStepColumn = column + v.horisontalV;
                int nexStepRow = row + v.verticalV;
                int rotateCounter = 0;
                ShowAndClear(matrix, new[] { ( column, row, ConsoleColor.Green), ( nexStepColumn, nexStepRow, ConsoleColor.Red) }, delay);
                while ((!((nexStepColumn >= 0) && (nexStepRow >= 0) && (nexStepColumn < nmbrColumns) && (nexStepRow < nmbrRows))
                     || ret.Any(r => (r.origColumn == nexStepColumn) && (r.origRow == nexStepRow) && r.handled)
                    )
                    && rotateCounter < 4
                   
                    )
                {
                    v.RotateClockwise();
                    rotateCounter++;
                    nexStepColumn = column + v.horisontalV;
                    nexStepRow = row + v.verticalV;
                    ShowAndClear(matrix, new[] { ( column, row, ConsoleColor.Green), ( nexStepColumn, nexStepRow, ConsoleColor.Red) }, delay);
                   // if (rotateCounter == 4) throw new Exception("Повернулись вокруг своей оси");
                }
                
                if (!((nexStepColumn >= 0) && (nexStepRow >= 0) && (nexStepColumn < nmbrColumns) && (nexStepRow < nmbrRows))) 
                    throw new Exception("Почему то выбрали кривые новые координаты");
                column = nexStepColumn;
                row = nexStepRow;
                ShowAndClear(matrix, new[] { ( column, row, ConsoleColor.Green) }, delay);
            }
            return ret.Select(r=>r.value).ToList();
        }
        
        public static T[][] Show<T>(T[][] matrix,(int column,int row, ConsoleColor color)[] markers=null)
        {
            int maxLength = 0;
            {
                for (int row = 0; row < matrix.Length; row++)
                    for (int column = 0; column < matrix[row].Length; column++)
                    {
                        if( maxLength<(matrix[row][column].ToString().Length)) maxLength= matrix[row][column].ToString().Length;
                    }
            }
            ConsoleColor swapColor = Console.ForegroundColor;
                for (int row = -1; row < matrix.Length+1; row++)
                {
                    //Console.Write($"|");
                    for (int column = -1; column < matrix[0].Length+1; column++)
                    {
                    if(markers!=null && markers.Any(m=>(m.column==column)&&(m.row==row))) Console.ForegroundColor= markers.First(m => (m.column == column) && (m.row == row)).color;
                    if(InBorder(matrix,column,row))    
                    Console.Write($"{matrix[row][column].ToString().PadLeft(maxLength)}|");
                    else Console.Write($"X|".PadLeft(maxLength));
                    Console.ForegroundColor=swapColor;
                    }
                    Console.WriteLine("");
                }
                return matrix;

            }
        public static T[][] ShowAndClear<T>(T[][] matrix, (int column, int row, ConsoleColor color)[] markers ,int delay=1000)
        {
            var ret = Show(matrix, markers);
            Thread.Sleep(delay);
            Console.SetCursorPosition(0, Console.CursorTop -matrix.Length-2 );
            for(int row =0;row<matrix.Length+2;row++)Console.WriteLine($"                              ");
            Console.SetCursorPosition(0, Console.CursorTop - matrix.Length-2);
            return ret ;
        }
            static void Main(string[] args)
        {
            //[[1,2,3,4],[5,6,7,8],[9,10,11,12]]
            
            var matrix = new int[][] { new int[] { 1, 2, 3, 4 }, new int[] { 5, 6, 7, 8 }, new int[] { 9, 10, 11, 12 } };

             //   Show(matrix, markers:new[] { (1, 1, ConsoleColor.Red), (1, 2, ConsoleColor.Green) });
            //     ShowAndClear(matrix, markers: new[] { (1, 2, ConsoleColor.Red), (0, 0, ConsoleColor.Green) });
         //   Show(matrix);
            Console.WriteLine("{" + string.Join(" ,", SpiralOrder(matrix)) + "}");
            
            
            //     Console.WriteLine("{" + string.Join(" ,", SpiralOrder(new int[][] { new int[] { 1, 2, 3 }, new int[] { 4, 5, 6 } })) + "}");
            //    Console.WriteLine("{" + string.Join(" ,", SpiralOrder(new int[][] { new int[] { 1, 2 }, new int[] { 3, 4 }, new int[] { 5, 6 } })) + "}");
            //   Console.WriteLine("{" + string.Join(" ,", SpiralOrder(new int[][] { new int[] { 1, 2 }, new int[] { 3, 4 } })) + "}");
            Console.ReadLine();
        }
    }
}
