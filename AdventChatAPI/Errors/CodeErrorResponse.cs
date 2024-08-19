using Newtonsoft.Json;

namespace Ecommerce.Api.Errors
{
    public class CodeErrorResponse
    {
        [JsonProperty(PropertyName="statusCode")]
        public int StatusCode { get; set; }
        [JsonProperty(PropertyName = "message")]
        public string[]? Message { get; set; }

        public CodeErrorResponse(int statusCode, string[]? message=null)
        {
            StatusCode = statusCode;
            if (message is null) {
                Message = new string[0];
                var text = GetDefaultMessageStatusCode(statusCode);
            }
            else
            {
                Message = message;
            } 
        }

        private string GetDefaultMessageStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "Request sended has errors",
                401 => "Haven´t Authorization for this resource",
                404 => "No Founded resource asked it",
                500 => "Founded error in the Server",
                _ => string.Empty
            };
        }
    }
}
