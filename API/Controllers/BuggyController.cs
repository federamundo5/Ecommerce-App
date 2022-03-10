using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using API.Errors;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    public class BuggyController : BaseApiController
    {
        public StoreContext _context { get; }
        public BuggyController(StoreContext context){
            _context = context;

        }

        [HttpGet("testAuth")]
        [Authorize]
        public ActionResult<string> GetSecretText(){
            return "test";
        }

        [HttpGet("notfound")]
        public ActionResult GetNotFoundRequest()
        {

            var thing = _context.Products.Find(42);

            if (thing == null){
                return NotFound(new ApiResponse(400));
            }
                return Ok();
        }

      [HttpGet("servererror")]
        public ActionResult GetServerError()
        {
              
             var thing = _context.Products.Find(42);

                var thingToReturn = thing.ToString();


                return Ok();
        }

        [HttpGet("badrequest")]
        public ActionResult GetBadRequest()
        {
                return BadRequest(new ApiResponse(42));
        }


          [HttpGet("badrequest/{id}")]
        public ActionResult GetNotFoundRequest(int id)
        {
                return Ok();
        }
    }
}