using BCrypt.Net;
using Identity.Application.DTOs.Auth;
using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Interfaces;

namespace Identity.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        IUserRepository userRepository,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
            throw new Exception("User already exists");

        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = Role.User
        };
        await _userRepository.AddAsync(user);

        return await GenerateTokensAsync(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
            throw new Exception("Invalid credentials");

        var validPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!validPassword)
            throw new Exception("Invalid credentials");

        return await GenerateTokensAsync(user);
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        var hashedToken = _jwtTokenGenerator.HashToken(refreshToken);

        var user = await _userRepository.GetUserByRefreshTokenHashAsync(hashedToken);
        if (user == null)
            throw new Exception("Invalid refresh token");

        var storedToken = user.RefreshTokens
            .FirstOrDefault(rt => rt.TokenHash == hashedToken && !rt.IsRevoked);

        if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow)
            throw new Exception("Refresh token expired");

        storedToken.IsRevoked = true;
        await _userRepository.SaveChangesAsync();

        return await GenerateTokensAsync(user);
    }

    private async Task<AuthResponse> GenerateTokensAsync(User user)
    {
        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);

        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
        var hashedRefreshToken = _jwtTokenGenerator.HashToken(refreshToken);

        var refreshTokenEntity = new RefreshToken
        {
            TokenHash = hashedRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            UserId = user.Id
        };

        await _userRepository.AddRefreshTokenAsync(refreshTokenEntity);
        await _userRepository.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var hashed = _jwtTokenGenerator.HashToken(refreshToken);
        await _userRepository.RevokeRefreshTokenAsync(hashed);
    }
}