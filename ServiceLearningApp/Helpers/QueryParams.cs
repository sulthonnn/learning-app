namespace ServiceLearningApp.Helpers
{
    public class QueryParams
    {
        public int Page { get; set; }
        public int PerPage { get; set; }
        public string Sort { get; set; } = "Id";
        public string? Search { get; set; }

        //for sub-chapter
        public int? ChapterId { get; set; }

        //for question
        public int? SubChapterId { get; set; }

        //for option
        public int? QuestionId { get; set; }

    }
}
