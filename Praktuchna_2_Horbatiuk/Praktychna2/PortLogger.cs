using System;
using System.IO;
using System.Text;

namespace Praktychna2;

// Реалізовано в гілці feature/stringbuilder-logger
// Клас PortLogger з використанням StringBuilder

public class PortLogger
{
  
    private readonly StringBuilder _log = new();


    public int EntryCount { get; private set; }


    public void LogOperation(string operation, int portNumber, string details)
    {
        if (string.IsNullOrWhiteSpace(operation))
            throw new ArgumentException("Назва операції не може бути порожньою.");

        _log.Append('[')
            .Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            .Append("] ")
            .Append(operation.ToUpper())
            .Append(" | порт #")
            .Append(portNumber)
            .Append(" | ")
            .AppendLine(details);

        EntryCount++;
    }

 
    public void LogInfo(string message)
    {
        _log.Append('[')
            .Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            .Append("] INFO | ")
            .AppendLine(message);
        EntryCount++;
    }

   
    public string GetFullLog() => _log.ToString();


    public void SaveLogToFile(string filename)
    {
        File.WriteAllText(filename, _log.ToString());
    }


    public void Clear()
    {
        _log.Clear();
        EntryCount = 0;
    }


    public string GenerateBigReport(int recordsCount = 100)
    {
        var report = new StringBuilder();
        report.AppendLine("═══════════════════════════════════════════════════════════");
        report.AppendLine($"  ЗВІТ ПО ОПЕРАЦІЯМ З ПОРТАМИ — {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
        report.AppendLine($"  Загалом подій: {recordsCount}");
        report.AppendLine("═══════════════════════════════════════════════════════════");
        report.AppendLine();

        var rnd = new Random(42); 
        string[] operations = { "OPEN", "WRITE", "READ", "CLOSE", "CLEAR" };
        for (int i = 1; i <= recordsCount; i++)
        {
            int portNum = rnd.Next(0, 256);
            string op = operations[rnd.Next(operations.Length)];
            int bytes = rnd.Next(1, 65);

            report.Append("Запис ").Append(i).Append(": ")
                  .Append(DateTime.Now.AddSeconds(-i).ToString("HH:mm:ss"))
                  .Append(" | ").Append(op)
                  .Append(" | порт #").Append(portNum)
                  .Append(" | байт: ").AppendLine(bytes.ToString());
        }

        report.AppendLine();
        report.AppendLine("═══════════════════════════════════════════════════════════");
        report.AppendLine("  Кінець звіту");
        report.AppendLine("═══════════════════════════════════════════════════════════");

        return report.ToString();
    }
}
