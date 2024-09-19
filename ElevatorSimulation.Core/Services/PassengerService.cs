using System.Collections.Generic;
using ElevatorSimulation.Core.Interfaces;
using ElevatorSimulation.Entities.Models;

namespace ElevatorSimulation.Core.Services
{
    public class PassengerService : IPassengerService
    {
        private readonly List<Passenger> passengers;
        private int nextId;

        public PassengerService()
        {
            passengers = new List<Passenger>();
            nextId = 1; // Start IDs from 1
        }

        public void AddPassenger(int destinationFloor)
        {
            var passenger = new Passenger(nextId++, destinationFloor);
            passengers.Add(passenger);
        }

        public void RemovePassenger(Passenger passenger)
        {
            passengers.Remove(passenger);
        }

        public IEnumerable<Passenger> GetAllPassengers()
        {
            return passengers;
        }

        public IEnumerable<Passenger> GetPassengersByFloor(int floorNumber)
        {
            return passengers.Where(p => p.DestinationFloor == floorNumber);
        }
    }
}
