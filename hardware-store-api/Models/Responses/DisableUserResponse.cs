namespace hardware_store_api.Models.Responses
{
    public record DisableUserResponse
    {
        public RegisterUserResponse OldUser { get; }
        public RegisterUserResponse NewUser { get; }

        public DisableUserResponse(RegisterUserResponse oldUser, RegisterUserResponse newUser)
        {
            OldUser = oldUser;
            NewUser = newUser;
        }
    }
}
