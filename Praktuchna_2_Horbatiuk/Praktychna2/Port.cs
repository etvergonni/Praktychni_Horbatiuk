using System;
using System.Linq;
using System.Text;

namespace Praktychna2;

// Реалізовано в гілці feature/port-class
// Клас Port з 64-байтним буфером DataBuffer

public class Port : ICloneable
{

    public int PortNumber { get; }

   
    public byte[] DataBuffer { get; private set; } = new byte[64];


    public bool IsOpen { get; private set; }


    public string DeviceName { get; }


    public DeviceType DeviceType { get; }

   
    public int DataLength { get; private set; }

   
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


    public void Open() => IsOpen = true;


    public void Close() => IsOpen = false;


    public void Write(byte[] data)
    {
        if (!IsOpen)
            throw new PortClosedException(PortNumber);
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        if (data.Length > DataBuffer.Length)
            throw new ArgumentException(
                $"Дані ({data.Length} байт) перевищують розмір буфера (64 байти).");


        Array.Clear(DataBuffer, 0, DataBuffer.Length);
        Array.Copy(data, DataBuffer, data.Length);
        DataLength = data.Length;
    }

    
    public byte[] Read()
    {
        if (!IsOpen)
            throw new PortClosedException(PortNumber);
        var result = new byte[DataLength];
        Array.Copy(DataBuffer, result, DataLength);
        return result;
    }


    public void ClearBuffer()
    {
        if (!IsOpen)
            throw new PortClosedException(PortNumber);
        Array.Clear(DataBuffer, 0, DataBuffer.Length);
        DataLength = 0;
    }

  
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


    public object Clone()
    {
        var copy = new Port(PortNumber, DeviceName, DeviceType);
        copy.DataBuffer = (byte[])DataBuffer.Clone();
        copy.DataLength = DataLength;
        if (IsOpen) copy.Open();
        return copy;
    }
}
