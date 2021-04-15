using AutoMapper;
using CityGuide.Api.DTOs;
using CityGuide.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityGuide.Api.Helpers
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<City, CityForListDTO>()
                .ForMember(dest=>dest.PhototUrl,opt=>
                {
                    opt.MapFrom(source => source.Photos.FirstOrDefault(p => p.IsMain).Url);
                });

            CreateMap<City, CityForDetailDTO>();
            CreateMap<Photo, PhotoForCreationDTO>();
            CreateMap<PhotoForReturnDTO, Photo>();
        }
    }
}
