using System;
using System.Net.Http;

using NBomber;
using NBomber.Contracts;
using NBomber.CSharp;

namespace NBomberTest
{
    class Program
    {
        static void Main(string[] args)
        {   
            using var httpClient = new HttpClient();
         

            var step = Step.Create("fetch_html_page", async context =>
            {
                var response = await httpClient.GetAsync("https://nbomber.com");

                return response.IsSuccessStatusCode
                    ? Response.Ok()
                    : Response.Fail();
            });

            var scenario = ScenarioBuilder.CreateScenario("simple_http", step).WithWarmUpDuration(TimeSpan.FromSeconds(5))
                .WithLoadSimulations(
                    Simulation.InjectPerSec(rate: 100, during: TimeSpan.FromSeconds(30))
                );;

            NBomberRunner
                .RegisterScenarios(scenario)
                .Run();
        }
    }
}