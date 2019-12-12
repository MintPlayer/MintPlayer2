using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MintPlayer.Data.Helpers;
using MintPlayer.Data.Options;
using MintPlayer.Data.Repositories;
using MintPlayer.Data.Repositories.Interfaces;

namespace MintPlayer.Data.Extensions
{
	public static class MintPlayerExtensions
	{
		public static IServiceCollection AddMintPlayer(this IServiceCollection services, Action<MintPlayerOptions> options)
		{
			var opt = new MintPlayerOptions();
			options.Invoke(opt);

			services.AddDbContext<MintPlayerContext>(db_options =>
			{
				db_options.UseSqlServer(opt.ConnectionString, b => b.MigrationsAssembly("MintPlayer.Data"));
			});

			services.AddIdentity<Entities.User, Entities.Role>()
				.AddEntityFrameworkStores<MintPlayerContext>()
				.AddDefaultTokenProviders();

			services
				.AddAuthentication()
				.AddJwtBearer(jwt_options =>
				{
					jwt_options.Audience = opt.JwtIssuerOptions.Audience;
					jwt_options.ClaimsIssuer = opt.JwtIssuerOptions.Issuer;
					jwt_options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateAudience = true,
						ValidAudience = opt.JwtIssuerOptions.Audience,
						ValidateIssuer = true,
						ValidIssuer = opt.JwtIssuerOptions.Issuer,
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new Func<SymmetricSecurityKey>(() =>
						{
							var key = opt.JwtIssuerOptions.Key;
							var bytes = System.Text.Encoding.UTF8.GetBytes(key);
							var signing_key = new SymmetricSecurityKey(bytes);
							return signing_key;
						}).Invoke(),
						ValidateLifetime = true
					};
					jwt_options.SaveToken = false;
				});
				//.AddFacebook(fb_options => {
				//	fb_options.AppId = opt.FacebookOptions.AppId;
				//	fb_options.AppSecret = opt.FacebookOptions.AppSecret;
				//})
				//.AddMicrosoftAccount(ms_options => {
				//	ms_options.ClientId = opt.MicrosoftOptions.ClientId;
				//	ms_options.ClientSecret = opt.MicrosoftOptions.ClientSecret;
				//})
				//.AddGoogle(g_options => {
				//	g_options.ClientId = opt.GoogleOptions.ClientId;
				//	g_options.ClientSecret = opt.GoogleOptions.ClientSecret;
				//})
				//.AddTwitter(tw_options => {
				//	tw_options.ConsumerKey = opt.TwitterOptions.ConsumerKey;
				//	tw_options.ConsumerSecret = opt.TwitterOptions.ConsumerSecret;
				//	tw_options.RetrieveUserDetails = true;
				//});

			services.AddDataProtection();

			services
				.AddScoped<IAccountRepository, AccountRepository>()
				.AddScoped<IRoleRepository, RoleRepository>()
				.AddScoped<IPersonRepository, PersonRepository>()
				.AddScoped<IArtistRepository, ArtistRepository>()
				.AddScoped<ISongRepository, SongRepository>()
				.AddScoped<ISubjectRepository, SubjectRepository>()
				.AddScoped<IMediumTypeRepository, MediumTypeRepository>()
				.AddScoped<IMediumRepository, MediumRepository>()
				.AddScoped<Repositories.Jobs.Interfaces.IElasticSearchJobRepository, Repositories.Jobs.ElasticSearchJobRepository>()
				.AddTransient<ArtistHelper>()
				.AddTransient<SongHelper>();

			services
				.Configure<JwtIssuerOptions>(jwt_options =>
				{
					jwt_options.Audience = opt.JwtIssuerOptions.Audience;
					jwt_options.Issuer = opt.JwtIssuerOptions.Issuer;
					jwt_options.Key = opt.JwtIssuerOptions.Key;
					jwt_options.Subject = opt.JwtIssuerOptions.Subject;
					jwt_options.ValidFor = opt.JwtIssuerOptions.ValidFor;
				});

			return services;
		}
	}
}
