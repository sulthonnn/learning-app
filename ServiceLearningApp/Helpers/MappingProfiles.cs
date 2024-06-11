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
            CreateMap<SubChapter, SubChapterDto>();
            CreateMap<Question, QuestionDto>();
            CreateMap<Option, OptionDto>();
            CreateMap<ExerciseTransaction, ExerciseTransactionDto>()
                .ForMember(dest => dest.HistoryAnswer, opt => opt.MapFrom(src => src.HistoryAnswer));
            ;
        }
    }
}
