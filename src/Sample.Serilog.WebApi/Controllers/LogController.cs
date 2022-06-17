using System;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Sample.Serilog.WebApi.Controllers;

[Route("api/[controller]")]
public class LogController : Controller
{

    [HttpPost("sample")]
    public IActionResult PostSampleData()
    {
        Log.Information("Sample information.");
        return Ok(new { Result = "Data successfully registered with Elasticsearch" });
    }

    [HttpGet("exception")]
    public IActionResult GetByName()
    {
        //Sample middlware exception log
        Log.Error("This is an error"); // (new Exception("Sample Exception"), "This is an error");
        return BadRequest();
        //throw new Exception("This is an error.");
    }

}