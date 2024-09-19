namespace ElevatorSimulation.Entities.Models
{
    public class Floor
    {
        public int FloorNumber { get; set; }
        public int WaitingPassengers { get;  set; }
        public List<int> DestinationFloors { get; set; } // List of destination floors for waiting passengers

        public Floor(int floorNumber)
        {
            FloorNumber = floorNumber;
            WaitingPassengers = 0;
            DestinationFloors = new List<int>();
        }

      
    }
}
