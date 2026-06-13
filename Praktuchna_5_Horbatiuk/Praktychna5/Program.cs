using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Praktychna5;

Console.OutputEncoding = Encoding.UTF8;

var group = new StudentGroup
{
    GroupName = "К-320",
    Specialization = "Комп'ютерна інженерія",
    Course = 3
};
var logger = new AdvancedLogger();
var mood = new GroupMoodAnalyzer();

SeedDemoData();
logger.Info("Програму запущено");

bool running = true;
while (running)
{
    ShowMenu();
    Console.Write("Ваш вибір: ");
    string choice = (Console.ReadLine() ?? "").Trim();
    Console.WriteLine();

    switch (choice)
    {
        case "1": AddStudent(); break;
        case "2": RemoveStudent(); break;
        case "3": ShowAllStudents(); break;
        case "4": FindStudent(); break;
        case "5": EditStudent(); break;
        case "6": ShowExcellentAndFailing(); break;
        case "7": ShowStatistics(); break;
        case "8": SaveGroup(); break;
        case "9": LoadGroup(); break;
        case "10": SearchByFragment(); break;
        case "11": ShowGroupReport(); break;
        case "12": NormalizeAllNotes(); break;
        case "13": CheckPalindromesInNotes(); break;
        case "14": ExportCsv(); break;
        case "15": ImportFromText(); break;
        case "16": ShowLogs(); break;
        case "17": ShowPerformance(); break;
        case "18": TextProcessingMenu(); break;
        case "19": CompareTwoStudents(); break;
        case "20": MergeGroupsDemo(); break;
        case "21": DemoVector(); break;
        case "22": DemoGradePoint(); break;
        case "23": FindBest(); break;
        case "24": TestOperators(); break;
        case "25": AddRegularStudent(); break;
        case "26": AddSpecialStudent(); break;
        case "27": ShowAllMembers(); break;
        case "28": CalculateScholarships(); break;
        case "29": ShowByType(); break;
        case "30": TestHierarchy(); break;
        case "31": DemoVehicles(); break;
        case "0":
            running = false;
            logger.Info("Вихід із програми");
            Console.WriteLine("До побачення!");
            break;
        default:
            Console.WriteLine("Невідома команда, спробуйте ще раз.");
            break;
    }

    if (running)
    {
        Console.WriteLine("\nНатисніть Enter, щоб продовжити...");
        Console.ReadLine();
        Console.Clear();
    }
}

void ShowMenu()
{
    Console.WriteLine("===== СИСТЕМА УПРАВЛІННЯ ГРУПОЮ (ПР №5) =====");
    Console.WriteLine(" 1. Додати студента        2. Видалити студента");
    Console.WriteLine(" 3. Вивести всіх студентів 4. Пошук студента");
    Console.WriteLine(" 5. Редагувати студента    6. Відмінники / відстаючі");
    Console.WriteLine(" 7. Статистика групи       8. Зберегти дані");
    Console.WriteLine(" 9. Завантажити дані      10. Пошук за фрагментом ПІБ");
    Console.WriteLine("11. Звіт групи            12. Нормалізувати нотатки");
    Console.WriteLine("13. Паліндроми в нотатках 14. Експорт у CSV");
    Console.WriteLine("15. Імпорт студентів      16. Логи системи");
    Console.WriteLine("17. Тест продуктивності   18. Обробка тексту та аналіз");
    Console.WriteLine("-- Оператори (ПР №4) --");
    Console.WriteLine("19. Порівняти студентів   20. Об'єднати групи");
    Console.WriteLine("21. Демо Vector           22. Демо GradePoint");
    Console.WriteLine("23. Найкращий студент     24. Тест операторів");
    Console.WriteLine("-- Ієрархія та наслідування (ПР №5) --");
    Console.WriteLine("25. Додати звичайного студента");
    Console.WriteLine("26. Додати відмінника / іноземного / працюючого / аспіранта");
    Console.WriteLine("27. Вивести всіх членів університету (поліморфізм)");
    Console.WriteLine("28. Розрахувати стипендію для всіх");
    Console.WriteLine("29. Показати студентів за типом");
    Console.WriteLine("30. Тестування ієрархії та base/override");
    Console.WriteLine("31. Демонстрація Vehicle (Варіант 2)");
    Console.WriteLine(" 0. Вийти");
    Console.WriteLine("============================================");
}

