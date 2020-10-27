using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using DatingAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Microsoft.Extensions.Options;
using DatingAPI.Helpers;
using CloudinaryDotNet;
using DatingAPI.Dtos;
using System.Security.Claims;
using CloudinaryDotNet.Actions;
using DatingAPI.Services.Photo;

namespace DatingAPI.Controllers
{

  [Route("api/photo/{userId}/photos")]
  [ApiController]
  [Authorize]
  public class PhotoController : ControllerBase
  {
    private readonly IUserServices _userServices;
    private readonly IPhotoServices _photoServices;
    private readonly IMapper _mapper;
    private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
    private readonly Cloudinary _cloudinary;

    public PhotoController(IUserServices userServices, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig, IPhotoServices photoServices)
    {
      _userServices = userServices;
      _photoServices = photoServices;
      _mapper = mapper;
      _cloudinaryConfig = cloudinaryConfig;


      Account account = new Account(
          _cloudinaryConfig.Value.CloudName,
          _cloudinaryConfig.Value.ApiKey,
          _cloudinaryConfig.Value.ApiSecret
      );

      _cloudinary = new Cloudinary(account);
    }

    [HttpGet("{photoId}", Name = "GetPhoto")]
    public async Task<IActionResult> GetPhoto(string photoId)
    {
      var photoFromRepo = await _photoServices.GetPhoto(photoId);
      var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

      return Ok(photo);
    }

    [HttpGet("getPhotos")]
    public async Task<IActionResult> GetPhotos(string userId)
    {
      var photo = await _photoServices.GetPhotos(userId);
      return Ok(photo);
    }

    [HttpPost]
    public async Task<IActionResult> AddPhotoForUser(string userId, [FromForm] PhotoForCreationDto photoForCreationDto)
    {
      if (userId != User.FindFirst(ClaimTypes.NameIdentifier).Value)
      {
        return Unauthorized();
      }

      var userFromRepo = await _userServices.GetUser(userId);
      var file = photoForCreationDto.File;
      var uploadResult = new ImageUploadResult();


      if (file.Length > 0)
      {
        using (var stream = file.OpenReadStream())
        {
          var uploadParams = new ImageUploadParams()
          {
            File = new FileDescription(file.Name, stream)
          };

          uploadResult = _cloudinary.Upload(uploadParams);
        }
        photoForCreationDto.Url = uploadResult.Uri.ToString();
        photoForCreationDto.PublicId = uploadResult.PublicId;
      }


      var photo = _mapper.Map<PhotoModel>(photoForCreationDto);
      photo.UserId = userId;

      if (await _photoServices.AddPhoto(photo))
      {
        return Ok("Upload success.");
      }
      return BadRequest("Upload failed.");
    }

//    [HttpPost("{userId}/setMain")]
//    public async Task<IActionResult> SetMainPhoto(string userId, string photoUrl)
//    {
//      if (userId != User.FindFirst(ClaimTypes.NameIdentifier).Value)
//      {
//        return Unauthorized();
//      }

//      var userFromRepo = await _userServices.GetUser(userId);

////      var photoFromRepo = await _photoServices.GetPhoto(photoId);

//      if (photoFromRepo.IsMain)
//      {
//        return BadRequest("Photo already the main photo.");
//      }

//      var currentMainPhoto = await _userServices.GetMainPhotoCurrent(userId);
//      currentMainPhoto.IsMain = false;
//      photoFromRepo.IsMain = true;
//      if (await _userServices.SaveAll())
//      {
//        return NoContent();
//      }
//      return BadRequest("Could set photo to main");
//    }

    //[HttpDelete("{id}")]
    //public async Task<IActionResult> DeletePhoto(int userId, int id)
    //{
    //  if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
    //  {
    //    return Unauthorized();
    //  }

    //  var user = await _userServices.GetUser(userId);
    //  if (!user.Photos.Any(p => p.Id == id))
    //  {
    //    return Unauthorized();
    //  }

    //  var photoFromRepo = await _userServices.GetPhoto(id);
    //  if (photoFromRepo.IsMain)
    //  {
    //    return BadRequest("You cannot delete your main photo.");
    //  }

    //  if (photoFromRepo.PublicId != null)
    //  {
    //    var deleteParams = new DeletionParams(photoFromRepo.PublicId);
    //    var result = _cloudinary.Destroy(deleteParams);

    //    if (result.Result == "ok")
    //    {
    //      _userServices.Delete(photoFromRepo);
    //    }
    //  }
    //  else
    //  {
    //    _userServices.Delete(photoFromRepo);
    //  }

    //  if (await _userServices.SaveAll())
    //  {
    //    return Ok();
    //  }

    //  return BadRequest("Failed to delete to photo");
    //}
  }
}
