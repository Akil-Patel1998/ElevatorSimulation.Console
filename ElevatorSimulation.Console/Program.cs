using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElevatorSimulation.Core.Interfaces;
using ElevatorSimulation.Core.Services;
using ElevatorSimulation.Entities.Models;

namespace ElevatorSimulation
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Initialize the simulation
            int totalFloors = 10;
            int numberOfElevators = 3;
            int elevatorCapacity = 8;
            var floorService = new FloorService(totalFloors);
            var passengerService = new PassengerService();
            var elevatorService = new ElevatorService(numberOfElevators, elevatorCapacity, floorService, passengerService);

            // Main loop
            while (true)
            {
                // Display menu
                Console.WriteLine("Elevator Control System");
                Console.WriteLine("1. Call an elevator");
                Console.WriteLine("2. Update passengers on a floor");
                Console.WriteLine("3. Show elevator status");
                Console.WriteLine("4. Show floor status");
                Console.WriteLine("5. Add a new elevator");
                Console.WriteLine("6. Exit");
                Console.Write("Select an option: ");
                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        // Call an elevator
                        Console.Write("Enter the floor number to call the elevator to: ");
                        var floorNumber = int.Parse(Console.ReadLine());
                        Console.Write("Enter the number of passengers waiting: ");
                        var numPassengers = int.Parse(Console.ReadLine());
                        await elevatorService.DispatchElevatorAsync(floorNumber, numPassengers);
                        break;
                    case "2":
                        // Update passengers on a floor
                        Console.Write("Enter the floor number to update passengers: ");
                        var updateFloorNumber = int.Parse(Console.ReadLine());
                        Console.Write("Enter the number of passengers waiting: ");
                        var updateNumPassengers = int.Parse(Console.ReadLine());
                        Console.Write("Enter destination floors (comma-separated): ");
                        var updateDestFloorsInput = Console.ReadLine();
                        var updateDestFloors = updateDestFloorsInput.Split(',').Select(int.Parse).ToList();
                        floorService.UpdateWaitingPassengers(updateFloorNumber, updateNumPassengers);
                        break;
                    case "3":
                        // Show elevator status
                        elevatorService.ShowElevatorStatus();
                        break;
                    case "4":
                        // Show floor status
                        foreach (var floor in floorService.GetAllFloors())
                        {
                            Console.WriteLine($"Floor {floor.FloorNumber}: People Waiting: {floor.WaitingPassengers}");
                        }
                        break;
                    case "5":
                        // Add a new elevator
                        Console.Write("Enter elevator type (standard/highspeed/glass/freight): ");
                        var elevatorType = Console.ReadLine();
                        elevatorService.AddElevator(elevatorType);
                        break;
                    case "6":
                        // Exit the program
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
    }
}