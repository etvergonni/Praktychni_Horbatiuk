using System;

namespace Praktychna5;

public abstract class UniversityMember
{
    // Абстрактний метод: кожен похідний клас рахує стипендію по-своєму.
    public abstract decimal CalculateScholarship();

    // Віртуальний метод: поведінку можна перевизначити в похідних класах.
    public virtual void Enroll()
    {
        Console.WriteLine("Члена університету зараховано.");
    }

    // Віртуальний метод: базова інформація про члена університету.
    // Похідні класи (Person і далі) перевизначають його через override.
    public virtual string GetInfo()
    {
        return "Член університету";
    }
}
