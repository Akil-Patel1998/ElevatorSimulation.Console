using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using ElevatorSimulation.Core.Services;
using ElevatorSimulation.Entities.Models;
using ElevatorSimulation.Core.Interfaces;

namespace ElevatorSimulation.Tests
{
    [TestFixture]
    public class ElevatorServiceTests
    {
        private ElevatorService elevatorService;
        private MockFloorService floorService;

        [SetUp]
        public void Setup()
        {
            floorService = new MockFloorService(10); // Mock service with 10 floors
            elevatorService = new ElevatorService(3, 10, floorService); // 3 elevators with a max capacity of 10 passengers
        }

        [Test]
        public void FindNearestAvailableElevator_ReturnsNearestElevator()
        {
            var elevator1 = elevatorService.FindNearestAvailableElevator(5, 5);
            Assert.NotNull(elevator1, "No elevator found.");

            var elevator2 = elevatorService.FindNearestAvailableElevator(5, 15);
            Assert.IsNull(elevator2, "Elevator should not be available due to capacity constraints.");
        }

   
        [Test]
        public void Board_AddsPassengersCorrectly()
        {
            var elevator = elevatorService.FindNearestAvailableElevator(0, 5);
            elevatorService.Board(elevator, 5);
            Assert.AreEqual(5, elevator.PeopleOnBoard, "Elevator should have 5 passengers on board.");

            Assert.Throws<InvalidOperationException>(() => elevatorService.Board(elevator, 6), "Elevator should not board more passengers than its capacity.");
        }

        [Test]
        public void Exit_RemovesPassengersCorrectly()
        {
            var elevator = elevatorService.FindNearestAvailableElevator(0, 5);
            elevatorService.Board(elevator, 7);
            elevatorService.Exit(elevator, 3);
            Assert.AreEqual(4, elevator.PeopleOnBoard, "Elevator should have 4 passengers remaining.");

            Assert.Throws<InvalidOperationException>(() => elevatorService.Exit(elevator, 5), "Cannot remove more passengers than currently on board.");
        }

        [Test]
        public async Task MoveToFloorAsync_MovesElevatorCorrectly()
        {
            var elevator = elevatorService.FindNearestAvailableElevator(0, 5);
            await elevatorService.MoveToFloorAsync(elevator, 5);
            Assert.AreEqual(5, elevator.CurrentFloor, "Elevator should be at floor 5.");

            await elevatorService.MoveToFloorAsync(elevator, 3);
            Assert.AreEqual(3, elevator.CurrentFloor, "Elevator should be at floor 3.");
        }

        [Test]
        public void MoveToFloorAsync_ThrowsArgumentOutOfRangeException_ForInvalidFloor()
        {
            var elevator = elevatorService.FindNearestAvailableElevator(0, 5);
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
                await elevatorService.MoveToFloorAsync(elevator, -1), "Expected exception for out-of-range floor.");

            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
                await elevatorService.MoveToFloorAsync(elevator, 11), "Expected exception for out-of-range floor.");
        }
    }

    public class MockFloorService : IFloorService
    {
        private readonly Dictionary<int, Floor> floors;

        public MockFloorService(int numberOfFloors)
        {
            floors = Enumerable.Range(0, numberOfFloors)
                               .ToDictionary(i => i, i => new Floor ( i ));
        }

        public Floor GetFloor(int floorNumber)
        {
            return floors.ContainsKey(floorNumber) ? floors[floorNumber] : null;
        }

        public void AddPassengers(Floor floor, int numberOfPassengers)
        {
            if (floors.ContainsKey(floor.FloorNumber))
            {
                floors[floor.FloorNumber].WaitingPassengers += numberOfPassengers;
            }
        }

        public void RemovePassengers(Floor floor, int numberOfPassengers)
        {
            if (floors.ContainsKey(floor.FloorNumber))
            {
                floors[floor.FloorNumber].WaitingPassengers =
                    Math.Max(floors[floor.FloorNumber].WaitingPassengers - numberOfPassengers, 0);
            }
        }

        // Implement the UpdateFloor method required by IFloorService
        public void UpdateFloor(int floorNumber, int numberOfPassengers)
        {
            if (floors.ContainsKey(floorNumber))
            {
                floors[floorNumber].WaitingPassengers = numberOfPassengers;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(floorNumber), "Floor number is out of range.");
            }
        }
    }

}
