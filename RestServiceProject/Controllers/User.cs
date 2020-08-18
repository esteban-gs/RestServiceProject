using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestServiceProject.Data;
using RestServiceProject.Models;
using RestServiceProject.Service;
using RestServiceProject.ViewModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RestServiceProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class User : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;

        public User(ApplicationDBContext context,
            IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        // GET: api/<User>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserViewModel>>> Get()
        {
            var dbUsers = await _context.Users.ToListAsync();
            var usersToReturn = _mapper.Map<IEnumerable<UserViewModel>>(dbUsers);
            return Ok(usersToReturn);
        }

        // GET api/<User>/5
        [HttpGet("{id}", Name = nameof(GetUser))]
        public async Task<ActionResult<UserViewModel>> GetUser(Guid id)
        {
            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == (Guid?)id);
            if (dbUser == null)
            {
                return NotFound();
            }
            var userToReturn = _mapper.Map<UserViewModel>(dbUser);
            return Ok(userToReturn);
        }

        // POST api/<User>
        [HttpPost]
        public async Task<ActionResult<UserViewModel>> Post([FromBody] UserInputModel userInputModel)
        {
            if (userInputModel.Password != userInputModel.PasswordConfirm)
                return BadRequest(new { ErrorDescription = "Password confirmation failed, please try again" });

            var user = _mapper.Map<Models.User>(userInputModel);
            user.Password = PasswordEncryptor.Hash(userInputModel.Password).PasswordHash;
            user.PasswordSalt = PasswordEncryptor.Hash(userInputModel.Password).PasswordSalt;
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();

            var userToReturn = _mapper.Map<UserViewModel>(user);

            return new CreatedAtRouteResult(nameof(GetUser), new { userToReturn.Id }, userToReturn);
        }

        // PUT api/<User>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<UserViewModel>> Put(Guid id, [FromBody] UserInputModel userInputModel)
        {
            if (userInputModel.Password != userInputModel.PasswordConfirm)
                return BadRequest(new { ErrorDescription = "Password confirmation failed, please try again" });

            if (!await _context.Users.AnyAsync(u => u.Id == id))
            {
                return NotFound();
            }

            var user = _mapper.Map<Models.User>(userInputModel);
            user.Id = id; // Needed for EFCore entity tracking
            user.Password = PasswordEncryptor.Hash(userInputModel.Password).PasswordHash;
            user.PasswordSalt = PasswordEncryptor.Hash(userInputModel.Password).PasswordSalt;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/<User>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
