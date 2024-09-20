using ElevatorSimulation.Core.Interfaces;
using ElevatorSimulation.Entities.Models;
using System.Collections.Generic;
using System.Linq;

namespace ElevatorSimulation.Core.Services
{
    public class FloorService : IFloorService
    {
        private readonly Dictionary<int, Floor> floors;

        public FloorService(int totalFloors)
        {
            floors = new Dictionary<int, Floor>();
            for (int i = 0; i < totalFloors; i++)
            {
                floors[i] = new Floor(i);
            }
        }

        public void UpdateWaitingPassengers(int floorNumber, int count)
        {
            if (floors.ContainsKey(floorNumber))
            {
                var floor = floors[floorNumber];
                floor.WaitingPassengers = count;
            }
        }

        public Floor GetFloor(int floorNumber)
        {
            return floors.ContainsKey(floorNumber) ? floors[floorNumber] : null;
        }

        public IEnumerable<Floor> GetAllFloors()
        {
            return floors.Values;
        }
    }
}
