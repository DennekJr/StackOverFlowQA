using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StackOverFlowQA.Data;
using StackOverFlowQA.Models;
using System.Diagnostics;
using System.Linq;

namespace StackOverFlowQA.Controllers
{
    [Authorize]
    public class HomeController : Controller

    {
        private UserManager<ApplicationUser> _userManager;
        public ApplicationDbContext db;

        private readonly ILogger<HomeController> _logger;
        private RoleManager<IdentityRole> _roleManager;

        public HomeController(ApplicationDbContext Db, ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            db = Db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [AllowAnonymous]
        public async Task<ActionResult> Index(string sortOrder)
        {
            ViewBag.role = null;
            if (User.Identity.Name != null)
            {
                string userMail = User.Identity.Name;
                ApplicationUser user = await _userManager.FindByEmailAsync(userMail);
                ViewBag.CurrentUSer = user;
                ViewBag.role = await _userManager.GetRolesAsync(user);
            }

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.MostAnsweredSortParm = sortOrder == "MostA" ? "LeastA" : "";
            // var quest = db.Questions.Include(q => q.Answers).ToList();
            var questions = from s in db.Questions.Include(q => q.Answers).Include(t => t.Tags).Include(v => v.Votes).Include(u => u.User)

                            select s;
            switch (sortOrder)
            {
                case "name_desc":
                    questions = questions.OrderByDescending(s => s.Name);
                    break;
                case "Date":
                    questions = questions.OrderBy(s => s.DOC);
                    break;
                case "date_desc":
                    questions = questions.OrderByDescending(s => s.DOC);
                    break;
                case "MostA":
                    questions = questions.OrderByDescending(s => s.Answers.Count());
                    break;
                case "LeastA":
                    questions = questions.OrderByDescending(s => s.Answers.Count()).Reverse();
                    break;
                default:
                    questions = questions.OrderBy(s => s.Description.Length);
                    break;
            }
            return View(questions.ToList());
        }

        [HttpPost]
        public async Task<ActionResult> Index(int QId)
        {
            var questions = from s in db.Questions.Include(q => q.Answers).Include(t => t.Tags).Include(v => v.Votes).Include(u => u.User)
                            select s;
            if (User.Identity.Name != null)
            {
                string userMail = User.Identity.Name;
                ApplicationUser user = await _userManager.FindByEmailAsync(userMail);
                Question questionToDelete = db.Questions.First(q => q.Id == QId);
                //questionToDelete.User.Questions.Remove(questionToDelete);
                var votes = db.Votes.Include(x => x.Answer).Include(q => q.Question);
                var answers = db.Answers.Include(q => q.QuestionToAnswer);
                foreach (var vote in votes)
                {
                    if(vote.Answer.Id == questionToDelete.Id)
                    {
                        db.Votes.Remove(vote);
                    }
                }
                foreach(var ans in answers)
                {
                    if(ans.QuestionId == questionToDelete.Id)
                    {
                        db.Answers.Remove(ans);

                    }
                }
                db.Questions.Remove(questionToDelete);
                
                ViewBag.CurrentUSer = user;
                ViewBag.role = await _userManager.GetRolesAsync(user);
            }
            // var quest = db.Questions.Include(q => q.Answers).ToList();

            db.SaveChanges();

            return View(questions.ToList());
        }


        [Authorize(Roles="Admin")]
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(string newRoleName)
        {
            await _roleManager.CreateAsync(new IdentityRole(newRoleName));


            string currentUser = User.Identity.Name;
            ApplicationUser user = await _userManager.FindByNameAsync(currentUser);
            if(await _roleManager.RoleExistsAsync(newRoleName))
            {
                if(!await _userManager.IsInRoleAsync(user, newRoleName))
                {
                    await _userManager.AddToRoleAsync(user, newRoleName);
                    user.Role = newRoleName;
                    db.SaveChanges();
                }

            }


            db.SaveChanges();
            return View();
        }
        public IActionResult CreateQuestion()
        {

            return View();
        }

        [HttpPost]
        public IActionResult CreateQuestion(string title,string question, string tag1, string? tag2, string? tag3)
        {
            string UserMail = User.Identity.Name;
            try
            {
                ApplicationUser user = db.Users.First(u => u.Email == UserMail);
                

                if (user != null)
                {
                    Question newQuestion = new Question { Description = question, Name = user.UserName, User = user};
                    Tag QTag1 = new Tag { Name = tag1 };
                    Tag QTag2 = new Tag { Name = tag2 };
                    Tag QTag3 = new Tag { Name = tag3 };
                    newQuestion.Tags.Add(QTag1);
                    newQuestion.Tags.Add(QTag2);
                    newQuestion.Tags.Add(QTag3);
                    QTag1.Questions.Add(newQuestion);
                    QTag2.Questions.Add(newQuestion);
                    QTag3.Questions.Add(newQuestion);
                    user.Questions.Add(newQuestion);
                    db.Questions.Add(newQuestion);
                    db.Tags.Add(QTag1);
                    db.Tags.Add(QTag2);
                    db.Tags.Add(QTag3);
                    db.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
            ViewBag.userMail = UserMail;
            return RedirectToAction("Index");
        }

        public IActionResult CreateAnswer(int Qid)
        {
            string UserMail = User.Identity.Name;
            ApplicationUser user = db.Users.First(u => u.Email == UserMail);
            ViewBag.QuestionToAnswer = db.Questions.Include(u => u.User).First(x => x.Id == Qid);

            return View();
        }

        [HttpPost]

        public IActionResult CreateAnswer(int Qid, string details)
        {
            ViewBag.QuestionToAnswer = db.Questions.First(x => x.Id == Qid);
            Question QuestionToAnswer = db.Questions.First(x => x.Id == Qid);

            string UserMail = User.Identity.Name;
            try
            {
                ApplicationUser user = db.Users.First(u => u.Email == UserMail);
                if (user != null)
                {
                    Answer newAnswer = new Answer { Description = details, User = user, QuestionToAnswer = QuestionToAnswer };
                    QuestionToAnswer.Answers.Add(newAnswer);
                    QuestionToAnswer.AnswerCount++;
                    user.Answers.Add(newAnswer);
                    db.Answers.Add(newAnswer);
                    db.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
            ViewBag.userMail = UserMail;
            return RedirectToAction("Index");
        }

        public IActionResult QuestionsWithTag(int TagId)
        {
            Tag tag = db.Tags.First(x => x.Id == TagId);
            //var Questions = db.Questions.Include(x => x.Tags).Where(x => x.Tags.Contains((Tag)tag));
            List<Question> questionWithTag = new List<Question>();
            var QuestionTags = db.Questions.Include(x => x.Tags);
            foreach(var question in QuestionTags)
            {
                foreach(var qTag in question.Tags)
                {
                    if(qTag.Name == tag.Name)
                    {
                        questionWithTag.Add(question);
                    }
                }
            }

            return View(questionWithTag);
        }

        public IActionResult QuestionsDetail(int qId)
        {
            
            Question questionToDetail = db.Questions.Include(x => x.Tags).Include(x => x.Answers).ThenInclude(v => v.Votes).First(x => x.Id == qId);
            string userMail = User.Identity.Name;
            ApplicationUser user = db.Users.First(u => u.UserName == userMail);
            ViewBag.userId = user.Id;
            //var Questions = db.Questions.Include(x => x.Tags).Where(x => x.Tags.Contains((Tag)tag));
            
            ViewBag.question = questionToDetail;
            return View(questionToDetail);
        }
        [HttpPost]
        public IActionResult QuestionsDetail(int qId, int? CAId)
        {

            Question questionToDetail = db.Questions.Include(x => x.Tags).Include(x => x.Answers).ThenInclude(v => v.Votes).First(x => x.Id == qId);
            string userMail = User.Identity.Name;
            ApplicationUser user = db.Users.First(u => u.UserName == userMail);
            ViewBag.userId = user.Id;
            //var Questions = db.Questions.Include(x => x.Tags).Where(x => x.Tags.Contains((Tag)tag));
            try
            {
                Answer answer = db.Answers.Include(q => q.QuestionToAnswer).First(x => x.Id == CAId);

                if (questionToDetail.CorrectAnswerCount < 1)
                {
                    questionToDetail.CorrectAnswerCount = 1;
                    answer.AnswerIsCorrent = true;
                    db.SaveChanges();
                    return View(questionToDetail);
                }
            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }
            ViewBag.question = questionToDetail;
            return View();
        }
        public IActionResult CommentOnQOrA(int? qId, int? aId)
        {
            string UserMail = User.Identity.Name;
            ApplicationUser user = db.Users.First(u => u.Email == UserMail);

            // List<Journal> journalsForUser = db.Journals.Where(u->u.Name == email).ToList();
            if (qId != null)
            {
                Question question = db.Questions.First(x => x.Id == qId);
                ViewBag.questionFound = question;
            }
            if (aId != null)
            {
                Answer answer = db.Answers.First(x => x.Id == aId);
                ViewBag.answerFound = answer;
            }
            return View();
        }

        [HttpPost]
        public IActionResult CommentOnQOrA(string comment,int? qId, int? aId)
        {
            string UserMail = User.Identity.Name;
            ApplicationUser user = db.Users.First(u => u.Email == UserMail);
            // List<Journal> journalsForUser = db.Journals.Where(u->u.Name == email).ToList();
            if (qId != null)
            {
                Question question = db.Questions.First(x => x.Id == qId);
                Comment newComment = new Comment { Detail = comment, Question = question };
                db.Comments.Add(newComment);
                user.Comments.Add(newComment);
                question.Comments.Add(newComment);
                ViewBag.questionFound = question;
                db.SaveChanges();

            }
            if (aId != null)
            {
                Answer answer = db.Answers.First(x => x.Id == aId);
                Comment newComment = new Comment { Detail = comment, Answer = answer };
                db.Comments.Add(newComment);
                user.Comments.Add(newComment);
                answer.Comments.Add(newComment);
                ViewBag.answerFound = answer;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult VoteQOrA(int? UQId, int? DQId, int? UAId, int? DAId)
        {
            string UserMail = User.Identity.Name;
            ApplicationUser user = db.Users.First(u => u.Email == UserMail);
            // List<Journal> journalsForUser = db.Journals.Where(u->u.Name == email).ToList();
            if (UQId != null || DQId != null)
            {
                Question question = db.Questions.First(x => x.Id == UQId || x.Id == DQId || x.Id == UAId || x.Id == DAId);

                if (UQId.HasValue)
                {
                    Vote newVote = new Vote { Decision = true, User = user, Question = question, QuestionId = question.Id};
                    question.Votes.Add(newVote);
                    db.Votes.Add(newVote);
                    user.Votes.Add(newVote);
                } else
                {
                    Vote newVote = new Vote { Decision = false, User = user, Question = question, QuestionId = question.Id };
                    question.Votes.Add(newVote);
                    db.Votes.Add(newVote);
                    user.Votes.Add(newVote);
                }

                ViewBag.questionFound = question;
            }
            if (UAId != null || DAId != null)
            {

                Answer answer = db.Answers.First(x => x.Id == UAId || x.Id == DAId);
                if (UAId.HasValue)
                {
                    Vote newVote = new Vote { Decision = true, User = user, Answer = answer, AnswerId = answer.Id };
                    answer.Votes.Add(newVote);
                    db.Votes.Add(newVote);
                    user.Votes.Add(newVote);
                } else
                {
                    Vote newVote = new Vote { Decision = false, User = user, Answer = answer, AnswerId = answer.Id };
                    answer.Votes.Add(newVote);
                    db.Votes.Add(newVote);
                    user.Votes.Add(newVote);
                }
                ViewBag.answerFound = answer;
            }

            user.Reputation = 0;

            foreach (var vote in user.Votes)
            {
                if(vote.Decision == true)
                {
                    user.Reputation += 5;
                } else
                {
                    user.Reputation -= 5;
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult MarkAnswer(int aId)
        {
            string UserMail = User.Identity.Name;
            ApplicationUser user = db.Users.Include(x => x.Answers).First(u => u.Email == UserMail);
            try
            {
                Answer answer = db.Answers.Include(q => q.QuestionToAnswer).First(x => x.Id == aId);

                if (answer.QuestionToAnswer.AnswerCount == 0)
                {
                    answer.QuestionToAnswer.AnswerCount = 1;
                    answer.AnswerIsCorrent = true;
                    return RedirectToAction("QuestionsDetail", answer.QuestionToAnswer.Id); //how to pass to question detail page but with new data
                }
                

            } catch( Exception ex)
            {
                return View(ex.Message);
            }

            return View();
        }

       
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}