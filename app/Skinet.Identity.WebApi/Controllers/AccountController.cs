using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Skinet.Identity.Application.Dtos;
using Skinet.Identity.Application.Extensions;
using Skinet.Identity.Application.Mappings;
using Skinet.Identity.Domain.Entities;
using Skinet.Identity.Domain.Interfaces;
using Skinet.Identity.Domain.ValueObjects;
using skinet.identity.Dtos;
using skinet.identity.Errors;
using skinet.identity.Exceptions;

namespace skinet.identity.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            ITokenService tokenService)
        {
            _signInManager = signInManager;
            _tokenService = tokenService;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userManager.FindByEmailFromClaimsPrinciple(User);

            return new UserDto
            {
                Email = user.Email,
                Token = _tokenService.CreateToken(user),
                DisplayName = user.DisplayName

            };
        }

        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmailExistsAsync([FromQuery] string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }

        [HttpGet("address")]
        [Authorize]
        public async Task<ActionResult<AddressDto>> GetUserAddress()
        {
            var user = await _userManager.FindUserByClaimsPrincipleWithAddressAsync(User);

            return user.Address.ToDto();
        }

        [HttpPut("address")] 
        [Authorize]

        public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto addressDto)
        { 
            var user = await _userManager.FindUserByClaimsPrincipleWithAddressAsync(User);

            var userNameOrError = UserName.Create(addressDto.FirstName, addressDto.LastName);
            if (userNameOrError.IsFailure)
            {
                return BadRequest(new ApiResponse(400, userNameOrError.Error));
            }
            
            var deliveryDetailsOrError = DeliveryDetails.Create(addressDto.Street, addressDto.City, addressDto.Street, addressDto.ZipCode);
            if (deliveryDetailsOrError.IsFailure)
            {
                return BadRequest(new ApiResponse(400, deliveryDetailsOrError.Error));
            }
            
            var address = new Address(userNameOrError.Value, deliveryDetailsOrError.Value);
            user.ChangeAddress(address);

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded) return Ok(user.Address.ToDto());

            return BadRequest("Problem updating the user");
        }


        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null) return Unauthorized(new ApiResponse(401));
            
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded) return Unauthorized(new ApiResponse(401));

            return new UserDto
            {
                Email = user.Email,
                Token = _tokenService.CreateToken(user),
                DisplayName = user.DisplayName

            };
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (CheckEmailExistsAsync(registerDto.Email).Result.Value)
            {
                return new BadRequestObjectResult(new ApiValidationErrorResponse
                {
                    Errors = new[]
                    { "Email adress is in use" }
                });
            }

            var user = new AppUser
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded) return BadRequest(new ApiResponse(400));

            return new UserDto
            {
                DisplayName = user.DisplayName,
                Token = _tokenService.CreateToken(user),
                Email = user.Email
            };
        }

    }
}