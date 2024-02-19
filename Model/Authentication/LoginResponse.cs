using ComigleApi.Model.Response;

namespace ComigleApi.Model.Authentication
{
    public class LoginResponse: BaseResponse
    {
        public string? Token { get; set; }
    }
}
