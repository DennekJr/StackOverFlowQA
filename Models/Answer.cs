namespace StackOverFlowQA.Models
{
    public class Answer
    {
        public int Id { get; set; }
        
        public int QuestionId { get; set; }
        public bool AnswerIsCorrent { get; set; } = false;

        public Question QuestionToAnswer { get; set; }

        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public virtual ICollection<Comment>? Comments { get; set; }

        public virtual ICollection<Vote>? Votes { get; set; }
        public int UpVote { get; set; } = 0;
        public int DownVote { get; set; } = 0;
        public string Description { get; set; }

        public Answer()
        {
            Comments = new HashSet<Comment>();
            Votes = new HashSet<Vote>();
        }

    }
}
