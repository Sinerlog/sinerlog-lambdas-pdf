using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Sinerlog.FullCommerce.Common;
using Sinerlog.Lambda.Pdf.Common;
using Sinerlog.Lambda.Pdf.Common.Messages;
using Sinerlog.Lambda.Pdf.Label.Application;
using Sinerlog.Lambda.Pdf.Label.Application.Models;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Sinerlog.Lambda.Pdf.Label;

public class Function
{
    /// <summary>
    /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
    /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
    /// region the Lambda function is executed in.
    /// </summary>
    public Function()
    {

    }


    /// <summary>
    /// This method is called for every Lambda invocation. This method takes in an SQS event object and can be used 
    /// to respond to SQS messages.
    /// </summary>
    /// <param name="evnt"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
    {
        foreach(var message in evnt.Records)
        {
            await ProcessMessageAsync(message, context);
        }
    }

    private async Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
    {
        //context.Logger.LogInformation($"MESSAGE BODY: {message.Body}");

        var messageReceived = JsonUtils.Deserialize<CreateLabelMessage>(message.Body);

        var htmlConverter = PdfConfiguration.GetLabelConverter();


        if (messageReceived.LabelType == Common.Constants.LabelTypeEnum.International)
        {
            await LabelGenerator.ProcessInternationalLabel(JsonUtils.Deserialize<InternationalLabelDto>(messageReceived.LabelDtoBody.ToString()),htmlConverter,context);
        }

        if (messageReceived.LabelType == Common.Constants.LabelTypeEnum.Domestic)
        {
            await LabelGenerator.ProcessDomesticLabel(JsonUtils.Deserialize<DomesticLabelDto>(messageReceived.LabelDtoBody.ToString()), htmlConverter, context);
        }
      
        await Task.CompletedTask;
    }
}