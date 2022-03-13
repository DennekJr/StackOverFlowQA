using Microsoft.AspNetCore.Identity;

namespace StackOverFlowQA.Models
{
    
    public class Question
    {
        public int Id { get; set; }

        public string Name { get; set; } 

        public string Description { get; set; }
        public virtual ApplicationUser? User { get; set; }

        public DateTime DOC { get; set; }

        public string? UserId { get; set; }

        public HashTags? HashTags { get; set; }

        public int AnswerCount { get; set; } = 0;
        public int CorrectAnswerCount { get; set; } = 0;
        public virtual ICollection<Answer>? Answers { get; set; }
        public virtual ICollection<Tag>? Tags { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<Vote>? Votes { get; set; }
        public int UpVote { get; set; } = 0;
        public int DownVote { get; set; } = 0;


        public Question()
        {
            Answers = new HashSet<Answer>();
            Tags = new HashSet<Tag>();
            Comments = new HashSet<Comment>();
            Votes = new HashSet<Vote>();
        }

        
    }
    public enum HashTags
    {
        Science,
        Art,
        History,
        Biology,
        Geography
    }
}
