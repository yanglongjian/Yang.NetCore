Serve.Run(RunOptions.Default.AddWebComponent<WebComponent>());
public class WebComponent : IWebComponent
{
    public void Load(WebApplicationBuilder builder, ComponentContext componentContext)
    {
        builder.Logging.AddConsoleFormatter();
    }
}