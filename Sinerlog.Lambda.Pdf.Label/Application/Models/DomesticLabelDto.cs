namespace Sinerlog.Lambda.Pdf.Label.Application.Models
{
    public class DomesticLabelDto
    {
        #region Images
        public string LogoNameS3 { get; set; }
        public string DataMatrixBase64 { get; set; }
        #endregion

        #region Package Dimensions
        public decimal Weight { get; set; }

        #endregion

        #region Receiver Data
        public string ReceiverName { get; set; } = "";
       // public string ReceiverDocument { get; set; } = "";
        public string ReceiverPhone { get; set; } = "";
        public string ReceiverLatitude { get; set; } = "0000000000";
        public string ReceiverLongite { get; set; } = "0000000000";
        public string ReceiverStreet { get; set; } = "";
        public string ReceiverNeighborhood { get; set; } = "";
        public string ReceiverNumber { get; set; } = "";
        public string ReceiverState { get; set; } = "";
        public string ReceiverZipCode{ get; set; } = "";
        public string ReceiverComplement{ get; set; } = "";
        public string ReceiverCity { get; set; } = "";
        //public string ReceiverCountry { get; set; } = "";

        #endregion

        #region Sender Data

        public string SenderName { get; set; } = "";
        //public string SenderDocument { get; set; } = "";
        //public string SenderPhone { get; set; } = "";
       // public string SenderLatitude { get; set; } = "0000000000";
        //public string SenderLongite { get; set; } = "0000000000";
        public string SenderStreet { get; set; } = "";
        public string SenderNeighborhood { get; set; } = "";
        public string SenderNumber { get; set; } = "";
        public string SenderState { get; set; } = "";
        public string SenderZipCode { get; set; } = "";
        public string SenderComplement { get; set; } = "";
        public string SenderCity { get; set; } = "";
        public string SenderCountry { get; set; } = "";

        #endregion

        #region General Package Data

        public int LabelId { get; set; }
        public string ServiceType { get; set; } = "";
        public string TrackingCode { get; set; } = "";
       // public string SinerlogTrackingCode { get; set; } = "";
        public string PostalCard { get; set; } = "";
        public string ServiceCode { get; set; } = "";
        public decimal Amount { get; set; } = 0;
       // public string InvoiceNumber { get; set; } = "";
        public int? DeliveryCourier { get; set; }

        #endregion

        #region BarCode Configs

        public string FontSizeBarCode { get; set; }
        public string LineHeight { get; set; }
        public string ScaleBarCode { get; set; }
        public int ModuleSizeDataMatrix { get; set; }

        #endregion
    }
}
