﻿using Microsoft.AspNetCore.Mvc;

namespace Order.Api.Controllers
{
    [ApiController]
    [Route("/")]
    public class DefaultController : ControllerBase
    {
        [HttpGet]
        public string Index()
        {
            return "API Fumigación Online";
        }
    }
}
