using System;
using System.Diagnostics;
using System.Threading;

class Program
{
    class MetaInfoForSortArray
    {
        public int[,] Matrix { get; private set; }
        public int N { get; private set; }
        public int M { get; private set; }
        public int FirstRow { get; private set; }
        public int LastRow { get; private set; }

        public MetaInfoForSortArray(int[,] matrix, int n, int m, int firstRow, int lastRow)
        {
            Matrix = matrix;
            N = n;
            M = m;
            FirstRow = firstRow;
            LastRow = lastRow;
        }
    }

    static void SortArray(object obj)
    {
        MetaInfoForSortArray sortInfo = (MetaInfoForSortArray)obj;

        Console.WriteLine("Thread {0} is sorting from {1} to {2}", Thread.CurrentThread.Name, sortInfo.FirstRow, sortInfo.LastRow);

        int[] column = new int[sortInfo.M];
        for (int i = sortInfo.FirstRow; i < sortInfo.LastRow; i++)
        {
            for (int j = 0; j < sortInfo.M; j++)
            {
                column[j] = sortInfo.Matrix[i, j];
            }
            Array.Sort(column);
            for (int j = 0; j < sortInfo.M; j++)
            {
                sortInfo.Matrix[i, j] = column[j];
            }
        }
    }

    static void Main()
    {
        Console.WriteLine("Введите количество строк матрицы:");
        int n = int.Parse(Console.ReadLine());

        Console.WriteLine("Введите количество столбцов матрицы:");
        int m = int.Parse(Console.ReadLine());

        int[,] matrix = new int[n, m];

        Console.WriteLine("Введите количество потоков для сортировки:");
        int threadCount = int.Parse(Console.ReadLine());

        int rowsPerThread = n / threadCount;
        Thread[] threads = new Thread[threadCount];

        Random random = new Random();
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                matrix[i, j] = random.Next(100);
            }
        }

        Console.WriteLine("Исходная матрица:");
        PrintMatrix(matrix);

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        for (int i = 0; i < threadCount; ++i)
        {
            threads[i] = new Thread(SortArray);
            threads[i].Name = $"Thread {i}";
            int startRow = i * rowsPerThread;
            int endRow = (i == threadCount - 1) ? n : startRow + rowsPerThread;
            threads[i].Start(new MetaInfoForSortArray(matrix, n, m, startRow, endRow));
        }

        for (int i = 0; i < threadCount; ++i)
        {
            threads[i].Join();
        }

        stopwatch.Stop();

        Console.WriteLine("Отсортированная матрица:");
        PrintMatrix(matrix);

        Console.WriteLine($"Время, потраченное на сортировку: {stopwatch.Elapsed}");

        Console.ReadKey();
    }

    static void PrintMatrix(int[,] matrix)
    {
        int n = matrix.GetLength(0);
        int m = matrix.GetLength(1);

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                Console.Write(matrix[i, j] + " ");
            }
            Console.WriteLine();
        }
    }
}