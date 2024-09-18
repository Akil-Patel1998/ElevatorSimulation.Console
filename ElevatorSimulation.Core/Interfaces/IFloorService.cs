using ElevatorSimulation.Entities.Models;

namespace ElevatorSimulation.Core.Interfaces
{
    public interface IFloorService
    {
        Floor GetFloor(int floorNumber);
        void UpdateFloor(int floorNumber, int numberOfPassengers);
        void AddPassengers(Floor floor, int numberOfPassengers);
        void RemovePassengers(Floor floor, int numberOfPassengers);
    }
}
