using System;
using System.Diagnostics;
using System.Text;

namespace Praktychna4;

public static partial class TextProcessor
{
    // Порівняння двох способів складання рядка: повільне += та швидкий StringBuilder.
    public static string ComparePerformance(int iterations = 10000)
    {
        if (iterations < 1) iterations = 1;

        var sw1 = Stopwatch.StartNew();
        string slow = string.Empty;
        for (int i = 0; i < iterations; i++)
            slow += i + ";";
        sw1.Stop();

        var sw2 = Stopwatch.StartNew();
        var sb = new StringBuilder();
        for (int i = 0; i < iterations; i++)
            sb.Append(i).Append(';');
        string fast = sb.ToString();
        sw2.Stop();

        var report = new StringBuilder();
        report.AppendLine($"Порівняння продуктивності ({iterations} ітерацій):");
        report.AppendLine($"  string +=     : {sw1.ElapsedMilliseconds} мс");
        report.AppendLine($"  StringBuilder : {sw2.ElapsedMilliseconds} мс");

        if (sw2.ElapsedMilliseconds > 0)
        {
            long ratio = sw1.ElapsedMilliseconds / sw2.ElapsedMilliseconds;
            if (ratio > 1)
                report.AppendLine($"  StringBuilder швидший приблизно у {ratio} раз(и).");
            else
                report.AppendLine("  StringBuilder ефективніший (помітніше на великих обсягах).");
        }
        else
        {
            report.AppendLine("  StringBuilder настільки швидкий, що час майже нульовий.");
        }

        return report.ToString();
    }
}
