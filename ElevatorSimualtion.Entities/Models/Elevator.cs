using System;
using System.Threading.Tasks;
using ElevatorSimulation.Entities.Enums;

namespace ElevatorSimulation.Entities.Models
{
    public abstract class Elevator
    {
        public int Id { get; }
        public int CurrentFloor { get; set; }
        public bool IsMoving { get; set; }
        public ElevatorDirection Direction { get; set; }
        public int PeopleOnBoard { get; set; }
        public int Capacity { get; }

        protected Elevator(int id, int capacity)
        {
            Id = id;
            Capacity = capacity;
            CurrentFloor = 0;
            IsMoving = false;
            Direction = ElevatorDirection.Stationary;
            PeopleOnBoard = 0;
        }

        public abstract Task MoveToFloorAsync(int targetFloor);
    }

    public class StandardElevator : Elevator
    {
        public StandardElevator(int id, int capacity) : base(id, capacity) { }

        public override async Task MoveToFloorAsync(int targetFloor)
        {
            while (CurrentFloor != targetFloor)
            {
                Console.WriteLine($"{GetType().Name} {Id} is passing floor {CurrentFloor} going {(targetFloor > CurrentFloor ? "up" : "down")}.");
                await Task.Delay(1000); // Simulating movement time
                CurrentFloor += (targetFloor > CurrentFloor) ? 1 : -1;
            }
        }
    }

    public class HighSpeedElevator : Elevator
    {
        public HighSpeedElevator(int id, int capacity) : base(id, capacity) { }

        public override async Task MoveToFloorAsync(int targetFloor)
        {
            while (CurrentFloor != targetFloor)
            {
                Console.WriteLine($"{GetType().Name} {Id} is passing floor {CurrentFloor} going {(targetFloor > CurrentFloor ? "up" : "down")}.");
                await Task.Delay(500); // Simulating faster movement time
                CurrentFloor += (targetFloor > CurrentFloor) ? 1 : -1;
            }
        }
    }

    public class GlassElevator : Elevator
    {
        public GlassElevator(int id, int capacity) : base(id, capacity) { }

        public override async Task MoveToFloorAsync(int targetFloor)
        {
            while (CurrentFloor != targetFloor)
            {
                Console.WriteLine($"{GetType().Name} {Id} is passing floor {CurrentFloor} going {(targetFloor > CurrentFloor ? "up" : "down")}.");
                await Task.Delay(1200); // Simulating slightly slower movement for scenic views
                CurrentFloor += (targetFloor > CurrentFloor) ? 1 : -1;
            }
        }
    }

    public class FreightElevator : Elevator
    {
        public int WeightCapacity { get; }

        public FreightElevator(int id, int capacity, int weightCapacity) : base(id, capacity)
        {
            WeightCapacity = weightCapacity;
        }

        public override async Task MoveToFloorAsync(int targetFloor)
        {
            while (CurrentFloor != targetFloor)
            {
                Console.WriteLine($"{GetType().Name} {Id} is passing floor {CurrentFloor} going {(targetFloor > CurrentFloor ? "up" : "down")}.");
                await Task.Delay(1500); // Simulating slower movement due to heavy loads
                CurrentFloor += (targetFloor > CurrentFloor) ? 1 : -1;
            }
        }
    }
}
