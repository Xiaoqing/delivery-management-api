namespace DeliveryManagement.Domain.Models
{
    public class ErrorDetail
    {
        public ErrorDetail(string errorCode, string errorDescription = null, string errorUri = null)
        {
            this.ErrorCode = errorCode;
            this.ErrorDescription = errorDescription;
            this.ErrorUri = errorUri;
        }

        public string ErrorCode { get; }

        public string ErrorDescription { get; }

        public string ErrorUri { get; }

        public override string ToString()
        {
            var errorString = $"{this.ErrorCode}: {this.ErrorDescription}";

            if (!string.IsNullOrEmpty(this.ErrorUri))
            {
                return errorString + $" For more inforation on this error, see {this.ErrorUri}";
            }

            return errorString;
        }
    }
}
