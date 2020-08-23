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
            //CreateMap<TSource, TDestination>

            CreateMap<User, UserViewModel>()
                .ForMember(uvm => uvm.Id, o => o.MapFrom(s => s.Id.ToString()))
                .ForMember(uvm => uvm.Password, o => o.MapFrom(GetPasswordHashString))
                .ForMember(uvm => uvm.PasswordSalt, o => o.MapFrom(GetPasswordSaltHashString));

            CreateMap<UserInputModel, User>()
                .IgnoreAllPropertiesWithAnInaccessibleSetter()
                .ForPath(s => s.Id, opt => opt.Ignore())
                .ForMember(u => u.Password, o => o.MapFrom(GetPasswordHash))
                .ForMember(u => u.PasswordSalt, o => o.MapFrom(GetPasswordSalt));
        }

        private string GetPasswordHashString(User user, UserViewModel uvm)
        {
            return Encoding.UTF8.GetString(user.Password);
        }

        private string GetPasswordSaltHashString(User user, UserViewModel uvm)
        {
            return Encoding.UTF8.GetString(user.PasswordSalt);
        }

        private byte[] GetPasswordHash(UserInputModel uim, User u)
        {
            return PasswordEncryptor.Hash(uim.Password).PasswordHash;
        }
        private byte[] GetPasswordSalt(UserInputModel uim, User u)
        {
            return PasswordEncryptor.Hash(uim.Password).PasswordSalt;
        }
    }
}
