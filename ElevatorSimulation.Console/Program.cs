using System;
using System.Threading.Tasks;
using ElevatorSimulation.Core.Services;
using ElevatorSimulation.Entities.Models;
using ElevatorSimulation.Core.Interfaces;

namespace ElevatorSimulation
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Initialize services
            var floorService = new FloorService(10); // You can adjust the number of floors
            var elevatorService = new ElevatorService(3, 10, floorService); // Adjust elevator capacity and number

            while (true)
            {
                Console.WriteLine("Elevator Control System");
                Console.WriteLine("1. Call an elevator");
                Console.WriteLine("2. Update passengers on a floor");
                Console.WriteLine("3. Show elevator status");
                Console.WriteLine("4. Exit");
                Console.Write("Select an option: ");

                var input = Console.ReadLine();
                if (input == "4")
                {
                    break; // Exit the loop and end the program
                }

                switch (input)
                {
                    case "1":
                        await CallElevatorAsync(elevatorService, floorService);
                        break;
                    case "2":
                        UpdatePassengers(floorService);
                        break;
                    case "3":
                        ShowElevatorStatus(elevatorService);
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        private static async Task CallElevatorAsync(ElevatorService elevatorService, FloorService floorService)
        {
            Console.Write("Enter the floor number to call the elevator to (0-10): ");
            if (int.TryParse(Console.ReadLine(), out int targetFloor) && targetFloor >= 0 && targetFloor < 10)
            {
                var floor = floorService.GetFloor(targetFloor);
                Console.Write("Enter the number of passengers waiting: ");
                if (int.TryParse(Console.ReadLine(), out int waitingPassengers) && waitingPassengers >= 0)
                {
                    var elevator = elevatorService.FindNearestAvailableElevator(targetFloor, waitingPassengers);
                    if (elevator != null)
                    {
                        floorService.AddPassengers(floor, waitingPassengers);
                        Console.WriteLine($"Elevator {elevator.Id} is on its way to floor {targetFloor}.");
                        await elevatorService.MoveToFloorAsync(elevator, targetFloor);

                        if (elevatorService.CanBoard(elevator, waitingPassengers))
                        {
                            elevatorService.Board(elevator, waitingPassengers);
                            floor.WaitingPassengers = 0; // Assuming all passengers board
                            Console.WriteLine($"Elevator {elevator.Id} has boarded {waitingPassengers} passengers.");

                        }
                        else
                        {
                            Console.WriteLine("Elevator cannot board all passengers due to capacity constraints.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No available elevator found.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid number of passengers. Please enter a non-negative number.");
                }
            }
            else
            {
                Console.WriteLine("Invalid floor number. Please enter a number between 0 and 9.");
            }
        }

        private static void UpdatePassengers(FloorService floorService)
        {
            Console.Write("Enter the floor number to update passengers (0-10): ");
            if (int.TryParse(Console.ReadLine(), out int floorNumber) && floorNumber >= 0 && floorNumber < 10)
            {
                var floor = floorService.GetFloor(floorNumber);
                Console.Write("Enter the number of passengers to add: ");
                if (int.TryParse(Console.ReadLine(), out int numberOfPassengers) && numberOfPassengers >= 0)
                {
                    floorService.AddPassengers(floor, numberOfPassengers);
                    Console.WriteLine($"Added {numberOfPassengers} passengers to floor {floorNumber}.");
                }
                else
                {
                    Console.WriteLine("Invalid number of passengers. Please enter a non-negative number.");
                }
            }
            else
            {
                Console.WriteLine("Invalid floor number. Please enter a number between 0 and 9.");
            }
        }

        private static void ShowElevatorStatus(ElevatorService elevatorService)
        {
            Console.WriteLine("Elevator Status:");
            foreach (var elevator in elevatorService.GetElevators())
            {
                Console.WriteLine($"Elevator {elevator.Id}: Current Floor = {elevator.CurrentFloor}, Passengers on Board = {elevator.PeopleOnBoard}");
            }
        }
    }
}
