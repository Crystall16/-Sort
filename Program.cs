using System.Diagnostics;

class Program
{
    static void Main()
    {
        const int arrayCount = 69;
        const int minLength = 100;
        const int maxLength = 100000;

        string inputFilePath = "arrays_input.txt";
        string outputFilePath = "sorting_results.txt";

        GenerateAndSaveArrays(arrayCount, minLength, maxLength, inputFilePath);
        (int, long, int)[] results = ProcessArraysForSorting(inputFilePath);
        SaveResultsToFile(results, outputFilePath);

        Console.WriteLine("Сортировка завершена. Результаты сохранены в файл.");
    }

    static void GenerateAndSaveArrays(int count, int minLen, int maxLen, string filePath)
    {
        Random random = new Random();
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            for (int i = 0; i < count; i++)
            {
                int length = minLen + (maxLen - minLen) * i / (count - 1);
                int[] array = new int[length];
                for (int j = 0; j < length; j++)
                    array[j] = random.Next(1000);

                writer.Write(length);
                for (int j = 0; j < length; j++)
                    writer.Write($",{array[j]}");

                writer.WriteLine();
            }
        }
    }

    static (int, long, int)[] ProcessArraysForSorting(string filePath)
    {
        (int, long, int)[] results = new (int, long, int)[69];
        string[] lines = File.ReadAllLines(filePath);

        for (int i = 0; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(',');
            int length = int.Parse(parts[0]);
            int[] array = new int[length];
            for (int j = 0; j < length; j++)
                array[j] = int.Parse(parts[j + 1]);

            Stopwatch stopwatch = Stopwatch.StartNew();
            int iterations = BucketSort(array);
            stopwatch.Stop();

            results[i] = (length, stopwatch.ElapsedTicks, iterations);
        }
        return results;
    }

    static int BucketSort(int[] array)
    {
        if (array.Length == 0) return 0;

        int minValue = array[0], maxValue = array[0];
        for (int i = 1; i < array.Length; i++)
        {
            if (array[i] < minValue) 
                minValue = array[i];
            if (array[i] > maxValue) 
                maxValue = array[i];
        }

        int bucketCount = (int)Math.Sqrt(array.Length);
        List<int>[] buckets = new List<int>[bucketCount];
        for (int i = 0; i < bucketCount; i++)
            buckets[i] = new List<int>();

        int iterations = 0;
        double range = (double)(maxValue - minValue) / bucketCount;

        for (int i = 0; i < array.Length; i++)
        {
            int value = array[i];
            int bucketIndex = value == maxValue ? bucketCount - 1 : (int)((value - minValue) / range);
            buckets[bucketIndex].Add(value);
            iterations++;
        }

        for (int i = 0; i < bucketCount; i++)
        {
            List<int> bucket = buckets[i];
            bucket.Sort();
            iterations += bucket.Count;
        }

        int index = 0;
        for (int i = 0; i < bucketCount; i++)
            for (int j = 0; j < buckets[i].Count; j++)
            {
                array[index++] = buckets[i][j];
                iterations++;
            }
        return iterations;
    }

    static void SaveResultsToFile((int, long, int)[] results, string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("Длина массива\tВремя (тики)\tКоличество итераций");
            for (int i = 0; i < results.Length; i++)
                writer.WriteLine($"{results[i].Item1}\t{results[i].Item2}\t{results[i].Item3}");
        }
    }
}
