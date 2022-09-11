using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fibonacci.Client.Controllers;

public class ModelExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        context.Result = context.Exception switch
        {
            ArgumentException e => new BadRequestObjectResult(e.Message),
            _ => context.Result
        };
    }
}