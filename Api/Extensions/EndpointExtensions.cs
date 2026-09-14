using System.Reflection;

namespace Api.Extensions
{
    public static class EndpointExtensions
    {
        public static void MapEndpoints(this IEndpointRouteBuilder app)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var endpointTypes = assembly.GetTypes()
                .Where(IsEndpointType);

            foreach (var endpointType in endpointTypes)
            {
                var mapEndpointMethod =
                    endpointType.GetMethod(
                        "MapEndpoint",
                        BindingFlags.Public |
                        BindingFlags.Static);

                mapEndpointMethod!.Invoke(
                    null,
                    [app]);
            }

        }

        private static bool IsEndpointType(Type type)
        {
            if (!type.IsClass)
                return false;

            if (!type.IsAbstract || !type.IsSealed)
                return false;

            var method = type.GetMethod(
                "MapEndpoint",
                BindingFlags.Public |
                BindingFlags.Static);

            if (method is null)
                return false;

            var parameters =
                method.GetParameters();

            if (parameters.Length != 1)
                return false;

            return parameters[0].ParameterType ==
                   typeof(IEndpointRouteBuilder);
        }
    }
}
