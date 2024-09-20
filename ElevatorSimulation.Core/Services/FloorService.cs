using ElevatorSimulation.Core.Interfaces; 
using ElevatorSimulation.Entities.Models; 


namespace ElevatorSimulation.Core.Services
{
    public class FloorService : IFloorService
    {
        private readonly Dictionary<int, Floor> floors; // Dictionary to hold floor objects, keyed by floor number

        // Constructor to initialize the floor service with a specified number of floors
        public FloorService(int totalFloors)
        {
            floors = new Dictionary<int, Floor>(); // Initialize the floors dictionary
            for (int i = 0; i < totalFloors; i++)
            {
                floors[i] = new Floor(i); // Create a new Floor object for each floor and add it to the dictionary
            }
        }

        // Method to update the count of waiting passengers on a specific floor
        public void UpdateWaitingPassengers(int floorNumber, int count)
        {
            // Check if the specified floor exists
            if (floors.ContainsKey(floorNumber))
            {
                var floor = floors[floorNumber]; // Get the floor object
                floor.WaitingPassengers = count; // Update the waiting passengers count
            }
        }

        // Method to retrieve a specific floor by its number
        public Floor GetFloor(int floorNumber)
        {
            // Return the floor if it exists; otherwise, return null
            return floors.ContainsKey(floorNumber) ? floors[floorNumber] : null;
        }

        // Method to get all floors in the building
        public IEnumerable<Floor> GetAllFloors()
        {
            return floors.Values; // Return the collection of all floor objects
        }
    }
}
