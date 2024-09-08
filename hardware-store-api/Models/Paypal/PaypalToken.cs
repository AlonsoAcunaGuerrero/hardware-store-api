namespace hardware_store_api.Models.Paypal
{
    public class PaypalToken
    {
        public string scope { get; }
        public string access_token { get; }
        public string token_type { get; }
        public string app_id { get; }
        public int expires_in { get; }
        public string nonce { get; }

        public PaypalToken(string scope, string access_token, string token_type, string app_id, int expires_in, string nonce)
        {
            this.scope = scope;
            this.access_token = access_token;
            this.token_type = token_type;
            this.app_id = app_id;
            this.expires_in = expires_in;
            this.nonce = nonce;
        }
    }
}
