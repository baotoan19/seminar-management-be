using Microsoft.AspNetCore.Mvc.Filters;
using Seminar.CORE.ExceptionCustom;

namespace Seminar.API.Filters;

public class ValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(e => e.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            throw new ModelValidationException(errors);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}