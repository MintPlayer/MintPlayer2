﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MintPlayer.Data.Dtos;
using MintPlayer.Data.Exceptions.Account;
using MintPlayer.Data.Exceptions.Account.ExternalLogin;
using MintPlayer.Data.Options;

namespace MintPlayer.Data.Repositories
{
	internal class AccountRepository : Interfaces.IAccountRepository
	{
		private MintPlayerContext mintplayer_context;
		private UserManager<Entities.User> user_manager;
		private SignInManager<Entities.User> signin_manager;
		private JwtIssuerOptions jwtIssuerOptions;
		public AccountRepository(UserManager<Entities.User> user_manager, SignInManager<Entities.User> signin_manager, MintPlayerContext mintplayer_context, IOptions<JwtIssuerOptions> jwtIssuerOptions)
		{
			this.user_manager = user_manager;
			this.signin_manager = signin_manager;
			this.mintplayer_context = mintplayer_context;
			this.jwtIssuerOptions = jwtIssuerOptions.Value;
		}

		public async Task<Tuple<Dtos.User, string>> Register(Dtos.User user, string password)
		{
			try
			{
				var entity_user = ToEntity(user);
				var identity_result = await user_manager.CreateAsync(entity_user, password);
				if (identity_result.Succeeded)
				{
					var dto_user = ToDto(entity_user);
					var confirmation_token = await user_manager.GenerateEmailConfirmationTokenAsync(entity_user);
					return new Tuple<Dtos.User, string>(dto_user, confirmation_token);
				}
				else
				{
					throw new RegistrationException(identity_result.Errors);
				}
			}
			catch (RegistrationException RegistrationEx)
			{
				throw RegistrationEx;
			}
			catch (Exception ex)
			{
				throw new RegistrationException(ex);
			}
		}

		public async Task<LoginResult> LocalLogin(string email, string password, bool createCookie)
		{
			try
			{
				var user = await user_manager.FindByEmailAsync(email);
				if (user == null)
					throw new LoginException();

				var result = await signin_manager.CheckPasswordSignInAsync(user, password, false);
				if (result.Succeeded)
				{
					if (createCookie)
					{
						await signin_manager.SignInAsync(user, true);
						return new LoginResult
						{
							Status = true,
							Platform = "local",
							User = ToDto(user)
						};
					}
					else
					{
						return new LoginResult
						{
							Status = true,
							Platform = "local",
							User = ToDto(user),
							Token = CreateToken(user)
						};
					}
				}
				else
				{
					throw new LoginException();
				}
			}
			catch (LoginException loginEx)
			{
				throw loginEx;
			}
			catch (Exception ex)
			{
				throw new LoginException(ex);
			}
		}

		public async Task<IEnumerable<AuthenticationScheme>> GetExternalLoginProviders()
		{
			var providers = await signin_manager.GetExternalAuthenticationSchemesAsync();
			return providers.ToList();
		}

		public Microsoft.AspNetCore.Authentication.AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl)
		{
			var properties = signin_manager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
			return properties;
		}

		public async Task<LoginResult> PerfromExternalLogin()
		{
			var info = await signin_manager.GetExternalLoginInfoAsync();
			if (info == null)
				throw new UnauthorizedAccessException();

			var user = await user_manager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
			if (user == null)
			{
				string username = info.Principal.FindFirstValue(ClaimTypes.Name);
				string email = info.Principal.FindFirstValue(ClaimTypes.Email);

				var new_user = new Entities.User
				{
					UserName = username,
					Email = email,
					PictureUrl = null
				};

				var id_result = await user_manager.CreateAsync(new_user);
				if (id_result.Succeeded)
				{
					user = new_user;
				}
				else
				{
					// User creation failed, probably because the email address is already present in the database
					if (id_result.Errors.Any(e => e.Code == "DuplicateEmail"))
					{
						var existing = await user_manager.FindByEmailAsync(email);
						var existing_logins = await user_manager.GetLoginsAsync(existing);

						if (existing_logins.Any())
						{
							throw new OtherAccountException(existing_logins);
						}
						else
						{
							throw new Exception("Could not create account from social profile");
						}
					}
					else
					{
						throw new Exception("Could not create account from social profile");
					}
				}

				await user_manager.AddLoginAsync(user, new UserLoginInfo(info.LoginProvider, info.ProviderKey, info.ProviderDisplayName));
			}

			await signin_manager.SignInAsync(user, true);
			return new LoginResult
			{
				Status = true,
				Platform = info.LoginProvider,
				User = ToDto(user)
			};
		}

