using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using Netcorext.Contracts;

namespace Microsoft.Extensions.DependencyInjection.Middlewares;

public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _environment;
        private readonly ILogger<CustomExceptionMiddleware> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public CustomExceptionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IHostEnvironment environment)
        {
            _next = next;
            _environment = environment;
            _logger = loggerFactory.CreateLogger<CustomExceptionMiddleware>();

            _jsonSerializerOptions = new JsonSerializerOptions
                                     {
                                         AllowTrailingCommas = false,
                                         DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                                         DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                                         IgnoreReadOnlyProperties = false,
                                         PropertyNameCaseInsensitive = true,
                                         PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                         WriteIndented = false
                                     };
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.ToString());

                var status = "500000";
                var result = string.Empty;

                switch (e)
                {
                    case ValidationException validationEx:
                        var failure = validationEx.Errors.First();

                        var code = typeof(Result).GetFields(BindingFlags.Public | BindingFlags.Static)
                                                   .Select(t => t.GetValue(null)?.ToString())
                                                   .FirstOrDefault(t => t == failure.ErrorCode);

                        status = code ?? Result.InvalidInput;
                        result = await ToJsonAsync(new
                                                   {
                                                       Code = status,
                                                       Message = failure.ErrorMessage
                                                   });
                        break;
                    case ArgumentException argumentEx:
                        status = Result.InvalidInput;
                        result = await ToJsonAsync(new
                                                   {
                                                       Code = status,
                                                       argumentEx.Message
                                                   });
                        break;
                    case BadHttpRequestException badHttpRequestEx:
                        status = badHttpRequestEx.Message == "Request body too large." ? Result.PayloadTooLarge : Result.InvalidInput;
                        result = await ToJsonAsync(new
                                                   {
                                                       Code = status,
                                                       badHttpRequestEx.Message
                                                   });
                        break;
                    default:
                        status = Result.InternalServerError;

                        if (!_environment.IsProduction())
                        {
                            result = await ToJsonAsync(new
                                                       {
                                                           Code = status,
                                                           e.Source,
                                                           e.Message,
                                                           Stack = e.StackTrace,
                                                           InnerException = e.InnerException?.ToString()
                                                       });
                        }
                        else
                        {
                            result = await ToJsonAsync(new
                                                       {
                                                           Code = status,
                                                           e.Message
                                                       });
                        }

                        break;
                }

                context.Response.StatusCode = GetHttpStatus(status);
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result);
            }
        }

        private async Task<string> ToJsonAsync(object content)
        {
            var stream = new MemoryStream();

            await JsonSerializer.SerializeAsync(stream, content, _jsonSerializerOptions);

            stream.Seek(0, SeekOrigin.Begin);

            return await new StreamReader(stream).ReadToEndAsync();
        }

        private int GetHttpStatus(string? code)
        {
            if (string.IsNullOrWhiteSpace(code) || code.Length != 6) return 400;

            if (!int.TryParse(code, out var httpStatus)) httpStatus = 400;

            return httpStatus / 1000;
        }
    }