namespace StackOverFlowQA.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public ApplicationUser User { get; set; }

        public string Detail { get; set; }

        public Question? Question { get; set; }

        public Answer? Answer { get; set; }
    }
}
