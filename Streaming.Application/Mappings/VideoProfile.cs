using AutoMapper;
using Streaming.Domain.Models.Core;
using Streaming.Domain.Models.DTO;
using Streaming.Domain.Models.DTO.Video;
using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Application.Mappings
{
    public class VideoProfile : Profile
    {
        public VideoProfile()
        {
            CreateMap<VideoUploadDTO, Video>()
                .ForMember(x => x.CreatedDate, opt => opt.MapFrom(o => DateTime.Now))
                .ForMember(x => x.Description, opt => opt.MapFrom(o => o.Description))
                .ForMember(x => x.Title, opt => opt.MapFrom(o => o.Title))
                .ForMember(x => x.VideoId, opt => opt.MapFrom(o => Guid.NewGuid()))
                .ForMember(x => x.VideoOriginalName, opt => opt.MapFrom(o => o.File.FileName));
        }
    }
}
