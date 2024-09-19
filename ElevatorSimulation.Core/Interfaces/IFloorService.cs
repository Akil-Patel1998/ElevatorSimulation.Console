using ElevatorSimulation.Entities.Models;
using ElevatorSimulation.Core.Interfaces;

namespace ElevatorSimulation.Core.Interfaces
{
    public interface IFloorService
    {
        // Updates the number of waiting passengers and their destination floors at a specific floor
        void UpdateWaitingPassengers(int floorNumber, int count, List<int> destinationFloors);

        // Clears all waiting passengers at a specific floor
        void ClearWaitingPassengers(int floorNumber);
        // Retrieves the floor information
        Floor GetFloor(int floorNumber);
        // Gets all floors
        IEnumerable<Floor> GetAllFloors();
    }
}
