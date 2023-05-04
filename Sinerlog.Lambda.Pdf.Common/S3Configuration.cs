using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace Sinerlog.Lambda.Pdf.Common
{
    public static class S3Configuration
    {
        public static async Task<bool> SendToS3(string fileName, Stream pdfContent)
        {
            var config = new AmazonS3Config { RegionEndpoint = RegionEndpoint.USEast1 };
            var credentials = new BasicAWSCredentials(Environment.GetEnvironmentVariable("S3_AWS_ACESS_KEY"), Environment.GetEnvironmentVariable("S3_AWS_SECRET_KEY"));
            // Cria um cliente do Amazon S3
            using (var s3Client = new AmazonS3Client(credentials, config))
            {
                //// Configura as opções para a operação de upload do S3
                var putRequest = new PutObjectRequest
                {
                    BucketName = Environment.GetEnvironmentVariable("AWS_LABEL_BUCKET_NAME"),
                    Key = fileName,
                    InputStream = pdfContent
                };

                // Realiza o upload do arquivo para o S3
                var s3Result = s3Client.PutObjectAsync(putRequest).Result;

                return s3Result.HttpStatusCode == System.Net.HttpStatusCode.OK;

            }
        }
    }
}
