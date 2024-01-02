using System.Net;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace api_gateway_lambda
{

    public class Function
    {

        private static readonly string[] Summaries = new[]
       {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching", "El nino"
        };

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public List<WeatherForecast> FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
        {
            Console.WriteLine(JsonSerializer.Serialize(input));
            string cityName = null;
            input.QueryStringParameters?.TryGetValue("cityName", out cityName);
            cityName = cityName ?? "melbourne";

            var rng = new Random();
            // Enumerable.range generates 1,2,3,4,5 
            /* index => new WeatherForecast {...} is a lambda expression where index is the input parameter 
            * and new WeatherForecast {...} is the code block that gets executed for each input.*/
            //Select to transform each integer in the range (1, 2, 3, 4, 5) into a WeatherForecast object.

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                City = $"{cityName}-test",
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToList();
        }

        
        public APIGatewayProxyResponse FunctionHandlerPost(APIGatewayProxyRequest input, ILambdaContext context)
        {
            /*
             * This line deserializes the JSON body of the HTTP request into a WeatherForecast object. 
             * The JsonSerializer is used to convert the JSON string from input.Body into a WeatherForecast instance.
             */

            var data = JsonSerializer.Deserialize<WeatherForecast>(input.Body);

            string cityName = "melbourne";
            input.PathParameters?.TryGetValue("cityName", out cityName);

            data.City = cityName;
            return new APIGatewayProxyResponse()
            {
                Body = JsonSerializer.Serialize(data),
                StatusCode = (int)HttpStatusCode.OK
            };
        }
    
    }

    public class WeatherForecast
    {
        public string City { get; set; }
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }
    }
}