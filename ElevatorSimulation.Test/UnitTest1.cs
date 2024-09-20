using NUnit.Framework;
using Moq;
using ElevatorSimulation.Core.Interfaces;
using ElevatorSimulation.Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElevatorSimulation.Core.Services;

namespace ElevatorSimulation.Test
{
    [TestFixture]
    public class ElevatorServiceTests
    {
        private Mock<IFloorService> floorServiceMock;
        private Mock<IPassengerService> passengerServiceMock;
        private ElevatorService elevatorService;

        [SetUp]
        public void Setup()
        {
            floorServiceMock = new Mock<IFloorService>();
            passengerServiceMock = new Mock<IPassengerService>();
            elevatorService = new ElevatorService(1, 10, floorServiceMock.Object, passengerServiceMock.Object);
        }

        [Test]
        public void AddElevator_ShouldAddElevator()
        {
            elevatorService.AddElevator("standard");

            var elevators = elevatorService.GetElevators();
            Assert.AreEqual(2, elevators.Count); // One initial + one added
            Assert.IsInstanceOf<StandardElevator>(elevators[1]);
        }
        [Test]
        public void FindNearestAvailableElevator_ShouldReturnNearestElevator()
        {
            // Arrange
            elevatorService.AddElevator("standard"); // This adds an elevator with ID 1
            var expectedElevator = elevatorService.GetElevators().First(); // Get the elevator you just added

            // Act
            var result = elevatorService.FindNearestAvailableElevator(1, 2);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedElevator.Id, result.Id); // Compare IDs instead of instances
        }



        [Test]
        public void Board_ShouldBoardPassengers()
        {
            var elevator = new StandardElevator(1, 10) { PeopleOnBoard = 0 };
            elevatorService.GetElevators().Add(elevator);

            elevatorService.Board(elevator, 5);

            Assert.AreEqual(5, elevator.PeopleOnBoard);
        }

        [Test]
        public void Board_ShouldThrowException_WhenOverCapacity()
        {
            var elevator = new StandardElevator(1, 5) { PeopleOnBoard = 5 };
            elevatorService.GetElevators().Add(elevator);

            Assert.Throws<InvalidOperationException>(() => elevatorService.Board(elevator, 1));
        }

        [Test]
        public async Task MoveToFloorAsync_ShouldMoveElevator()
        {
            var elevator = new StandardElevator(1, 10) { CurrentFloor = 0 };
            elevatorService.GetElevators().Add(elevator);

            await elevatorService.MoveToFloorAsync(elevator, 3);

            Assert.AreEqual(3, elevator.CurrentFloor);
        }

        [Test]
        public async Task Exit_ShouldRemovePassengers()
        {
            var elevator = new StandardElevator(1, 10) { PeopleOnBoard = 5 };
            elevatorService.GetElevators().Add(elevator);

            elevatorService.Exit(elevator, 2);

            Assert.AreEqual(3, elevator.PeopleOnBoard);
        }

        [Test]
        public void Exit_ShouldThrowException_WhenRemovingTooManyPassengers()
        {
            var elevator = new StandardElevator(1, 10) { PeopleOnBoard = 2 };
            elevatorService.GetElevators().Add(elevator);

            Assert.Throws<InvalidOperationException>(() => elevatorService.Exit(elevator, 3));
        }

        [Test]
        public void ShowElevatorStatus_ShouldDisplayStatus()
        {
            var elevator = new StandardElevator(1, 10) { CurrentFloor = 0, IsMoving = false };
            elevatorService.GetElevators().Add(elevator);

            elevatorService.ShowElevatorStatus();

            Assert.Pass(); // Placeholder for console output verification
        }

        [Test]
        public void AddElevator_ShouldThrowException_WhenInvalidType()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => elevatorService.AddElevator("invalidtype"));
        }

       

        [Test]
        public async Task MoveToFloorAsync_ShouldThrowException_WhenMovingToInvalidFloor()
        {
            // Arrange
            var elevator = new StandardElevator(1, 10) { CurrentFloor = 0 };
            elevatorService.GetElevators().Add(elevator);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => elevatorService.MoveToFloorAsync(elevator, 11)); // Assuming 10 is max
        }

        [Test]
        public void ShowElevatorStatus_ShouldNotThrowException()
        {
            // Arrange
            var elevator = new StandardElevator(1, 10) { CurrentFloor = 0, IsMoving = false };
            elevatorService.GetElevators().Add(elevator);

            // Act & Assert
            Assert.DoesNotThrow(() => elevatorService.ShowElevatorStatus());
        }

        [Test]
        public void Board_ShouldNotExceedCapacity()
        {
            // Arrange
            var elevator = new StandardElevator(1, 5) { PeopleOnBoard = 3 };
            elevatorService.GetElevators().Add(elevator);

            // Act
            elevatorService.Board(elevator, 2); // Should be okay

            // Assert
            Assert.AreEqual(5, elevator.PeopleOnBoard);
        }

       

    }
}
