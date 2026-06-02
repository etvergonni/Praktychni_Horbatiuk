using System;
using System.Linq;
using System.Text;

namespace Praktychna2;

/// <summary>
/// Симуляція одного порту вводу/виводу.
/// Має 64-байтний буфер даних та підключений пристрій.
/// </summary>
public class Port : ICloneable
{
    /// <summary>Унікальний номер порту.</summary>
    public int PortNumber { get; }

    /// <summary>Буфер даних розміром 64 байти (одновимірний масив).</summary>
    public byte[] DataBuffer { get; private set; } = new byte[64];

    /// <summary>Чи відкритий порт для роботи.</summary>
    public bool IsOpen { get; private set; }

    /// <summary>Назва підключеного пристрою.</summary>
    public string DeviceName { get; }

    /// <summary>Тип пристрою (для пошуку в матриці).</summary>
    public DeviceType DeviceType { get; }

    /// <summary>Кількість записаних байт у буфер (бо буфер фіксований 64).</summary>
    public int DataLength { get; private set; }

    /// <summary>Конструктор: створює порт із номером, пристроєм і типом.</summary>
    public Port(int portNumber, string deviceName, DeviceType deviceType)
    {
        if (portNumber < 0)
            throw new ArgumentOutOfRangeException(nameof(portNumber), "Номер порту має бути ≥ 0.");
        if (string.IsNullOrWhiteSpace(deviceName))
            throw new ArgumentException("Назва пристрою не може бути порожньою.");

        PortNumber = portNumber;
        DeviceName = deviceName;
        DeviceType = deviceType;
        IsOpen = false;
    }

    /// <summary>Відкрити порт.</summary>
    public void Open() => IsOpen = true;

    /// <summary>Закрити порт.</summary>
    public void Close() => IsOpen = false;

    /// <summary>Записати дані у буфер порту.</summary>
    public void Write(byte[] data)
    {
        if (!IsOpen)
            throw new PortClosedException(PortNumber);
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        if (data.Length > DataBuffer.Length)
            throw new ArgumentException(
                $"Дані ({data.Length} байт) перевищують розмір буфера (64 байти).");

        // Очистити буфер і скопіювати нові дані
        Array.Clear(DataBuffer, 0, DataBuffer.Length);
        Array.Copy(data, DataBuffer, data.Length);
        DataLength = data.Length;
    }

    /// <summary>Прочитати дані з буфера порту (тільки записану частину).</summary>
    public byte[] Read()
    {
        if (!IsOpen)
            throw new PortClosedException(PortNumber);
        var result = new byte[DataLength];
        Array.Copy(DataBuffer, result, DataLength);
        return result;
    }

    /// <summary>Очистити буфер порту.</summary>
    public void ClearBuffer()
    {
        if (!IsOpen)
            throw new PortClosedException(PortNumber);
        Array.Clear(DataBuffer, 0, DataBuffer.Length);
        DataLength = 0;
    }

    /// <summary>Короткий опис стану порту через StringBuilder.</summary>
    public string GetStatusInfo()
    {
        var sb = new StringBuilder();
        sb.Append("Порт #").Append(PortNumber)
          .Append(" | ").Append(DeviceName)
          .Append(" (").Append(DeviceType).Append(")")
          .Append(" | ").Append(IsOpen ? "ВІДКРИТО" : "закрито")
          .Append(" | дані: ").Append(DataLength).Append("/64 байт");
        return sb.ToString();
    }

    /// <summary>ICloneable — глибока копія порту.</summary>
    public object Clone()
    {
        var copy = new Port(PortNumber, DeviceName, DeviceType);
        copy.DataBuffer = (byte[])DataBuffer.Clone();
        copy.DataLength = DataLength;
        if (IsOpen) copy.Open();
        return copy;
    }
}
