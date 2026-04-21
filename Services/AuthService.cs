using HotelBookingAPI.Data;
using HotelBookingAPI.DTOs.Auth;
using HotelBookingAPI.Models;
using HotelBookingAPI.Repositories.Interfaces;
using HotelBookingAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly AppDbContext _context;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IJwtService jwtService,
            AppDbContext context,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _context = context;
            _logger = logger;
        }

        public async Task<TokenDto> RegisterAsync(RegisterDto dto)
        {
            if (await _userRepository.EmailExistsAsync(dto.Email))
                throw new InvalidOperationException("Email is already registered.");

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.CreateAsync(user);
            _logger.LogInformation("New user registered: {Email}", dto.Email);

            return await IssueTokensAsync(user);
        }

        public async Task<TokenDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            if (!user.IsActive)
                throw new UnauthorizedAccessException("Account is deactivated.");

            _logger.LogInformation("User logged in: {Email}", dto.Email);
            return await IssueTokensAsync(user);
        }

        public async Task<TokenDto> RefreshTokenAsync(string token)
        {
            var hashedToken = HashToken(token);
            var refreshToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == hashedToken);

            if (refreshToken == null || refreshToken.ExpiresAt < DateTime.UtcNow || refreshToken.IsRevoked)
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");

            // Rotate: revoke old, issue new
            refreshToken.IsRevoked = true;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Refresh token rotated for user ID: {UserId}", refreshToken.UserId);
            return await IssueTokensAsync(refreshToken.User!);
        }

        public async Task RevokeTokenAsync(string token)
        {
            var hashedToken = HashToken(token);
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == hashedToken);

            if (refreshToken != null)
            {
                refreshToken.IsRevoked = true;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Refresh token revoked for user ID: {UserId}", refreshToken.UserId);
            }
        }

        // ── Private helpers ──────────────────────────────────────────────────────

        private async Task<TokenDto> IssueTokensAsync(User user)
        {
            var accessToken = _jwtService.GenerateAccessToken(user);
            var rawRefreshToken = _jwtService.GenerateRefreshToken();
            var expiryMinutes = 60; // fall-back; access token expiry already embedded in JWT

            var refreshTokenEntity = new RefreshToken
            {
                Token = HashToken(rawRefreshToken),
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new TokenDto
            {
                AccessToken = accessToken,
                RefreshToken = rawRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes)
            };
        }

        private static string HashToken(string token)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(token);
            return Convert.ToBase64String(sha.ComputeHash(bytes));
        }
    }

}
