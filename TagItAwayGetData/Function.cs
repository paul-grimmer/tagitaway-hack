using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.Runtime;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace TagItAwayGetData
{
    public class Function
    {
        private static AmazonDynamoDBClient client = new AmazonDynamoDBClient();

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var rekognitionClient = new AmazonRekognitionClient(Amazon.RegionEndpoint.USEast2);

            var tags = Table.LoadTable(client, "TagItAwayTags");
            var id = request.QueryStringParameters["id"];

            var labelDetectionRequest = new GetLabelDetectionRequest
                {
                JobId = id
                };

            var result = await rekognitionClient.GetLabelDetectionAsync(labelDetectionRequest);

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonConvert.SerializeObject(result.Labels),
                Headers = new Dictionary<string, string> { { "Access-Control-Allow-Origin", "*" } }
            };
        }
    }
}
