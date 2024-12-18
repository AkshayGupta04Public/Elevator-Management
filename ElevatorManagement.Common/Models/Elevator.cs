namespace ElevatorManagement.Common.Models
{
    public class Elevator
    {
        public int Id { get; }
        public int CurrentFloor { get; set; } = 0;
        public ElevatorDirection Direction { get; set; }

        public readonly Queue<int> targets = new Queue<int>();
        public int NumberOfFloors { get; }
        public int TravelTimePerFloor { get; }
        public int StopTimePerFloor { get; }
        public Elevator(int id, int numberOfFloors, int travelTimePerFloor, int stopTimePerFloor)
        {
            Id = id;
            NumberOfFloors = numberOfFloors;
            TravelTimePerFloor = travelTimePerFloor;
            StopTimePerFloor = stopTimePerFloor;
        }
    }
}
