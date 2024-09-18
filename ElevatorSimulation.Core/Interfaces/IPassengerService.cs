using ElevatorSimulation.Entities.Models;

namespace ElevatorSimulation.Core.Interfaces
{
    public interface IPassengerService
    {
        void AddPassenger(Passenger passenger);
        void RemovePassenger(Passenger passenger);
    }
}
