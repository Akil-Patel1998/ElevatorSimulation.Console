using System;
using System.Threading.Tasks;
using ElevatorSimulation.Core.Interfaces;
using ElevatorSimulation.Core.Services;

namespace ElevatorSimulation
{
    class Program
    {
        static async Task Main(string[] args)
        {
            int totalFloors = 10; // Set total number of floors
            int numberOfElevators = 3; // Set number of elevators
            int elevatorCapacity = 8; // Set elevator capacity (number of passengers it can hold)

            var floorService = new FloorService(totalFloors);
            var elevatorService = new ElevatorService(numberOfElevators, elevatorCapacity, floorService);

            while (true)
            {
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
                        Console.Write("Enter the floor number to call the elevator to: ");
                        var floorNumber = int.Parse(Console.ReadLine());
                        Console.Write("Enter the number of passengers waiting: ");
                        var numPassengers = int.Parse(Console.ReadLine());
                        Console.Write("Enter destination floors (comma-separated): ");
                        var destinationFloorsInput = Console.ReadLine();
                        var destinationFloors = destinationFloorsInput.Split(',').Select(int.Parse).ToList();

                        floorService.UpdateWaitingPassengers(floorNumber, numPassengers, destinationFloors);
                        await elevatorService.DispatchElevatorAsync(floorNumber, numPassengers, destinationFloors);
                        break;

                    case "2":
                        Console.Write("Enter the floor number to update passengers: ");
                        var updateFloorNumber = int.Parse(Console.ReadLine());
                        Console.Write("Enter the number of passengers waiting: ");
                        var updateNumPassengers = int.Parse(Console.ReadLine());
                        Console.Write("Enter destination floors (comma-separated): ");
                        var updateDestFloorsInput = Console.ReadLine();
                        var updateDestFloors = updateDestFloorsInput.Split(',').Select(int.Parse).ToList();

                        floorService.UpdateWaitingPassengers(updateFloorNumber, updateNumPassengers, updateDestFloors);
                        break;

                    case "3":
                        elevatorService.ShowElevatorStatus();
                        break;

                    case "4":
                        foreach (var floor in floorService.GetAllFloors())
                        {
                            Console.WriteLine($"Floor {floor.FloorNumber}: People Waiting: {floor.WaitingPassengers}, Destination Floors: {string.Join(",", floor.DestinationFloors)}");
                        }
                        break;

                    case "5":
                        elevatorService.AddElevator();
                        break;

                    case "6":
                        return;

                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
    }
}
