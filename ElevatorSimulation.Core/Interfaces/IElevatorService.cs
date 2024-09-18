using ElevatorSimulation.Entities.Models;


namespace ElevatorSimulation.Core.Interfaces
{
    public interface IElevatorService
    {
        Elevator FindNearestAvailableElevator(int targetFloor, int waitingPassenger);
        void ShowElevatorStatus();
        void AddElevator();
        void Board(Elevator elevator, int passengers);
        void Exit(Elevator elevator, int passengers);
        void MoveToFloor(Elevator elevator, int targetFloor);
    }
}
