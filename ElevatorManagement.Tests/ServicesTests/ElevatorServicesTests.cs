using ElevatorManagement.Common;
using ElevatorManagement.Common.Models;
using ElevatorManagement.Services;
using FluentAssertions;
using NUnit.Framework;

namespace ElevatorManagement.Tests.ServicesTests
{
    public class ElevatorServicesTests
    {
        private Elevator _elevator;
        private ElevatorServices? _elevatorServices;

        private int _numberOfElevators = 4;
        private int _numberOfFloors = 10;
        private int _travelTimePerFloor = 1;
        private int _stopTimePerFloor = 1;
        [SetUp]
        public void Setup()
        {
            _elevator = new Elevator(1, _numberOfFloors, _travelTimePerFloor, _stopTimePerFloor);
            _elevatorServices = new ElevatorServices(_elevator);

        }

        [Test]
        public void AddRequest_ShouldAddStopToQueue()
        {
            // Arrange
            var request = new Request(5, ElevatorDirection.Up);

            // Act
            _elevatorServices!.AddRequest(request);

            // Assert
            _elevator.targets.Count.Should().Be(1);
            _elevator.targets.First().Should().Be(request.Floor);
            _elevator.Direction.Should().Be(ElevatorDirection.Idle);// as it has been assigned but elevator has not moved yet
        }

        [Test]
        public async Task MoveToFloor_ShouldMoveElevatorToTargetFloor()
        {
            // Arrange
            var request = new Request(5, ElevatorDirection.Up);
            _elevatorServices!.AddRequest(request);

            // Act
            await _elevatorServices.MoveToFloor(_elevator.targets.First());

            // Assert
            _elevator.CurrentFloor.Should().Be(request.Floor); // Elevator should be on floor 7
            _elevator.Direction.Should().Be(request.Direction); // Direction should be Up
        }

    }
}
