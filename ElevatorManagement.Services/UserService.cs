using ElevatorManagement.Common;
using ElevatorManagement.Common.Models;

namespace ElevatorManagement.Services
{
    public class UserService
    {
        public readonly Queue<Request> _requestQueue;
        private readonly int _numberOfFloors;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestQueue">All Users Requests(from all floors for all elevators)</param>
        /// <param name="numberOfFloors">Total flors in the building</param>
        public UserService(Queue<Request> requestQueue, int numberOfFloors) 
        {
            _requestQueue = requestQueue;
            _numberOfFloors = numberOfFloors;
        }

        /// <summary>
        ///  Request for Elevator Stimulated after some delay
        /// </summary>
        public async Task SimulateUserElevatorRequests()
        {
            Console.WriteLine($"---------------  Requesting Elevator, Total Floors: {_numberOfFloors} ---------------");
            await Task.Run(RequestElevator);
        }

        /// <summary>
        /// Request For Evevator
        /// </summary>
        private async Task RequestElevator()
        {
            var random = new Random();
            await Task.Delay(1500);
            while (true)
            {
                int floor = random.Next(1, _numberOfFloors + 1);
                ElevatorDirection direction = (ElevatorDirection)random.Next(1, 3); // 0 to 2 (index of enum values)
                direction = floor == 0 && direction == ElevatorDirection.Down ? ElevatorDirection.Up
                    : floor == _numberOfFloors && direction == ElevatorDirection.Up ? ElevatorDirection.Down
                    : direction; // Ground and top floor cases

                lock (_requestQueue)
                {
                   _requestQueue.Enqueue(new Request(floor, direction));
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"---------------  {direction} request on floor {floor} received.  ---------------");
                Console.ResetColor(); // Reset to default color
                await Task.Delay(random.Next(3000, 7000)); // Random interval for new requests                
            }
        }
    }
}
