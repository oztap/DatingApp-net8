using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class UsersController(
        IUserRepository userRepository,
        IMapper mapper,
        IPhotoService photoService
    ) : BaseAPIController
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
            var user = await userRepository.GetUserbyuserNameAsync(User.GetUsername());

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

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await userRepository.GetUserbyuserNameAsync(User.GetUsername());
            if (user == null)
                return BadRequest("Cannot update user");

            var result = await photoService.AddPhotoAsync(file);

            if (result.Error != null)
                return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
            };

            if (user.Photos.Count == 0) photo.isMain = true;

            user.Photos.Add(photo);

            if (await userRepository.SaveAllAsync())
                return CreatedAtAction(
                    nameof(GetUser),
                    new { username = user.UserName },
                    mapper.Map<PhotoDto>(photo)
                );
            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId:int}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await userRepository.GetUserbyuserNameAsync(User.GetUsername());
            if (user == null)
                return BadRequest("Could not find user");
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null || photo.isMain)
                return BadRequest("Cannot use this as main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.isMain);

            if (currentMain != null)
                currentMain.isMain = false;
            photo.isMain = true;

            if (await userRepository.SaveAllAsync())
                return NoContent();

            return BadRequest("Problem setting main photo");
        }

        [HttpDelete("delete-photo/{photoId:int}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await userRepository.GetUserbyuserNameAsync(User.GetUsername());

            if (user == null)
                return BadRequest("User not found");

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null || photo.isMain)
                return BadRequest("This photo cannot be deleted");

            if (photo.PublicId != null)
            {
                var result = await photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null)
                    return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);

            if (await userRepository.SaveAllAsync())
                return Ok();
            return BadRequest("Problem deleting photo");
        }
    }
}
