using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Lambda;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Util;

namespace SchnitzelOrNotClient
{
	public class SchnitzelDetector
	{
		String accessKey = "";
		String secretKey = "";

		public SchnitzelDetector(string accessKey, string secretKey)
		{
			this.accessKey = accessKey;
			this.secretKey = secretKey;
		}

		public async Task<bool> IsSchnitzel(String filePath, String fileName)
		{
			await UploadToS3(filePath, fileName);

			AmazonLambdaClient alc = new AmazonLambdaClient(accessKey, secretKey, RegionEndpoint.EUCentral1);
			Amazon.Lambda.Model.InvokeRequest ir = new Amazon.Lambda.Model.InvokeRequest();
			ir.InvocationType = InvocationType.RequestResponse;
			ir.FunctionName = "SchnitzelOrNot";
		
			ir.Payload = "\"" + fileName + "\"";
			var res = await alc.InvokeAsync(ir);

			var strResponse = Encoding.ASCII.GetString(res.Payload.ToArray());

			if (bool.TryParse(strResponse, out bool retVal))
				return retVal;

			return false;
		}

		public async Task UploadToS3(String filePath, String fileName)
		{
			var client = new AmazonS3Client(accessKey, secretKey, Amazon.RegionEndpoint.USWest2);
			
			PutObjectRequest putRequest = new PutObjectRequest
			{
				BucketName = "schnitzelornot",
				Key = fileName,
				FilePath = filePath,
				ContentType = "text/plain"
			};
		
			PutObjectResponse response = await client.PutObjectAsync(putRequest);
		}
	}
}
