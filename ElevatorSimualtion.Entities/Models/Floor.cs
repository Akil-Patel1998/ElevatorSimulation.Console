namespace ElevatorSimulation.Entities.Models
{
    public class Floor
    {
        public int FloorNumber { get; }
        public int WaitingPassengers { get; private set; }

        public Floor(int floorNumber)
        {
            FloorNumber = floorNumber;
            WaitingPassengers = 0;
        }

        public void AddPassengers(int numberOfPassengers)
        {
            if (numberOfPassengers < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfPassengers), "Number of passengers to add cannot be negative.");
            }
            WaitingPassengers += numberOfPassengers;
            Console.WriteLine($"Added {numberOfPassengers} passengers to floor {FloorNumber}. Total waiting: {WaitingPassengers}");
        }

        public void RemovePassengers(int numberOfPassengers)
        {
            if (numberOfPassengers < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfPassengers), "Number of passengers to remove cannot be negative.");
            }
            if (numberOfPassengers > WaitingPassengers)
            {
                throw new InvalidOperationException("Cannot remove more passengers than currently waiting.");
            }
            WaitingPassengers -= numberOfPassengers;
            Console.WriteLine($"Removed {numberOfPassengers} passengers from floor {FloorNumber}. Total waiting: {WaitingPassengers}");
        }
    }
}