void SeedDemoData()
{
    var s1 = new Student("Горбатюк Олександра Іванівна", new DateTime(2005, 5, 12), "12345678")
    {
        Notes = "Активна студентка, відмінно показує себе, помітний прогрес.",
        CourseProgress = 90
    };
    s1.Journal.SetGrade("Програмування", 95);
    s1.Journal.SetGrade("Математика", 88);
    s1.RecalculateFromJournal();
    s1.GradePoints.Add(new GradePoint(9));
    s1.GradePoints.Add(new GradePoint(8.5));
    group.AddStudent(s1);

    var s2 = new Student("Петренко Іван Сергійович", new DateTime(2004, 9, 3), "87654321")
    {
        Notes = "Є проблема з відвідуванням, накопичив борг, відстає.",
        CourseProgress = 45
    };
    s2.Journal.SetGrade("Програмування", 60);
    s2.Journal.SetGrade("Математика", 55);
    s2.RecalculateFromJournal();
    group.AddStudent(s2);

    var s3 = new Student("Коваленко Марія Олегівна", new DateTime(2005, 1, 20), "11223344")
    {
        Notes = "Старанна, чудові результати, молодець.",
        CourseProgress = 80
    };
    s3.Journal.SetGrade("Програмування", 90);
    s3.Journal.SetGrade("Математика", 92);
    s3.RecalculateFromJournal();
    group.AddStudent(s3);

    var ex = new ExcellentStudent("Шевченко Тарас Григорович", new DateTime(2005, 3, 9), "10101010", "Переможець олімпіади")
    {
        CourseProgress = 95
    };
    ex.Journal.SetGrade("Програмування", 98);
    ex.Journal.SetGrade("Математика", 96);
    ex.RecalculateFromJournal();
    group.AddMember(ex);

    var fs = new ForeignStudent("Сміт Джон Майкл", new DateTime(2004, 11, 15), "20202020", "США")
    {
        CourseProgress = 70
    };
    fs.Journal.SetGrade("Програмування", 80);
    fs.RecalculateFromJournal();
    group.AddMember(fs);

    var ws = new WorkingStudent("Бондаренко Олег Петрович", new DateTime(2003, 6, 1), "30303030", "SoftServe", 20)
    {
        CourseProgress = 75
    };
    ws.Journal.SetGrade("Програмування", 85);
    ws.RecalculateFromJournal();
    group.AddMember(ws);
}

void AddStudent()
{
    try
    {
        Console.Write("ПІБ (три слова): ");
        string name = Console.ReadLine() ?? "";
        Console.Write("Номер заліковки (8 цифр): ");
        string book = Console.ReadLine() ?? "";
        Console.Write("Рік народження (напр. 2005): ");
        int year = int.TryParse(Console.ReadLine(), out var y) ? y : 2005;
        Console.Write("Прогрес навчання 0-100 (Enter = 0): ");
        int progress = int.TryParse(Console.ReadLine(), out var p) ? p : 0;

        var student = new Student(name, new DateTime(year, 1, 1), book)
        {
            CourseProgress = progress
        };

        Console.Write("Нотатки (Enter щоб пропустити): ");
        string notes = Console.ReadLine() ?? "";
        if (!string.IsNullOrWhiteSpace(notes)) student.Notes = notes;

        group.AddStudent(student);
        logger.Success($"Додано студента: {name}");
        Console.WriteLine("Студента додано.");
    }
    catch (Exception ex)
    {
        logger.Error($"Помилка додавання: {ex.Message}");
        Console.WriteLine($"Помилка: {ex.Message}");
    }
}

void RemoveStudent()
{
    Console.Write("Номер заліковки для видалення: ");
    string book = (Console.ReadLine() ?? "").Trim();
    if (group.RemoveStudent(book))
    {
        logger.Warning($"Видалено студента із заліковкою {book}");
        Console.WriteLine("Студента видалено.");
    }
    else
    {
        Console.WriteLine("Студента з таким номером не знайдено.");
    }
}

void ShowAllStudents()
{
    var all = group.GetAllStudents();
    if (all.Count == 0)
    {
        Console.WriteLine("У групі немає студентів.");
        return;
    }
    Console.WriteLine($"Студентів у групі: {all.Count}");
    int i = 1;
    foreach (var s in all)
        Console.WriteLine($"{i++}. {s.GetFormattedInfo()}");
}

