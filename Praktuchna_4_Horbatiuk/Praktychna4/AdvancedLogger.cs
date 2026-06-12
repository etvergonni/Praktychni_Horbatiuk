using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Praktychna4;

public class AdvancedLogger
{
    private record LogEntry(DateTime Time, string Level, string Message);

    private readonly List<LogEntry> _entries = new();

    public int Count => _entries.Count;

    public void Log(string level, string message)
    {
        if (string.IsNullOrWhiteSpace(level)) level = "INFO";
        if (string.IsNullOrWhiteSpace(message)) message = "(порожнє повідомлення)";
        _entries.Add(new LogEntry(DateTime.Now, level.Trim().ToUpper(), message.Trim()));
    }

    public void Info(string message) => Log("INFO", message);
    public void Success(string message) => Log("SUCCESS", message);
    public void Warning(string message) => Log("WARNING", message);
    public void Error(string message) => Log("ERROR", message);

    public string GetFullLog()
    {
        if (_entries.Count == 0) return "Журнал порожній.";
        var sb = new StringBuilder();
        foreach (var e in _entries)
            sb.AppendLine(FormatEntry(e));
        return sb.ToString();
    }

    public string GetLogsByLevel(string level)
    {
        string target = (level ?? "").Trim().ToUpper();
        var filtered = _entries.Where(e => e.Level == target).ToList();
        if (filtered.Count == 0) return $"Записів рівня {target} немає.";

        var sb = new StringBuilder();
        foreach (var e in filtered)
            sb.AppendLine(FormatEntry(e));
        return sb.ToString();
    }

    public string GetLast(int count)
    {
        if (count < 1) return "Кількість має бути більше 0.";
        if (_entries.Count == 0) return "Журнал порожній.";

        var last = _entries.Skip(Math.Max(0, _entries.Count - count));
        var sb = new StringBuilder();
        foreach (var e in last)
            sb.AppendLine(FormatEntry(e));
        return sb.ToString();
    }

    public string GetStatistics()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Усього записів: {_entries.Count}");
        foreach (var grp in _entries.GroupBy(e => e.Level))
            sb.AppendLine($"  {grp.Key}: {grp.Count()}");
        return sb.ToString();
    }

    public void SaveToFile(string path)
    {
        File.WriteAllText(path, GetFullLog());
    }

    public void Clear() => _entries.Clear();

    private static string FormatEntry(LogEntry e)
    {
        return $"[{e.Time:HH:mm:ss}] [{e.Level}] {e.Message}";
    }
}
