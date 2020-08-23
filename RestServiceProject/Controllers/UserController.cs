using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestServiceProject.Data;
using RestServiceProject.Helpers;
using RestServiceProject.Models;
using RestServiceProject.Service;
using RestServiceProject.ViewModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RestServiceProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;

        public UserController(ApplicationDBContext context,
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
            var userSearch = await FindUser(id);

            if (!userSearch.Exists) return NotFound();

            var userToReturn = _mapper.Map<UserViewModel>(userSearch.User);
            return Ok(userToReturn);
        }

        // POST api/<User>
        [HttpPost]
        public async Task<ActionResult<UserViewModel>> Post([FromBody] UserInputModel userInputModel)
        {
            // if passwords don't match
            if (userInputModel.Password != userInputModel.PasswordConfirm)
                return BadRequest(new { ErrorDescription = AppConstants.PasswordMatchFailMessage });

            // automapper profile hashes password/salt in Profile.cs
            var user = _mapper.Map<User>(userInputModel);

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
                return BadRequest(new { ErrorDescription = AppConstants.PasswordMatchFailMessage });

            var userSearch = await FindUser(id);
            if (!userSearch.Exists) return NotFound();

            // automapper profile hashes password/salt in Profile.cs
            var user = _mapper.Map<UserInputModel, User>(userInputModel, userSearch.User);
            user.Id = id; // Needed for EFCore entity 

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/<User>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var userSearch = await FindUser(id);
            if (!userSearch.Exists) return NotFound();

            _context.Users.Remove(userSearch.User);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [NonAction]
        private async Task<FindUserResults> FindUser(Guid userId)
        {
            var dbUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == (Guid?)userId);

            var resultsObject = new FindUserResults();
            resultsObject.Exists = dbUser == null ? false : true;
            resultsObject.User = resultsObject.Exists ? dbUser : null;

            return resultsObject;
        }

        private struct FindUserResults
        {
            public Models.User User { get; set; }
            public bool Exists { get; set; }
        }
    }
}
