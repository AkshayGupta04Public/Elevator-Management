using ElevatorManagement.Common;
using ElevatorManagement.Common.Models;

namespace ElevatorManagement.Services
{
    public class ElevatorServices
    {
        private readonly Elevator _elevator;
        public ElevatorServices(Elevator elevator)
        {
            _elevator = elevator;
        }

        /// <summary>
        /// This method will run at times, it is the most important for elevator which handles its Movements and decides on which floor to stop next
        /// </summary>
        /// <returns>A Task</returns>
        public async Task Operate()
        {
            while (true)
            {
                int? nextTarget = null;

                if (_elevator.targets.Any())
                {
                    nextTarget = _elevator.targets.Peek();
                }

                if (nextTarget.HasValue)
                {
                    await MoveToFloor(nextTarget.Value);
                    Console.WriteLine($"Elevator {_elevator.Id} arrived at floor {nextTarget.Value}, Passengers boarding and de-boarding");
                    _elevator.targets.Dequeue();
                    await Task.Delay(_elevator.StopTimePerFloor); // Passengers enter/leave
                }
                else
                {
                    _elevator.Direction = ElevatorDirection.Idle;
                }
            }
        }

        /// <summary>
        /// This method add new request for an elevator
        /// </summary>
        /// <param name="request">User's Request for elevator</param>
        public void AddRequest(Request request)
        {
            if (!_elevator.targets.Contains(request.Floor))
            {
                lock (_elevator.targets)
                {
                    _elevator.targets.Enqueue(request.Floor);
                }
            }
            Console.WriteLine($"Elevator {_elevator.Id} assigned to floor {request.Floor} going ({request.Direction})");
        }

        /// <summary>
        /// This method actually moes the elevator to requested floor
        /// </summary>
        /// <param name="floor">Destination Floor</param>
        /// <returns></returns>
        public async Task MoveToFloor(int floor)
        {
            while (_elevator.CurrentFloor != floor)
            {
                _elevator.Direction = _elevator.CurrentFloor < floor ? ElevatorDirection.Up : ElevatorDirection.Down;
                await Task.Delay(_elevator.TravelTimePerFloor);
                _elevator.CurrentFloor += _elevator.Direction == ElevatorDirection.Up ? 1 : -1;
                Console.WriteLine($"Elevator {_elevator.Id} is on floor {_elevator.CurrentFloor}");
            }
        }
    }
}
