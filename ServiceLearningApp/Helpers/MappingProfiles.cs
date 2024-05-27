using AutoMapper;
using ServiceLearningApp.Model;

namespace ServiceLearningApp.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() 
        {
            CreateMap<Chapter, Chapter>();
        }
    }
}