void FindStudent()
{
    Console.Write("Номер заліковки: ");
    string book = (Console.ReadLine() ?? "").Trim();
    var s = group[book];
    if (s is null)
    {
        Console.WriteLine("Студента не знайдено.");
        return;
    }
    Console.WriteLine(s.GetFormattedInfo(true));
}

void EditStudent()
{
    Console.Write("Номер заліковки студента для редагування: ");
    string book = (Console.ReadLine() ?? "").Trim();
    var s = group[book];
    if (s is null)
    {
        Console.WriteLine("Студента не знайдено.");
        return;
    }

    Console.WriteLine($"Поточні нотатки: {s.Notes ?? "немає"}");
    Console.Write("Нові нотатки (Enter щоб не змінювати): ");
    string notes = Console.ReadLine() ?? "";
    if (!string.IsNullOrWhiteSpace(notes)) s.Notes = notes;

    Console.Write("Новий прогрес 0-100 (Enter щоб не змінювати): ");
    string pr = Console.ReadLine() ?? "";
    if (int.TryParse(pr, out var p))
    {
        try { s.CourseProgress = p; }
        catch (Exception ex) { Console.WriteLine($"Прогрес не змінено: {ex.Message}"); }
    }

    logger.Info($"Відредаговано дані студента {s.FullName}");
    Console.WriteLine("Дані оновлено.");
}

void ShowExcellentAndFailing()
{
    var excellent = group.GetExcellentStudents();
    var failing = group.GetFailingStudents();

    Console.WriteLine($"Відмінники ({excellent.Count}):");
    if (excellent.Count == 0) Console.WriteLine("  немає");
    foreach (var s in excellent)
        Console.WriteLine($"  {s.FullName} - {s.AverageGrade:F2}");

    Console.WriteLine($"\nВідстаючі ({failing.Count}):");
    if (failing.Count == 0) Console.WriteLine("  немає");
    foreach (var s in failing)
        Console.WriteLine($"  {s.FullName} - {s.AverageGrade:F2}");
}

void ShowStatistics()
{
    var sb = new StringBuilder();
    sb.AppendLine($"Група: {group.GroupName} ({group.Specialization}), курс {group.Course}");
    sb.AppendLine($"Студентів: {group.GroupSize}, усього членів університету: {group.MembersCount}");
    sb.AppendLine($"Середній бал групи: {group.AverageGroupGrade:F2}");
    sb.AppendLine($"Відмінників: {group.GetExcellentStudents().Count} ({group.ExcellentPercent}%)");
    sb.AppendLine($"Відстаючих: {group.GetFailingStudents().Count}");
    sb.AppendLine("Розподіл за статусом:");
    foreach (StudentStatus st in Enum.GetValues<StudentStatus>())
        sb.AppendLine($"  {st}: {group.GetStudentsByStatus(st).Count}");
    Console.Write(sb.ToString());
}

void SaveGroup()
{
    try
    {
        group.SaveToFile("group.json");
        logger.Success("Групу збережено у group.json");
        Console.WriteLine("Збережено у файл group.json");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Помилка збереження: {ex.Message}");
    }
}

void LoadGroup()
{
    try
    {
        group.LoadFromFile("group.json");
        logger.Success("Групу завантажено з group.json");
        Console.WriteLine($"Завантажено. Студентів: {group.GroupSize}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Помилка завантаження: {ex.Message}");
    }
}

void SearchByFragment()
{
    Console.Write("Фрагмент ПІБ: ");
    string frag = Console.ReadLine() ?? "";
    Console.Write(group.SearchByNameFragment(frag));
}

void ShowGroupReport()
{
    Console.Write(TextProcessor.BuildGroupReport(group));
    logger.Info("Згенеровано повний звіт групи");
}

void NormalizeAllNotes()
{
    int changed = 0;
    foreach (var s in group.GetAllStudents())
    {
        if (string.IsNullOrWhiteSpace(s.Notes)) continue;
        string normalized = TextProcessor.Normalize(s.Notes);
        if (normalized != s.Notes)
        {
            s.Notes = normalized;
            changed++;
        }
    }
    logger.Info($"Нормалізовано нотаток: {changed}");
    Console.WriteLine($"Нотатки нормалізовано. Змінено записів: {changed}");
}

