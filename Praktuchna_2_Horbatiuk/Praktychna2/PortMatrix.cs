using System;
using System.Collections.Generic;
using System.Text;

namespace Praktychna2;

/// <summary>
/// Матриця портів вводу/виводу 16×16.
/// Демонструє використання двовимірного масиву Port[,].
/// </summary>
public class PortMatrix
{
    public const int Rows = 16;
    public const int Cols = 16;

    // ───────── Двовимірний масив портів ─────────
    private readonly Port[,] _matrix = new Port[Rows, Cols];

    /// <summary>Перевірка, чи координати в межах матриці.</summary>
    private static void ValidateCoordinates(int row, int col)
    {
        if (row < 0 || row >= Rows || col < 0 || col >= Cols)
            throw new PortMatrixIndexException(row, col);
    }

    /// <summary>Ініціалізувати всю матрицю портами із заданими пристроями.</summary>
    public void Initialize()
    {
        // Назви пристроїв розподіляємо за рядками
        DeviceType[] deviceByRow = new DeviceType[]
        {
            DeviceType.Oscilloscope,
            DeviceType.Multimeter,
            DeviceType.SignalGenerator,
            DeviceType.PowerSupply,
            DeviceType.LogicAnalyzer,
            DeviceType.Oscilloscope,
            DeviceType.Multimeter,
            DeviceType.SignalGenerator,
            DeviceType.PowerSupply,
            DeviceType.LogicAnalyzer,
            DeviceType.Oscilloscope,
            DeviceType.Multimeter,
            DeviceType.SignalGenerator,
            DeviceType.PowerSupply,
            DeviceType.LogicAnalyzer,
            DeviceType.Unknown
        };

        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                int portNumber = r * Cols + c; // 0..255
                var device = deviceByRow[r];
                string name = $"{device}-{r:D2}{c:D2}";
                _matrix[r, c] = new Port(portNumber, name, device);
            }
        }
    }

    /// <summary>Отримати порт за координатами.</summary>
    public Port GetPort(int row, int col)
    {
        ValidateCoordinates(row, col);
        if (_matrix[row, col] == null)
            throw new InvalidOperationException("Матриця не ініціалізована. Викличте Initialize().");
        return _matrix[row, col];
    }

    /// <summary>Відкрити порт за координатами.</summary>
    public void OpenPort(int row, int col)
    {
        GetPort(row, col).Open();
    }

    /// <summary>Закрити порт за координатами.</summary>
    public void ClosePort(int row, int col)
    {
        GetPort(row, col).Close();
    }

    /// <summary>Записати дані в порт за координатами.</summary>
    public void WriteToPort(int row, int col, byte[] data)
    {
        GetPort(row, col).Write(data);
    }

    /// <summary>Прочитати дані з порту за координатами.</summary>
    public byte[] ReadFromPort(int row, int col)
    {
        return GetPort(row, col).Read();
    }

    /// <summary>
    /// Сканувати всю матрицю — повернути список відкритих портів.
    /// Демонструє роботу з двовимірним масивом через подвійний цикл.
    /// </summary>
    public List<(int Row, int Col, Port Port)> ScanMatrix()
    {
        var openPorts = new List<(int, int, Port)>();
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                var port = _matrix[r, c];
                if (port != null && port.IsOpen)
                    openPorts.Add((r, c, port));
            }
        }
        return openPorts;
    }

    /// <summary>Знайти всі порти певного типу пристрою.</summary>
    public List<(int Row, int Col, Port Port)> FindByDeviceType(DeviceType type)
    {
        var found = new List<(int, int, Port)>();
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                var port = _matrix[r, c];
                if (port != null && port.DeviceType == type)
                    found.Add((r, c, port));
            }
        }
        return found;
    }

    /// <summary>Кількість відкритих портів.</summary>
    public int OpenPortsCount
    {
        get
        {
            int count = 0;
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    if (_matrix[r, c]?.IsOpen == true) count++;
            return count;
        }
    }

    /// <summary>
    /// Форматоване виведення стану матриці у вигляді таблиці.
    /// Використовує StringBuilder для накопичення великого тексту.
    /// </summary>
    public string GetMatrixView()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Матриця портів {Rows}×{Cols}:");
        sb.AppendLine("Легенда: . — закритий, O — відкритий, ? — не ініціалізовано");
        sb.AppendLine();

        // Заголовок з номерами колонок
        sb.Append("     ");
        for (int c = 0; c < Cols; c++)
            sb.Append($"{c:D2} ");
        sb.AppendLine();

        // Рядки матриці
        for (int r = 0; r < Rows; r++)
        {
            sb.Append($"  {r:D2} ");
            for (int c = 0; c < Cols; c++)
            {
                var port = _matrix[r, c];
                char mark = port == null ? '?' : (port.IsOpen ? 'O' : '.');
                sb.Append($" {mark} ");
            }
            sb.AppendLine();
        }
        sb.AppendLine();
        sb.AppendLine($"Загалом відкритих портів: {OpenPortsCount}");
        return sb.ToString();
    }
}
