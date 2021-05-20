using API.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository _repository;
        public UserController(IUserRepository repository) 
        {
            _repository = repository;
        }
        [HttpPost("register")]
        public IActionResult Hello() {
            return Ok("Success");
        }
    }
}
