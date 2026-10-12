namespace AhirMilap.DTO
{
    public class CommonResponse
    {
        public Status status { get; set; }
        public string message { get; set; }
        public object? data { get; set; }

        public CommonResponse(Status status, string message, object? data = null)
        {
            this.status = status;
            this.message = message;
            this.data = data;
        }
    }


    public enum Status
    {
        Error = 0,
        Success = 1,
        NotFound = 2
    }
}
