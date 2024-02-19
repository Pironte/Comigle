using ComigleApi.Model.Response;
using Microsoft.AspNetCore.Identity;

namespace ComigleApi.Model.Authentication
{
    public class RegisterResponse: BaseResponse
    {
        public IEnumerable<IdentityError>? Errors { get; set; }
    }
}
