using AutoMapper;
using Streaming.Domain.Models.Core;
using Streaming.Application.DTO.Video;
namespace Streaming.Application.Automapper
{
    public class DtoProfile : Profile
    {
        public DtoProfile() : base()
        {
            CreateMap<Video, VideoMetadataDTO>()
                .ForMember(x => x.CreatedDate, option => option.MapFrom(o => o.FinishedProcessingDate));
        }
    }
}
