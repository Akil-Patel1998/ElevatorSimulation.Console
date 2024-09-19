using ElevatorSimulation.Core.Interfaces;
using ElevatorSimulation.Entities.Models;
using System.Collections.Generic;

namespace ElevatorSimulation.Core.Services
{
    public class FloorService : IFloorService
    {
        private readonly Dictionary<int, Floor> floors;
        private readonly int minFloor;
        private readonly int maxFloor;

        public FloorService(int totalFloors, int minFloor = 0, int maxFloor = 10)
        {
            this.minFloor = minFloor;
            this.maxFloor = maxFloor;
            floors = new Dictionary<int, Floor>();

            for (int i = minFloor; i <= maxFloor; i++)
            {
                floors[i] = new Floor(i);
            }
        }

        public Floor GetFloor(int floorNumber)
        {
            return floors.TryGetValue(floorNumber, out var floor) ? floor : null;
        }

        public void UpdateWaitingPassengers(int floorNumber, int numberOfPassengers)
        {
            var floor = GetFloor(floorNumber);
            if (floor != null)
            {
                floor.WaitingPassengers = numberOfPassengers;
                Console.WriteLine($"Added {numberOfPassengers} passengers waiting on floor {floorNumber}.");
            }
        }

        public void ClearWaitingPassengers(int floorNumber,int peopleOnBoard)
        {
            var floor = GetFloor(floorNumber);
            if (floor != null)
            {
                floor.WaitingPassengers = floor.WaitingPassengers-peopleOnBoard;
                Console.WriteLine($"Cleared all waiting passengers on floor {floorNumber}.");
            }
        }
    }
}
