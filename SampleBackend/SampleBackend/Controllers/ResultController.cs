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
    public class ResultController : BaseApiController
    {
        #region Constructor        
        public ResultController(ApplicationDbContext context,
                                RoleManager<IdentityRole> roleManager,
                                UserManager<ApplicationUser> userManager,
                                IConfiguration configuration) :
        base(context, roleManager, userManager, configuration)
        {
        }
        #endregion




        #region RESTful conventions methods
        /// <summary>     
        /// Retrieves the Result with the given {id}
        /// </summary>       
        /// <param name="id">The ID of an existing Result</param>    
        /// <returns>the Result with the given {id}</returns>   
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var result = DbContext.Results.Where(i => i.Id == id).FirstOrDefault();

            // handle requests asking for non-existing results 
            if (result == null)
            {
                return NotFound(new { Error = String.Format("Result ID {0} has not been found", id) });
            }

            return new JsonResult(result.Adapt<ResultViewModel>(), JsonSettings);
        }

        /// <summary>        
        /// Adds a new Result to the Database   
        /// </summary>      
        /// <param name="model">The ResultViewModel containing the data to insert</param> 
        [HttpPut]
        public IActionResult Put([FromBody]ResultViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error) 
            // if the client payload is invalid.   
            if (model == null)
                return new StatusCodeResult(500);

            // map the ViewModel to the Model         
            var result = model.Adapt<Result>();

            // override those properties         
            // that should be set from the server-side only  
            result.CreatedDate = DateTime.Now;
            result.LastModifiedDate = result.CreatedDate;

            // add the new result          
            DbContext.Results.Add(result);

            // persist the changes into the Database.  
            DbContext.SaveChanges();

            // return the newly-created Result to the client.
            return new JsonResult(result.Adapt<ResultViewModel>(), JsonSettings);
        }



        /// <summary>       
        /// Edit the Result with the given {id}
        /// </summary>       
        /// <param name="model">The ResultViewModel containing the data to update</param>
        [HttpPost]
        public IActionResult Post([FromBody]ResultViewModel model)
        {
            // return a generic HTTP Status 500 (Server Error) 
            // if the client payload is invalid.  
            if (model == null)
                return new StatusCodeResult(500);

            // retrieve the result to edit        
            var result = DbContext.Results.Where(q => q.Id == model.Id).FirstOrDefault();

            // handle requests asking for non-existing results   
            if (result == null)
            {
                return NotFound(new { Error = String.Format("Result ID {0} has not been found", model.Id) });
            }

            // handle the update (without object-mapping)  
            // by manually assigning the properties     
            // we want to accept from the request  
            result.QuizId = model.QuizId;
            result.Text = model.Text;
            result.MinValue = model.MinValue;
            result.MaxValue = model.MaxValue;
            result.Notes = model.Notes;

            // properties set from server-side       
            result.LastModifiedDate = result.CreatedDate;

            // persist the changes into the Database.  
            DbContext.SaveChanges();

            // return the updated Quiz to the client.    
            return new JsonResult(result.Adapt<ResultViewModel>(), JsonSettings);
        }


        /// <summary>       
        /// Deletes the Result with the given {id} from the Database  
        /// </summary>        
        /// <param name="id">The ID of an existing Result</param> 
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // retrieve the result from the Database 
            var result = DbContext.Results.Where(i => i.Id == id).FirstOrDefault();

            // handle requests asking for non-existing results 
            if (result == null)
            {
                return NotFound(new { Error = String.Format("Result ID {0} has not been found", id) });
            }

            // remove the quiz from the DbContext.    
            DbContext.Results.Remove(result);

            // persist the changes into the Database.  
            DbContext.SaveChanges();

            // return an HTTP Status 200 (OK).     
            return new OkResult();
        }
        #endregion


        // GET api/result/all      
        [HttpGet("All/{quizId}")]
        public IActionResult All(int quizId)
        {
            var results = DbContext.Results.Where(q => q.QuizId == quizId).ToArray();
            return new JsonResult(results.Adapt<ResultViewModel[]>(), JsonSettings);
        }
                     









        // GET api/question/all     
        //[HttpGet("All/{quizId}")]
        //public IActionResult All(int quizId)
        //{
        //    var sampleResults = new List<ResultViewModel>();

        //    // add a first sample result    
        //    sampleResults.Add(new ResultViewModel()
        //    {
        //        Id = 1,
        //        QuizId = quizId,
        //        Text = "What do you value most in your life?",
        //        CreatedDate = DateTime.Now,
        //        LastModifiedDate = DateTime.Now
        //    });

        //    // add a bunch of other sample results  
        //    for (int i = 2; i <= 5; i++)
        //    {
        //        sampleResults.Add(new ResultViewModel()
        //        {
        //            Id = i,
        //            QuizId = quizId,
        //            Text = String.Format("Sample Question {0}", i),
        //            CreatedDate = DateTime.Now,
        //            LastModifiedDate = DateTime.Now
        //        });
        //    }

        //    // output the result in JSON format    
        //    return new JsonResult(sampleResults, JsonSettings);
        //}
    }
}