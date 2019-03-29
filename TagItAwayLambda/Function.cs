using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.S3;
using Amazon.S3.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace TagItAwayLambda
{
    public class Function
    {
        const string bucketName = "tag-it-away-video-input";
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<object> FunctionHandler(string fileName, ILambdaContext context)
        {
            var rekognitionClient = new AmazonRekognitionClient( Amazon.RegionEndpoint.USEast2);


            return await GetVideoLabelsResult(fileName, rekognitionClient);

            //startJobId = startLabelDetectionResult.getJobId();

            // Java example https://docs.aws.amazon.com/rekognition/latest/dg/video-analyzing-with-sqs.html
            //StartLabelDetectionRequest req = new StartLabelDetectionRequest()
            //    .withVideo(new Video()
            //        .withS3Object(new S3Object()
            //            .withBucket(bucket)
            //            .withName(video)))
            //    .withMinConfidence(50F)
            //    .withJobTag("DetectingLabels")
            //    .withNotificationChannel(channel);

            //StartLabelDetectionResult startLabelDetectionResult = rek.startLabelDetection(req);
            //startJobId = startLabelDetectionResult.getJobId();



            //var imageLabels = GetImageLabels(fileName, rekognitionClient);
            //return imageLabels;
            
            //foreach (var label in detectResponses.Labels)
            //{
            //	if (label.Name == "Fried Chicken" || label.Name == "Nuggets")
            //		return true;
            //}

            //return false; ;
        }

        private static async Task<object> GetVideoLabelsResult(string fileName, AmazonRekognitionClient rekognitionClient)
        {
            var startRequest = new StartLabelDetectionRequest
            {
                MinConfidence = 50,
                Video = new Video
                {
                    S3Object = new Amazon.Rekognition.Model.S3Object
                    {
                        Bucket = bucketName,
                        Name = fileName
                    }
                },
                JobTag = "DetectingLabels"
            };

            var startLabelDetectionResult = await rekognitionClient.StartLabelDetectionAsync(startRequest);
            return startLabelDetectionResult.JobId;
        }


        private async Task<object> GetImageLabels(string fileName, AmazonRekognitionClient rekognitionClient)
        {
            var detectResponses = await rekognitionClient.DetectLabelsAsync(new DetectLabelsRequest
            {
                MinConfidence = 50,

                Image = new Image
                {
                    S3Object = new Amazon.Rekognition.Model.S3Object
                    {
                        Bucket = bucketName,
                        Name = fileName
                    }
                }
            });

            return detectResponses.Labels;
        }

        //static void IdentifyFaces(string filename)
        //{
        //    // Using USWest2, not the default region
        //    var rekognitionClient = new AmazonRekognitionClient(accesKey, secretKey, Amazon.RegionEndpoint.USEast2);

        //    DetectFacesRequest dfr = new DetectFacesRequest();

        //    // Request needs image butes, so read and add to request
        //    Amazon.Rekognition.Model.Image img = new Amazon.Rekognition.Model.Image();
        //    byte[] data = null;
        //    using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
        //    {
        //        data = new byte[fs.Length];
        //        fs.Read(data, 0, (int)fs.Length);
        //    }
        //    img.Bytes = new MemoryStream(data);
        //    dfr.Image = img;
        //    var outcome = rekoClient.DetectFaces(dfr);

        //    if (outcome.FaceDetails.Count > 0)
        //    {
        //        // Load a bitmap to modify with face bounding box rectangles
        //        System.Drawing.Bitmap facesHighlighted = new System.Drawing.Bitmap(filename);
        //        Pen pen = new Pen(Color.Black, 3);

        //        // Create a graphics context
        //        using (var graphics = Graphics.FromImage(facesHighlighted))
        //        {
        //            foreach (var fd in outcome.FaceDetails)
        //            {
        //                // Get the bounding box
        //                BoundingBox bb = fd.BoundingBox;
        //                Console.WriteLine("Bounding box = (" + bb.Left + ", " + bb.Top + ", " +
        //                    bb.Height + ", " + bb.Width + ")");
        //                // Draw the rectangle using the bounding box values
        //                // They are percentages so scale them to picture
        //                graphics.DrawRectangle(pen, x: facesHighlighted.Width * bb.Left,
        //                    y: facesHighlighted.Height * bb.Top,
        //                    width: facesHighlighted.Width * bb.Width,
        //                    height: facesHighlighted.Height * bb.Height);
        //            }
        //        }
        //        // Save the image with highlights as PNG
        //        string fileout = filename.Replace(Path.GetExtension(filename), "_faces.png");
        //        facesHighlighted.Save(fileout, System.Drawing.Imaging.ImageFormat.Png);
        //        Console.WriteLine(">>> " + outcome.FaceDetails.Count + " face(s) highlighted in file " + fileout);
        //    }
        //    else
        //        Console.WriteLine(">>> No faces found");
        //}

    }
}
