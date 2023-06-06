using Syncfusion.HtmlConverter;
using Syncfusion.Pdf.Graphics;

namespace Sinerlog.Lambda.Pdf.Common
{
    public static class PdfConfiguration
    {
        public static void Configure()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NRAiBiAaIQQuGjN/V0d+Xk9CfV5AQmBIYVp/TGpJfl96cVxMZVVBJAtUQF1hSn5WdENiWn5Wc3NWQ2Ba");
        }
        public static HtmlToPdfConverter GetLabelConverter()
        {
            Configure();
            //Initialize HTML to PDF converter with Blink rendering engine.
            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter(HtmlRenderingEngine.Blink);

            BlinkConverterSettings blinkConverterSettings = new BlinkConverterSettings();

            blinkConverterSettings.BlinkPath = Path.GetFullPath("BlinkBinariesAws");
            blinkConverterSettings.CommandLineArguments.Add("--no-sandbox");
            blinkConverterSettings.CommandLineArguments.Add("--disable-setuid-sandbox");
            blinkConverterSettings.Scale = 1.8f;
            blinkConverterSettings.EnableOfflineMode = false;


            var margins = new PdfMargins
            {
                Bottom = 7,
                Left = 7,
                Right = 10,
                Top = 7
            };
            blinkConverterSettings.Margin = margins;

            blinkConverterSettings.AdditionalDelay = 300;
            blinkConverterSettings.EnableJavaScript = true;

            htmlConverter.ConverterSettings = blinkConverterSettings;

            return htmlConverter;
        }

        public static HtmlToPdfConverter GetInvoiceConverter()
        {
            Configure();
            //Initialize HTML to PDF converter with Blink rendering engine.
            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter(HtmlRenderingEngine.Blink);

            BlinkConverterSettings blinkConverterSettings = new BlinkConverterSettings();

            blinkConverterSettings.BlinkPath = Path.GetFullPath("BlinkBinariesAws");
            blinkConverterSettings.CommandLineArguments.Add("--no-sandbox");
            blinkConverterSettings.CommandLineArguments.Add("--disable-setuid-sandbox");
            blinkConverterSettings.Scale = 1f;

            htmlConverter.ConverterSettings = blinkConverterSettings;

            return htmlConverter;
        }
    }
}