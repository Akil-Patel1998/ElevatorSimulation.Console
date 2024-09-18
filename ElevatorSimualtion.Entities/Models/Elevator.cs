using System;
using ElevatorSimualtion.Entities.Enums;

namespace ElevatorSimualtion.Entities.Models
{
    public class Elevator
    {
        public int Id { get; }
        public int CurrentFloor { get; set; }
        public bool IsMoving { get; set; }
        public ElevatorDirection Direction { get; set; }
        public int PeopleOnBoard { get; set; }
        public int Capacity { get; }


        public Elevator(int id, int capacity)
        {
            Id = id;
            Capacity = capacity;
            PeopleOnBoard = 0;
            CurrentFloor = 0;
            IsMoving = false;
            Direction = ElevatorDirection.Stationary;

        }


    }
}
