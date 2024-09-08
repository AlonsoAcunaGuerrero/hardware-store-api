using System.Net;

namespace hardware_store_api.Exceptions
{
    public class HttpStatusException : Exception
    {
        public HttpStatusCode Status { get; private set; }

        public HttpStatusException(HttpStatusCode status, string? message) : base(message)
        {
            Status = status;
        }
    }
}
