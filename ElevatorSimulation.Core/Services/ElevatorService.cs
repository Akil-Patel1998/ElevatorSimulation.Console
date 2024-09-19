using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElevatorSimulation.Core.Interfaces;
using ElevatorSimulation.Entities.Enums;
using ElevatorSimulation.Entities.Models;

namespace ElevatorSimulation.Core.Services
{
    public class ElevatorService : IElevatorService
    {
        private readonly List<Elevator> elevators;
        private readonly int elevatorCapacity;
        private readonly IFloorService floorService;
        private readonly int minFloor;
        private readonly int maxFloor;

        public ElevatorService(int numberOfElevators, int elevatorCapacity, IFloorService floorService, int minFloor = 0, int maxFloor = 10)
        {
            this.elevatorCapacity = elevatorCapacity;
            this.floorService = floorService;
            this.minFloor = minFloor;
            this.maxFloor = maxFloor;

            elevators = Enumerable.Range(0, numberOfElevators)
                                  .Select(i => new Elevator(i, elevatorCapacity))
                                  .ToList();
        }

        public Elevator FindNearestAvailableElevator(int targetFloor, int waitingPassenger)
        {
            return elevators
                .Where(e => !e.IsMoving && CanBoard(e, waitingPassenger))  // Check if the elevator can board waiting passengers
                .OrderBy(e => Math.Abs(e.CurrentFloor - targetFloor))
                .FirstOrDefault();
        }

        public void ShowElevatorStatus()
        {
            foreach (var elevator in elevators)
            {
                Console.WriteLine($"Elevator {elevator.Id}: Floor {elevator.CurrentFloor}, " +
                                  $"Moving: {elevator.IsMoving}, Direction: {elevator.Direction}, " +
                                  $"People On Board: {elevator.PeopleOnBoard}, Capacity: {elevator.Capacity}");
            }
        }

        public void AddElevator()
        {
            // Ensure the new elevator ID is unique
            var newId = elevators.Any() ? elevators.Max(e => e.Id) + 1 : 0;
            var newElevator = new Elevator(newId, elevatorCapacity);
            elevators.Add(newElevator);
            Console.WriteLine($"Added new elevator with ID {newId}.");
        }

        public List<Elevator> GetElevators()
        {
            return elevators;
        }

        public bool CanBoard(Elevator elevator, int passengers)
        {
            return elevator.PeopleOnBoard + passengers <= elevator.Capacity;
        }

        public void Board(Elevator elevator, int passengers)
        {
            if (CanBoard(elevator, passengers))
            {
                elevator.PeopleOnBoard += passengers;
                Console.WriteLine($"Elevator {elevator.Id} boarded {passengers} passengers.");
            }
            else
            {
                throw new InvalidOperationException("Elevator cannot board more passengers than its capacity.");
            }
        }

        public void Exit(Elevator elevator, int passengers)
        {
            if (passengers <= elevator.PeopleOnBoard)
            {
                elevator.PeopleOnBoard -= passengers;
                Console.WriteLine($"Elevator {elevator.Id} exited {passengers} passengers.");
            }
            else
            {
                throw new InvalidOperationException("Cannot remove more passengers than currently on board.");
            }
        }

       
         public async Task DispatchElevatorAsync(int targetFloor, int waitingPassenger)
         {
                var remainingPassengers = waitingPassenger;
                var dispatchedElevators = new List<Elevator>();

                while (remainingPassengers > 0)
                {
                    // Find all available elevators that can accommodate some passengers
                    var availableElevators = elevators
                        .Where(e => !e.IsMoving && CanBoard(e, 1)) // Check if the elevator can board at least one passenger
                        .OrderBy(e => Math.Abs(e.CurrentFloor - targetFloor)) // Sort by proximity to target floor
                        .ToList();

                    if (!availableElevators.Any())
                    {
                        Console.WriteLine("No available elevators that can board any additional passengers.");
                        break;
                    }

                    // Dispatch elevators to handle the remaining passengers
                    foreach (var elevator in availableElevators)
                    {
                        if (remainingPassengers <= 0) break;

                        // Move the elevator to the target floor
                        Console.WriteLine($"Elevator {elevator.Id} is on its way to floor {targetFloor}.");
                        floorService.UpdateWaitingPassengers(targetFloor, waitingPassenger);
                        await MoveToFloorAsync(elevator, targetFloor);
                   
                        // Determine how many passengers this elevator can board
                        var passengersToBoard = Math.Min(remainingPassengers, elevator.Capacity - elevator.PeopleOnBoard);
                        if (passengersToBoard > 0)
                        {
                            Board(elevator, passengersToBoard);
                            Console.WriteLine($"Elevator {elevator.Id} boarded {passengersToBoard} passengers.");
                            floorService.ClearWaitingPassengers(targetFloor,elevator.PeopleOnBoard);
                            remainingPassengers -= passengersToBoard;
                            dispatchedElevators.Add(elevator);
                        }
                    }

                    if (remainingPassengers > 0)
                    {
                        // If there are still remaining passengers, continue to try with the next available elevators
                        Console.WriteLine("Still have remaining passengers. Trying with more elevators...");
                    }
                }

                if (dispatchedElevators.Any())
                {
                    Console.WriteLine("Dispatched elevators:");
                    foreach (var elevator in dispatchedElevators)
                    {
                        Console.WriteLine($"Elevator {elevator.Id} has arrived at floor {elevator.CurrentFloor}.");
                    }
                }
                else
                {
                    Console.WriteLine("No elevators dispatched.");
                }
         }

        

        public async Task MoveToFloorAsync(Elevator elevator, int targetFloor)
        {
            if (targetFloor == elevator.CurrentFloor)
            {
                Console.WriteLine($"Elevator {elevator.Id} is already on floor {elevator.CurrentFloor}. No movement required.");
                return;
            }

            if (targetFloor < minFloor || targetFloor > maxFloor) // Flexible floor range
            {
                throw new ArgumentOutOfRangeException(nameof(targetFloor), "Target floor is out of range.");
            }

            elevator.IsMoving = true;
            elevator.Direction = targetFloor > elevator.CurrentFloor ? ElevatorDirection.Up : ElevatorDirection.Down;

            while (elevator.CurrentFloor != targetFloor)
            {
                Console.WriteLine($"Elevator {elevator.Id} is passing floor {elevator.CurrentFloor} going {elevator.Direction}.");
                elevator.CurrentFloor += (elevator.Direction == ElevatorDirection.Up) ? 1 : -1;

                // Simulate delay for realistic movement
                await Task.Delay(1000);

                var currentFloorStatus = floorService.GetFloor(elevator.CurrentFloor);
                if (currentFloorStatus != null)
                {
                    Console.WriteLine($"Floor {elevator.CurrentFloor}: People Waiting: {currentFloorStatus.WaitingPassengers}");
                }
                else
                {
                    Console.WriteLine($"Floor {elevator.CurrentFloor}: Status unknown.");
                }
            }

            Console.WriteLine($"Elevator {elevator.Id} has arrived at floor {elevator.CurrentFloor}.");
            elevator.IsMoving = false;
            elevator.Direction = ElevatorDirection.Stationary;
        }
    }
}
