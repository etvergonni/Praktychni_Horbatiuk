using System;

namespace Praktychna5;

public class ForeignStudent : Student
{
    public string Country { get; set; }

    public ForeignStudent(string fullName, DateTime dateOfBirth, string recordBookNumber, string country)
        : base(fullName, dateOfBirth, recordBookNumber)
    {
        Country = country;
    }

    public override decimal CalculateScholarship()
    {
        // Іноземні студенти навчаються на контракті, стипендії немає.
        return 0m;
    }

    public override string GetInfo()
    {
        return base.GetInfo() + $" [іноземний студент, країна: {Country}]";
    }
}
