using ComigleApi.Model.Interfaces;

namespace ComigleApi.Model.Response
{
    public abstract class BaseResponse : IResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
