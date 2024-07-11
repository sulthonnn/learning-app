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

            CreateMap<Question, RandomQuestionDto>()
                .ForMember(dest => dest.FkImageId, opt => opt.MapFrom(src => src.FkImageId))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Image.Url))
                .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(src => src.Image.ThumbnailUrl))
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options));

            CreateMap<Option, OptionDto>();

            CreateMap<ExerciseTransaction, ExerciseTransactionListDto>()
                .ForMember(dest => dest.SubChapter, opt => opt.MapFrom(src => src.SubChapter.Title))
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User.FullName));

            CreateMap<ExerciseTransaction, ExerciseTransactionDto>()
                .ForMember(dest => dest.HistoryAnswer, opt => opt.MapFrom(src => src.HistoryAnswer))
                .ForMember(dest => dest.SubChapter, opt => opt.MapFrom(src => src.SubChapter.Title))
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User.FullName));

            CreateMap<HistoryAnswer, HistoryAnswerDto>()
            .ForMember(dest => dest.Question, opt => opt.MapFrom(src => src.Question.QuestionText))
            .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Question.Image.Url))
            .ForMember(dest => dest.ThumbnailUrl, opt => opt.MapFrom(src => src.Question.Image.ThumbnailUrl))
            .ForMember(dest => dest.Option, opt => opt.MapFrom(src => src.Option.OptionText))
            .ForMember(dest => dest.IsAnswer, opt => opt.MapFrom(src => src.Option.IsAnswer));

        }
    }
}
