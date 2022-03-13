namespace StackOverFlowQA.Models
{
    public class Vote
    {
        public int Id { get; set; }
        public ApplicationUser User { get; set; }

        public bool Decision { get; set; }

        public Question? Question { get; set; }

        public int? QuestionId { get; set; }
        public int? AnswerId { get; set; }

        public Answer? Answer { get; set; }
    }
}
