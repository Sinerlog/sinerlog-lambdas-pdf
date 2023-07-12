namespace Sinerlog.Lambda.Pdf.Invoice.Application.Models
{
    public class InvoiceReportDto
    {
        public int OrderId { get; set; }
        public string? LogoNameS3 { get; set; }
        public string? LogoBase64 { get; set; }
        public string QrCodeBase64 { get; set; }
        public string StampDate { get; set; }
        public string UniqueId { get; set; }
        public string InvoiceNumber { get; set; }
        public string OrderDate { get; set; }
        public string SellerName { get; set; }
        public string SellerDocumentType { get; set; }
        public string SellerEmail { get; set; }
        public string SellerDocument { get; set; }
        public string SellerPhone { get; set; }
        public string SalesChannelName { get; set; }
        public string SellerAddressDescription { get; set; }
        public string SellerAddressCity { get; set; }
        public string SellerAddressNumber { get; set; }
        public string SellerAddressState { get; set; }
        public string SellerAddressComplement { get; set; }
        public string SellerAddressZipCode { get; set; }
        public string SellerAddressNeighborhood { get; set; }
        public string SellerAddressCountry { get; set; }
        public string BuyerName { get; set; }
        public string BuyerDocumentType { get; set; }
        public string BuyerEmail { get; set; }
        public string BuyerDocument { get; set; }
        public string BuyerPhone { get; set; }
        public string BillingAddressDescription { get; set; }
        public string BillingAddressCity { get; set; }
        public string BillingAddressNumber { get; set; }
        public string BillingAddressState { get; set; }
        public string BillingAddressComplement { get; set; }
        public string BillingAddressZipCode { get; set; }
        public string BillingAddressNeighborhood { get; set; }
        public string BillingAddressCountry { get; set; }
        public string TrackingCode { get; set; }
        public string ShippingMethod { get; set; }
        public string ConsigneeName { get; set; }
        public string ShippingAddressComplement { get; set; }
        public string ShippingAddressZipCode { get; set; }
        public string ServiceType { get; set; }
        public string EstimatedDueDate { get; set; }
        public string ShippingAddressDescription { get; set; }
        public string ShippingAddressNeighborhood { get; set; }
        public string ShippingAddressState { get; set; }
        public string ShipmentWeigth { get; set; }
        public string ShippingAddressNumber { get; set; }
        public string ShippingAddressCity { get; set; }
        public string ShippingAddressCountry { get; set; }
        public string HtmlDetails { get; set; }
        public string OrderCurrency { get; set; }
        public string OrderProductCost { get; set; }
        public string OrderDiscount { get; set; }
        public string OrderShippingCost { get; set; }
        public string OrderTax { get; set; }
        public string OrderTotal { get; set; }
        public string OrderInsurance { get; set; }
        public string HtmlPaymentInfo { get; set; }
        public string AccountTaxModality { get; set; }
        public List<InvoiceReportDtoPayment> Payments { get; set; }
        public List<InvoiceReportDtoItem> Items { get; set; }
        public class InvoiceReportDtoPayment
        {
            public string Currency { get; set; }
            public string Method { get; set; }
            public string Total { get; set; }
            public string CardBrand { get; set; }
        }
        public class InvoiceReportDtoItem
        {
            public string Name { get; set; }
            public string Sku { get; set; }
            public string Quantity { get; set; }
            public string Price { get; set; }
            public string HsCode { get; set; }
            public string Ncm { get; set; }
            public string Total { get; set; }
            public string TaxModality { get; set; }
            public bool HasRciEnabled { get; set; }
            public InvoiceReportDtoItemTaxes Taxes { get; set; }

            public class InvoiceReportDtoItemTaxes
            {
                public string IPI { get; set; }
                public string ICMS { get; set; }
                public string II { get; set; }
                public string PIS { get; set; }
                public string COFINS { get; set; }
            }
        }
    }
}
