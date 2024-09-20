using ElevatorSimulation.Core.Interfaces;
using ElevatorSimulation.Entities.Enums;
using ElevatorSimulation.Entities.Models;

namespace ElevatorSimulation.Core.Services
{
    public class ElevatorService : IElevatorService
    {
        private readonly List<Elevator> elevators; // List to store elevators
        private readonly IFloorService floorService; // Service to manage floor operations
        private readonly IPassengerService passengerService; // Service to manage passenger operations
        private readonly int minFloor; // Minimum floor number
        private readonly int maxFloor; // Maximum floor number

        // Constructor to initialize the elevator service
        public ElevatorService(int numberOfElevators, int elevatorCapacity, IFloorService floorService, IPassengerService passengerService, int minFloor = 0, int maxFloor = 10)
        {
            this.floorService = floorService;
            this.passengerService = passengerService;
            this.minFloor = minFloor;
            this.maxFloor = maxFloor;

            elevators = new List<Elevator>(); // Initialize the elevator list
            InitializeElevators(numberOfElevators, elevatorCapacity); // Create elevators
        }

        // Method to initialize elevators based on specified parameters
        private void InitializeElevators(int numberOfElevators, int elevatorCapacity)
        {
            for (int i = 0; i < numberOfElevators; i++)
            {
                // Add different types of elevators based on the index
                switch (i % 4)
                {
                    case 0:
                        elevators.Add(new StandardElevator(i, elevatorCapacity));
                        break;
                    case 1:
                        elevators.Add(new HighSpeedElevator(i, elevatorCapacity));
                        break;
                    case 2:
                        elevators.Add(new GlassElevator(i, elevatorCapacity));
                        break;
                    case 3:
                        elevators.Add(new FreightElevator(i, elevatorCapacity, elevatorCapacity * 200));
                        break;
                }
            }
        }

        // Method to find the nearest available elevator to a specified floor
        public Elevator FindNearestAvailableElevator(int targetFloor, int waitingPassengers)
        {
            return elevators
                .Where(e => !e.IsMoving && CanBoard(e, waitingPassengers)) // Filter for available elevators
                .OrderBy(e => Math.Abs(e.CurrentFloor - targetFloor)) // Order by distance to target floor
                .FirstOrDefault(); // Return the nearest one
        }

        // Method to display the status of all elevators
        public void ShowElevatorStatus()
        {
            foreach (var elevator in elevators)
            {
                // Print elevator details
                Console.WriteLine($"Elevator {elevator.Id} ({elevator.GetType().Name}): " +
                                  $"Floor {elevator.CurrentFloor}, Moving: {elevator.IsMoving}, " +
                                  $"Direction: {elevator.Direction}, People On Board: {elevator.PeopleOnBoard}, " +
                                  $"Capacity: {elevator.Capacity}");
            }
        }

        // Method to add a new elevator of a specified type
        public void AddElevator(string elevatorType)
        {
            // Determine new elevator ID
            var newId = elevators.Any() ? elevators.Max(e => e.Id) + 1 : 1;
            Elevator newElevator; // Declare a new elevator

            // Create a new elevator based on the specified type
            switch (elevatorType.ToLower())
            {
                case "standard":
                    newElevator = new StandardElevator(newId, 10);
                    break;
                case "highspeed":
                    newElevator = new HighSpeedElevator(newId, 15);
                    break;
                case "glass":
                    newElevator = new GlassElevator(newId, 8);
                    break;
                case "freight":
                    newElevator = new FreightElevator(newId, 5, 2000);
                    break;
                default:
                    throw new ArgumentException("Invalid elevator type"); // Handle invalid elevator type
            }

            elevators.Add(newElevator); // Add new elevator to the list
            Console.WriteLine($"Added new {elevatorType} elevator with ID {newId}.");
        }

        // Method to get the list of all elevators
        public List<Elevator> GetElevators()
        {
            return elevators; // Return the elevator list
        }

        // Method to check if an elevator can board a specified number of passengers
        public bool CanBoard(Elevator elevator, int passengers)
        {
            // Check if the number of passengers does not exceed capacity
            return elevator.PeopleOnBoard + passengers <= elevator.Capacity;
        }

        // Method to board passengers onto an elevator
        public void Board(Elevator elevator, int passengers)
        {
            if (CanBoard(elevator, passengers))
            {
                elevator.PeopleOnBoard += passengers; // Increase passenger count
                Console.WriteLine($"Elevator {elevator.Id} boarded {passengers} passengers.");
            }
            else
            {
                throw new InvalidOperationException("Elevator cannot board more passengers than its capacity."); // Handle overflow
            }
        }

