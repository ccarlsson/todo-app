namespace TodoApp.Api.Endpoints;

public static class ApiEndpoints
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapHealthEndpoints();
        app.MapAuthEndpoints();
        app.MapTodoEndpoints();

        return app;
    }
}
