using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using static Program;

class Program
{
    public class Point2D
    {
        public int X { get; set; }
        public int Y { get; set; }
        public float Value { get; set; }

        public static Point2D[,] CreateMatrix(bool[,] grid, Point2D start, Point2D finish)
        {
            int height = grid.GetLength(0);
            int length = grid.GetLength(1);
            Point2D[,] matrix = new Point2D[height, length];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    if (grid[i, j])
                    {
                        matrix[i, j] = new Point2D() { X = j, Y = i, Value = float.MaxValue };
                    }
                    else
                    {
                        matrix[i, j] = new Point2D() { X = j, Y = i };
                    }
                }
            }

            return matrix;
        }

        public override bool Equals(object obj)
        {
            Point2D point = (Point2D)obj;
            return X == point.X && Y == point.Y && Value == point.Value;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    static bool[,] GetBoolGrid(string[] gamefield, int height, int length)
    {
        bool[,] grid = new bool[gamefield.Count(), gamefield[0].Length];

        for (int i = 0; i < gamefield.Count(); i++)
        {
            for (int j = 0; j < gamefield[0].Length; j++)
            {
                if (gamefield[i][j].Equals(' '))
                {
                    grid[i, j] = false;
                }
                else
                {
                    grid[i, j] = true;
                }
            }
        }

        return grid;
    }

    static void SetValue(Point2D[,] matrix, int x, int y, float value, List<Point2D> pointsInCheck, bool diagonal, List<Point2D> checkedPoints)
    {
        if (!checkedPoints.Contains(matrix[y, x]) && matrix[y, x].Value != float.MaxValue)
        {
            matrix[y, x].Value = Math.Min((float)(value + (diagonal ? Math.Sqrt(2) : 1)), matrix[y, x].Value == 0 ? float.MaxValue : matrix[y, x].Value);

            if (!pointsInCheck.Contains(matrix[y, x]))
            {
                pointsInCheck.Add(matrix[y, x]);
            }
        }
    }

    static List<Point2D> FindPath(bool[,] grid, Point2D start, Point2D finish)
    {
        int height = grid.GetLength(0);
        int length = grid.GetLength(1);
        Point2D[,] matrix = Point2D.CreateMatrix(grid, start, finish);
        int borderCounter = 0;

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < length; j++)
            {
                if (grid[i, j])
                {
                    borderCounter++;
                }
            }
        }

        List<Point2D> pointsInCheck = new List<Point2D>() { finish };
        List<Point2D> checkedPoints = new List<Point2D>();

        for (int i = 0; i < height * length - borderCounter; i++)
        {
            Point2D minValuePoint = pointsInCheck.OrderBy(x => x.Value).First();

            if (minValuePoint.Y - 1 >= 0)
            {
                if (minValuePoint.X - 1 >= 0)
                {
                    SetValue(matrix, minValuePoint.X - 1, minValuePoint.Y - 1, minValuePoint.Value, pointsInCheck, true, checkedPoints);
                }

                SetValue(matrix, minValuePoint.X, minValuePoint.Y - 1, minValuePoint.Value, pointsInCheck, false, checkedPoints);

                if (minValuePoint.X + 1 < length)
                {
                    SetValue(matrix, minValuePoint.X + 1, minValuePoint.Y - 1, minValuePoint.Value, pointsInCheck, true, checkedPoints);
                }
            }

            if (minValuePoint.X + 1 < length)
            {
                SetValue(matrix, minValuePoint.X + 1, minValuePoint.Y, minValuePoint.Value, pointsInCheck, false, checkedPoints);
            }

            if (minValuePoint.Y + 1 < height)
            {
                if (minValuePoint.X - 1 >= 0)
                {
                    SetValue(matrix, minValuePoint.X - 1, minValuePoint.Y + 1, minValuePoint.Value, pointsInCheck, true, checkedPoints);
                }

                SetValue(matrix, minValuePoint.X, minValuePoint.Y + 1, minValuePoint.Value, pointsInCheck, false, checkedPoints);

                if (minValuePoint.X + 1 < length)
                {
                    SetValue(matrix, minValuePoint.X + 1, minValuePoint.Y + 1, minValuePoint.Value, pointsInCheck, true, checkedPoints);
                }
            }

            if (minValuePoint.X - 1 >= 0)
            {
                SetValue(matrix, minValuePoint.X - 1, minValuePoint.Y, minValuePoint.Value, pointsInCheck, false, checkedPoints);
            }

            checkedPoints.Add(minValuePoint);
            pointsInCheck.Remove(minValuePoint);
        }

        int xPos = start.X;
        int yPos = start.Y;

        List<Point2D> path = new List<Point2D>();
        Point2D minPoint = matrix[0, 0];

        while (!(xPos == finish.X && yPos == finish.Y))
        {
            if (yPos - 1 >= 0)
            {
                if (xPos - 1 >= 0 && matrix[yPos - 1, xPos - 1].Value < minPoint.Value)
                {
                    minPoint = matrix[yPos - 1, xPos - 1];
                }

                if (matrix[yPos - 1, xPos].Value < minPoint.Value)
                {
                    minPoint = matrix[yPos - 1, xPos];
                }

                if (xPos + 1 < length && matrix[yPos - 1, xPos + 1].Value < minPoint.Value)
                {
                    minPoint = matrix[yPos - 1, xPos + 1];
                }
            }

            if (xPos + 1 < length && matrix[yPos, xPos + 1].Value < minPoint.Value)
            {
                minPoint = matrix[yPos, xPos + 1];
            }

            if (yPos + 1 < height)
            {
                if (xPos - 1 >= 0 && matrix[yPos + 1, xPos - 1].Value < minPoint.Value)
                {
                    minPoint = matrix[yPos + 1, xPos - 1];
                }

                if (matrix[yPos + 1, xPos].Value < minPoint.Value)
                {
                    minPoint = matrix[yPos + 1, xPos];
                }

                if (xPos + 1 < length && matrix[yPos + 1, xPos + 1].Value < minPoint.Value)
                {
                    minPoint = matrix[yPos + 1, xPos + 1];
                }
            }

            if (xPos - 1 >= 0 && matrix[yPos, xPos - 1].Value < minPoint.Value)
            {
                minPoint = matrix[yPos, xPos - 1];
            }

            xPos = minPoint.X;
            yPos = minPoint.Y;
            path.Add(minPoint);
        }

        return path;
    }

    static void PrintGameField(bool[,] grid)
    {
        int height = grid.GetLength(0);
        int length = grid.GetLength(1);

        for (int i = 0; i < length + 2; i++)
        {
            Console.Write("▓");
        }

        Console.WriteLine();

        for (int i = 0; i < height; i++)
        {
            Console.Write("▓");

            for (int j = 0; j < length; j++)
            {
                if (i == 0 && j == 0)
                {
                    Console.Write("o");
                    continue;
                }

                if (i == height - 1 && j == length - 1)
                {
                    Console.Write("x");
                    continue;
                }

                if (grid[i, j])
                {
                    Console.Write("▓");
                }
                else
                {
                    Console.Write(" ");
                }
            }

            Console.WriteLine("▓");
        }

        for (int i = 0; i < length + 2; i++)
        {
            Console.Write("▓");
        }

        Console.WriteLine();
    }

    static void PathGo(List<Point2D> path)
    {
        Console.CursorTop = 1;
        Console.CursorLeft = 1;

        foreach (var p in path)
        {
            Thread.Sleep(500);

            if (p.X == 0 && p.Y == 0)
            {
                continue;
            }

            Console.Write(" ");
            Console.CursorTop = p.Y + 1;
            Console.CursorLeft = p.X + 1;
            Console.Write("o");
            Console.CursorLeft--;

            Thread.Sleep(500);
        }

        Console.CursorTop = 20;
        Console.CursorLeft = 0;
    }

    static void Main(string[] args)
    {
        Point2D start = new Point2D() { X = 0, Y = 0 };
        Point2D finish = new Point2D() { X = 5, Y = 4 };

        string[] gamefield =
        {
            " ▓▓   ",
            " ▓ ▓▓ ",
            " ▓▓ ▓ ",
            " ▓ ▓▓ ",
            "   ▓  "
        };

        bool[,] grid = GetBoolGrid(gamefield, gamefield.Count(), gamefield[0].Length);
        List<Point2D> path = FindPath(grid, start, finish);
        PrintGameField(grid);
        PathGo(path);
    }
}