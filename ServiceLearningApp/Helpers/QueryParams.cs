namespace ServiceLearningApp.Helpers
{
    public class QueryParams
    {
        public int Page { get; set; } = 1;
        public int PerPage { get; set; } = 10;
        public string Sort { get; set; } = "Id";
        public string? Search { get; set; }
    }
}
