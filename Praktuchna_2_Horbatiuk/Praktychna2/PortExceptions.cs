using System;

namespace Praktychna2;

/// <summary>Базовий виняток для будь-яких помилок роботи з портом.</summary>
public class PortException : Exception
{
    public int PortNumber { get; }

    public PortException(int portNumber, string message) : base(message)
    {
        PortNumber = portNumber;
    }
}

/// <summary>Виникає при спробі прочитати/записати дані в закритий порт.</summary>
public class PortClosedException : PortException
{
    public PortClosedException(int portNumber)
        : base(portNumber, $"Порт №{portNumber} закритий. Відкрийте його перед операцією.")
    {
    }
}

/// <summary>Виникає при виході координат за межі матриці.</summary>
public class PortMatrixIndexException : PortException
{
    public PortMatrixIndexException(int row, int col)
        : base(-1, $"Координати ({row}, {col}) виходять за межі матриці портів (0–15).")
    {
    }
}
