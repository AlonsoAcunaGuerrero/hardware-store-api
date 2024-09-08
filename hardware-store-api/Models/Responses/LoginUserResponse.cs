namespace hardware_store_api.Models.Responses
{
    public record LoginUserResponse
    {
        public string Username { get; }
        public string Role { get; }
        public string AccessToken { get; }
        public string RefreshToken { get; }

        public LoginUserResponse(string username, string role, string accessToken, string refreshToken)
        {
            Username = username;
            Role = role;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}
