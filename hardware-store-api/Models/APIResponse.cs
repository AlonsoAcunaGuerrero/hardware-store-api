namespace hardware_store_api.Models
{
    public record APIResponse
    {
        public int code { get; }
        public string message { get; }

        public APIResponse(int code, string message)
        {
            this.code = code;
            this.message = message;
        }
    }
}
