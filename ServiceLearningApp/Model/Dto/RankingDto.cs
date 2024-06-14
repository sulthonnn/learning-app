namespace ServiceLearningApp.Model.Dto
{
    public class RankingDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Score { get; set; }
        public required string SubChapter { get; set; }
        public required string UserFullName { get; set; }
    }
}
