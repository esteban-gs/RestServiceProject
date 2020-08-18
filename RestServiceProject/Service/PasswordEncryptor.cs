using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore.Update;
using RestServiceProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace RestServiceProject.Service
{
    public static class PasswordEncryptor
    {
        public struct PassworEncryptorResult
        {
            public byte[] PasswordHash { get;  set; }
            public byte[] PasswordSalt { get; set; }
        }

        public static PassworEncryptorResult Hash(string password)
        {
            PassworEncryptorResult results = new PassworEncryptorResult();

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            results.PasswordHash = passwordHash;
            results.PasswordSalt = passwordSalt;

            return results;
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }

}
