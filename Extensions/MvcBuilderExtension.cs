using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Netcorext.Contracts;

namespace Microsoft.Extensions.DependencyInjection;

public static class MvcBuilderExtension
{
    public static IMvcBuilder AddCustomInvalidModelStateResponse(this IMvcBuilder builder, Func<ActionContext, IActionResult>? responseFactory = null)
    {
        return builder.ConfigureApiBehaviorOptions(options =>
                                                   {
                                                       options.InvalidModelStateResponseFactory = responseFactory ?? (context =>
                                                                                                                      {
                                                                                                                          var result = Result.InvalidInput.Clone();

                                                                                                                          result.Errors = context.ModelState
                                                                                                                                                 .Select(t => new ValidationFailure
                                                                                                                                                              {
                                                                                                                                                                  PropertyName = t.Key,
                                                                                                                                                                  ErrorMessage = t.Value?
                                                                                                                                                                                  .Errors
                                                                                                                                                                                  .Select(t2 => t2.ErrorMessage)
                                                                                                                                                                                  .Aggregate((c, n) => c + "\n" + n)
                                                                                                                                                              });

                                                                                                                          return new ObjectResult(result)
                                                                                                                                 {
                                                                                                                                     StatusCode = 400
                                                                                                                                 };
                                                                                                                      });
                                                   });
    }
}