void CheckPalindromesInNotes()
{
    bool foundAny = false;
    foreach (var s in group.GetAllStudents())
    {
        if (string.IsNullOrWhiteSpace(s.Notes)) continue;

        var words = s.Notes.Split(
            new[] { ' ', ',', '.', '!', '?', ';' }, StringSplitOptions.RemoveEmptyEntries);
        var palindromes = words.Where(w => w.Length > 2 && TextProcessor.IsPalindrome(w)).ToList();

        if (palindromes.Count > 0)
        {
            foundAny = true;
            Console.WriteLine($"{s.FullName}: {string.Join(", ", palindromes)}");
        }
    }
    if (!foundAny)
        Console.WriteLine("Паліндромів у нотатках не знайдено.");
}

void ExportCsv()
{
    try
    {
        string csv = group.ExportToCsv();
        Console.WriteLine(csv);
        File.WriteAllText("students.csv", csv);
        logger.Success("Експортовано у students.csv");
        Console.WriteLine("Збережено у файл students.csv");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Помилка експорту: {ex.Message}");
    }
}

void ImportFromText()
{
    Console.WriteLine("Введіть студентів, кожен з нового рядка у форматі:");
    Console.WriteLine("Прізвище Імя Побатькові;НомерЗаліковки;ДатаНародження");
    Console.WriteLine("(порожній рядок завершує введення)");

    var sb = new StringBuilder();
    while (true)
    {
        string line = Console.ReadLine() ?? "";
        if (string.IsNullOrWhiteSpace(line)) break;
        sb.AppendLine(line);
    }

    try
    {
        int before = group.GroupSize;
        group.ImportStudentsFromText(sb.ToString());
        int added = group.GroupSize - before;
        logger.Success($"Імпортовано студентів: {added}");
        Console.WriteLine($"Додано студентів: {added}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Помилка імпорту: {ex.Message}");
    }
}

void ShowLogs()
{
    Console.WriteLine("--- Журнал подій ---");
    Console.Write(logger.GetFullLog());
    Console.WriteLine("--- Статистика журналу ---");
    Console.Write(logger.GetStatistics());
}

void ShowPerformance()
{
    Console.Write("Кількість ітерацій (Enter = 20000): ");
    string input = Console.ReadLine() ?? "";
    int iter = int.TryParse(input, out var n) ? n : 20000;
    Console.Write(TextProcessor.ComparePerformance(iter));
    logger.Info($"Виконано тест продуктивності ({iter})");
}

void TextProcessingMenu()
{
    Console.WriteLine("--- Обробка тексту та аналіз ---");
    Console.WriteLine("1. Перевернути рядок");
    Console.WriteLine("2. Підрахувати слова");
    Console.WriteLine("3. Підрахувати символи");
    Console.WriteLine("4. Нормалізувати текст");
    Console.WriteLine("5. Перевірити паліндром");
    Console.WriteLine("6. Множинна заміна за словником");
    Console.WriteLine("7. Розбити на речення");
    Console.WriteLine("8. Аналіз настрою групи (Варіант ПР3)");
    Console.Write("Вибір: ");
    string c = (Console.ReadLine() ?? "").Trim();
    Console.WriteLine();

    if (c == "1")
    {
        Console.Write("Текст: ");
        string t = Console.ReadLine() ?? "";
        Console.WriteLine($"Результат: {TextProcessor.Reverse(t)}");
    }
    else if (c == "2")
    {
        Console.Write("Текст: ");
        string t = Console.ReadLine() ?? "";
        Console.WriteLine($"Кількість слів: {TextProcessor.CountWords(t)}");
    }
    else if (c == "3")
    {
        Console.Write("Текст: ");
        string t = Console.ReadLine() ?? "";
        Console.WriteLine($"Символів без пробілів: {TextProcessor.CountCharacters(t)}");
        Console.WriteLine($"Символів з пробілами: {TextProcessor.CountCharacters(t, false)}");
    }
    else if (c == "4")
    {
        Console.Write("Текст із зайвими пробілами: ");
        string t = Console.ReadLine() ?? "";
        Console.WriteLine($"Нормалізовано: \"{TextProcessor.Normalize(t)}\"");
    }
    else if (c == "5")
    {
        Console.Write("Слово або фраза: ");
        string t = Console.ReadLine() ?? "";
        Console.WriteLine(TextProcessor.IsPalindrome(t) ? "Так, це паліндром." : "Ні, не паліндром.");
    }
    else if (c == "6")
    {
        Console.Write("Текст: ");
        string t = Console.ReadLine() ?? "";
        var dict = new Dictionary<string, string>();
        Console.WriteLine("Заміни у форматі що=наЩо (порожній рядок завершує):");
        while (true)
        {
            Console.Write("  заміна: ");
            string line = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(line)) break;
            var p = line.Split('=');
            if (p.Length == 2) dict[p[0]] = p[1];
        }
        Console.WriteLine($"Результат: {TextProcessor.ReplaceMultiple(t, dict)}");
    }
    else if (c == "7")
    {
        Console.Write("Текст із кількох речень: ");
        string t = Console.ReadLine() ?? "";
        var sentences = TextProcessor.SplitIntoSentences(t);
        Console.WriteLine($"Знайдено речень: {sentences.Length}");
        int i = 1;
        foreach (var s in sentences)
            Console.WriteLine($"  {i++}. {s}");
    }
    else if (c == "8")
    {
        Console.Write(mood.AnalyzeGroup(group));
        Console.WriteLine();
        Console.Write(mood.GetTopKeywords(group));
    }
    else
    {
        Console.WriteLine("Невідома команда.");
    }

    logger.Info("Використано обробку тексту");
}

