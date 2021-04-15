using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CityGuide.Api.Data;
using CityGuide.Api.DTOs;
using CityGuide.Api.Helpers;
using CityGuide.Api.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CityGuide.Api.Controllers
{
    [Route("api/cities/{cityId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        IAppRepository _appRepository;
        IMapper _mapper;
        IOptions<CloudinarySettings> _cloudinaryConfig;
        Cloudinary _cloudinary;


        public PhotosController(IAppRepository appRepository, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _appRepository = appRepository;
            _mapper = mapper;
            _cloudinaryConfig = cloudinaryConfig;
            Account account = new Account(_cloudinaryConfig.Value.CloudName, _cloudinaryConfig.Value.ApiKey, _cloudinaryConfig.Value.ApiSecret);
            _cloudinary = new Cloudinary(account);
        }



        [HttpPost]
        public IActionResult AddPhotoForCity(int cityId, [FromBody] PhotoForCreationDTO photoForCreationDTO)
        {
            var city = _appRepository.GetCityById(cityId);
            if (city==null)
            {
                return BadRequest("could not find CiTy");
            }
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (currentUserId!=city.UserId)
            {
                return Unauthorized();
            }
            var file = photoForCreationDTO.File;
            var uploadResult = new ImageUploadResult();
            if (file.Length>0)
            {
                using (var stream=file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.Name, stream)
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }
            photoForCreationDTO.Url = uploadResult.Uri.ToString();
            photoForCreationDTO.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreationDTO);
            photo.City = city;
            if (!city.Photos.Any(p=>p.IsMain))
            {
                photo.IsMain = true;
            }
            city.Photos.Add(photo);

            if (_appRepository.SaveAll())
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDTO>(photo);
                return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoToReturn);
            }
            return BadRequest("Could not add PhOtO");
        }



        [HttpGet("{id}",Name ="GetPhoto")]
        public IActionResult GetPhotos(int id)
        {
            var photoFromDb = _appRepository.GetPhoto(id);
            var photo = _mapper.Map<PhotoForReturnDTO>(photoFromDb);
            return Ok(photo);
        }
    }
}
