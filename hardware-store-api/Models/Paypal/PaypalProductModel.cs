namespace hardware_store_api.Models.Paypal
{
    public class PaypalProductModel
    {
        public string reference_id { get; }

        public string name { get; }

        public string description { get; }

        public string quantity {  get; }

        public PaypalAmountModel amount { get; }

        public PaypalProductModel(string reference_id, string name, string description, string quantity, PaypalAmountModel amount)
        {
            this.reference_id = reference_id;
            this.name = name;
            this.description = description;
            this.quantity = quantity;
            this.amount = amount;
        }
    }

    public class PaypalAmountModel
    {
        public string currency_code {  get; }
        public string value { get; }

        public PaypalAmountModel(string currency_code, string value)
        {
            this.currency_code = currency_code;
            this.value = value;
        }
    }
}
