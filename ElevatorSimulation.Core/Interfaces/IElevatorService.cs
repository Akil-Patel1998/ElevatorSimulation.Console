using ElevatorSimulation.Entities.Models;


namespace ElevatorSimulation.Core.Interfaces
{
    public interface IElevatorService
    {
        Elevator FindNearestAvailableElevator(int targetFloor, int waitingPassenger);
        void ShowElevatorStatus();
        void AddElevator();
        List<Elevator> GetElevators();
        void Board(Elevator elevator, int passengers);
        void Exit(Elevator elevator, int passengers);
        Task MoveToFloorAsync(Elevator elevator, int targetFloor);
    }
}
