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
                        int targetFloor = int.Parse(Console.ReadLine());
                        Console.Write("Enter the number of passengers waiting: ");
                        int waitingPassengers = int.Parse(Console.ReadLine());

                        await elevatorService.DispatchElevatorAsync(targetFloor, waitingPassengers);
                        break;

                    case "2":
                        Console.Write("Enter the floor number to update: ");
                        int updateFloor = int.Parse(Console.ReadLine());
                        Console.Write("Enter the number of waiting passengers: ");
                        int numberOfPassengers = int.Parse(Console.ReadLine());

                        floorService.UpdateWaitingPassengers(updateFloor, numberOfPassengers);
                        break;

                    case "3":
                        elevatorService.ShowElevatorStatus();
                        break;

                    case "4":
                        for (int i = 0; i <= totalFloors; i++)
                        {
                            var floor = floorService.GetFloor(i);
                            if (floor != null)
                            {
                                Console.WriteLine($"Floor {floor.FloorNumber}: People Waiting: {floor.WaitingPassengers}");
                            }
                        }
                        break;

                    case "5":
                        elevatorService.AddElevator();
                        break;

                    case "6":
                        return;

                    default:
                        Console.WriteLine("Invalid option. Please select a valid option.");
                        break;
                }
            }
        }
    }
}
