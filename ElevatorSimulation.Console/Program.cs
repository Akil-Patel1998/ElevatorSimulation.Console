using ElevatorSimulation.Core.Services;

namespace ElevatorSimulation
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Initialize the simulation with total floors and elevator configurations
            int totalFloors = 10; // Total number of floors in the building
            int numberOfElevators = 3; // Number of elevators in the system
            int elevatorCapacity = 8; // Capacity of each elevator

            // Create instances of floor, passenger, and elevator services
            var floorService = new FloorService(totalFloors);
            var passengerService = new PassengerService();
            var elevatorService = new ElevatorService(numberOfElevators, elevatorCapacity, floorService, passengerService);

            // Main loop for user interaction
            while (true)
            {
                // Display menu options to the user
                Console.WriteLine("Elevator Control System");
                Console.WriteLine("1. Call an elevator");
                Console.WriteLine("2. Update passengers on a floor");
                Console.WriteLine("3. Show elevator status");
                Console.WriteLine("4. Show floor status");
                Console.WriteLine("5. Add a new elevator");
                Console.WriteLine("6. Exit");
                Console.Write("Select an option: ");
                var option = Console.ReadLine(); // Get user input

                switch (option)
                {
                    case "1":
                        // Call an elevator to a specified floor
                        Console.Write("Enter the floor number to call the elevator to: ");
                        var floorNumber = int.Parse(Console.ReadLine()); // Parse floor number input
                        Console.Write("Enter the number of passengers waiting: ");
                        var numPassengers = int.Parse(Console.ReadLine()); // Parse number of waiting passengers
                        await elevatorService.DispatchElevatorAsync(floorNumber, numPassengers); // Dispatch the elevator
                        break;

                    case "2":
                        // Update the number of passengers waiting on a specified floor
                        Console.Write("Enter the floor number to update passengers: ");
                        var updateFloorNumber = int.Parse(Console.ReadLine()); // Parse floor number
                        Console.Write("Enter the number of passengers waiting: ");
                        var updateNumPassengers = int.Parse(Console.ReadLine()); // Parse updated passenger count
                        Console.Write("Enter destination floors (comma-separated): ");
                        var updateDestFloorsInput = Console.ReadLine(); // Get destination floors as input
                        var updateDestFloors = updateDestFloorsInput.Split(',').Select(int.Parse).ToList(); // Parse input into list
                        floorService.UpdateWaitingPassengers(updateFloorNumber, updateNumPassengers); // Update waiting passengers
                        break;

                    case "3":
                        // Display the status of all elevators
                        elevatorService.ShowElevatorStatus();
                        break;

                    case "4":
                        // Display the status of all floors
                        foreach (var floor in floorService.GetAllFloors())
                        {
                            Console.WriteLine($"Floor {floor.FloorNumber}: People Waiting: {floor.WaitingPassengers}");
                        }
                        break;

                    case "5":
                        // Add a new elevator of a specified type
                        Console.Write("Enter elevator type (standard/highspeed/glass/freight): ");
                        var elevatorType = Console.ReadLine(); // Get elevator type input
                        elevatorService.AddElevator(elevatorType); // Add the new elevator
                        break;

                    case "6":
                        // Exit the program
                        return; // Terminate the main loop

                    default:
                        // Handle invalid input
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
    }
}
