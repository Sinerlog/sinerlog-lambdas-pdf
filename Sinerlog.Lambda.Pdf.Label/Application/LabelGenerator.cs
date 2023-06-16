using Amazon.Lambda.Core;
using DataMatrix.net;
using Sinerlog.Lambda.Pdf.Common;
using Sinerlog.Lambda.Pdf.Common.Persistency;
using Sinerlog.Lambda.Pdf.Label.Application.Models;
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;
using System.Drawing;
using System.Globalization;
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

            texto = texto.Replace("{logo}", label.LogoBase64);
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
        private static string HtmlInternationalGet(InternationalLabelDto label)
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

            texto = texto.Replace("{logo}", label.LogoBase64);
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
            texto = texto.Replace("{weight}", label.Weight.ToString());
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
            var html = HtmlInternationalGet(labelDto);

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
