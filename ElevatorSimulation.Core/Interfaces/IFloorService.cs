using ElevatorSimulation.Entities.Models;
using ElevatorSimulation.Core.Interfaces;

namespace ElevatorSimulation.Core.Interfaces
{
    public interface IFloorService
    {
        Floor GetFloor(int floorNumber);
        void UpdateWaitingPassengers(int floorNumber, int numberOfPassengers);
        void  ClearWaitingPassengers(int floorNumber, int peopleOnBoard);
    }
}
