using System.Collections.Generic;
using ElevatorSimulation.Core.Interfaces;
using ElevatorSimulation.Entities.Models; 

namespace ElevatorSimulation.Core.Services
{
    public class PassengerService : IPassengerService
    {
        private readonly List<Passenger> passengers; // List to store all passengers
        private int nextId; // Variable to generate the next passenger ID

        // Constructor to initialize the passenger service
        public PassengerService()
        {
            passengers = new List<Passenger>(); // Initialize the list of passengers
            nextId = 1; // Start IDs from 1
        }

        // Method to add a new passenger with a specified destination floor
        public void AddPassenger(int destinationFloor)
        {
            var passenger = new Passenger(nextId++, destinationFloor); // Create a new passenger object
            passengers.Add(passenger); // Add the passenger to the list
        }

        // Method to remove a specified passenger from the list
        public void RemovePassenger(Passenger passenger)
        {
            passengers.Remove(passenger); // Remove the passenger from the list
        }

        // Method to retrieve all passengers
        public IEnumerable<Passenger> GetAllPassengers()
        {
            return passengers; // Return the list of all passengers
        }

        // Method to get passengers by their destination floor
        public IEnumerable<Passenger> GetPassengersByFloor(int floorNumber)
        {
            return passengers.Where(p => p.DestinationFloor == floorNumber); // Filter passengers by destination floor
        }

        // Method to remove all passengers whose destination is a specific floor
        public void RemovePassengersByFloor(int floorNumber)
        {
            passengers.RemoveAll(p => p.DestinationFloor == floorNumber); // Remove passengers with the specified destination floor
        }
    }
}