		public async Task<IEnumerable<UserLoginInfo>> GetExternalLogins(ClaimsPrincipal userProperty)
		{
			// Get current user
			var user = await user_manager.GetUserAsync(userProperty);
			if (user == null) throw new UnauthorizedAccessException();

			var user_logins = await user_manager.GetLoginsAsync(user);
			return user_logins;
		}

		public async Task AddExternalLogin(ClaimsPrincipal userProperty)
		{
			// Get current user
			var user = await user_manager.GetUserAsync(userProperty);
			if (user == null) throw new UnauthorizedAccessException();

			// Get login info
			var info = await signin_manager.GetExternalLoginInfoAsync();
			if (info == null) throw new UnauthorizedAccessException();

			var result = await user_manager.AddLoginAsync(user, info);
			if (!result.Succeeded) throw new Exception(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));
		}

		public async Task RemoveExternalLogin(ClaimsPrincipal userProperty, string provider)
		{
			// Get current user
			var user = await user_manager.GetUserAsync(userProperty);
			if (user == null) throw new UnauthorizedAccessException();

			var user_logins = await user_manager.GetLoginsAsync(user);
			var login = user_logins.FirstOrDefault(l => l.LoginProvider == provider);

			if (login == null) throw new InvalidOperationException($"Could not remove {provider} login");

			var result = await user_manager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
			if (!result.Succeeded) throw new Exception($"Could not remove {provider} login");
		}

		public async Task<User> GetCurrentUser(ClaimsPrincipal userProperty)
		{
			var user = await user_manager.GetUserAsync(userProperty);
			return ToDto(user);
		}

		public async Task Logout()
		{
			await signin_manager.SignOutAsync();
		}

		#region Helper methods
		private string CreateToken(Entities.User user)
		{
			var token_descriptor = new SecurityTokenDescriptor
			{
				Issuer = jwtIssuerOptions.Issuer,
				IssuedAt = DateTime.UtcNow,
				Audience = jwtIssuerOptions.Audience,
				NotBefore = DateTime.UtcNow,
				Expires = DateTime.UtcNow.Add(jwtIssuerOptions.ValidFor),
				Subject = new ClaimsIdentity(new[]
				{
					new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
					new Claim(ClaimTypes.Name, user.UserName),
					new Claim(ClaimTypes.Email, user.Email)
				}),
				SigningCredentials = new Func<SigningCredentials>(() => {
					var bytes = Encoding.UTF8.GetBytes(jwtIssuerOptions.Key);
					var signing_key = new SymmetricSecurityKey(bytes);
					var signing_credentials = new SigningCredentials(signing_key, SecurityAlgorithms.HmacSha256Signature);
					return signing_credentials;
				}).Invoke()
			};

			var token_handler = new JwtSecurityTokenHandler();
			var token = token_handler.CreateToken(token_descriptor);
			var str_token = token_handler.WriteToken(token);

			return str_token;
		}
		#endregion
		#region Conversion methods
		internal static Entities.User ToEntity(Dtos.User user)
		{
			if (user == null) return null;
			return new Entities.User
			{
				Id = user.Id,
				Email = user.Email,
				UserName = user.UserName,
				PictureUrl = user.PictureUrl
			};
		}
		internal static User ToDto(Entities.User user)
		{
			if (user == null) return null;
			return new Dtos.User
			{
				Id = user.Id,
				Email = user.Email,
				UserName = user.UserName,
				PictureUrl = user.PictureUrl
			};
		}
		#endregion
	}
}
