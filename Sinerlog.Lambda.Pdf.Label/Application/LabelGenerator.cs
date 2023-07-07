using Amazon.Lambda.Core;
using DataMatrix.net;
using Sinerlog.Lambda.Pdf.Common;
using Sinerlog.Lambda.Pdf.Common.Persistency;
using Sinerlog.Lambda.Pdf.Label.Application.Models;
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Sinerlog.Lambda.Pdf.Label.Application
{
    public static class LabelGenerator
    {
        static CultureInfo US = new CultureInfo("en-US");
        static string TEMPLATE;

        #region BaseMethods
        public static void LoadTemplate(string templateFileName)
        {
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var pathName = Path.Combine(location, "Templates", $"{templateFileName}.html");
            var sr = new StreamReader(pathName);
            TEMPLATE = sr.ReadToEnd();
        }
        public static string HtmlDomesticGet(DomesticLabelDto label)
        {
            LoadTemplate("DomesticLabel");

            string texto = TEMPLATE;

            // Fix SVG dimension
            label.DataMatrixBase64 = DataMatrixGet(label).Replace(@"height=""220""", "").Replace(@"width=""220""", @"viewBox=""0 0 220 220""");

            texto = texto.Replace("{logo}", label.LogoNameS3);
            texto = texto.Replace("{dataMatrix}", label.DataMatrixBase64);
            texto = texto.Replace("{trackingCode}", label.TrackingCode);
            texto = texto.Replace("{lineheight}", label.LineHeight);
            texto = texto.Replace("{scaleBarCode}", label.ScaleBarCode);
            texto = texto.Replace("{fontSizeBarCode}", label.FontSizeBarCode);
            texto = texto.Replace("{script}", label.Script);
            texto = texto.Replace("{senderName}", label.SenderName);
            texto = texto.Replace("{senderStreet}", label.SenderStreet);
            texto = texto.Replace("{senderNumber}", label.SenderNumber);
            texto = texto.Replace("{senderDistrict}", label.SenderNeighborhood);
            texto = texto.Replace("{senderComplement}", label.SenderComplement);
            texto = texto.Replace("{senderZipcode}", label.SenderZipCode);
            texto = texto.Replace("{senderCity}", label.SenderCity);
            texto = texto.Replace("{senderState}", label.SenderState);
            texto = texto.Replace("{receiverName}", label.ReceiverName);
            texto = texto.Replace("{receiverStreet}", label.ReceiverStreet);
            texto = texto.Replace("{receiverNumber}", label.ReceiverNumber);
            texto = texto.Replace("{receiverComplement}", label.ReceiverComplement);
            texto = texto.Replace("{receiverDistrict}", label.ReceiverNeighborhood);
            texto = texto.Replace("{receiverZipcode}", label.ReceiverZipCode);
            texto = texto.Replace("{receiverCity}", label.ReceiverCity);
            texto = texto.Replace("{receiverState}", label.ReceiverState);
            texto = texto.Replace("{serviceType}", label.ServiceType);
            texto = texto.Replace("{weight}", ((int)label.Weight).ToString());

            return texto;
        }
        public static string DataMatrixGet(DomesticLabelDto label)
        {
            var zipcodeValidator = (10 - (label.ReceiverZipCode.Sum(c => c - '0') % 10));
            string matrixData = label.ReceiverZipCode; // CEP destino
            matrixData += "00000"; // complemento CEP destino
            matrixData += label.SenderZipCode; // CEP origem
            matrixData += "00000"; // complemento CEP origem
            matrixData += zipcodeValidator; // Validador CEP destino
            matrixData += "51"; // IDV: 51-Encomendas / 81-malotes
            matrixData += label.TrackingCode; // Codigo de rastrio
            matrixData += "00000000"; // Servicos adicionais: AR-Aviso recebimento / MP-Mao propria / DD-devolucao de documentos / VD-valor declarado
            matrixData += label.PostalCard; // Cartao Postagem
            matrixData += label.ServiceCode;
            matrixData += "00"; // informacao de agrupamento
            matrixData += label.ReceiverNumber.PadLeft(5); // numero do logradouro
            matrixData += (label.ReceiverComplement ?? "").PadLeft(20); // complemento logradouro
            matrixData += "12345";//label.Amount.ToString("D5"); // valor declarado;
            matrixData += label.ReceiverPhone; // DDD + Telefone destinatario
            matrixData += label.ReceiverLatitude.Substring(0, 10).PadRight(10); //latitude
            matrixData += label.ReceiverLongite.Substring(0, 10).PadRight(10); // longitude
            matrixData += "|"; // pipe
            matrixData += "".PadRight(30); // reserva para cliente 30

            var data = new DmtxImageEncoder();
            var img = data.EncodeSvgImage(matrixData, new DmtxImageEncoderOptions
            {
                Scheme = DmtxScheme.DmtxSchemeText,
                MarginSize = 10,
                ModuleSize = label.ModuleSizeDataMatrix,
                BackColor = Color.White,
                ForeColor = Color.Black

            });

            return img;
        }
        private static string HtmlInternationalGet(InternationalLabelDto label, string logoBase64)
        {
            LoadTemplate("InternationalLabel");

            var texto = TEMPLATE;

            var items = new List<string>();

            foreach (var item in label.Order.Items)
            {
                var price = item.Price.ToString("N2", US);
                var total = item.Total.ToString("N2", US);
                items.Add($"<tr style='height:6px;'><td>{item.SHCode}</td><td>{item.Quantity}</td><td colspan=\"3\">{item.Name}</td><td>{price}</td><td>{total}</td></tr>");
            }
            for (var i = 0; i < (5 - label.Order.Items.Count); i++)
                items.Add($"<tr style='height:6px;'><td><br></td><td></td><td colspan=\"3\"></td><td></td><td></td></tr>");

            texto = texto.Replace("{logo}", logoBase64);
            texto = texto.Replace("{trackingCode}", label.TrackingCode);
            texto = texto.Replace("{lineheight}", label.LineHeight.ToString(US));
            texto = texto.Replace("{scaleBarCode}", label.ScaleBarCode.ToString(US));
            texto = texto.Replace("{fontSizeBarCode}", label.FontSizeBarCode.ToString());
            texto = texto.Replace("{script}", label.Script);
            texto = texto.Replace("{senderName}", label.SenderName);
            texto = texto.Replace("{senderStreet}", label.SenderStreet);
            texto = texto.Replace("{senderNumber}", label.SenderNumber);
            texto = texto.Replace("{senderComplement}", label.SenderComplement);
            texto = texto.Replace("{senderZipcode}", label.SenderZipCode);
            texto = texto.Replace("{senderCity}", label.SenderCity);
            texto = texto.Replace("{senderState}", label.SenderState);
            texto = texto.Replace("{senderCountry}", label.SenderCountry);
            texto = texto.Replace("{receiverName}", label.ReceiverName);
            texto = texto.Replace("{receiverStreet}", label.ReceiverStreet);
            texto = texto.Replace("{receiverNumber}", label.ReceiverNumber);
            texto = texto.Replace("{receiverComplement}", label.ReceiverComplement);
            texto = texto.Replace("{receiverNeighborhood}", label.ReceiverNeighborhood);
            texto = texto.Replace("{receiverZipcode}", label.ReceiverZipCode);
            texto = texto.Replace("{receiverCity}", label.ReceiverCity);
            texto = texto.Replace("{receiverState}", label.ReceiverState);
            texto = texto.Replace("{serviceType}", label.ServiceType);
            texto = texto.Replace("{weight}", ((int)label.Weight).ToString());
            texto = texto.Replace("{items}", string.Concat(items));
            texto = texto.Replace("{currency}", label.Order.Currency);
            texto = texto.Replace("{shipping}", label.Order.ShippingCost.ToString("N2", US));
            texto = texto.Replace("{insurance}", 0.ToString("N2", US));
            texto = texto.Replace("{others}", label.Order.Others.ToString("N2", US));
            texto = texto.Replace("{discount}", label.Order.Discount.ToString("N2", US));
            texto = texto.Replace("{total}", label.Order.Total.ToString("N2", US));

            //order.Currency
            return texto;
        }
        #endregion

        public static async Task<bool> ProcessInternationalLabel(InternationalLabelDto labelDto, HtmlToPdfConverter htmlConverter, ILambdaContext lambdaContext)
        {
            var logo = await GetLogoAsync(labelDto.LogoNameS3);

            var html = HtmlInternationalGet(labelDto, logo);

            PdfDocument document = htmlConverter.Convert(html,"");

            MemoryStream memoryStream = new MemoryStream();

            //Save and Close the PDFDocument.
            document.Save(memoryStream);
            document.Close(true);

            var fileName = $"{Guid.NewGuid()}.pdf";

            var fileUpload = await S3Configuration.SendToS3(fileName, memoryStream);

            if(fileUpload)
            {
                var updateCommandParams = new
                {
                    FileName = fileName,
                    LastModifiedBy = "LambdaS3",
                    LastModified = DateTime.Now,
                    LabelId = labelDto.LabelId,
                };
                var updateCommand = @$"UPDATE public.labels 
                                      SET pdf_file_name = @FileName, last_modified_by = @LastModifiedBy, last_modified = @LastModified
                                      WHERE id = @LabelId";
                
                var updateResult = await CommandRepository.ExecuteAsync(updateCommand,updateCommandParams);
                if(!updateResult)
                {
                    throw new Exception($"Error to save PDF Filename dor tracking {labelDto.TrackingCode}, Filename = {fileName}");
                }
            }
            else
            {
                throw new Exception($"Error to send {labelDto.TrackingCode} to S3");
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
                else
                {
                    return "";
                }
            }
            else
            {
                return "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";
            }
        }
        public static async Task<bool> ProcessDomesticLabel(DomesticLabelDto labelDto, HtmlToPdfConverter htmlConverter, ILambdaContext lambdaContext)
        {

            lambdaContext.Logger.LogInformation($"{lambdaContext.AwsRequestId} Iniciou ProcessDomesticLabel");

            var html = HtmlDomesticGet(labelDto);


            lambdaContext.Logger.LogInformation($"{lambdaContext.AwsRequestId} Gerou HTML : {html}");           


            lambdaContext.Logger.LogInformation($"{lambdaContext.AwsRequestId} Iniciou COnversão");
            PdfDocument document = new();
            try
            {
                document = htmlConverter.Convert(html, "");
            }
            catch (Exception e)
            {
                lambdaContext.Logger.LogError(e.Message);
            }

            lambdaContext.Logger.LogInformation($"{lambdaContext.AwsRequestId} Gerou o PDF");

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
                    LabelId = labelDto.LabelId,
                };
                var updateCommand = @$"UPDATE public.labels 
                                      SET pdf_file_name = @FileName, last_modified_by = @LastModifiedBy, last_modified = @LastModified
                                      WHERE id = @LabelId";

                var updateResult = await CommandRepository.ExecuteAsync(updateCommand, updateCommandParams);

                lambdaContext.Logger.LogInformation($"{lambdaContext.AwsRequestId} Salvou registro na base");

                if (!updateResult)
                {
                    throw new Exception($"Error to save PDF Filename dor tracking {labelDto.TrackingCode}, Filename = {fileName}");
                }
            }
            else
            {
                throw new Exception($"Error to send {labelDto.TrackingCode} to S3");
            }

            return true;
        }
    }
}
