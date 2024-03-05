﻿using WebTest.Domains.Auth.Repositories;
using WebTest.Domains.Interfaces;
using WebTest.Dto.Auth.Request;
using WebTest.Dto.Auth.Response;
using WebTest.Exeptions.Concrete;
using WebTest.Http.Responses;
using WebTest.Http.Transformers;
using WebTest.Models.Auth;
using WebTest.Services;
using WebTest.Services.Database;
using WebTest.Utils;

namespace WebTest.Domains.Auth.Handlers
{
    public class Login(
        DatabaseContext context,
        TokenRepository tokenRepository,
        UserRepository userRepository
        ) : IRequestResponseHandler<AuthDto, SuccessDto>
    {
        public SuccessDto Handle(AuthDto dto)
        {
            return context.Transaction(() =>
            {
                return Process(dto);
            });
        }

        private SuccessDto Process(AuthDto dto)
        {
            var user = userRepository.GetUserByLogin(dto.Login) ?? throw new ApiException("User not found", 404);
            if (!AuthService.CheckPassword(user, dto.Password))
            {
                throw new ApiException("Incorrect password", 403);
            }

            tokenRepository.DeleteAllByUser(user.Id);

            var token = new Token()
            {
                CreatedAt = DateTime.UtcNow,
                UserId = user.Id,
                Value = StringUtils.RandomString(64)
            };

            tokenRepository.Save(token);

            var tokenDto = new TokenDto() { Token = token.Value };

            return SuccessResponseTransformer.Build(tokenDto);
        }
    }
}
