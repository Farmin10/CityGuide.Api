using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CityGuide.Api.Data;
using CityGuide.Api.DTOs;
using CityGuide.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CityGuide.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private IAppRepository _appRepository;
        private IMapper _mapper;

        public CitiesController(IAppRepository appRepository, IMapper mapper)
        {
            _appRepository = appRepository;
            _mapper = mapper;
        }


        public IActionResult GetCities()
        {
            var cities = _appRepository.GetCities();
            var citiesToReturn = _mapper.Map<List<CityForListDTO>>(cities);
            return Ok(citiesToReturn);
        }



        [HttpGet]
        [Route("detail")]
        public IActionResult GetCityById(int id)
        {
            var city = _appRepository.GetCityById(id);
            var cityToReturn = _mapper.Map<CityForDetailDTO>(city);
            return Ok(cityToReturn);
        }




        [HttpGet]
        [Route("Photos")]
        public IActionResult GetPhotosByCity(int cityId)
        {
            var photos = _appRepository.GetPhotoByCity(cityId);
            return Ok(photos);
        }




        [HttpPost]
        [Route("add")]
        public IActionResult Add([FromBody]City city)
        {
            _appRepository.Add(city);
            _appRepository.SaveAll();
            return Ok();
        }
    }
}
