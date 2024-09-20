using ElevatorSimulation.Entities.Models;
using ElevatorSimulation.Core.Interfaces;
using System.Security.Cryptography;

namespace ElevatorSimulation.Core.Interfaces
{
    public interface IFloorService
    {
        Floor GetFloor(int floorNumber);
        void UpdateWaitingPassengers(int floorNumber, int waitingPassengers);
        IEnumerable<Floor> GetAllFloors();
    }
}
