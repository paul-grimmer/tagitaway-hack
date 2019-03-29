using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;

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
        public async Task<string> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var tags = Table.LoadTable(client, "TagItAwayTags");
            var id = request.QueryStringParameters["filename"];
            GetItemOperationConfig config = new GetItemOperationConfig
            {
                AttributesToGet = new List<string> { "Id", "Value" },
                ConsistentRead = true
            };
             Document document = await tags.GetItemAsync(id, config);

            return document["Value"];
        }
    }
}
