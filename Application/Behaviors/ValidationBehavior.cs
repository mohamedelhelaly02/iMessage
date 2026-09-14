using FluentValidation;
using MediatR;

namespace Application.Behaviors;

internal sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse> where TRequest : class
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Only validate if at least one validator exists for this request type.

        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResult = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResult.SelectMany(r => r.Errors)
                 .Where(f => f is not null)
                 .ToList();


            if (failures.Count > 0)
                throw new ValidationException(failures);
        }

        return await next(cancellationToken);

    }
}
