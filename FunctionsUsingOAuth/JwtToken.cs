using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

namespace FunctionsUsingOAuth
{
	static class JwtToken
	{
		const string SiteUrl = "https://localhost/";
		static readonly string SecretKey = Guid.NewGuid().ToString();   // 短すぎると怒られるよ

		public static (string token, DateTime expirationDate) CreateToken(string user)
		{
			// JWT に含めるクレーム(よくわからないので適当)
			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Jti, user),
				new Claim(ClaimTypes.Sid, user),
				new Claim(ClaimTypes.Name, user),
			};

			var handler = new JwtSecurityTokenHandler();
			var token = handler.CreateJwtSecurityToken(
				issuer: SiteUrl,
				audience: SiteUrl,
				subject: new ClaimsIdentity(claims),
				expires: DateTime.UtcNow.AddSeconds(10),
				signingCredentials: new SigningCredentials(
					new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey)),
					SecurityAlgorithms.HmacSha256
				)
			);

			return (handler.WriteToken(token), token.ValidTo);

		}

		/// <summary>
		/// Bearerトークンからユーザー名と有効期限でも返そう
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static (string name, DateTime? expirationDate) GetInfoFromHeader(StringValues value)
		{
			var authKey = value.ToString();
			if (authKey.Length < 10) return (null, null);
			var scheme = authKey.Substring(0, 7).Trim();
			if (scheme != "Bearer") return (null, null);
			var tokenString = authKey.Substring(7);

			var handler = new JwtSecurityTokenHandler();
			var validationParameters = new TokenValidationParameters
			{
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey)),
				ValidAudience = SiteUrl,
				ValidIssuer = SiteUrl,
				ValidateLifetime = true		// ?
			};

			try
			{
				var principal = handler.ValidateToken(tokenString, validationParameters, out var token);
				var jwtToken = token as JwtSecurityToken;
				return (jwtToken?.Payload.Jti, jwtToken?.ValidTo);
			}
			catch(Exception e)
			{
				// 不正なトークンだと例外
				return (null, null);
			}
		}
	}
}
