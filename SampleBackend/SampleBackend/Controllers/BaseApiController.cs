using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SampleBackend.Data;
using SampleBackend.Models;

namespace SampleBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        #region Constructor        
        public BaseApiController(ApplicationDbContext context,
                                 RoleManager<IdentityRole> roleManager,
                                 UserManager<ApplicationUser> userManager,
                                 IConfiguration configuration)
        {
            // Instantiate the ApplicationDbContext through DI  
            DbContext = context;


            RoleManager = roleManager;
            UserManager = userManager;
            Configuration = configuration;

            // Instantiate a single JsonSerializerSettings object   
            // that can be reused multiple times.      
            JsonSettings = new JsonSerializerSettings() { Formatting = Formatting.Indented };
        }
        #endregion

        #region Shared Properties  
        protected ApplicationDbContext DbContext { get; private set; }
        protected RoleManager<IdentityRole> RoleManager { get; private set; }
        protected UserManager<ApplicationUser> UserManager { get; private set; }
        protected IConfiguration Configuration { get; private set; }
        protected JsonSerializerSettings JsonSettings { get; private set; }
        #endregion
    }
}