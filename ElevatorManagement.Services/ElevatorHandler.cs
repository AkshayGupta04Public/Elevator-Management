using ElevatorManagement.Common;
using ElevatorManagement.Common.Models;

namespace ElevatorManagement.Services
{
    public class ElevatorHandler
    {
        private readonly ElevatorServices? _elevatorServices;
        private readonly Elevator? _elevator;

        private readonly List<Elevator> elevators;
        public readonly Queue<Request> _requestQueue;

        /// <summary>
        /// This Constructor will initialize all requested elevators and starts operating it
        /// </summary>
        /// <param name="numberOfElevators">Total Elevators in Building</param>
        /// <param name="numberOfFloors">Total Floors in Building</param>
        /// <param name="travelTimePerFloor">Time Taken by Lelevator to travel each floor</param>
        /// <param name="stopTimePerFloor">Time elevator will take to board/Deboard passengers</param>
        public ElevatorHandler(Queue<Request> requestQueue, int numberOfElevators, int numberOfFloors, int travelTimePerFloor, int stopTimePerFloor)
        {
            _requestQueue = requestQueue;
            elevators = new List<Elevator>();
            for (int i = 0; i < numberOfElevators; i++)
            {
                _elevator = new Elevator(i + 1, numberOfFloors, travelTimePerFloor, stopTimePerFloor);
                _elevatorServices = new ElevatorServices(_elevator);
                elevators.Add(_elevator);

                Task.Run(_elevatorServices.Operate);
            }
        }

        /// <summary>
        /// This will generate requests at random time for random floors and assign elevator accordingly
        /// </summary>
        public async Task AssignElevatorSimulation()
        {
            Console.WriteLine($"---------------  Assign Elevator, Total Elevators: {elevators.Count}  ---------------");

            _ = Task.Run(AssignElevator);

            while (true)
            {
                DisplayStatus();
                await Task.Delay(10000); // Display status after specific delay
            }

        }

        /// <summary>
        /// To Assign Request to a Elevator
        /// </summary>
        public async Task AssignElevator()
        {
            while (true)
            {
                Request? request = null;
                if (_requestQueue?.Count > 0)
                {
                    lock (_requestQueue)
                    {
                        request = _requestQueue.Dequeue();
                    }
                     await AssignRequest(request);
                }

                await Task.Delay(1000); // Allow time for elevator operations
            }
        }

        /// <summary>
        /// This will assign request to a particular elevator
        /// </summary>
        /// <param name="request"></param>
        private async Task AssignRequest(Request request)
        {
            Elevator? bestElevator = null;
            int bestDistance = _elevator!.NumberOfFloors * _elevator.TravelTimePerFloor * 2;

            while (bestElevator == null) // Continue looping until a suitable elevator is found
            {
                foreach (var elevator in elevators)
                {
                    int distance = CalculateTime(request, elevator);
                    if (distance < bestDistance)
                    {
                        bestElevator = elevator;
                        bestDistance = distance;
                        bestElevator = elevator;
                    }
                }

                if (bestElevator == null)
                {
                    Console.WriteLine($"All elevators Busy now, Kindly wait for sometime, Thanks....");
                    await Task.Delay(5000); // Wait for few seconds before rechecking
                }
            }

            var elevatorServices = new ElevatorServices(bestElevator);
            elevatorServices.AddRequest(request);

        }

        /// <summary>
        /// This will calsulate the time reuired for each elevator to each the requested floor, taking into consideration the stop time also
        /// </summary>
        /// <param name="request"></param>
        /// <param name="elevator"></param>
        /// <returns></returns>
        public int CalculateTime(Request request, Elevator elevator)
        {
            int maxtime = _elevator!.NumberOfFloors * _elevator.TravelTimePerFloor * 2;
            if (elevator.Direction == ElevatorDirection.Idle)
            {
                return Math.Abs(elevator.CurrentFloor - request.Floor) * elevator.TravelTimePerFloor;
            }

            if (CanElevatorBeUsedForRequestedDirection(request, elevator))
            {
                // Time to reach the requested floor
                int travelTime = Math.Abs(elevator.CurrentFloor - request.Floor) * elevator.TravelTimePerFloor;

                // Stop time for each target only if it's not already the current request floor
                int stopTime = elevator.targets.Where(target => target != request.Floor).Count() * elevator.StopTimePerFloor;

                return travelTime + stopTime;
            }
            return maxtime;
        }

        /// <summary>
        /// Check for Elevator's direction and is it can be assigned for a particular destination floor
        /// </summary>
        /// <param name="request"></param>
        /// <param name="elevator"></param>
        /// <returns></returns>
        private static bool CanElevatorBeUsedForRequestedDirection(Request request, Elevator elevator)
        {
            if (elevator.Direction != request.Direction)
                return false;

            return (elevator.Direction == ElevatorDirection.Up && request.Floor >= elevator.CurrentFloor) ||
                (elevator.Direction == ElevatorDirection.Down && request.Floor <= elevator.CurrentFloor);
        }

        /// <summary>
        /// Displays the current statu of all elevators
        /// </summary>
        private void DisplayStatus()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n---------------  Elevators Status  ---------------");
            foreach (var elevator in elevators)
            {
                Console.WriteLine(
                    $"Elevator {elevator.Id} - Floor: {elevator.CurrentFloor}, Status: {elevator.Direction}\n    Stops: {string.Join(", ", elevator.targets.Count)}"
                );
            }
            Console.WriteLine("\n");
            Console.ResetColor();
        }
    }
}
