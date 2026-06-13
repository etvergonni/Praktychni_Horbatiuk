using System;

namespace Praktychna5;

// ВАРІАНТ 2: ієрархія транспортних засобів.
public abstract class Vehicle
{
    public string Brand { get; set; }
    public int MaxSpeed { get; set; }

    protected Vehicle(string brand, int maxSpeed)
    {
        Brand = brand;
        MaxSpeed = maxSpeed;
    }

    public abstract string GetVehicleType();

    public virtual string GetInfo()
    {
        return $"{GetVehicleType()}: {Brand}, макс. швидкість {MaxSpeed} км/год";
    }
}

public class Car : Vehicle
{
    public int NumberOfDoors { get; set; }

    public Car(string brand, int maxSpeed, int numberOfDoors)
        : base(brand, maxSpeed)
    {
        NumberOfDoors = numberOfDoors;
    }

    public override string GetVehicleType() => "Легковий автомобіль";

    public override string GetInfo()
        => base.GetInfo() + $", дверей: {NumberOfDoors}";
}

public class Bus : Vehicle
{
    public int PassengerCapacity { get; set; }

    public Bus(string brand, int maxSpeed, int passengerCapacity)
        : base(brand, maxSpeed)
    {
        PassengerCapacity = passengerCapacity;
    }

    public override string GetVehicleType() => "Автобус";

    public override string GetInfo()
        => base.GetInfo() + $", пасажиромісткість: {PassengerCapacity}";
}

public class Truck : Vehicle
{
    public double LoadCapacityTons { get; set; }

    public Truck(string brand, int maxSpeed, double loadCapacityTons)
        : base(brand, maxSpeed)
    {
        LoadCapacityTons = loadCapacityTons;
    }

    public override string GetVehicleType() => "Вантажівка";

    public override string GetInfo()
        => base.GetInfo() + $", вантажопідйомність: {LoadCapacityTons} т";
}
