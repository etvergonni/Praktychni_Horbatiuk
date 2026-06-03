using System;

namespace Praktychna2;
// Реалізовано в гілці feature/refactoring
// Власні винятки PortException та PortClosedException


public class PortException : Exception
{
    public int PortNumber { get; }

    public PortException(int portNumber, string message) : base(message)
    {
        PortNumber = portNumber;
    }
}


public class PortClosedException : PortException
{
    public PortClosedException(int portNumber)
        : base(portNumber, $"Порт №{portNumber} закритий. Відкрийте його перед операцією.")
    {
    }
}


public class PortMatrixIndexException : PortException
{
    public PortMatrixIndexException(int row, int col)
        : base(-1, $"Координати ({row}, {col}) виходять за межі матриці портів (0–15).")
    {
    }
}
