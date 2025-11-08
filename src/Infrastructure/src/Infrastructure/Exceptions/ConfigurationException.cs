namespace Giantnodes.Infrastructure;

public class ConfigurationException : Exception
{
    public string ParameterName { get; }

    public ConfigurationException(
        string parameterName,
        string message = "Configuration value for parameter '{0}' is missing or invalid.")
        : base(string.Format(message, parameterName))
    {
        ParameterName = parameterName;
    }
}
