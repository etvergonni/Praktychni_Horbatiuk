namespace Praktychna2;

/// <summary>Статус студента у навчальному закладі.</summary>
public enum StudentStatus
{
    Active,
    AcademicLeave,
    Expelled,
    Graduated
}

/// <summary>Тип пристрою, підключеного до порту.</summary>
public enum DeviceType
{
    Unknown,
    Oscilloscope,    // осцилограф
    Multimeter,      // мультиметр
    SignalGenerator, // генератор сигналів
    PowerSupply,     // блок живлення
    LogicAnalyzer    // логічний аналізатор
}
