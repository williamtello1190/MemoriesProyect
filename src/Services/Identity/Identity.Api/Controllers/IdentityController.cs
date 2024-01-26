﻿using Identity.Services.Queries.DTOs.RequestFilter;
using Identity.Services.Queries.DTOs.StoredProcedure;
using Identity.Services.Queries.StoredProcedure;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Api.Controllers
{
    [ApiController]
    [Route("identity")]
    [EnableCors("AllowOrigin")]
    public class IdentityController : ControllerBase
    {
        private readonly string _keyData;
        private IConfiguration _configuration { get; }
        private readonly IGetUserQueryService _IGetUserQueryService;

        public IdentityController(
          IConfiguration configuration,
          IGetUserQueryService IGetUserQueryService
          )
        {
            _configuration = configuration;
            _keyData = _configuration.GetSection("ConfigDocument").GetSection("keyData").Value;
            _IGetUserQueryService = IGetUserQueryService;
        }

        [HttpPost]
        public async Task<UserDto> Identity(RequestGetUser request)
        {
            UserDto result = await _IGetUserQueryService.GetUser(request.code, request.password);

            if (result != null)
            {
                result.Token = GetToken(request);
            }

            return result;
        }

        private string GetToken(RequestGetUser request)
        {
            // generate token that is valid for 7 days
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_keyData);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("user", request.code), new Claim("code", request.password) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