void CompareTwoStudents()
{
    Console.Write("Заліковка першого студента: ");
    var s1 = group[(Console.ReadLine() ?? "").Trim()];
    Console.Write("Заліковка другого студента: ");
    var s2 = group[(Console.ReadLine() ?? "").Trim()];

    if (s1 is null || s2 is null)
    {
        Console.WriteLine("Один зі студентів не знайдений.");
        return;
    }

    Console.WriteLine($"{s1.FullName}: бал {s1.AverageGrade:F2}, прогрес {s1.CourseProgress}%");
    Console.WriteLine($"{s2.FullName}: бал {s2.AverageGrade:F2}, прогрес {s2.CourseProgress}%");

    if (s1 > s2) Console.WriteLine($"Вищий бал у: {s1.FullName}");
    else if (s1 < s2) Console.WriteLine($"Вищий бал у: {s2.FullName}");
    else Console.WriteLine("Студенти рівні за балом і прогресом (оператор ==).");

    logger.Info("Виконано порівняння студентів");
}

void MergeGroupsDemo()
{
    var other = new StudentGroup
    {
        GroupName = "К-321",
        Specialization = "Комп'ютерна інженерія",
        Course = 3
    };
    other.AddStudent(new Student("Сидоренко Олег Вікторович", new DateTime(2005, 3, 3), "55667788") { CourseProgress = 70 });
    other.AddStudent(new Student("Мельник Анна Юріївна", new DateTime(2004, 7, 7), "99887766") { CourseProgress = 85 });

    var combined = group + other;
    Console.WriteLine($"Група А: {group.GroupName} ({group.GroupSize} студентів)");
    Console.WriteLine($"Група Б: {other.GroupName} ({other.GroupSize} студентів)");
    Console.WriteLine($"Об'єднана: {combined.GroupName} ({combined.GroupSize} студентів)");
    foreach (var s in combined.GetAllStudents())
        Console.WriteLine($"  {s.FullName}");

    logger.Info("Виконано об'єднання груп");
}

void DemoVector()
{
    var v1 = new Vector(1, 2, 2);
    var v2 = new Vector(3, 0, 4);

    Console.WriteLine($"v1 = {v1}, довжина = {v1.Length:F2}");
    Console.WriteLine($"v2 = {v2}, довжина = {v2.Length:F2}");
    Console.WriteLine($"v1 + v2 = {v1 + v2}");
    Console.WriteLine($"v1 - v2 = {v1 - v2}");
    Console.WriteLine($"v1 * 3 = {v1 * 3}");
    Console.WriteLine($"v1 > v2 ? {(v1 > v2 ? "так" : "ні")}");

    var v3 = v1;
    v3++;
    Console.WriteLine($"v1 з ++ = {v3}");

    double len = (double)v2;
    Console.WriteLine($"(double)v2 = {len:F2}  (це довжина вектора)");

    logger.Info("Демонстрація Vector");
}

