using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using Amazon.SimpleNotificationService.Util;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace TagItAwaySNSLambda
{
    public class Function
    {

        private static AmazonDynamoDBClient client = new AmazonDynamoDBClient();
        private static string tableName = "TagItAwayTags";

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(Message message, ILambdaContext context)
        {
            // Message message;
            //var snsMessage = Amazon.SimpleNotificationService.Util.Message.ParseMessage(message);

            //return input?.ToUpper();


            var tagsTable = Table.LoadTable(client, tableName);
            await Create(tagsTable, message);
        }



        // Creates a sample book item.
        private async Task Create(Table tagsTable, Message message)
        {
            Console.WriteLine("\n*** Executing CreateBookItem() ***");
            var item = new Document
            {
                ["Id"] = message.MessageId,
                ["Value"] = message.MessageText
            };


            tagsTable.PutItemAsync(item);
        }

        //private static void RetrieveBook(Table productCatalog)
        //{
        //    Console.WriteLine("\n*** Executing RetrieveBook() ***");
        //    // Optional configuration.
        //    GetItemOperationConfig config = new GetItemOperationConfig
        //    {
        //        AttributesToGet = new List<string> { "Id", "ISBN", "Title", "Authors", "Price" },
        //        ConsistentRead = true
        //    };
        //    Document document = productCatalog.GetItem(sampleBookId, config);
        //    Console.WriteLine("RetrieveBook: Printing book retrieved...");
        //    PrintDocument(document);
        //}
    }
}
