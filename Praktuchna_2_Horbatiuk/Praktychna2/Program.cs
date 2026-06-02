using System;
using System.Linq;
using System.Text;

namespace Praktychna2;

internal class Program
{
    private static StudentGroup _group = new()
    {
        GroupName = "К-320",
        Specialization = "Комп'ютерна інженерія",
        Course = 3
    };

    private static PortMatrix _matrix = new();
    private static PortLogger _logger = new();
    private static bool _matrixInitialized = false;

    private const string DataFile = "group.json";
    private const string LogFile = "ports.log";

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        SeedDemoData();

        while (true)
        {
            ShowMenu();
            string? choice = Console.ReadLine();
            try
            {
                switch (choice)
                {
                    case "1": AddStudent(); break;
                    case "2": RemoveStudent(); break;
                    case "3": ShowStudentsWithLabs(); break;
                    case "4": InitializeMatrix(); break;
                    case "5": OpenOrClosePort(); break;
                    case "6": WriteToPort(); break;
                    case "7": ReadFromPort(); break;
                    case "8": ShowMatrixState(); break;
                    case "9": AssignStudentToPort(); break;
                    case "10": SimulateLab(); break;
                    case "11": ShowLog(); break;
                    case "12": Search(); break;
                    case "13": SaveOrLoad(); break;
                    case "14": ShowStatistics(); break;
                    case "15": GenerateBigReport(); break;
                    case "0": return;
                    default: Console.WriteLine("Невірний вибір."); break;
                }
            }
            catch (PortException ex)
            {
                Console.WriteLine($"Помилка порту: {ex.Message}");
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine($"Помилка індексу: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }

            Console.WriteLine();
            Console.WriteLine("Натисніть будь-яку клавішу для продовження...");
            Console.ReadKey();
        }
    }

    static void ShowMenu()
    {
        Console.Clear();
        Console.WriteLine($"Група: {_group.GroupName} | Студентів: {_group.GroupSize} | Матриця: {(_matrixInitialized ? "ініціалізована" : "не ініціалізована")}");
        Console.WriteLine();
        Console.WriteLine("1.  Додати студента");
        Console.WriteLine("2.  Видалити студента");
        Console.WriteLine("3.  Вивести студентів з лабораторними");
        Console.WriteLine("4.  Ініціалізувати матрицю портів 16x16");
        Console.WriteLine("5.  Відкрити / закрити порт");
        Console.WriteLine("6.  Записати дані в порт");
        Console.WriteLine("7.  Прочитати дані з порту");
        Console.WriteLine("8.  Вивести стан матриці портів");
        Console.WriteLine("9.  Прив'язати студента до порту");
        Console.WriteLine("10. Симулювати лабораторну роботу");
        Console.WriteLine("11. Переглянути лог портів");
        Console.WriteLine("12. Пошук порту / студента");
        Console.WriteLine("13. Зберегти / завантажити дані (JSON)");
        Console.WriteLine("14. Статистика групи і портів");
        Console.WriteLine("15. Згенерувати великий звіт (100+ записів)");
        Console.WriteLine("0.  Вийти");
        Console.WriteLine();
        Console.Write("Ваш вибір: ");
    }

    // ─────────────── 1. Додати студента ───────────────
    static void AddStudent()
    {
        Console.Write("ПІБ (мін. 5 символів): ");
        var name = Console.ReadLine() ?? string.Empty;

        Console.Write("Номер заліковки (8 цифр): ");
        var record = Console.ReadLine() ?? string.Empty;

        Console.Write("Дата народження (дд.мм.рррр): ");
        var dob = DateTime.Parse(Console.ReadLine() ?? string.Empty);

        Console.Write("Email (можна пропустити): ");
        var email = Console.ReadLine();

        var s = new Student
        {
            FullName = name,
            RecordBookNumber = record,
            DateOfBirth = dob,
            PersonalEmail = string.IsNullOrWhiteSpace(email) ? null : email
        };
        _group.AddStudent(s);
        Console.WriteLine($"Студента {s.FullName} додано.");
    }

    // ─────────────── 2. Видалити ───────────────
    static void RemoveStudent()
    {
        Console.Write("Номер заліковки: ");
        var id = Console.ReadLine() ?? string.Empty;
        Console.WriteLine(_group.RemoveStudent(id) ? "Видалено." : "Не знайдено.");
    }

    // ─────────────── 3. Студенти з лабораторними ───────────────
    static void ShowStudentsWithLabs()
    {
        var all = _group.GetAllStudents();
        if (all.Count == 0) { Console.WriteLine("Група порожня."); return; }

        var sb = new StringBuilder();
        foreach (var s in all)
        {
            sb.AppendLine();
            sb.Append("[").Append(s.RecordBookNumber).Append("] ").AppendLine(s.FullName);
            sb.Append("  Лабораторні: [");
            for (int i = 0; i < s.LabGrades.Length; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append(s.LabGrades[i] == 0 ? "—" : s.LabGrades[i].ToString());
            }
            sb.Append("]  виконано: ").Append(s.CompletedLabsCount).Append("/10");
            if (s.CompletedLabsCount > 0)
            {
                sb.Append("  середній: ").Append(s.GetAverageLabGrade().ToString("F2"));
            }
            sb.AppendLine();
            // Сортована версія
            var sorted = s.GetSortedLabGrades();
            sb.Append("  Сортовано:   [");
            for (int i = 0; i < sorted.Length; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append(sorted[i] == 0 ? "—" : sorted[i].ToString());
            }
            sb.AppendLine("]");
        }
        Console.Write(sb.ToString());
    }

    // ─────────────── 4. Ініціалізувати матрицю ───────────────
    static void InitializeMatrix()
    {
        _matrix.Initialize();
        _matrixInitialized = true;
        _logger.LogInfo("Матрицю портів 16x16 ініціалізовано.");
        Console.WriteLine("Матрицю портів 16x16 ініціалізовано.");
        Console.WriteLine($"Створено {PortMatrix.Rows * PortMatrix.Cols} портів.");
    }

    // ─────────────── 5. Відкрити / закрити порт ───────────────
    static void OpenOrClosePort()
    {
        if (!_matrixInitialized) { Console.WriteLine("Спочатку ініціалізуйте матрицю (пункт 4)."); return; }

        Console.Write("Рядок (0-15): ");
        int row = int.Parse(Console.ReadLine() ?? "0");
        Console.Write("Колонка (0-15): ");
        int col = int.Parse(Console.ReadLine() ?? "0");

        var port = _matrix.GetPort(row, col);
        if (port.IsOpen)
        {
            _matrix.ClosePort(row, col);
            _logger.LogOperation("CLOSE", port.PortNumber, $"координати ({row}, {col})");
            Console.WriteLine($"Порт ({row}, {col}) закрито.");
        }
        else
        {
            _matrix.OpenPort(row, col);
            _logger.LogOperation("OPEN", port.PortNumber, $"координати ({row}, {col})");
            Console.WriteLine($"Порт ({row}, {col}) відкрито.");
        }
    }

    // ─────────────── 6. Записати в порт ───────────────
    static void WriteToPort()
    {
        if (!_matrixInitialized) { Console.WriteLine("Спочатку ініціалізуйте матрицю."); return; }

        Console.Write("Рядок (0-15): ");
        int row = int.Parse(Console.ReadLine() ?? "0");
        Console.Write("Колонка (0-15): ");
        int col = int.Parse(Console.ReadLine() ?? "0");
        Console.Write("Текст для запису: ");
        string text = Console.ReadLine() ?? string.Empty;

        var data = Encoding.UTF8.GetBytes(text);
        if (data.Length > 64)
        {
            Console.WriteLine($"Дані ({data.Length} байт) перевищують 64 байти. Скорочую.");
            Array.Resize(ref data, 64);
        }

        _matrix.WriteToPort(row, col, data);
        _logger.LogOperation("WRITE", _matrix.GetPort(row, col).PortNumber,
            $"({row}, {col}), {data.Length} байт");
        Console.WriteLine($"Записано {data.Length} байт у порт.");
    }

    // ─────────────── 7. Прочитати з порту ───────────────
    static void ReadFromPort()
    {
        if (!_matrixInitialized) { Console.WriteLine("Спочатку ініціалізуйте матрицю."); return; }

        Console.Write("Рядок (0-15): ");
        int row = int.Parse(Console.ReadLine() ?? "0");
        Console.Write("Колонка (0-15): ");
        int col = int.Parse(Console.ReadLine() ?? "0");

        var data = _matrix.ReadFromPort(row, col);
        _logger.LogOperation("READ", _matrix.GetPort(row, col).PortNumber,
            $"({row}, {col}), {data.Length} байт");

        if (data.Length == 0)
        {
            Console.WriteLine("Буфер порту порожній.");
        }
        else
        {
            Console.WriteLine($"Прочитано {data.Length} байт:");
            Console.WriteLine($"  Як текст: {Encoding.UTF8.GetString(data)}");
            Console.WriteLine($"  Як байти: [{string.Join(", ", data)}]");
        }
    }

    // ─────────────── 8. Вивести стан матриці ───────────────
    static void ShowMatrixState()
    {
        if (!_matrixInitialized) { Console.WriteLine("Спочатку ініціалізуйте матрицю."); return; }
        Console.Write(_matrix.GetMatrixView());
    }

    // ─────────────── 9. Прив'язати студента до порту ───────────────
    static void AssignStudentToPort()
    {
        if (!_matrixInitialized) { Console.WriteLine("Спочатку ініціалізуйте матрицю."); return; }

        Console.Write("Номер заліковки студента: ");
        var id = Console.ReadLine() ?? string.Empty;
        var s = _group.FindStudent(id);
        if (s == null) { Console.WriteLine("Студента не знайдено."); return; }

        Console.Write("Рядок порту (0-15): ");
        int row = int.Parse(Console.ReadLine() ?? "0");
        Console.Write("Колонка порту (0-15): ");
        int col = int.Parse(Console.ReadLine() ?? "0");

        _group.AssignStudentToPort(s, row, col);
        _logger.LogOperation("ASSIGN", _matrix.GetPort(row, col).PortNumber,
            $"студент={s.FullName}");
        Console.WriteLine($"Студента {s.FullName} прив'язано до порту ({row}, {col}).");
    }

    // ─────────────── 10. Симулювати лабораторну ───────────────
    static void SimulateLab()
    {
        if (!_matrixInitialized) { Console.WriteLine("Спочатку ініціалізуйте матрицю."); return; }

        Console.Write("Номер заліковки студента: ");
        var id = Console.ReadLine() ?? string.Empty;
        var s = _group.FindStudent(id);
        if (s == null) { Console.WriteLine("Не знайдено."); return; }
        if (!s.IsAssignedToPort)
        {
            Console.WriteLine("Студент не прив'язаний до порту. Спочатку прив'яжіть (пункт 9).");
            return;
        }

        Console.Write("Номер лабораторної (1-10): ");
        int lab = int.Parse(Console.ReadLine() ?? "0");
        Console.Write("Оцінка (0-100): ");
        byte grade = byte.Parse(Console.ReadLine() ?? "0");

        _group.SimulateLab(s, lab, grade, _matrix, _logger);
        Console.WriteLine($"Лабораторну {lab} зараховано для {s.FullName}: {grade}.");
        Console.WriteLine($"Середня лабораторна: {s.GetAverageLabGrade():F2}");
    }

    // ─────────────── 11. Переглянути лог ───────────────
    static void ShowLog()
    {
        var log = _logger.GetFullLog();
        if (string.IsNullOrEmpty(log))
        {
            Console.WriteLine("Лог порожній.");
            return;
        }
        Console.WriteLine($"Записів у лозі: {_logger.EntryCount}");
        Console.WriteLine();
        Console.WriteLine(log);

        Console.Write("Зберегти лог у файл? (y/n): ");
        if (Console.ReadLine()?.Trim().ToLower() == "y")
        {
            _logger.SaveLogToFile(LogFile);
            Console.WriteLine($"Лог збережено у {LogFile}");
        }
    }

    // ─────────────── 12. Пошук ───────────────
    static void Search()
    {
        Console.WriteLine("1 - пошук студента, 2 - пошук портів за типом пристрою");
        var k = Console.ReadLine();
        if (k == "1")
        {
            Console.Write("Частина ПІБ або номер заліковки: ");
            var q = Console.ReadLine() ?? string.Empty;
            // спочатку точне співпадіння за заліковкою
            var exact = _group.FindStudent(q);
            if (exact != null) { exact.ShowDetailedInfo(); return; }
            // інакше пошук за ПІБ
            var found = _group.FindStudent(q, byName: true);
            Console.WriteLine($"Знайдено: {found.Count}");
            foreach (var s in found) s.ShowDetailedInfo();
        }
        else if (k == "2")
        {
            if (!_matrixInitialized) { Console.WriteLine("Спочатку ініціалізуйте матрицю."); return; }
            Console.WriteLine("Типи: 1-Oscilloscope, 2-Multimeter, 3-SignalGenerator, 4-PowerSupply, 5-LogicAnalyzer");
            Console.Write("Виберіть: ");
            int t = int.Parse(Console.ReadLine() ?? "1");
            var type = (DeviceType)t;
            var found = _matrix.FindByDeviceType(type);
            Console.WriteLine($"Знайдено {found.Count} портів типу {type}:");
            foreach (var (r, c, port) in found.Take(20))
                Console.WriteLine($"  ({r}, {c}) {port.GetStatusInfo()}");
            if (found.Count > 20) Console.WriteLine($"... та ще {found.Count - 20}");
        }
    }

    // ─────────────── 13. Save / Load ───────────────
    static void SaveOrLoad()
    {
        Console.WriteLine("1 - зберегти, 2 - завантажити");
        var p = Console.ReadLine();
        if (p == "1")
        {
            _group.SaveToFile(DataFile);
            Console.WriteLine($"Збережено у {DataFile}");
        }
        else if (p == "2")
        {
            _group.LoadFromFile(DataFile);
            Console.WriteLine($"Завантажено. Студентів: {_group.GroupSize}");
        }
    }

    // ─────────────── 14. Статистика ───────────────
    static void ShowStatistics()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Статистика групи {_group.GroupName}:");
        sb.AppendLine($"  Студентів: {_group.GroupSize}");
        sb.AppendLine($"  Середній бал: {_group.AverageGroupGrade:F2}");
        sb.AppendLine($"  Відмінників: {_group.GetExcellentStudents().Count} ({_group.ExcellentPercent}%)");
        sb.AppendLine($"  З низьким балом: {_group.GetFailingStudents().Count}");

        // Середня по лабораторних
        var allStudents = _group.GetAllStudents();
        var avgLab = allStudents.Where(s => s.CompletedLabsCount > 0)
            .Select(s => s.GetAverageLabGrade()).DefaultIfEmpty(0).Average();
        sb.AppendLine($"  Середня лабораторна: {avgLab:F2}");
        sb.AppendLine($"  Прив'язано до портів: {allStudents.Count(s => s.IsAssignedToPort)}");

        if (_matrixInitialized)
        {
            sb.AppendLine();
            sb.AppendLine($"Статистика портів:");
            sb.AppendLine($"  Усього: {PortMatrix.Rows * PortMatrix.Cols}");
            sb.AppendLine($"  Відкритих: {_matrix.OpenPortsCount}");
            sb.AppendLine($"  Подій у лозі: {_logger.EntryCount}");
        }
        Console.Write(sb.ToString());
    }

