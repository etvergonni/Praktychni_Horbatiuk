using System;
using System.Linq;
using System.Text;

namespace Praktychna1;

internal class Program
{
    private static StudentGroup _group = new()
    {
        GroupName = "К-320",
        Specialization = "Комп'ютерна інженерія",
        Course = 3
    };

    private const string DataFile = "group.json";

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
                    case "3": ShowAllStudents(); break;
                    case "4": SearchStudent(); break;
                    case "5": EditStudent(); break;
                    case "6": ShowExcellentAndFailing(); break;
                    case "7": ShowStatistics(); break;
                    case "8": SaveOrLoad(); break;
                    case "9": AddGrade(); break;
                    case "0": return;
                    default: Console.WriteLine("Невірний вибір."); break;
                }
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
        Console.WriteLine($"Група: {_group.GroupName} | {_group.Specialization} | Курс {_group.Course}");
        Console.WriteLine($"Студентів у групі: {_group.GroupSize}");
        Console.WriteLine();
        Console.WriteLine("1. Додати студента");
        Console.WriteLine("2. Видалити студента");
        Console.WriteLine("3. Вивести всіх (з пагінацією по 10)");
        Console.WriteLine("4. Пошук студента (ПІБ або заліковка)");
        Console.WriteLine("5. Редагувати дані студента");
        Console.WriteLine("6. Відмінники та ті, у кого менше 60");
        Console.WriteLine("7. Статистика групи");
        Console.WriteLine("8. Зберегти / завантажити з файлу");
        Console.WriteLine("9. Додати оцінку за предмет");
        Console.WriteLine("0. Вийти");
        Console.WriteLine();
        Console.Write("Ваш вибір: ");
    }

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

        Console.Write("Нотатки (можна пропустити): ");
        var notes = Console.ReadLine();

        var student = new Student
        {
            FullName = name,
            RecordBookNumber = record,
            DateOfBirth = dob,
            PersonalEmail = string.IsNullOrWhiteSpace(email) ? null : email,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes
        };

        _group.AddStudent(student);
        Console.WriteLine($"Студента {student.FullName} додано.");
    }

    static void RemoveStudent()
    {
        Console.Write("Номер заліковки для видалення: ");
        var id = Console.ReadLine() ?? string.Empty;
        if (_group.RemoveStudent(id))
            Console.WriteLine("Студента видалено.");
        else
            Console.WriteLine("Студента з таким номером не знайдено.");
    }

    static void ShowAllStudents()
    {
        var all = _group.GetAllStudents();
        if (all.Count == 0)
        {
            Console.WriteLine("Група порожня.");
            return;
        }

        const int pageSize = 10;
        int totalPages = (all.Count + pageSize - 1) / pageSize;
        int page = 0;

        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Сторінка {page + 1} з {totalPages}");
            Console.WriteLine();
            var slice = all.Skip(page * pageSize).Take(pageSize);
            foreach (var s in slice) s.ShowDetailedInfo();

            if (totalPages == 1) break;
            Console.WriteLine();
            Console.WriteLine("N - наступна, P - попередня, Q - вихід");
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.N && page < totalPages - 1) page++;
            else if (key == ConsoleKey.P && page > 0) page--;
            else if (key == ConsoleKey.Q) break;
        }
    }

    static void SearchStudent()
    {
        Console.WriteLine("1 - за ПІБ, 2 - за номером заліковки");
        Console.Write("Тип пошуку: ");
        var kind = Console.ReadLine();

        if (kind == "1")
        {
            Console.Write("Частина ПІБ: ");
            var name = Console.ReadLine() ?? string.Empty;
            var found = _group.FindStudent(name, byName: true);
            Console.WriteLine($"Знайдено {found.Count} студент(ів):");
            foreach (var s in found) s.ShowDetailedInfo();
        }
        else if (kind == "2")
        {
            Console.Write("Номер заліковки: ");
            var id = Console.ReadLine() ?? string.Empty;
            var s = _group.FindStudent(id);
            if (s != null) s.ShowDetailedInfo();
            else Console.WriteLine("Не знайдено.");
        }
    }

    static void EditStudent()
    {
        Console.Write("Номер заліковки студента: ");
        var id = Console.ReadLine() ?? string.Empty;
        var s = _group.FindStudent(id);
        if (s == null) { Console.WriteLine("Не знайдено."); return; }

        Console.WriteLine("Що змінити? 1-ПІБ, 2-Email, 3-Статус, 4-Нотатки");
        var pick = Console.ReadLine();

        switch (pick)
        {
            case "1":
                Console.Write("Новий ПІБ: ");
                s.FullName = Console.ReadLine() ?? string.Empty;
                break;
            case "2":
                Console.Write("Новий email: ");
                s.PersonalEmail = Console.ReadLine();
                break;
            case "3":
                Console.WriteLine("Статус: 0-Active, 1-AcademicLeave, 2-Expelled, 3-Graduated");
                if (int.TryParse(Console.ReadLine(), out int n) && Enum.IsDefined(typeof(StudentStatus), n))
                    s.Status = (StudentStatus)n;
                break;
            case "4":
                Console.Write("Нові нотатки: ");
                s.Notes = Console.ReadLine();
                break;
        }
        Console.WriteLine("Дані оновлено.");
    }

    static void ShowExcellentAndFailing()
    {
        var excellent = _group.GetExcellentStudents();
        var failing = _group.GetFailingStudents();

        Console.WriteLine($"Відмінники ({excellent.Count}):");
        foreach (var s in excellent)
            Console.WriteLine($"  {s.FullName} - {s.AverageGrade:F2}");

        Console.WriteLine();
        Console.WriteLine($"З низьким балом ({failing.Count}):");
        foreach (var s in failing)
            Console.WriteLine($"  {s.FullName} - {s.AverageGrade:F2}");
    }

    static void ShowStatistics()
    {
        Console.WriteLine($"Статистика групи {_group.GroupName}:");
        Console.WriteLine($"Усього студентів: {_group.GroupSize}");
        Console.WriteLine($"Середній бал: {_group.AverageGroupGrade:F2}");
        Console.WriteLine($"Відмінників: {_group.GetExcellentStudents().Count} ({_group.ExcellentPercent}%)");
        Console.WriteLine($"З низьким балом: {_group.GetFailingStudents().Count}");
        Console.WriteLine();
        Console.WriteLine("За статусами:");
        foreach (StudentStatus status in Enum.GetValues<StudentStatus>())
            Console.WriteLine($"  {status}: {_group.GetStudentsByStatus(status).Count}");
    }

    static void SaveOrLoad()
    {
        Console.WriteLine("1 - зберегти, 2 - завантажити");
        var pick = Console.ReadLine();
        if (pick == "1")
        {
            _group.SaveToFile(DataFile);
            Console.WriteLine($"Збережено у файл {DataFile}");
        }
        else if (pick == "2")
        {
            _group.LoadFromFile(DataFile);
            Console.WriteLine($"Завантажено з {DataFile}. Студентів: {_group.GroupSize}");
        }
    }

    static void AddGrade()
    {
        Console.Write("Номер заліковки: ");
        var id = Console.ReadLine() ?? string.Empty;
        var s = _group.FindStudent(id);
        if (s == null) { Console.WriteLine("Не знайдено."); return; }

        Console.Write("Предмет: ");
        var subject = Console.ReadLine() ?? string.Empty;
        Console.Write("Оцінка (0-100): ");
        if (!double.TryParse(Console.ReadLine(), out double grade))
        {
            Console.WriteLine("Невірний формат оцінки.");
            return;
        }

        s.Journal.SetGrade(subject, grade);
        s.RecalculateFromJournal();
        Console.WriteLine($"Оцінку додано. Новий середній бал: {s.AverageGrade:F2}");
    }

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
        s1.RecalculateFromJournal();

        var s2 = new Student
        {
            FullName = "Петренко Марія Олександрівна",
            RecordBookNumber = "23456789",
            DateOfBirth = new DateTime(2005, 8, 20),
            PersonalEmail = "maria@example.com"
        };
        s2.Journal.SetGrade("Математика", 55);
        s2.Journal.SetGrade("ООП", 50);
        s2.RecalculateFromJournal();

        _group.AddStudent(s1);
        _group.AddStudent(s2);
    }
}
