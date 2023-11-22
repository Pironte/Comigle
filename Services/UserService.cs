using AutoMapper;
using Comigle.Model;
using Comigle.Model.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Comigle.Services
{
    public class UserService
    {
        private IMapper _mapper;
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private TokenService _tokenService;

        public UserService(IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager, TokenService tokenService)
        {
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        internal async Task<string> Login(LoginUserDto loginUserDto)
        {
            if (string.IsNullOrWhiteSpace(loginUserDto.UserName))
            {
                throw new ArgumentNullException("Deve-se informar o usuário");
            }

            if (string.IsNullOrWhiteSpace(loginUserDto.Password))
            {
                throw new ArgumentNullException("Deve-se informar a senha");
            }

            SignInResult result = await _signInManager.PasswordSignInAsync(loginUserDto.UserName, loginUserDto.Password, false, false);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Não foi possível autenticar o usuário");
            }

            var user = _signInManager.UserManager.Users.FirstOrDefault(user => user.NormalizedUserName == loginUserDto.UserName.ToUpper());

            if (user == null)
            {
                throw new NullReferenceException("Usuário não existe");
            }

            var token = _tokenService.GenerateToken(user);

            return token;
        }

        internal async void LogOut()
        {
            await _signInManager.SignOutAsync();
        }

        internal async Task Register(CreateUserDto createUserDto)
        {
            var user = _mapper.Map<User>(createUserDto);

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
                throw new InvalidOperationException("Falha ao cadastrar usuário");
            }
        }
    }
}
