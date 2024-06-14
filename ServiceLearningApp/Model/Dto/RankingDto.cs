namespace ServiceLearningApp.Model.Dto
{
    public class RankingDto
    {
        public int TotalScore { get; set; }
        public decimal AverageTime { get; set; }
        public int ChapterId { get; set; }
        public required string UserFullName { get; set; }
        public List<ExerciseTransactionDto> ExerciseTransactions { get; set; }
    }
}
