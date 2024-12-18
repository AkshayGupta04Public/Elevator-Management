using ElevatorManagement.Common.Models;
using ElevatorManagement.Services;
using Microsoft.Extensions.Configuration;

namespace ElevatorManagement
{
    public class Program
    {
        public static readonly Queue<Request> requestQueue = new Queue<Request>();

        static async Task Main(string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                IConfiguration configuration = builder.Build();

                var floors = int.Parse(configuration.GetSection("Presets:Floors").Value ?? "");
                var elevators = int.Parse(configuration.GetSection("Presets:Elevators").Value ?? "");
                var travelTimePerFloor = int.Parse(configuration.GetSection("Presets:TravelTimePerFloor").Value ?? "");
                var stopTimePerFloor = int.Parse(configuration.GetSection("Presets:StopTimePerFloor").Value ?? "");

                // throw error if no floors or elevators 0.
                if (floors == 0 || elevators == 0)
                    throw new InvalidOperationException("Invalid configuration: Floors and elevators must be greater than 0.");


                ElevatorHandler handler = new ElevatorHandler(requestQueue, elevators, floors, travelTimePerFloor, stopTimePerFloor);
                UserService userService = new UserService(requestQueue, floors);

                await Task.WhenAll(
                    userService.SimulateUserElevatorRequests(),
                    handler.AssignElevatorSimulation()
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
