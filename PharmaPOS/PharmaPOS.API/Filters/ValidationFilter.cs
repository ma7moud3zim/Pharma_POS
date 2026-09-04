using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PharmaPOS.API.Filters;

public class ValidationFilter<T> : IAsyncActionFilter where T : class
{
    private readonly IValidator<T> _validator;

    public ValidationFilter(IValidator<T> validator)
    {
        _validator = validator;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var argument = context.ActionArguments.Values
            .OfType<T>()
            .FirstOrDefault();

        if (argument is not null)
        {
            var result = await _validator.ValidateAsync(argument);

            if (!result.IsValid)
            {
                var errors = result.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );

                context.Result = new BadRequestObjectResult(new
                {
                    message = "Validation failed.",
                    errors
                });

                return;
            }
        }

        await next();
    }
}