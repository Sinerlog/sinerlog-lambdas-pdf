using Syncfusion.HtmlConverter;
using Syncfusion.Pdf.Graphics;

namespace Sinerlog.Lambda.Pdf.Common
{
    public static class PdfConfiguration
    {
        public static void Configure()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NRAiBiAaIQQuGjN/V0d+XU9Ad1RHQmFMYVF2R2BJe1RycV9HaEwxOX1dQl9gSXpRc0RlWnhbcnxQQmQ=");
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

            var margins = new PdfMargins
            {
                Bottom = 7,
                Left = 7,
                Right = 10,
                Top = 7
            };
            blinkConverterSettings.Margin = margins;

            blinkConverterSettings.AdditionalDelay = 1000;
            blinkConverterSettings.EnableJavaScript = true;

            htmlConverter.ConverterSettings = blinkConverterSettings;

            return htmlConverter;
        }
    }
}