        // Method to exit passengers from an elevator
        public void Exit(Elevator elevator, int passengers)
        {
            if (passengers <= elevator.PeopleOnBoard)
            {
                elevator.PeopleOnBoard -= passengers; // Decrease passenger count
                Console.WriteLine($"Elevator {elevator.Id} exited {passengers} passengers.");
            }
            else
            {
                throw new InvalidOperationException("Cannot remove more passengers than currently on board."); // Handle underflow
            }
        }

        // Asynchronous method to move an elevator to a specified floor
        public async Task MoveToFloorAsync(Elevator elevator, int targetFloor)
        {
            if (targetFloor == elevator.CurrentFloor)
            {
                Console.WriteLine($"{elevator.GetType().Name} {elevator.Id} is already on floor {elevator.CurrentFloor}. No movement required.");
                return; // No movement needed
            }

            // Check if the target floor is within valid range
            if (targetFloor < minFloor || targetFloor > maxFloor)
            {
                throw new ArgumentOutOfRangeException(nameof(targetFloor), "Target floor is out of range.");
            }

            elevator.IsMoving = true; // Set elevator as moving
            elevator.Direction = targetFloor > elevator.CurrentFloor ? ElevatorDirection.Up : ElevatorDirection.Down; // Determine direction

            // Call the elevator's own method to handle movement
            await elevator.MoveToFloorAsync(targetFloor);

            Console.WriteLine($"{elevator.GetType().Name} {elevator.Id} has arrived at floor {elevator.CurrentFloor}.");
            elevator.IsMoving = false; // Mark elevator as not moving
            elevator.Direction = ElevatorDirection.Stationary; // Reset direction
        }

        // Asynchronous method to dispatch elevators to pick up passengers
        public async Task DispatchElevatorAsync(int sourceFloor, int waitingPassengers)
        {
            var remainingPassengers = waitingPassengers; // Track remaining passengers
            var dispatchedElevators = new List<Elevator>(); // List to track dispatched elevators

            // Loop until all passengers are boarded
            while (remainingPassengers > 0)
            {
                // Get available elevators that can board at least one passenger
                var availableElevators = elevators
                    .Where(e => !e.IsMoving && CanBoard(e, 1))
                    .OrderBy(e => Math.Abs(e.CurrentFloor - sourceFloor))
                    .ToList();

                if (!availableElevators.Any())
                {
                    Console.WriteLine("No available elevators that can board any additional passengers.");
                    break; // Break if no elevators are available
                }

                foreach (var elevator in availableElevators)
                {
                    if (remainingPassengers <= 0) break; // Exit if no remaining passengers

                    Console.WriteLine($"{elevator.GetType().Name} {elevator.Id} is on its way to floor {sourceFloor}.");
                    await MoveToFloorAsync(elevator, sourceFloor); // Move elevator to pick up passengers

                    // Determine how many passengers can board
                    var passengersToBoard = Math.Min(remainingPassengers, elevator.Capacity - elevator.PeopleOnBoard);
                    if (passengersToBoard > 0)
                    {
                        Board(elevator, passengersToBoard); // Board the passengers
                        Console.WriteLine($"{elevator.GetType().Name} {elevator.Id} boarded {passengersToBoard} passengers.");

                        var destinationFloors = new List<int>(); // List to track destination floors
                        for (var i = 0; i < passengersToBoard; i++)
                        {
                            int destinationFloor;
                            // Get valid destination floor from user
                            while (true)
                            {
                                Console.Write($"Enter destination floor for passenger {i + 1}: ");
                                if (int.TryParse(Console.ReadLine(), out destinationFloor) &&
                                    destinationFloor >= minFloor && destinationFloor <= maxFloor)
                                {
                                    break; // Valid input
                                }
                                Console.WriteLine($"Please enter a valid floor between {minFloor} and {maxFloor}.");
                            }

                            passengerService.AddPassenger(destinationFloor); // Add passenger to the service
                            destinationFloors.Add(destinationFloor); // Track the destination floor
                        }

                        remainingPassengers -= passengersToBoard; // Decrease remaining passengers
                        floorService.UpdateWaitingPassengers(sourceFloor, remainingPassengers); // Update waiting passengers
                        dispatchedElevators.Add(elevator); // Track dispatched elevator

                        // Move to each destination floor and exit passengers
                        foreach (var destinationFloor in destinationFloors.Distinct())
                        {
                            Console.WriteLine($"{elevator.GetType().Name} {elevator.Id} is on its way to floor {destinationFloor}.");
                            await MoveToFloorAsync(elevator, destinationFloor); // Move elevator to destination
                            Exit(elevator, passengersToBoard); // Exit passengers
                        }
                    }
                }
            }

            // Show status of all elevators after dispatching
            ShowElevatorStatus();
        }
    }
}
