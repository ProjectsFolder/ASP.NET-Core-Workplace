﻿using Microsoft.AspNetCore.Mvc;
using WebTest.Domains.Interfaces;
using WebTest.Dto;
using WebTest.Http.Responses;

namespace WebTest.Http.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class AppController : ControllerBase
    {
        protected IActionResult Success<TRequest, TResponse>(IRequestResponseHandler<TRequest, TResponse> handler, TRequest request)
            where TRequest : CommandBase
            where TResponse : class
        {
            var response = handler.Handle(request);

            return Ok(response);
        }

        protected IActionResult Success<TRequest>(IRequestHandler<TRequest> handler, TRequest request)
            where TRequest : CommandBase
        {
            handler.Handle(request);

            return Ok(new SuccessDto<object>());
        }

        protected IActionResult Success<TResponse>(IResponseHandler<TResponse> handler)
            where TResponse : class
        {
            var response = handler.Handle();

            return Ok(response);
        }

        protected IActionResult Success(ISimpleHandler handler)
        {
            handler.Handle();

            return Ok(new SuccessDto<object>());
        }
    }
}
