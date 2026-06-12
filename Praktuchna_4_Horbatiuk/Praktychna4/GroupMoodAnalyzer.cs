using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Praktychna4;

// ВАРІАНТ 2: аналіз настрою групи за ключовими словами в нотатках студентів.
public class GroupMoodAnalyzer
{
    // Зберігаються корені слів, щоб ловити різні форми (активн -> активна, активний).
    private readonly List<string> _positiveKeywords = new()
    {
        "відмінно", "добре", "чудово", "успіх", "активн", "старанн",
        "молодець", "прогрес", "досягнен", "перемог", "талант", "вмотивован"
    };

    private readonly List<string> _negativeKeywords = new()
    {
        "погано", "проблема", "борг", "пропуск", "прогул", "відстає",
        "незадовільно", "конфлікт", "складно", "запізн", "недостатньо"
    };

    private readonly List<string> _neutralKeywords = new()
    {
        "відпустка", "академ", "переведен", "поновлен"
    };

    public record MoodResult(int Positive, int Negative, int Neutral)
    {
        public int Score => Positive - Negative;
    }

    private int CountKeywords(string lowerText, List<string> keywords)
    {
        int total = 0;
        foreach (string key in keywords)
        {
            int index = 0;
            while ((index = lowerText.IndexOf(key, index, StringComparison.Ordinal)) != -1)
            {
                total++;
                index += key.Length;
            }
        }
        return total;
    }

    public MoodResult AnalyzeText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new MoodResult(0, 0, 0);

        string lower = text.ToLower();
        int pos = CountKeywords(lower, _positiveKeywords);
        int neg = CountKeywords(lower, _negativeKeywords);
        int neu = CountKeywords(lower, _neutralKeywords);
        return new MoodResult(pos, neg, neu);
    }

    public string GetMoodLabel(int score)
    {
        if (score > 0) return "позитивний";
        if (score < 0) return "негативний";
        return "нейтральний";
    }

    public string AnalyzeGroup(StudentGroup group)
    {
        var sb = new StringBuilder();
        sb.AppendLine("====================================");
        sb.AppendLine($"  АНАЛІЗ НАСТРОЮ ГРУПИ {group.GroupName}");
        sb.AppendLine("====================================");

        var students = group.GetAllStudents();
        if (students.Count == 0)
        {
            sb.AppendLine("У групі немає студентів для аналізу.");
            return sb.ToString();
        }

        int totalPos = 0, totalNeg = 0, totalNeu = 0;
        int withNotes = 0;

        sb.AppendLine("Розбір по студентах:");
        foreach (var s in students)
        {
            if (string.IsNullOrWhiteSpace(s.Notes))
            {
                sb.AppendLine($"  {s.FullName}: нотаток немає");
                continue;
            }

            withNotes++;
            var r = AnalyzeText(s.Notes);
            totalPos += r.Positive;
            totalNeg += r.Negative;
            totalNeu += r.Neutral;

            sb.Append($"  {s.FullName}: ");
            sb.Append($"+{r.Positive} / -{r.Negative}");
            if (r.Neutral > 0) sb.Append($" / нейтр.{r.Neutral}");
            sb.Append(" -> ");
            sb.AppendLine(GetMoodLabel(r.Score));
        }

        int groupScore = totalPos - totalNeg;
        sb.AppendLine("------------------------------------");
        sb.AppendLine("Підсумок по групі:");
        sb.AppendLine($"  Студентів з нотатками: {withNotes} з {students.Count}");
        sb.AppendLine($"  Позитивних згадок: {totalPos}");
        sb.AppendLine($"  Негативних згадок: {totalNeg}");
        sb.AppendLine($"  Нейтральних згадок: {totalNeu}");
        sb.AppendLine($"  Загальний баланс: {groupScore}");
        sb.AppendLine($"  ЗАГАЛЬНИЙ НАСТРІЙ ГРУПИ: {GetMoodLabel(groupScore).ToUpper()}");
        sb.AppendLine("====================================");
        return sb.ToString();
    }

    public string GetTopKeywords(StudentGroup group)
    {
        var allText = new StringBuilder();
        foreach (var s in group.GetAllStudents())
            if (!string.IsNullOrWhiteSpace(s.Notes))
                allText.Append(s.Notes).Append(' ');

        string lower = allText.ToString().ToLower();
        if (lower.Length == 0) return "Нотаток для аналізу немає.";

        var counts = new Dictionary<string, int>();
        foreach (string key in _positiveKeywords.Concat(_negativeKeywords).Concat(_neutralKeywords))
        {
            int c = CountKeywords(lower, new List<string> { key });
            if (c > 0) counts[key] = c;
        }

        if (counts.Count == 0) return "Жодного ключового слова не знайдено.";

        var sb = new StringBuilder();
        sb.AppendLine("Найчастіші ключові слова в нотатках групи:");
        foreach (var kvp in counts.OrderByDescending(k => k.Value))
            sb.AppendLine($"  \"{kvp.Key}\" - {kvp.Value} раз(и)");
        return sb.ToString();
    }
}