void DemoGradePoint()
{
    GradePoint g1 = 7.5;
    GradePoint g2 = 9.0;

    Console.WriteLine($"g1 = {g1}, g2 = {g2}");
    Console.WriteLine($"g1 + g2 = {g1 + g2}");
    Console.WriteLine($"g1 > g2 ? {(g1 > g2 ? "так" : "ні")}");

    double d = g2;
    Console.WriteLine($"g2 як double = {d}");

    if (g2) Console.WriteLine($"g2 ({g2}) - достатня оцінка (оператор true, бо >= 8)");
    else Console.WriteLine($"g2 ({g2}) - недостатня");

    if (g1) Console.WriteLine($"g1 ({g1}) - достатня оцінка");
    else Console.WriteLine($"g1 ({g1}) - недостатня (оператор false, бо < 8)");

    logger.Info("Демонстрація GradePoint");
}

void FindBest()
{
    var best = group.BestStudent();
    if (best is null)
    {
        Console.WriteLine("Група порожня.");
        return;
    }
    Console.WriteLine($"Найкращий студент: {best.FullName}");
    Console.WriteLine($"Середній бал: {best.AverageGrade:F2}, прогрес: {best.CourseProgress}%");
    logger.Info("Знайдено найкращого студента");
}

void TestOperators()
{
    Console.WriteLine("=== Тестування перевантажених операторів ===");

    Console.WriteLine("\n-- Vector --");
    var va = new Vector(2, 2, 1);
    var vb = new Vector(1, 1, 1);
    Console.WriteLine($"{va} + {vb} = {va + vb}");

    Console.WriteLine("\n-- GradePoint --");
    GradePoint ga = 8.5;
    Console.WriteLine($"{ga} достатня? {(ga ? "так" : "ні")}");

    Console.WriteLine("\n-- Fraction (Варіант ПР4) --");
    var fa = new Fraction(2, 4);
    var fb = new Fraction(1, 3);
    Console.WriteLine($"2/4 автоматично скоротилось до {fa}");
    Console.WriteLine($"{fa} + {fb} = {fa + fb}");
    Console.WriteLine($"{fa} * {fb} = {fa * fb}");

    Console.WriteLine("\n-- Student --");
    var students = group.GetAllStudents();
    if (students.Count >= 2)
    {
        var team = students[0] + students[1];
        Console.WriteLine($"s1 + s2 = \"{team.FullName}\", бал {team.AverageGrade:F2}");
    }

    logger.Info("Виконано тестування операторів");
}

void AddRegularStudent()
{
    try
    {
        Console.Write("ПІБ (три слова): ");
        string name = Console.ReadLine() ?? "";
        Console.Write("Номер заліковки (8 цифр): ");
        string book = Console.ReadLine() ?? "";
        Console.Write("Рік народження: ");
        int year = int.TryParse(Console.ReadLine(), out var y) ? y : 2005;

        var student = new Student(name, new DateTime(year, 1, 1), book);
        student.Enroll();
        group.AddMember(student);
        logger.Success($"Додано звичайного студента: {name}");
        Console.WriteLine($"Додано: {student.GetInfo()}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Помилка: {ex.Message}");
    }
}

void AddSpecialStudent()
{
    Console.WriteLine("Який тип студента додати?");
    Console.WriteLine("1. Відмінник (ExcellentStudent)");
    Console.WriteLine("2. Іноземний студент (ForeignStudent)");
    Console.WriteLine("3. Працюючий студент (WorkingStudent)");
    Console.WriteLine("4. Аспірант (GraduateStudent)");
    Console.Write("Вибір: ");
    string t = (Console.ReadLine() ?? "").Trim();

    Console.Write("ПІБ (три слова): ");
    string name = Console.ReadLine() ?? "";
    Console.Write("Номер заліковки (8 цифр): ");
    string book = Console.ReadLine() ?? "";
    Console.Write("Рік народження: ");
    int year = int.TryParse(Console.ReadLine(), out var y) ? y : 2005;
    var dob = new DateTime(year, 1, 1);

    try
    {
        Student student;
        if (t == "1")
        {
            Console.Write("Досягнення: ");
            student = new ExcellentStudent(name, dob, book, Console.ReadLine() ?? "");
        }
        else if (t == "2")
        {
            Console.Write("Країна: ");
            student = new ForeignStudent(name, dob, book, Console.ReadLine() ?? "");
        }
        else if (t == "3")
        {
            Console.Write("Компанія: ");
            string company = Console.ReadLine() ?? "";
            Console.Write("Годин на тиждень: ");
            int hours = int.TryParse(Console.ReadLine(), out var h) ? h : 0;
            student = new WorkingStudent(name, dob, book, company, hours);
        }
        else if (t == "4")
        {
            Console.Write("Тема дослідження: ");
            student = new GraduateStudent(name, dob, book, Console.ReadLine() ?? "");
        }
        else
        {
            Console.WriteLine("Невідомий тип.");
            return;
        }

        group.AddMember(student);
        logger.Success($"Додано {student.GetType().Name}: {name}");
        Console.WriteLine($"Додано: {student.GetInfo()}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Помилка: {ex.Message}");
    }
}

