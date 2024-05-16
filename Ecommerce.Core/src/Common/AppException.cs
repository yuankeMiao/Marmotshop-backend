using System.Net;

namespace Ecommerce.Core.src.Common
{
    public class AppException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public AppException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
            Message = message;
        }

        #region Custom Exceptions

        public static AppException NotFound(string message = "Not Found")
        {
            return new AppException(HttpStatusCode.BadRequest, message)
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = message
            };
        }
        public static AppException UserNotFound(string message = "User Not Found")
        {
            return new AppException(HttpStatusCode.BadRequest, message)
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = message
            };
        }

        public static AppException ProductNotFound(string message = "Product Not Found")
        {
            return new AppException(HttpStatusCode.BadRequest, message)
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = message
            };
        }

        public static AppException InvalidLoginCredentials(string message = "Invalid Login Credentials")
        {
            return new AppException(HttpStatusCode.BadRequest, message)
            {
                StatusCode = HttpStatusCode.BadRequest, // 400
                Message = message
            };
        }

        public static AppException NotLogin(string message = "User is not logged in")
        {
            return new AppException(HttpStatusCode.BadRequest, message)
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = message
            };
        }

        public static AppException Duplicate(string duplicatedItem)
        {
            return new AppException(HttpStatusCode.BadRequest, duplicatedItem)
            {
                StatusCode = HttpStatusCode.Conflict, // 409
                Message = $"{duplicatedItem} already exists."
            };
        }

        public static AppException InvalidInput(string message = "Invalid input")
        {
            return new AppException(HttpStatusCode.BadRequest, message)
            {
                StatusCode = HttpStatusCode.BadRequest, // 400
                Message = message
            };
        }

        public static AppException InternalServerError(string message = "Internal Server Error")
        {
            return new AppException(HttpStatusCode.BadRequest, message)
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = message
            };
        }
        #endregion

    }
}