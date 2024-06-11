using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceLearningApp.Model
{
    public class HistoryAnswer
    {
        public int Id { get; set; }

        [ForeignKey(nameof(FkQuestionId))]
        public Question? Question { get; set; }
        public int FkQuestionId { get; set; }

        [ForeignKey(nameof(FkOptionId))]
        public Option? Option { get; set; }
        public int FkOptionId { get; set; }

        [ForeignKey(nameof(FkExerciseTransactionId))]
        public ExerciseTransaction? ExerciseTransaction { get; set; }
        public int FkExerciseTransactionId { get; set; }

    }
}
