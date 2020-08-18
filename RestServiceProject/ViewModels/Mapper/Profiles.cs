using AutoMapper;
using AutoMapper.Mappers;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using RestServiceProject.Models;
using RestServiceProject.Service;
using RestServiceProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RestServiceProject.Mapper
{
    public class Profiles : Profile
    {
        public Profiles()
        {
            CreateMap<User, UserViewModel>()
                .ForMember(uvm => uvm.Id, o => o.MapFrom(s => s.Id.ToString()))
                .ForMember(uvm => uvm.Password, o => o.MapFrom(GetPasswordHashString))
                .ForMember(uvm => uvm.PasswordSalt, o => o.MapFrom(GetPasswordSaltHashString));

            CreateMap<UserInputModel, User>()
                .ForMember(u => u.Password, o => o.Ignore())
                .ForMember(u => u.PasswordSalt, o => o.Ignore());
        }

        private string GetPasswordHashString(User user, UserViewModel uvm)
        {
            return Encoding.UTF8.GetString(user.Password);
        }

        private string GetPasswordSaltHashString(User user, UserViewModel uvm)
        {
            return Encoding.UTF8.GetString(user.PasswordSalt);
        }
    }
}
