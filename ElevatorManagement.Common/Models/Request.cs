namespace ElevatorManagement.Common.Models
{
    public class Request
    {
        public int Floor { get; set; }
        public ElevatorDirection Direction { get; set; }
        public Request(int floor, ElevatorDirection direction)
        {
            Floor = floor;
            Direction = direction;
        }
    
    }
}
