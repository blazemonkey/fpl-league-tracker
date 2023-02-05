using System.Reflection;
using System.Text.Json;

namespace FPLV2.UnitTests;

public class EmbeddedResourceHelper
{
    private static Assembly _assembly;

    static EmbeddedResourceHelper()
    {
        _assembly = IntrospectionExtensions.GetTypeInfo(typeof(EmbeddedResourceHelper)).Assembly;
    }

    public static T GetResourceFromJson<T>(string name)
    {
        using var stream = _assembly.GetManifestResourceStream($"FPLV2.UnitTests.SampleData.{name}.json");
        if (stream == null)
            return default;

        var resource = JsonSerializer.Deserialize<T>(stream);
        return resource;
    }

    public static string GetResourceFromJson(string name)
    {
        using var stream = _assembly.GetManifestResourceStream($"FPLV2.UnitTests.SampleData.{name}.json");
        if (stream == null)
            return default;

        var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();
        return json;
    }
}
