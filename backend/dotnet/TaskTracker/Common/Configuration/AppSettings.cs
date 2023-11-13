namespace Common.Configuration;

public class AppSettings
{
    public static string SectionKey => nameof(AppSettings);
    public string ApplicationDbSchema { get; set; } = "v1";
    public string ServiceName { get; set; } = default!;
}