using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
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

            // GN-03-R2, GN-07: a response the framework produced (e.g. 401/403 from MW-04) has an English default title
            var status = context.ProblemDetails.Status ?? context.HttpContext.Response.StatusCode;
            if (context.ProblemDetails.Title is null || context.ProblemDetails.Title == ReasonPhrases.GetReasonPhrase(status))
                context.ProblemDetails.Title = status switch
                {
                    StatusCodes.Status401Unauthorized => ErrorMessages.LoginRequired,
                    StatusCodes.Status403Forbidden => ErrorMessages.Forbidden,
                    StatusCodes.Status404NotFound => ErrorMessages.NotFound,
                    StatusCodes.Status500InternalServerError => ErrorMessages.ServerError,
                    _ => context.ProblemDetails.Title,
                };
        });

        // A body that is not valid JSON: no English parser text in errors
        services.Configure<JsonOptions>(options => options.AllowInputFormatterExceptionMessages = false);

        services.Configure<MvcOptions>(options =>
        {
            // The DTO attributes decide what is required; no extra English "The request field is required."
            options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;

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

            // A JSON value that cannot be read has no message of its own, and its key is a JSON path ("$.projectId" -> "ProjectId")
            var modelState = new ModelStateDictionary();
            foreach (var (key, entry) in context.ModelState)
                foreach (var error in entry.Errors)
                    modelState.AddModelError(FieldName(key), string.IsNullOrEmpty(error.ErrorMessage) ? ValidationMessages.InvalidValue : error.ErrorMessage);

            var factory = context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
            var problem = factory.CreateValidationProblemDetails(context.HttpContext, modelState, StatusCodes.Status400BadRequest, title);
            return new BadRequestObjectResult(problem) { ContentTypes = { "application/problem+json" } };
        });

        return services;
    }

    private static string FieldName(string key) =>
        key.StartsWith("$.") && key.Length > 2 ? char.ToUpperInvariant(key[2]) + key[3..] : key;
}
