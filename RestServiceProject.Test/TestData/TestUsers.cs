using AutoMapper;
using RestServiceProject.Data;
using RestServiceProject.Mapper;
using RestServiceProject.Models;
using RestServiceProject.Service;
using RestServiceProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestServiceProject.Test.TestData
{
    public static class TestUsers
    {
        public const string TestPassWord = "Secret1234%";
        public const string PostEmail = "test10@test.com";
        public static Guid ExistingGuid = new Guid("1a8d0bfb-74a5-48f4-a729-0a945011ee4f");
    }
}
