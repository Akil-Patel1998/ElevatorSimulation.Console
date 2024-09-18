namespace ElevatorSimualtion.Entities.Models
{
    public class Passenger
    {
        public int Id { get; }
        public int DestinationFloor { get; }

        public Passenger(int id, int destinationFloor)
        {
            Id = id;
            DestinationFloor = destinationFloor;
        }
    }
}
