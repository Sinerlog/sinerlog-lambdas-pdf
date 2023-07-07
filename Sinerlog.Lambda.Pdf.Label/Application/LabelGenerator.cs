﻿using Amazon.Lambda.Core;
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
                return "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAqUAAADqCAYAAACbSTyiAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAydpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDcuMS1jMDAwIDc5LmIwZjhiZTkwLCAyMDIxLzEyLzE1LTIxOjI1OjE1ICAgICAgICAiPiA8cmRmOlJERiB4bWxuczpyZGY9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkvMDIvMjItcmRmLXN5bnRheC1ucyMiPiA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtbG5zOnhtcE1NPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvbW0vIiB4bWxuczpzdFJlZj0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL3NUeXBlL1Jlc291cmNlUmVmIyIgeG1wOkNyZWF0b3JUb29sPSJBZG9iZSBQaG90b3Nob3AgMjMuMiAoTWFjaW50b3NoKSIgeG1wTU06SW5zdGFuY2VJRD0ieG1wLmlpZDo2ODg5N0ZGQUJEOEUxMUVDODU1QUZGQzkxODFBMkM2NCIgeG1wTU06RG9jdW1lbnRJRD0ieG1wLmRpZDo2ODg5N0ZGQkJEOEUxMUVDODU1QUZGQzkxODFBMkM2NCI+IDx4bXBNTTpEZXJpdmVkRnJvbSBzdFJlZjppbnN0YW5jZUlEPSJ4bXAuaWlkOjY4ODk3RkY4QkQ4RTExRUM4NTVBRkZDOTE4MUEyQzY0IiBzdFJlZjpkb2N1bWVudElEPSJ4bXAuZGlkOjY4ODk3RkY5QkQ4RTExRUM4NTVBRkZDOTE4MUEyQzY0Ii8+IDwvcmRmOkRlc2NyaXB0aW9uPiA8L3JkZjpSREY+IDwveDp4bXBtZXRhPiA8P3hwYWNrZXQgZW5kPSJyIj8+cCua+QAATLRJREFUeNrs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";
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
