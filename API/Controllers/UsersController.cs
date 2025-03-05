using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class UsersController(IUserRepository userRepository, IMapper mapper) : BaseAPIController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await userRepository.GetMembersAsync();

            return Ok(users);
        }

        [HttpGet("{username}")] //api/users/id
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            var user = await userRepository.GetMemberAsync(username);

            if (user == null)
                return NotFound();

            return user;
        }

        [HttpPut]
        [Consumes("application/json")]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (username == null)
            {
                return BadRequest("No username found in token");
            }

            var user = await userRepository.GetUserbyuserNameAsync(username);

            // Debugging logs
            Console.WriteLine(
                $"Incoming Data - City: {memberUpdateDto.City}, Country: {memberUpdateDto.Country}"
            );

            if (user == null)
                return BadRequest("Could not find user");

            mapper.Map(memberUpdateDto, user);

            if (await userRepository.SaveAllAsync())
                return NoContent();

            return BadRequest("Failed with update the user");
        }
    }
}
