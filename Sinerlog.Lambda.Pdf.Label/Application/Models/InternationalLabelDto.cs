namespace Sinerlog.Lambda.Pdf.Label.Application.Models
{
    public class InternationalLabelDto : DomesticLabelDto
    {
        public OrderDto Order { get; set; }
        public class OrderDto
        {
            public string Currency { get; set; }
            public decimal ShippingCost { get; set; }
            public decimal Insurance { get; set; }
            public decimal Others { get; set; }
            public decimal Discount { get; set; }
            public decimal Total { get; set; }
            public List<OrderItem> Items { get; set; }
            public class OrderItem
            {
                public string SHCode { get; set; }
                public int Quantity { get; set; }
                public decimal Price { get; set; }
                public decimal Total { get; set; }
                public string Name { get; set; }
            }
        }
    }
}
