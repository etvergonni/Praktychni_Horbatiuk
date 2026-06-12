using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Praktychna4;

public static partial class TextProcessor
{
    public static string Reverse(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        char[] chars = input.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }

    public static int CountWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return 0;
        return text
            .Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .Length;
    }

    public static int CountCharacters(string text, bool ignoreWhitespace = true)
    {
        if (string.IsNullOrEmpty(text)) return 0;
        if (!ignoreWhitespace) return text.Length;
        return text.Count(c => !char.IsWhiteSpace(c));
    }

    public static string Normalize(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;
        string[] words = text.Split(
            new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        return string.Join(" ", words);
    }

    public static bool IsPalindrome(string text, bool ignoreCase = true, bool ignoreSpaces = true)
    {
        if (string.IsNullOrWhiteSpace(text)) return false;

        var sb = new StringBuilder();
        foreach (char c in text)
        {
            if (ignoreSpaces && char.IsWhiteSpace(c)) continue;
            sb.Append(ignoreCase ? char.ToLower(c) : c);
        }

        string s = sb.ToString();
        if (s.Length == 0) return false;

        int i = 0, j = s.Length - 1;
        while (i < j)
        {
            if (s[i] != s[j]) return false;
            i++;
            j--;
        }
        return true;
    }

    public static string ReplaceMultiple(string text, Dictionary<string, string> replacements)
    {
        if (string.IsNullOrEmpty(text) || replacements == null || replacements.Count == 0)
            return text;

        var sb = new StringBuilder(text);
        foreach (var kvp in replacements)
            sb.Replace(kvp.Key, kvp.Value);
        return sb.ToString();
    }

    public static string[] SplitIntoSentences(string text)
    {
        var result = new List<string>();
        if (string.IsNullOrWhiteSpace(text)) return result.ToArray();

        var sentence = new StringBuilder();
        foreach (char c in text)
        {
            sentence.Append(c);
            if (c == '.' || c == '!' || c == '?')
            {
                string s = sentence.ToString().Trim();
                if (s.Length > 0) result.Add(s);
                sentence.Clear();
            }
        }

        string last = sentence.ToString().Trim();
        if (last.Length > 0) result.Add(last);
        return result.ToArray();
    }

    public static string BuildGroupReport(StudentGroup group)
    {
        var sb = new StringBuilder();
        sb.AppendLine("====================================");
        sb.AppendLine($"  ЗВІТ ПО ГРУПІ {group.GroupName}");
        sb.AppendLine("====================================");
        sb.AppendLine($"Спеціальність: {group.Specialization}");
        sb.AppendLine($"Курс: {group.Course}");
        sb.AppendLine($"Кількість студентів: {group.GroupSize}");
        sb.AppendLine($"Середній бал групи: {group.AverageGroupGrade:F2}");
        sb.AppendLine($"Відмінників: {group.GetExcellentStudents().Count} ({group.ExcellentPercent}%)");
        sb.AppendLine("------------------------------------");
        sb.AppendLine("Список студентів:");

        int index = 1;
        foreach (var s in group.GetAllStudents())
        {
            sb.Append(index++);
            sb.Append(". ");
            sb.Append(s.FullName);
            sb.Append(" - середній бал: ");
            sb.Append(s.AverageGrade.ToString("F2"));
            sb.Append(", статус: ");
            sb.AppendLine(s.Status.ToString());
        }

        sb.AppendLine("====================================");
        sb.Append("Звіт згенеровано: ");
        sb.AppendLine(DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
        return sb.ToString();
    }
}
