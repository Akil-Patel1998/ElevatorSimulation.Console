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

        public FloorService(int totalFloors)
        {
            minFloor = 0;
            maxFloor = totalFloors - 1;
            floors = new Dictionary<int, Floor>();

            // Initialize floors with zero waiting passengers
            for (int i = minFloor; i <= maxFloor; i++)
            {
                floors[i] = new Floor(i);
            }
        }


        public void UpdateWaitingPassengers(int floorNumber, int count, List<int> destinationFloors)
        {
            if (floors.ContainsKey(floorNumber))
            {
                var floor = floors[floorNumber];
                floor.WaitingPassengers = count;
                floor.DestinationFloors = destinationFloors;
            }
        }

        public Floor GetFloor(int floorNumber)
        {
            return floors.ContainsKey(floorNumber) ? floors[floorNumber] : null;
        }

        public void ClearWaitingPassengers(int floorNumber)
        {
            if (floors.ContainsKey(floorNumber))
            {
                var floor = floors[floorNumber];
                floor.WaitingPassengers = 0;
                floor.DestinationFloors.Clear();
            }
        }
        public IEnumerable<Floor> GetAllFloors()
        {
            return floors.Values;
        }
    }
}
