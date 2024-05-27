namespace ServiceLearningApp.Helpers
{
    public class QueryParams
    {
        public int Page { get; set; }
        public int PerPage { get; set; }
        public string Sort { get; set; } = "Id";
        public string? Search { get; set; }
    }
}
