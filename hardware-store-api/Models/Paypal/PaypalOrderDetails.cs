namespace hardware_store_api.Models.Paypal
{
    public class PaypalOrderDetails
    {
        public string id { get; }
        public string status { get; }
        public string intent { get; }
        public PaymentSource payment_source { get; }
        public List<PaypalProductModel> purchase_units { get; }
        public Payer payer { get; }
        public string create_time { get; }
        public List<Link> links { get; }

        public PaypalOrderDetails(string id, string status, string intent, PaymentSource payment_source, List<PaypalProductModel> purchase_units, 
            Payer payer, string create_time, List<Link> links)
        {
            this.id = id;
            this.status = status;
            this.intent = intent;
            this.payment_source = payment_source;
            this.purchase_units = purchase_units;
            this.payer = payer;
            this.create_time = create_time;
            this.links = links;
        }
    }

    public class Payer
    {
        public PayerName name { get; }
        public string email_address { get; }
        public string payer_id { get; }

        public Payer(PayerName name, string email_address, string payer_id)
        {
            this.name = name;
            this.email_address = email_address;
            this.payer_id = payer_id;
        }
    }

    public class PayerName
    {
        public string given_name { get; }
        public string surname { get; }

        public PayerName(string given_name, string surname)
        {
            this.given_name = given_name;
            this.surname = surname;
        }
    }
}
