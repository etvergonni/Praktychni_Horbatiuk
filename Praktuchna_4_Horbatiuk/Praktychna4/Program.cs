using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Praktychna4;

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
        case "25": ShowMoodAnalysis(); break;
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
    Console.WriteLine("===== СИСТЕМА УПРАВЛІННЯ ГРУПОЮ (ПР №4) =====");
    Console.WriteLine(" 1. Додати студента");
    Console.WriteLine(" 2. Видалити студента");
    Console.WriteLine(" 3. Вивести всіх студентів");
    Console.WriteLine(" 4. Пошук студента");
    Console.WriteLine(" 5. Редагування даних студента");
    Console.WriteLine(" 6. Відмінники / відстаючі");
    Console.WriteLine(" 7. Статистика групи");
    Console.WriteLine(" 8. Зберегти дані");
    Console.WriteLine(" 9. Завантажити дані");
    Console.WriteLine("10. Пошук за фрагментом ПІБ");
    Console.WriteLine("11. Згенерувати повний звіт групи");
    Console.WriteLine("12. Нормалізувати нотатки всіх студентів");
    Console.WriteLine("13. Перевірити паліндроми в нотатках");
    Console.WriteLine("14. Експорт групи у CSV");
    Console.WriteLine("15. Імпорт студентів з текстового блоку");
    Console.WriteLine("16. Переглянути логи системи");
    Console.WriteLine("17. Порівняти продуктивність string vs StringBuilder");
    Console.WriteLine("18. Обробка тексту (реверс, слова, паліндром тощо)");
    Console.WriteLine("-- Оператори (ПР №4) --");
    Console.WriteLine("19. Порівняти двох студентів (>, <, ==)");
    Console.WriteLine("20. Об'єднати дві групи (оператор +)");
    Console.WriteLine("21. Демонстрація класу Vector");
    Console.WriteLine("22. Демонстрація класу GradePoint");
    Console.WriteLine("23. Знайти найкращого студента");
    Console.WriteLine("24. Тестування перевантажених операторів");
    Console.WriteLine("25. Аналіз настрою групи (ПР №3, Варіант 2)");
    Console.WriteLine(" 0. Вийти");
    Console.WriteLine("============================================");
}

void SeedDemoData()
{
    var s1 = new Student
    {
        FullName = "Горбатюк Олександра Іванівна",
        RecordBookNumber = "12345678",
        DateOfBirth = new DateTime(2005, 5, 12),
        Notes = "Активна студентка, відмінно показує себе, помітний прогрес.",
        CourseProgress = 90
    };
    s1.Journal.SetGrade("Програмування", 95);
    s1.Journal.SetGrade("Математика", 88);
    s1.RecalculateFromJournal();
    s1.GradePoints.Add(new GradePoint(9));
    s1.GradePoints.Add(new GradePoint(8.5));

    var s2 = new Student
    {
        FullName = "Петренко Іван Сергійович",
        RecordBookNumber = "87654321",
        DateOfBirth = new DateTime(2004, 9, 3),
        Notes = "Є проблема з відвідуванням, накопичив борг, відстає.",
        CourseProgress = 45
    };
    s2.Journal.SetGrade("Програмування", 60);
    s2.Journal.SetGrade("Математика", 55);
    s2.RecalculateFromJournal();
    s2.GradePoints.Add(new GradePoint(6));

    var s3 = new Student
    {
        FullName = "Коваленко Марія Олегівна",
        RecordBookNumber = "11223344",
        DateOfBirth = new DateTime(2005, 1, 20),
        Notes = "Старанна, чудові результати, молодець.",
        CourseProgress = 80
    };
    s3.Journal.SetGrade("Програмування", 90);
    s3.Journal.SetGrade("Математика", 92);
    s3.RecalculateFromJournal();
    s3.GradePoints.Add(new GradePoint(9.5));

    group.AddStudent(s1);
    group.AddStudent(s2);
    group.AddStudent(s3);
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

        var student = new Student
        {
            FullName = name,
            RecordBookNumber = book,
            DateOfBirth = new DateTime(year, 1, 1),
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
    sb.AppendLine($"Кількість студентів: {group.GroupSize}");
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
    Console.WriteLine("--- Обробка тексту ---");
    Console.WriteLine("1. Перевернути рядок");
    Console.WriteLine("2. Підрахувати слова");
    Console.WriteLine("3. Підрахувати символи");
    Console.WriteLine("4. Нормалізувати текст");
    Console.WriteLine("5. Перевірити паліндром");
    Console.WriteLine("6. Множинна заміна за словником");
    Console.WriteLine("7. Розбити на речення");
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
    other.AddStudent(new Student
    {
        FullName = "Сидоренко Олег Вікторович",
        RecordBookNumber = "55667788",
        DateOfBirth = new DateTime(2005, 3, 3),
        CourseProgress = 70
    });
    other.AddStudent(new Student
    {
        FullName = "Мельник Анна Юріївна",
        RecordBookNumber = "99887766",
        DateOfBirth = new DateTime(2004, 7, 7),
        CourseProgress = 85
    });

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
    Console.WriteLine($"v1 == v2 ? {(v1 == v2 ? "так" : "ні")}");

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

    var g3 = g1;
    g3++;
    Console.WriteLine($"g1 ({g1}) після ++ = {g3}");

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
    Console.WriteLine($"{va} > {vb} ? {(va > vb ? "так" : "ні")}");

    Console.WriteLine("\n-- GradePoint --");
    GradePoint ga = 8.5;
    GradePoint gb = 6.0;
    Console.WriteLine($"{ga} + {gb} = {ga + gb}");
    Console.WriteLine($"{ga} достатня? {(ga ? "так" : "ні")}");
    Console.WriteLine($"{gb} достатня? {(gb ? "так" : "ні")}");

    Console.WriteLine("\n-- Fraction (Варіант 2) --");
    var fa = new Fraction(2, 4);
    var fb = new Fraction(1, 3);
    Console.WriteLine($"2/4 автоматично скоротилось до {fa}");
    Console.WriteLine($"{fa} + {fb} = {fa + fb}");
    Console.WriteLine($"{fa} * {fb} = {fa * fb}");
    Console.WriteLine($"{fa} > {fb} ? {(fa > fb ? "так" : "ні")}");

    Console.WriteLine("\n-- Student --");
    var students = group.GetAllStudents();
    if (students.Count >= 2)
    {
        var s1 = students[0];
        var s2 = students[1];
        Console.WriteLine($"{s1.FullName} > {s2.FullName} ? {(s1 > s2 ? "так" : "ні")}");
        var team = s1 + s2;
        Console.WriteLine($"s1 + s2 = \"{team.FullName}\", бал {team.AverageGrade:F2}");
    }

    logger.Info("Виконано тестування операторів");
}

void ShowMoodAnalysis()
{
    Console.Write(mood.AnalyzeGroup(group));
    Console.WriteLine();
    Console.Write(mood.GetTopKeywords(group));
    logger.Info("Виконано аналіз настрою групи");
}
