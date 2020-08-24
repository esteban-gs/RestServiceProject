using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using RestServiceProject.Models;
using RestServiceProject.Service;
using System;
using System.Collections.Generic;

namespace RestServiceProject.Data
{
    public static class DataGenerator
    {
        public static void Initialize(ApplicationDBContext context)
        {
            // Look for any users
            if (context.Users.AnyAsync().Result)
            {
                return;   // Data was already seeded
            }

            List<User> users = GenerateUsers();

            context.Users.AddRange(users);

            context.SaveChanges();

        }

        public static List<User> GenerateUsers()
        {
            // a list of users to seed
            List<User> users = new List<User>();


            //!! Can't go over 9, or guid.parse won't work
            for (var i = 0; i < 9; i++)
            {
                int index = i + 1;

                var tempGuid = Guid.Parse($"{index}a8d0bfb-74a5-48f4-a729-0a945011ee4f");
                users.Add(
                    new User
                    {
                        //Id = new Guid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, (byte)index, }),
                        Id = tempGuid,
                        Email = $"test{index}@test.com",
                        Password = PasswordEncryptor.Hash($"Secret{index}").PasswordHash,
                        PasswordSalt = PasswordEncryptor.Hash($"Secret{index}").PasswordSalt,
                        CreatedDate = DateTime.UtcNow.AddDays(-index)
                    });
            }

            return users;
        }
    }
}
