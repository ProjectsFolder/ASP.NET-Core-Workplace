﻿using WebTest.Domains.User.Repositories;
using WebTest.Domains.User.Handlers;

namespace WebTest.Boot.Register
{
    public static class RegisterUserServices
    {
        public static void AddUserServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<GetList>();
        }
    }
}
