using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TaskManager.Core.Results;
using TaskManager.Core.Validation;

namespace TaskManager.Api.Extensions;

// GN-03: every error is a ProblemDetails with status, title, correlationId, and errors only on 400
public static class ProblemDetailsExtensions
{
    public static IServiceCollection AddApiProblemDetails(this IServiceCollection services)
    {
        // GN-03: correlationId on every ProblemDetails; MW-02 (task 31) sets TraceIdentifier to the request's id
        services.AddProblemDetails(options => options.CustomizeProblemDetails = context =>
        {
            context.ProblemDetails.Extensions.Remove("traceId");
            context.ProblemDetails.Extensions["correlationId"] = context.HttpContext.TraceIdentifier;
        });

        services.Configure<MvcOptions>(options =>
        {
            // GN-07-R1: Hebrew field messages for values that cannot be bound at all (e.g. status=Closed)
            var messages = options.ModelBindingMessageProvider;
            messages.SetAttemptedValueIsInvalidAccessor((_, _) => ValidationMessages.InvalidValue);
            messages.SetNonPropertyAttemptedValueIsInvalidAccessor(_ => ValidationMessages.InvalidValue);
            messages.SetUnknownValueIsInvalidAccessor(_ => ValidationMessages.InvalidValue);
            messages.SetNonPropertyUnknownValueIsInvalidAccessor(() => ValidationMessages.InvalidValue);
            messages.SetValueIsInvalidAccessor(_ => ValidationMessages.InvalidValue);
            messages.SetValueMustBeANumberAccessor(_ => ValidationMessages.InvalidValue);
            messages.SetNonPropertyValueMustBeANumberAccessor(() => ValidationMessages.InvalidValue);
            messages.SetMissingBindRequiredValueAccessor(_ => ValidationMessages.Required);
            messages.SetMissingKeyOrValueAccessor(() => ValidationMessages.Required);
            messages.SetMissingRequestBodyRequiredValueAccessor(() => ValidationMessages.Required);
            messages.SetValueMustNotBeNullAccessor(_ => ValidationMessages.Required);
        });

        // AP-02: invalid ModelState -> 400; list parameters (GN-02) and body fields have different titles (GN-07)
        services.Configure<ApiBehaviorOptions>(options => options.InvalidModelStateResponseFactory = context =>
        {
            var fromQuery = context.ActionDescriptor.Parameters.Any(p => p.BindingInfo?.BindingSource == BindingSource.Query);
            var title = fromQuery ? ErrorMessages.InvalidParameter : ErrorMessages.InvalidData;

            // A JSON value that cannot be read has no message of its own
            var modelState = new ModelStateDictionary();
            foreach (var (key, entry) in context.ModelState)
                foreach (var error in entry.Errors)
                    modelState.AddModelError(key, string.IsNullOrEmpty(error.ErrorMessage) ? ValidationMessages.InvalidValue : error.ErrorMessage);

            var factory = context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
            var problem = factory.CreateValidationProblemDetails(context.HttpContext, modelState, StatusCodes.Status400BadRequest, title);
            return new BadRequestObjectResult(problem) { ContentTypes = { "application/problem+json" } };
        });

        return services;
    }
}
