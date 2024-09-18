using System;
using System.Threading.Tasks;
using ElevatorSimulation.Core.Services;

namespace ElevatorSimulation
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Initialize services
            var floorService = new FloorService(10);
            var elevatorService = new ElevatorService(3, 20, floorService);

            // Create some floors and add passengers to simulate real-time scenario
            var floor5 = floorService.GetFloor(5);
            floorService.AddPassengers(floor5,15); // Add more passengers than an elevator can handle

            Console.WriteLine("Initial Elevator Status:");
            elevatorService.ShowElevatorStatus();

            Console.WriteLine("\nRequesting elevator to floor 5...");
            var elevator = elevatorService.FindNearestAvailableElevator(5, floor5.WaitingPassengers);
            if (elevator != null)
            {
                Console.WriteLine($"Elevator {elevator.Id} is assigned to floor 5.");

                // Move elevator to floor 5
                await elevatorService.MoveToFloorAsync(elevator, 5);

                // Simulate boarding passengers
                var passengersAtFloor5 = floor5.WaitingPassengers;
                if (elevatorService.CanBoard(elevator, passengersAtFloor5))
                {
                    elevatorService.Board(elevator, passengersAtFloor5);
                    floorService.RemovePassengers(floor5,passengersAtFloor5);
                    Console.WriteLine($"Elevator {elevator.Id} has boarded {passengersAtFloor5} passengers.");
                }
                else
                {
                    Console.WriteLine("Elevator cannot board all passengers due to capacity constraints.");
                    // Optionally add more elevators if needed
                    elevatorService.AddElevator();
                    Console.WriteLine("Added a new elevator.");
                }

                // Move elevator to floor 2
                await elevatorService.MoveToFloorAsync(elevator, 2);

                // Simulate passengers exiting at floor 2
                var passengersToExit = 2; // Example number of passengers to exit
                elevatorService.Exit(elevator, passengersToExit);
                Console.WriteLine($"Elevator {elevator.Id} has dropped off {passengersToExit} passengers at floor 2.");

                elevatorService.ShowElevatorStatus();
            }
            else
            {
                Console.WriteLine("No available elevator found.");
            }
        }
    }
}
