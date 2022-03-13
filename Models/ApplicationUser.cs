using Microsoft.AspNetCore.Identity;

namespace StackOverFlowQA.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Question> Questions { get; set; } = null!;
        public ICollection<Answer> Answers { get; set; } = null!;
        public ICollection<Tag> Tags { get; set; } = null!;
        public ICollection<Comment> Comments { get; set; } = null!;
        public ICollection<Vote> Votes { get; set; } = null!;

       // public Answer CorrectAnswer { get; set; }
        public int CorrectAnswerCount { get; set; } = 0;
        public int? Reputation { get; set; } = 0;

        public string? Role { get; set; }

        public ApplicationUser()
        {
            Questions = new HashSet<Question>();
            Answers = new HashSet<Answer>();
            Tags = new HashSet<Tag>();
            Comments = new HashSet<Comment>();
            Votes = new HashSet<Vote>();
        }
    }
}
