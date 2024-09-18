using System.Collections.Generic;
using ElevatorSimulation.Core.Interfaces;
using ElevatorSimulation.Entities.Models;

namespace ElevatorSimulation.Core.Services
{
    public class PassengerService : IPassengerService
    {
        private readonly List<Passenger> passengers;

        public PassengerService()
        {
            passengers = new List<Passenger>();
        }

        public void AddPassenger(Passenger passenger)
        {
            passengers.Add(passenger);
        }

        public void RemovePassenger(Passenger passenger)
        {
            passengers.Remove(passenger);
        }
    }
}
