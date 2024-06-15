namespace ServiceLearningApp.Model.Dto
{
    public class AllRankingDto
    {
        public int TotalScore { get; set; }
        public decimal AverageTime { get; set; }
        public required string UserFullName { get; set; }
        public List<ExerciseTransactionDto> ExerciseTransactions { get; set; }
    }
}
