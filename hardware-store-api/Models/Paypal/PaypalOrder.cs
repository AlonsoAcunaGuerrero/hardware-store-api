namespace hardware_store_api.Models.Paypal
{
    public class PaypalOrder
    {
        public string id { get; }
        public string status { get; }
        public PaymentSource payment_source { get; }
        public List<Link> links { get; }

        public PaypalOrder(string id, string status, PaymentSource payment_source, List<Link> links)
        {
            this.id = id;
            this.status = status;
            this.payment_source = payment_source;
            this.links = links;
        }
    }
    public class PaymentSource
    {
        public PayPal payPal { get; }

        public PaymentSource(PayPal payPal)
        {
            this.payPal = payPal;
        }
    }

    public class PayPal
    {
        
    }

    public class Link
    {
        public string href { get; }
        public string rel { get; }
        public string method { get; }

        public Link(string href, string rel, string method)
        {
            this.href = href;
            this.rel = rel;
            this.method = method;
        }
    }

}
