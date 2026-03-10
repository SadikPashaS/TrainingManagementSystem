using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TrainingManagement.Application.DTOs.Auth;
using TrainingManagement.Application.Interfaces;
using TrainingManagement.Domain.Entities;
using TrainingManagement.Domain.Interfaces;

// Use fully qualified name for BCrypt to avoid any ambiguity
using BCryptNet = BCrypt.Net.BCrypt;

namespace TrainingManagement.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _unitOfWork.Users.GetByUserIdAsync(loginDto.UserId);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid User ID or Password");
            }

            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid User ID or Password");
            }

            // Verify password using fully qualified name
            bool isValid = BCryptNet.Verify(loginDto.Password, user.PasswordHash);

            if (!isValid)
            {
                throw new UnauthorizedAccessException("Invalid User ID or Password");
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("User account is deactivated");
            }

            // Update last login
            user.LastLoginAt = DateTime.UtcNow;
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.CompleteAsync();

            var response = _mapper.Map<AuthResponseDto>(user);
            response.Token = GenerateJwtToken(user);
            response.TokenExpiry = DateTime.UtcNow.AddHours(8);

            return response;
        }

        public Task LogoutAsync(int userId)
        {
            return Task.CompletedTask;
        }

        public Task<bool> ValidateTokenAsync(string token)
        {
            return Task.FromResult(!string.IsNullOrEmpty(token));
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "YourSuperSecretKeyForJWTTokenGenerationThatIsAtLeast32CharactersLong");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserId),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}