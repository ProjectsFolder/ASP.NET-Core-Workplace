﻿namespace WebTest.Dto.Auth.Command
{
    public class AuthCommand(string login, string password) : CommandBase
    {
        public string Login { get; set; } = login;

        public string Password { get; set; } = password;
    }
}