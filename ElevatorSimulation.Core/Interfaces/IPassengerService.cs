using ElevatorSimulation.Entities.Models;

namespace ElevatorSimulation.Core.Interfaces
{
    public interface IPassengerService
    {
        void AddPassenger(int destinationFloor);
        void RemovePassenger(Passenger passenger);
        IEnumerable<Passenger> GetAllPassengers();
        IEnumerable<Passenger> GetPassengersByFloor(int floorNumber);
    }
}
