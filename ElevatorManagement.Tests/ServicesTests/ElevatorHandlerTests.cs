using ElevatorManagement.Common;
using ElevatorManagement.Common.Models;
using ElevatorManagement.Services;
using FluentAssertions;
using NUnit.Framework;

namespace ElevatorManagement.Tests.ServicesTests
{
    public class ElevatorHandlerTests
    {
        private ElevatorHandler _elevatorHandler;
        private Elevator? _elevator;

        private readonly int _numberOfElevators = 4;
        private readonly int _numberOfFloors = 10;
        private readonly int _travelTimePerFloor = 100;
        private readonly int _stopTimePerFloor = 300;
        private readonly Queue<Request> _requestQueue = new Queue<Request>();

        [SetUp]
        public void Setup()
        {
            _elevatorHandler = new ElevatorHandler(_requestQueue , _numberOfElevators, _numberOfFloors, _travelTimePerFloor, _stopTimePerFloor);
            _elevator = new Elevator(1, _numberOfFloors, _travelTimePerFloor, _stopTimePerFloor)
            {
                CurrentFloor = 5,
                Direction = ElevatorDirection.Up
            };

        }

        [TestCase(7, ElevatorDirection.Up)]
        [TestCase(9, ElevatorDirection.Up)]
        public void CalculateTime_ShouldCalculateCorrectTimeForGoingUp(int floor, ElevatorDirection direction)
        {
            // Arrange
            var request = new Request(floor, direction);

            // Act
            var travelTime = _elevatorHandler.CalculateTime(request, _elevator!);

            // Assert
            travelTime.Should().Be((floor - _elevator!.CurrentFloor )* _elevator!.TravelTimePerFloor);  // 2 floors (from floor 5 to 7) * travel time per floor
        }

        [Test]
        public void CalculateTime_ShouldCalculateCorrectTimeWithOtherFlors()
        {
            // Arrange
            var request = new Request(9, ElevatorDirection.Up);
            _elevator!.targets.Enqueue(6);
            _elevator!.targets.Enqueue(7);

            // Act
            var travelTime = _elevatorHandler.CalculateTime(request, _elevator!);

            // Assert
            travelTime.Should().Be((4 * _elevator!.TravelTimePerFloor) +
                       (_elevator.targets.Where(target => target != request.Floor).Count() * _elevator.StopTimePerFloor));
        }
    }
}