﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoneyEzBank.Services.BusinessModels;

namespace MoneyEzBank.API.Controllers
{
    [Route("api/base-controller")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected async Task<IActionResult> ValidateAndExecute(Func<Task<BaseResultModel>> func)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return GetActionResponse(await func());
        }

        protected IActionResult GetActionResponse(BaseResultModel baseResponse)
        {
            return baseResponse.Status switch
            {
                StatusCodes.Status401Unauthorized => Unauthorized(baseResponse),
                StatusCodes.Status200OK => Ok(baseResponse),
                StatusCodes.Status400BadRequest => BadRequest(baseResponse),
                StatusCodes.Status404NotFound => NotFound(baseResponse),
                StatusCodes.Status409Conflict => Conflict(baseResponse),
                StatusCodes.Status500InternalServerError => StatusCode(StatusCodes.Status400BadRequest, baseResponse),
                _ => StatusCode(baseResponse.Status, baseResponse)
            };
        }
    }
}
