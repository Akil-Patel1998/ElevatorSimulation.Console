using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;  // For asynchronous tasks
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
