using AutoMapper;
using ServiceLearningApp.Model;
using ServiceLearningApp.Model.Dto;

namespace ServiceLearningApp.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() 
        {
            CreateMap<Chapter, Chapter>();

            CreateMap<SubChapter, SubChapterDto>()
                .ForMember(e => e.ChapterTitle, e => e.MapFrom(e => e.Chapter.Title))
                .ForMember(e => e.FileName, e => e.MapFrom(e => e.Upload.Name))
                .ForMember(e => e.FileUrl, e => e.MapFrom(e => e.Upload.Url));
        }
    }
}
