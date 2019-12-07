using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MintPlayer.Data.Dtos;
using MintPlayer.Data.Exceptions.Account;
using MintPlayer.Data.Exceptions.Account.ExternalLogin;
using MintPlayer.Data.Repositories.Interfaces;
using MintPlayer.Web.ViewModels.Account;

namespace MintPlayer.Web.Controllers.Web
{
	[Route("web/[controller]")]
	public class AccountController : Controller
	{
		private IAccountRepository accountRepository;
		public AccountController(IAccountRepository accountRepository)
		{
			this.accountRepository = accountRepository;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody]UserDataVM userCreateVM)
		{
			try
			{
				var registration_data = await accountRepository.Register(userCreateVM.User, userCreateVM.Password);
				var user = registration_data.Item1;
				var confirmation_token = registration_data.Item2;
				return Ok();
			}
			catch (RegistrationException registrationEx)
			{
				return BadRequest(registrationEx.Errors.Select(e => e.Description));
			}
			catch (Exception ex)
			{
				return StatusCode(500);
			}
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody]LoginVM loginVM)
		{
			try
			{
				var login_result = await accountRepository.LocalLogin(loginVM.Email, loginVM.Password, true);
				return Ok(login_result);
			}
			catch (LoginException loginEx)
			{
				return Unauthorized();
			}
			catch (Exception ex)
			{
				return StatusCode(500);
			}
		}

		[AllowAnonymous]
		[HttpGet("providers")]
		public async Task<List<string>> Providers()
		{
			var result = await accountRepository.GetExternalLoginProviders();
			return result.Select(s => s.DisplayName).ToList();
		}

		[AllowAnonymous]
		[HttpGet("connect/{provider}")]
		public async Task<ActionResult> ExternalLogin(string provider)
		{
			var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { provider });
			var properties = accountRepository.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
			return Challenge(properties, provider);
		}

		[HttpGet("connect/{provider}/callback")]
		public async Task<ActionResult> ExternalLoginCallback([FromRoute]string provider)
		{
			var model = new LoginResultVM();
			try
			{
				var login_result = await accountRepository.PerfromExternalLogin();
				if (login_result.Status)
				{
					model.Status = true;
					model.Platform = login_result.Platform;

					return View(model);
				}
				else
				{
					model.Status = false;
					model.Platform = login_result.Platform;

					model.Error = login_result.Error;
					model.ErrorDescription = login_result.ErrorDescription;

					return View(model);
				}
			}
			catch (OtherAccountException otherAccountEx)
			{
				model.Status = false;
				model.Platform = provider;

				model.Error = "Could not login";
				model.ErrorDescription = otherAccountEx.Message;

				return View(model);
			}
			catch (Exception ex)
			{
				model.Status = false;
				model.Platform = provider;

				model.Error = "Could not login";
				model.ErrorDescription = "There was an error with your social login";

				return View(model);
			}
		}

		[Authorize]
		[HttpGet("logins")]
		public async Task<List<string>> GetExternalLogins()
		{
			var logins = await accountRepository.GetExternalLogins(User);
			return logins.Select(l => l.ProviderDisplayName).ToList();
		}

		[Authorize]
		[HttpGet("add/{provider}")]
		public async Task<ActionResult> AddExternalLogin(string provider)
		{
			var redirectUrl = Url.Action(nameof(AddExternalLoginCallback), "Account", new { provider });
			var properties = accountRepository.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
			return Challenge(properties, provider);
		}

		[Authorize]
		[HttpGet("add/{provider}/callback")]
		public async Task<ActionResult> AddExternalLoginCallback([FromRoute]string provider)
		{
			var model = new LoginResultVM();
			try
			{
				await accountRepository.AddExternalLogin(User);

				model.Status = true;
				model.Platform = provider;

				return View(model);
			}
			catch (Exception)
			{
				model.Status = false;
				model.Platform = provider;

				model.Error = "Could not login";
				model.ErrorDescription = "There was an error with your social login";

				return View(model);
			}
		}

		[Authorize]
		[HttpDelete("logins/{provider}")]
		public async Task<IActionResult> DeleteLogin(string provider)
		{
			await accountRepository.RemoveExternalLogin(User, provider);
			return Ok();
		}

		[Authorize]
		[HttpGet("current-user")]
		public async Task<User> GetCurrentUser()
		{
			var user = await accountRepository.GetCurrentUser(User);
			return user;
		}

		[Authorize]
		[HttpPost("logout")]
		public async Task<IActionResult> Logoout()
		{
			await accountRepository.Logout();
			return Ok();
		}
	}
}