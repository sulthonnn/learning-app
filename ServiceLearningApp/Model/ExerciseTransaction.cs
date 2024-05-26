using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceLearningApp.Model
{
    public class ExerciseTransaction
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Score { get; set; }


        [ForeignKey(nameof(FkSubChapterId))]
        public SubChapter? SubChapter { get; set; }
        public int FkSubChapterId { get; set; }

        [ForeignKey(nameof(FkUserId))]
        public ApplicationUser? User { get; set; }
        public string FkUserId { get; set; }
    }
}
