namespace ElevatorSimulation.Entities.Models
{
    public class Floor
    {
        public int FloorNumber { get; set; }
        public int WaitingPassengers { get;  set; }

        public Floor(int floorNumber)
        {
            FloorNumber = floorNumber;
            WaitingPassengers = 0;
        }

      
    }
}
