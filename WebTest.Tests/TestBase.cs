﻿using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebTest.Models.Auth;
using WebTest.Models.OrgStructure;
using WebTest.Services;
using WebTest.Services.Database;
using WebTest.Tests.Seeders;
using WebTest.Utils;

namespace WebTest.Tests
{
    public abstract class TestBase : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly TestWebApplicationFactory<Program> factory;
        protected IServiceProvider services;
        protected DatabaseContext db;

        public TestBase(TestWebApplicationFactory<Program> factory)
        {
            this.factory = factory;
            services = factory.Services.CreateScope().ServiceProvider;
            db = services.GetService<DatabaseContext>() ?? throw new Exception("DataContext not found");
            ReinitializeDatabase();
        }

        protected T GetService<T>()
        {
            return services.GetService<T>() ?? throw new Exception("Service not found");
        }

        protected HttpClient CreateClient(
            string? authToken = default,
            WebApplicationFactoryClientOptions? options = default)
        {
            options ??= new WebApplicationFactoryClientOptions() { AllowAutoRedirect = false };

            var client = factory.CreateClient(options);
            if (authToken != null)
            {
                var config = GetService<ConfigService>();
#pragma warning disable CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
                client.DefaultRequestHeaders.Add(config?.Get("UserTokenHeaderName", "Authorization"), $"Bearer {authToken}");
#pragma warning restore CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
            }

            return client;
        }

        protected HttpClient CreateClient(User user)
        {
            var token = AuthorizedAs(user);

            return CreateClient(token);
        }

        protected string AuthorizedAs(User user)
        {
            db.Users.Add(user);

            db.SaveChanges();

            var token = new Token()
            {
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                Value = StringUtils.RandomString(64),
            };
            db.Tokens.Add(token);

            db.SaveChanges();

            return token.Value;
        }

        private void ReinitializeDatabase()
        {
            var tableNames = db.Model.GetEntityTypes()
                .Select(t => t.GetTableName())
                .Distinct()
                .ToList();
            foreach (var tableName in tableNames)
            {
                db.Database.ExecuteSqlRaw("DELETE FROM " + tableName);
            }
            BaseSeeder.Seed(db);
            db.SaveChanges();
        }
    }
}