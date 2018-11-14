using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SampleBackend.Data;
using SampleBackend.Models;
using SampleBackend.ViewModels;

namespace SampleBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : BaseApiController
    {
        /// <summary>
        /// Quiz Controller
        /// </summary>
        /// <param name="context">ApplicationDbContext</param>
        /// <param name="roleManager">RoleManager<IdentityRole></param>
        /// <param name="userManager">UserManager<ApplicationUser></param>
        /// <param name="configuration">IConfiguration</param>


        #region Constructor        
        public QuizController(ApplicationDbContext context,
                              RoleManager<IdentityRole> roleManager,
                              UserManager<ApplicationUser> userManager,
                              IConfiguration configuration) :
        base(context, roleManager, userManager, configuration)
        {
        }
        #endregion


        #region RESTful conventions methods 

        [HttpGet]
        public IActionResult Get()
        {
            var q = DbContext.Quizzes;


            return new JsonResult(q.Adapt<QuizViewModel[]>(), JsonSettings);
        }




        /// <summary> 
        /// GET: api/quiz/{id} 
        /// Retrieves the Quiz with the given {id} 
        /// </summary> 
        /// <param name="id">The ID of an existing Quiz</param> 
        /// <returns>the Quiz with the given {id}</returns> 
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var quiz = DbContext.Quizzes.Where(i => i.Id == id).FirstOrDefault();

            // handle requests asking for non-existing quizzes 
            if (quiz == null)
            {
                return NotFound(new { Error = String.Format("Quiz ID {0} has not been found", id) });
            }

            return new JsonResult(quiz.Adapt<QuizViewModel>(), JsonSettings);
        }

        /// <summary> 
        /// Adds a new Quiz to the Database 
        /// </summary> 
        /// <param name="model">The QuizViewModel containing the data to insert</param>
        [HttpPut]
        public IActionResult Put([FromBody]QuizViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error) 
            // if the client payload is invalid.    
            if (model == null)
                return new StatusCodeResult(500);

            // handle the insert (without object-mapping)   
            var quiz = new Quiz();
            // properties taken from the request 
            quiz.Title = model.Title;
            quiz.Description = model.Description;
            quiz.Text = model.Text;
            quiz.Notes = model.Notes;

            // properties set from server-side   
            quiz.CreatedDate = DateTime.Now;
            quiz.LastModifiedDate = quiz.CreatedDate;

            // Set a temporary author using the Admin user's userId 
            // as user login isn't supported yet: we'll change this later on.   
            quiz.UserId = DbContext.Users.Where(u => u.UserName == "Admin").FirstOrDefault().Id;

            // add the new quiz 
            DbContext.Quizzes.Add(quiz);

            // persist the changes into the Database.  
            DbContext.SaveChanges();

            // return the newly-created Quiz to the client. 
            return new JsonResult(quiz.Adapt<QuizViewModel>(), JsonSettings);
        }

        /// <summary> 
        /// Edit the Quiz with the given {id} 
        /// </summary> 
        /// <param name="model">The QuizViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody]QuizViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error)    
            // if the client payload is invalid.   
            if (model == null)
                return new StatusCodeResult(500);

            // retrieve the quiz to edit
            var quiz = DbContext.Quizzes.Where(q => q.Id == model.Id).FirstOrDefault();

            // handle requests asking for non-existing quizzes 
            if (quiz == null)
            {
                return NotFound(new { Error = String.Format("Quiz ID {0} has not been found", model.Id) });
            }

            // handle the update (without object-mapping)   
            //   by manually assigning the properties  
            //   we want to accept from the request 
            quiz.Title = model.Title;
            quiz.Description = model.Description;
            quiz.Text = model.Text;
            quiz.Notes = model.Notes;

            // properties set from server-side   
            quiz.LastModifiedDate = quiz.CreatedDate;

            // persist the changes into the Database.  
            DbContext.SaveChanges();

            // return the updated Quiz to the client.
            return new JsonResult(quiz.Adapt<QuizViewModel>(), JsonSettings);
        }

        /// <summary> 
        /// Deletes the Quiz with the given {id} from the Database 
        /// </summary> 
        /// <param name="id">The ID of an existing Test</param> 
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the quiz from the Database 
            var quiz = DbContext.Quizzes.Where(i => i.Id == id).FirstOrDefault();

            // handle requests asking for non-existing quizzes
            if (quiz == null)
            {
                return NotFound(new { Error = String.Format("Quiz ID {0} has not been found", id) });
            }

            // remove the quiz from the DbContext. 
            DbContext.Quizzes.Remove(quiz);

            // persist the changes into the Database. 
            DbContext.SaveChanges();

            // return an HTTP Status 200 (OK). 
            return new OkResult();
        }
        #endregion







        // GET api/quiz/latest     
        //[HttpGet("Latest/{num}")]
        //public IActionResult Latest(int num = 10)
        //{
        //    var sampleQuizzes = new List<QuizViewModel>();

        //    // add a first sample quiz  
        //    sampleQuizzes.Add(new QuizViewModel()
        //    {
        //        Id = 1,
        //        Title = "Which Shingeki No Kyojin character are you?",
        //        Description = "Anime-related personality test",
        //        CreatedDate = DateTime.Now,
        //        LastModifiedDate = DateTime.Now
        //    });

        //    // add a bunch of other sample quizzes   
        //    for (int i = 2; i <= num; i++)
        //    {
        //        sampleQuizzes.Add(new QuizViewModel()
        //        {
        //            Id = i,
        //            Title = String.Format("Sample Quiz {0}", i),
        //            Description = "This is a sample quiz",
        //            CreatedDate = DateTime.Now,
        //            LastModifiedDate = DateTime.Now
        //        });

        //    }

        //    // output the result in JSON format  
        //    return new JsonResult(sampleQuizzes, new JsonSerializerSettings()
        //    {
        //        Formatting = Formatting.Indented
        //    });
        //}




        /// <summary> 
        /// GET: api/quiz/ByTitle 
        /// Retrieves the {num} Quizzes sorted by Title (A to Z) 
        /// </summary>
        /// <param name="num">the number of quizzes to retrieve</param> 
        /// <returns>{num} Quizzes sorted by Title</returns> 
        //[HttpGet("ByTitle/{num:int?}")]
        //public IActionResult ByTitle(int num = 10)
        //{
        //    var sampleQuizzes = ((JsonResult)Latest(num)).Value as List<QuizViewModel>;

        //    return new JsonResult(sampleQuizzes.OrderBy(t => t.Title), new JsonSerializerSettings() { Formatting = Formatting.Indented });
        //}





    }
}