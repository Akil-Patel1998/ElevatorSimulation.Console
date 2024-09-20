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
        private readonly IFloorService floorService;
        private readonly IPassengerService passengerService;
        private readonly int minFloor;
        private readonly int maxFloor;

        public ElevatorService(
            int numberOfElevators,
            int elevatorCapacity,
            IFloorService floorService,
            IPassengerService passengerService,
            int minFloor = 0,
            int maxFloor = 10)
        {
            this.floorService = floorService;
            this.passengerService = passengerService;
            this.minFloor = minFloor;
            this.maxFloor = maxFloor;

            elevators = new List<Elevator>();
            InitializeElevators(numberOfElevators, elevatorCapacity);
        }

        private void InitializeElevators(int numberOfElevators, int elevatorCapacity)
        {
            for (int i = 0; i < numberOfElevators; i++)
            {
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

        public Elevator FindNearestAvailableElevator(int targetFloor, int waitingPassengers)
        {
            return elevators
                .Where(e => !e.IsMoving && CanBoard(e, waitingPassengers))
                .OrderBy(e => Math.Abs(e.CurrentFloor - targetFloor))
                .FirstOrDefault();
        }

        public void ShowElevatorStatus()
        {
            foreach (var elevator in elevators)
            {
                Console.WriteLine($"Elevator {elevator.Id} ({elevator.GetType().Name}): " +
                                  $"Floor {elevator.CurrentFloor}, Moving: {elevator.IsMoving}, " +
                                  $"Direction: {elevator.Direction}, People On Board: {elevator.PeopleOnBoard}, " +
                                  $"Capacity: {elevator.Capacity}");
            }
        }

        public void AddElevator(string elevatorType)
        {
            var newId = elevators.Any() ? elevators.Max(e => e.Id) + 1 : 0;
            Elevator newElevator;

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
                    throw new ArgumentException("Invalid elevator type");
            }

            elevators.Add(newElevator);
            Console.WriteLine($"Added new {elevatorType} elevator with ID {newId}.");
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
                Console.WriteLine($"{elevator.GetType().Name} {elevator.Id} is already on floor {elevator.CurrentFloor}. No movement required.");
                return;
            }

            if (targetFloor < minFloor || targetFloor > maxFloor)
            {
                throw new ArgumentOutOfRangeException(nameof(targetFloor), "Target floor is out of range.");
            }

            elevator.IsMoving = true;
            elevator.Direction = targetFloor > elevator.CurrentFloor ? ElevatorDirection.Up : ElevatorDirection.Down;

            while (elevator.CurrentFloor != targetFloor)
            {
                Console.WriteLine($"{elevator.GetType().Name} {elevator.Id} is passing floor {elevator.CurrentFloor} going {elevator.Direction}.");
                elevator.CurrentFloor += (elevator.Direction == ElevatorDirection.Up) ? 1 : -1;

                await elevator.MoveToFloorAsync(elevator.CurrentFloor);

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

            Console.WriteLine($"{elevator.GetType().Name} {elevator.Id} has arrived at floor {elevator.CurrentFloor}.");
            elevator.IsMoving = false;
            elevator.Direction = ElevatorDirection.Stationary;
        }

        public async Task DispatchElevatorAsync(int sourceFloor, int waitingPassengers)
        {
            var remainingPassengers = waitingPassengers;
            var dispatchedElevators = new List<Elevator>();

            while (remainingPassengers > 0)
            {
                var availableElevators = elevators
                    .Where(e => !e.IsMoving && CanBoard(e, 1))
                    .OrderBy(e => Math.Abs(e.CurrentFloor - sourceFloor))
                    .ToList();

                if (!availableElevators.Any())
                {
                    Console.WriteLine("No available elevators that can board any additional passengers.");
                    break;
                }

                foreach (var elevator in availableElevators)
                {
                    if (remainingPassengers <= 0) break;

                    Console.WriteLine($"{elevator.GetType().Name} {elevator.Id} is on its way to floor {sourceFloor}.");
                    await MoveToFloorAsync(elevator, sourceFloor);

                    var passengersToBoard = Math.Min(remainingPassengers, elevator.Capacity - elevator.PeopleOnBoard);
                    if (passengersToBoard > 0)
                    {
                        Board(elevator, passengersToBoard);
                        Console.WriteLine($"{elevator.GetType().Name} {elevator.Id} boarded {passengersToBoard} passengers.");

                        var destinationFloors = new List<int>();
                        for (var i = 0; i < passengersToBoard; i++)
                        {
                            Console.Write($"Enter destination floor for passenger {i + 1}: ");
                            var destinationFloor = int.Parse(Console.ReadLine());
                            passengerService.AddPassenger(destinationFloor);
                            destinationFloors.Add(destinationFloor);
                        }

                        remainingPassengers -= passengersToBoard;
                        floorService.UpdateWaitingPassengers(sourceFloor, remainingPassengers);
                        dispatchedElevators.Add(elevator);

                        foreach (var destinationFloor in destinationFloors.Distinct())
                        {
                            Console.WriteLine($"{elevator.GetType().Name} {elevator.Id} is on its way to floor {destinationFloor}.");
                            await MoveToFloorAsync(elevator, destinationFloor);

                            var passengersToExit = passengerService.GetPassengersByFloor(destinationFloor).Count();
                            Exit(elevator, passengersToExit);
                            passengerService.RemovePassengersByFloor(destinationFloor);
                            Console.WriteLine($"{elevator.GetType().Name} {elevator.Id} exited {passengersToExit} passengers at floor {destinationFloor}.");
                        }
                    }
                }

                if (remainingPassengers > 0)
                {
                    Console.WriteLine("Still have remaining passengers. Trying with more elevators...");
                }
            }

            if (dispatchedElevators.Any())
            {
                Console.WriteLine("Dispatched elevators:");
                foreach (var elevator in dispatchedElevators)
                {
                    Console.WriteLine($"{elevator.GetType().Name} {elevator.Id} has completed its route.");
                }
            }
            else
            {
                Console.WriteLine("No elevators dispatched.");
            }
        }
    }
}