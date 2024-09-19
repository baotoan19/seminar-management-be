using Microsoft.AspNetCore.Http;
using Seminar.CORE.Constants;

namespace Seminar.CORE.Base
{
    public class BaseResponse<T>
    {
        public T? Data { get; set; }
        public object? AdditionalData { get; set; }
        public string? Message { get; set; }
        public int StatusCode { get; set; }
        public string Code { get; set; }

        public BaseResponse(int statusCode, string code, T? data, object? additionalData = null, string? message = null)
        {
            this.StatusCode = statusCode;
            this.Code = code;
            this.Data = data;
            this.AdditionalData = additionalData;
            this.Message = message;
        }

        public BaseResponse(int statusCode, string code, string? message)
        {
            this.StatusCode = statusCode;
            this.Code = code;
            this.Message = message;
        }

        public static BaseResponse<T> OkResponseModel(T data, object? additionalData = null, string code = ResponseCodeConstants.SUCCESS)
        {
            return new BaseResponse<T>(StatusCodes.Status200OK, code, data, additionalData);
        }

        public static BaseResponse<T> NotFoundResponseModel(T? data, object? additionalData = null, string code = ResponseCodeConstants.NOT_FOUND)
        {
            return new BaseResponse<T>(StatusCodes.Status404NotFound, code, data, additionalData);
        }

        public static BaseResponse<T> BadRequestResponseModel(T? data, object? additionalData = null, string code = ResponseCodeConstants.FAILED)
        {
            return new BaseResponse<T>(StatusCodes.Status400BadRequest, code, data, additionalData);
        }

        public static BaseResponse<T> InternalErrorResponseModel(T? data, object? additionalData = null, string code = ResponseCodeConstants.FAILED)
        {
            return new BaseResponse<T>(StatusCodes.Status500InternalServerError, code, data, additionalData);
        }
    }

    public class BaseResponse : BaseResponse<object>
    {
        public BaseResponse(int statusCode, string code, object? data, object? additionalData = null, string? message = null) : base(statusCode, code, data, additionalData, message)
        {
        }

        public BaseResponse(int statusCode, string code, string? message) : base(statusCode, code, message)
        {
        }
    }
}