void ShowAllMembers()
{
    Console.Write(group.GetMembersInfo());
    logger.Info("Виведено всіх членів (поліморфізм)");
}

void CalculateScholarships()
{
    Console.WriteLine("Стипендії по членах університету:");
    foreach (var m in group.GetAllMembers())
        Console.WriteLine($"  {m.GetInfo()} -> {m.CalculateScholarship()} грн");
    Console.WriteLine($"\nЗагальна сума стипендій: {group.GetTotalScholarship()} грн");
    logger.Info("Розраховано стипендії");
}

void ShowByType()
{
    Console.WriteLine($"Відмінників (ExcellentStudent): {group.GetMembersByType<ExcellentStudent>().Count}");
    Console.WriteLine($"Іноземних (ForeignStudent): {group.GetMembersByType<ForeignStudent>().Count}");
    Console.WriteLine($"Працюючих (WorkingStudent): {group.GetMembersByType<WorkingStudent>().Count}");
    Console.WriteLine($"Аспірантів (GraduateStudent): {group.GetMembersByType<GraduateStudent>().Count}");
    Console.WriteLine($"Усього студентів (Student і похідні): {group.GetMembersByType<Student>().Count}");

    var foreign = group.GetMembersByType<ForeignStudent>();
    if (foreign.Count > 0)
    {
        Console.WriteLine("\nІноземні студенти детально:");
        foreach (var f in foreign)
            Console.WriteLine($"  {f.FullName}, країна: {f.Country}");
    }

    logger.Info("Показано студентів за типом");
}

void TestHierarchy()
{
    Console.WriteLine("=== Тестування ієрархії, base та override ===");

    var ordinary = new Student("Тестенко Тест Тестович", new DateTime(2005, 1, 1), "99999999");
    ordinary.UpdateAverageGrade(80);
    var excellent = new ExcellentStudent("Розумний Геній Олександрович", new DateTime(2005, 1, 1), "88888888", "Грант");
    excellent.UpdateAverageGrade(95);
    var grad = new GraduateStudent("Науковий Дослідник Іванович", new DateTime(2000, 1, 1), "77777777", "Машинне навчання");

    Console.WriteLine("\nGetInfo() через override (кожен показує своє):");
    foreach (Student s in new Student[] { ordinary, excellent, grad })
        Console.WriteLine($"  {s.GetInfo()}");

    Console.WriteLine("\nCalculateScholarship() через override:");
    Console.WriteLine($"  Звичайний студент (бал 80): {ordinary.CalculateScholarship()} грн");
    Console.WriteLine($"  Відмінник (бал 95, base + надбавка): {excellent.CalculateScholarship()} грн");
    Console.WriteLine($"  Аспірант (фіксована): {grad.CalculateScholarship()} грн");

    Console.WriteLine("\nEnroll() через override:");
    excellent.Enroll();

    logger.Info("Виконано тест ієрархії");
}

void DemoVehicles()
{
    var vehicles = new List<Vehicle>
    {
        new Car("Toyota", 200, 4),
        new Bus("MAN", 120, 50),
        new Truck("Volvo", 140, 20.5)
    };

    Console.WriteLine("Транспортні засоби (Варіант 2, поліморфізм через GetInfo):");
    foreach (var v in vehicles)
        Console.WriteLine($"  {v.GetInfo()}");

    logger.Info("Демонстрація Vehicle");
}