    // ─────────────── 15. Згенерувати великий звіт ───────────────
    static void GenerateBigReport()
    {
        Console.Write("Кількість записів (за замовч. 100): ");
        var input = Console.ReadLine();
        int count = string.IsNullOrWhiteSpace(input) ? 100 : int.Parse(input);

        var report = _logger.GenerateBigReport(count);
        Console.WriteLine($"Згенеровано звіт ({report.Length} символів, {count} записів).");
        Console.WriteLine();

        // Покажемо лише перші 20 рядків, щоб не засипати консоль
        var lines = report.Split('\n');
        for (int i = 0; i < Math.Min(20, lines.Length); i++)
            Console.WriteLine(lines[i]);
        if (lines.Length > 20)
            Console.WriteLine($"... (показано перші 20 рядків з {lines.Length})");

        Console.Write("Зберегти повний звіт у big_report.txt? (y/n): ");
        if (Console.ReadLine()?.Trim().ToLower() == "y")
        {
            System.IO.File.WriteAllText("big_report.txt", report);
            Console.WriteLine("Звіт збережено.");
        }
    }

    // ─────────────── Демо-дані ───────────────
    static void SeedDemoData()
    {
        var s1 = new Student
        {
            FullName = "Іваненко Іван Іванович",
            RecordBookNumber = "12345678",
            DateOfBirth = new DateTime(2004, 5, 12),
            PersonalEmail = "ivan@example.com"
        };
        s1.Journal.SetGrade("Математика", 95);
        s1.Journal.SetGrade("ООП", 92);
        s1.AddLabGrade(1, 95);
        s1.AddLabGrade(2, 88);
        s1.AddLabGrade(3, 91);
        s1.RecalculateFromJournal();

        var s2 = new Student
        {
            FullName = "Петренко Марія Олександрівна",
            RecordBookNumber = "23456789",
            DateOfBirth = new DateTime(2005, 8, 20),
            PersonalEmail = "maria@example.com"
        };
        s2.Journal.SetGrade("Математика", 65);
        s2.Journal.SetGrade("ООП", 70);
        s2.AddLabGrade(1, 70);
        s2.AddLabGrade(2, 65);
        s2.RecalculateFromJournal();

        _group.AddStudent(s1);
        _group.AddStudent(s2);
    }
}
