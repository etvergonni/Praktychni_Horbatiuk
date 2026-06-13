using System;

namespace Praktychna5;

public class ExcellentStudent : Student
{
    public string Achievement { get; set; }

    public ExcellentStudent(string fullName, DateTime dateOfBirth, string recordBookNumber, string achievement)
        : base(fullName, dateOfBirth, recordBookNumber)
    {
        Achievement = achievement;
    }

    public override decimal CalculateScholarship()
    {
        // До звичайної стипендії додається надбавка відміннику.
        return base.CalculateScholarship() + 500m;
    }

    public override string GetInfo()
    {
        return base.GetInfo() + $" [відмінник: {Achievement}]";
    }
}
