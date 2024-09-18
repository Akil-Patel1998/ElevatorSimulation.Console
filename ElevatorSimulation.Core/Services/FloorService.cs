using System.Collections.Generic;
using ElevatorSimulation.Core.Interfaces;
using ElevatorSimulation.Entities.Models;

namespace ElevatorSimulation.Core.Services
{
    public class FloorService : IFloorService
    {
        private readonly Dictionary<int, Floor> floors;

        public FloorService(int numberOfFloors)
        {
            floors = Enumerable.Range(0, numberOfFloors)
                               .Select(i => new Floor(i))
                               .ToDictionary(f => f.FloorNumber);
        }

        public Floor GetFloor(int floorNumber)
        {
            return floors.TryGetValue(floorNumber, out var floor) ? floor : null;
        }

        public void UpdateFloor(int floorNumber, int numberOfPassengers)
        {
            if (floors.TryGetValue(floorNumber, out var floor))
            {
                AddPassengers(floor,numberOfPassengers);
            }
        }
        public void AddPassengers(Floor floor, int numberOfPassengers)
        {
            if (numberOfPassengers < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfPassengers), "Number of passengers to add cannot be negative.");
            }
            floor.WaitingPassengers+= numberOfPassengers;
            Console.WriteLine($"Added {numberOfPassengers} passengers to floor {floor.FloorNumber}. Total waiting: {floor.WaitingPassengers}");
        }

        public void RemovePassengers(Floor floor,int numberOfPassengers)
        {
            if (numberOfPassengers < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfPassengers), "Number of passengers to remove cannot be negative.");
            }
            if (numberOfPassengers > floor.WaitingPassengers)
            {
                throw new InvalidOperationException("Cannot remove more passengers than currently waiting.");
            }
            floor.WaitingPassengers -= numberOfPassengers;
            Console.WriteLine($"Removed {numberOfPassengers} passengers from floor {floor.FloorNumber}. Total waiting: {floor.WaitingPassengers}");
        }
    }
}
