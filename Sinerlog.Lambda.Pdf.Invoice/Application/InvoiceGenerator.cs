using Amazon.Lambda.Core;
using Sinerlog.Lambda.Pdf.Common;
using Sinerlog.Lambda.Pdf.Common.Persistency;
using Sinerlog.Lambda.Pdf.Invoice.Application.Models;
using Syncfusion.HtmlConverter;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace Sinerlog.Lambda.Pdf.Invoice.Application
{
    internal class InvoiceGenerator
    {
        static string TEMPLATE;

        public static void LoadTemplate(string templateFileName)
        {
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var pathName = Path.Combine(location, "Templates", $"{templateFileName}.html");
            var sr = new StreamReader(pathName);
            TEMPLATE = sr.ReadToEnd();
        }
        public static async Task<bool> ProcessInvoice(InvoiceReportDto invoiceReportDto, HtmlToPdfConverter htmlConverter, ILambdaContext lambdaContext)
        {
            lambdaContext.Logger.Log("Invoice Generation Started");

            LoadTemplate("InvoiceTemplate");

            foreach (var payment in invoiceReportDto.Payments)
            {
                invoiceReportDto.HtmlPaymentInfo += @$" 
                  <div style='font-size: 15px; display: flex; margin-top: 2px; margin-left: 20px;'>
                  <div style='display: inline-grid; margin-left: 5px;'>
                    <div style='margin-top: -10px; padding-top: 17px;'>
                      <span style='font-weight: bolder; width: 25%'>Method: </span>
                      {payment.Method}
                    </div>
                  </div>
                  <div style='display: inline-grid; margin-left: 75px;'>
                    <div style='margin-top: -10px; padding-top: 17px;'>
                      <span style='font-weight: bolder; width: 25%'>Currency: </span>
                     {payment.Currency}
                    </div>
                  </div>
                  <div style='display: inline-grid; margin-left: 75px;'>
                    <div style='margin-top: -10px; padding-top: 17px;'>
                      <span style='font-weight: bolder; width: 25%'>Card Brand: </span>
                    {payment.CardBrand}
                    </div>
                  </div>
                  <div style='display: inline-grid; margin-left: 75px;'>
                    <div style='margin-top: -10px; padding-top: 17px;'>
                      <span style='font-weight: bolder; width: 25%'>Total Paid: </span>
                      {payment.Currency} {payment.Total}
                    </div>
                  </div>
                  </div>
                </div>
                        ";
            }

            foreach (var item in invoiceReportDto.Items)
            {
                var NOT_AVAILABLE = "N/A";
                var accountTaxModalityIsComplete = invoiceReportDto.AccountTaxModality == "Complete";

                invoiceReportDto.HtmlDetails += $@"<tr>
                        <td style='width: 5%; text-align: center'>{invoiceReportDto.Items.Count}</td>
                        <td style='width: 30%;padding-right: 20px; text-align: start'>{item.Name ?? NOT_AVAILABLE}</td>
                        <td style='width: 20%;padding-right: 20px; text-align: start'>{item.Sku ?? NOT_AVAILABLE}</td>
                        <td style='width: 7%;padding-right: 20px; text-align: center'>{item.Quantity}</td>
                        <td style='width: 6%;padding-right: 20px; text-align: center'>{invoiceReportDto.OrderCurrency}</td>
                        <td style='width: 6%;padding-right: 20px; text-align: center'>{item.Price:N2}</td>
                        <td style='width: 6%;padding-right: 20px; text-align: center'>{item.HsCode}</td>
                        <td style='width: 6%;padding-right: 20px; text-align: center'>{item.Ncm}</td>
                        <td style='width: 6%;padding-right: 20px; text-align: center'>{item.Total}</td>
                      </tr>";


                if (accountTaxModalityIsComplete && "Complete" == item.TaxModality && item.HasRciEnabled)
                {
                    invoiceReportDto.HtmlDetails += $@"<tr>
                        <td colspan='2' style='width: 30%; text-align: center'><b>Taxes Model:</b> RCI </td>
                        <td style='width: 9%; text-align: start'><b>II: </b>{item.Taxes.II}%</td>
                        <td style='width: 9%; text-align: start'><b>IPI: </b>{item.Taxes.IPI}%</td>
                        <td style='width: 9%; text-align: start'><b>PIS: </b>{item.Taxes.PIS}%</td>
                        <td colspan='2' style='width: 9%; text-align: center'><b>COFINS: </b>{item.Taxes.COFINS}%</td>
                        <td colspan='2' style='width: 25%; text-align: center'><b>ICMS: </b>{item.Taxes.ICMS}%</td>
                      </tr>";

                    invoiceReportDto.HtmlDetails += $@"<tr style='font-weight: bolder; border-bottom: 1px solid black'>
                        <td style='width: 5%;border-bottom: 1px solid black; margin: 0px; border-spacing: 0; text-align: center'></td>
                        <td colspan='2' style='width: 45%; border-bottom: 1px solid black; margin: 0px; border-spacing: 0; text-align: start'></td>
                        <td colspan='6' style='width: 55%;border-bottom: 1px solid black; margin: 0px; border-spacing: 0; text-align: center'>Taxes based on documentations of {DateTime.Today.ToString("dd/MM/yyyy")}</td>
                      </tr>";
                }
                else
                {
                    invoiceReportDto.HtmlDetails += $@"<tr style='font-weight: bolder; border-bottom: 1px solid black'>
                        <td style='width: 5%; text-align: center; border-bottom: 1px solid black; margin: 0px; border-spacing: 0;'></td>
                        <td style='width: 50%; border-bottom: 1px solid black; margin: 0px; border-spacing: 0; text-align: start'><b>Taxes Model:</b> RTS</td>
                        <td style='width: 7%; border-bottom: 1px solid black; margin: 0px; border-spacing: 0; text-align: center'></td>
                        <td style='width: 10%; border-bottom: 1px solid black; margin: 0px; border-spacing: 0; text-align: center'></td>
                        <td style='width: 10%; border-bottom: 1px solid black; margin: 0px; border-spacing: 0; text-align: center'></td>
                        <td style='width: 10%; border-bottom: 1px solid black; margin: 0px; border-spacing: 0; text-align: center'></td>
                        <td colspan='3' style='width: 20%; border-bottom: 1px solid black; margin: 0px; border-spacing: 0; text-align: center'><b>RTS Tax: 60%</b></td>
                      </tr>";
                }
            }

            invoiceReportDto.QrCodeBase64 = GetQrCode();

            invoiceReportDto.LogoBase64 = await GetLogoAsync(invoiceReportDto.LogoNameS3);


            foreach (PropertyInfo propertyInfo in invoiceReportDto.GetType().GetProperties())
            {
                if (propertyInfo.PropertyType == typeof(string))
                {
#if DEBUG
                    Debug.WriteLine($"{{{propertyInfo.Name}}}");
#endif
                    TEMPLATE = TEMPLATE.Replace($"{{{propertyInfo.Name}}}", (string)propertyInfo.GetValue(invoiceReportDto, null));
                }
            }

            lambdaContext.Logger.Log("Invoice Html Generated");
            var document = htmlConverter.Convert(TEMPLATE, "");
            lambdaContext.Logger.Log("Invoice Html Converted");

            MemoryStream memoryStream = new MemoryStream();

            //Save and Close the PDFDocument.
            document.Save(memoryStream);
            document.Close(true);

            var fileName = $"{Guid.NewGuid()}.pdf";
            lambdaContext.Logger.LogInformation($"{lambdaContext.AwsRequestId} Iniciou envio para o S3");
            var fileUpload = await S3Configuration.SendToS3(fileName, memoryStream);
            lambdaContext.Logger.LogInformation($"{lambdaContext.AwsRequestId} Finalizou envio para o S3");

            if (fileUpload)
            {
                var updateCommandParams = new
                {
                    FileName = fileName,
                    LastModifiedBy = "LambdaS3",
                    LastModified = DateTime.Now,
                    OrderId = invoiceReportDto.OrderId,
                };

                var updateCommand = @$"UPDATE public.orders 
                                      SET invoice_pdf_url_name = @FileName, last_modified_by = @LastModifiedBy, last_modified = @LastModified
                                      WHERE id = @OrderId";

                var updateResult = await CommandRepository.ExecuteAsync(updateCommand, updateCommandParams);

                lambdaContext.Logger.LogInformation($"{lambdaContext.AwsRequestId} Salvou registro na base");

                if (!updateResult)
                {
                    throw new Exception($"Error to save PDF Filename on Datanase , Filename = {fileName}");
                }
            }
            else
            {
                throw new Exception($"Error to send to S3");
            }

            return true;
        }

        private static async Task<string> GetLogoAsync(string? fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                var logoStream = await S3Configuration.GetLogoToS3(fileName);
                if (logoStream is not null && logoStream.Length > 0)
                {
                    return Convert.ToBase64String(logoStream.ToArray());
                }
            }

            return "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";
        }

        private static string GetQrCode()
        {
            return "data:image/png;base64,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";
        }
    }
}
