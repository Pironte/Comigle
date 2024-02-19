using AutoMapper;
using Comigle.Model;
using Comigle.Model.Dtos;
using ComigleApi.Model.Authentication;
using ComigleApi.Model.Email;
using ComigleApi.Model.Request;
using ComigleApi.Model.Response;
using ComigleApi.Services;
using Microsoft.AspNetCore.Identity;
using System.Web;

namespace Comigle.Services
{
    public class UserService
    {
        private const string DUPLICATEUSER = "DuplicateUserName";

        private IMapper _mapper;
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private TokenService _tokenService;
        private EmailService _emailService;

        public UserService(IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager, TokenService tokenService, EmailService emailService)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        /// <summary>
        /// Realiza a autenticação do usuário
        /// </summary>
        /// <param name="loginUserDto"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        internal async Task<LoginResponse> Login(LoginUserDto loginUserDto)
        {
            var loginResponse = new LoginResponse();

            if (string.IsNullOrWhiteSpace(loginUserDto.UserName))
            {
                loginResponse.Message = "Necessário informar o usuário";

                return loginResponse;
            }

            if (string.IsNullOrWhiteSpace(loginUserDto.Password))
            {
                loginResponse.Message = "Deve-se informar a senha";

                return loginResponse;
            }

            SignInResult result = await _signInManager.PasswordSignInAsync(loginUserDto.UserName, loginUserDto.Password, false, false);

            if (!result.Succeeded)
            {
                loginResponse.Message = "Falha ao autenticar o usuário";

                return loginResponse;
            }

            var user = _signInManager.UserManager.Users.FirstOrDefault(user => user.NormalizedUserName == loginUserDto.UserName.ToUpper());

            if (user == null)
            {
                loginResponse.Message = "Usuário informado não existe";

                return loginResponse;
            }

            loginResponse.Success = true;
            loginResponse.Token = _tokenService.GenerateToken(user);

            return loginResponse;
        }

        internal async void LogOut()
        {
            await _signInManager.SignOutAsync();
        }

        /// <summary>
        /// Registra o usuário no banco de dados
        /// </summary>
        /// <param name="createUserDto"></param>
        /// <returns>Retorna um booleano representando sucesso ou falha no processo</returns>
        /// <exception cref="ArgumentNullException"></exception>
        internal async Task<RegisterResponse> Register(CreateUserDto createUserDto)
        {
            var user = _mapper.Map<User>(createUserDto);
            var registerResponse = new RegisterResponse() { Success = true, Message = "Usuário cadastrado com sucesso!" };

            if (string.IsNullOrEmpty(createUserDto.UserName))
            {
                throw new ArgumentNullException("Usuário não pode ser vazio");
            }

            if (string.IsNullOrEmpty(createUserDto.Password))
            {
                throw new ArgumentNullException("Senha não pode ser vazia");
            }

            IdentityResult result = await _userManager.CreateAsync(user, createUserDto.Password);

            if (!result.Succeeded)
            {
                TreatRegisterUserErrors(ref result);
                registerResponse.Errors = result.Errors;
                registerResponse.Success = false;
                registerResponse.Message = result?.Errors?.FirstOrDefault()?.Description;
            }

            return registerResponse;
        }

        internal async Task<SendEmailResponse> SendEmailToResetPassword(SendEmailRequest sendEmailRequest)
        {
            var response = new SendEmailResponse() { Success = true };

            if (string.IsNullOrWhiteSpace(sendEmailRequest.UserName))
            {
                response.Success = false;
                response.Message = "Nome de usuário é obrigatório!";

                return response;
            }

            var user = _userManager.Users.FirstOrDefault(p => p.NormalizedUserName == sendEmailRequest.UserName.ToUpper());
            if (user == null || user.Email == null)
            {
                response.Success = false;
                response.Message = "Usuário não existe!";

                return response;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(token);
            var userId = HttpUtility.UrlEncode(user.Id);

            var callbackUrl = $"{sendEmailRequest.CallbackUrl}?userId={userId}&token={encodedToken}";

            var emailSubject = "Redefinir Senha";
            var emailBody = $"Por favor, redefina sua senha clicando <a href='{callbackUrl}'>aqui</a>.";

            var emailResponse = await _emailService.SendEmail("comiglecompany@gmail.com", user.Email, emailSubject, emailBody);
            if (!emailResponse.Success)
            {
                response.Success = false;
                response.Message = emailResponse.Message;

                return response;
            }

            return response;
        }

        internal async Task<ResetPasswordResponse> ResetPassword(ResetPasswordRequest resetPasswordRequest)
        {
            var response = new ResetPasswordResponse() { Success = true };
            if (string.IsNullOrWhiteSpace(resetPasswordRequest.NewPassword) || string.IsNullOrWhiteSpace(resetPasswordRequest.Token))
            {
                response.Success = false;
                response.Message = "Dados obrigatórios para a troca de senha não foram informados!";

                return response;
            }

            resetPasswordRequest.UserId = HttpUtility.UrlDecode(resetPasswordRequest.UserId);
            var user = _userManager.Users.FirstOrDefault(p => p.Id == resetPasswordRequest.UserId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "Usuário não existe!";

                return response;
            }

            resetPasswordRequest.Token = HttpUtility.UrlDecode(resetPasswordRequest.Token).Replace(" ", "+");

            var resetPasswordResponse = await _userManager.ResetPasswordAsync(user, resetPasswordRequest.Token, resetPasswordRequest.NewPassword);
            if (!resetPasswordResponse.Succeeded)
            {
                response.Success = false;
                response.Message = resetPasswordResponse?.Errors?.FirstOrDefault()?.Description;
            }

            return response;
        }

        /// <summary>
        /// Trata mensagens de erro retornadas pelo EntityFramework
        /// </summary>
        public void TreatRegisterUserErrors(ref IdentityResult identityResult)
        {
            foreach (var identityError in identityResult.Errors)
            {
                if (identityError.Code == DUPLICATEUSER)
                {
                    identityError.Description = "Usuário já existe";
                }
            }
        }
    }
}
