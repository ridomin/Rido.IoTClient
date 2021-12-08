namespace Rido.IoTClient.AzDps
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JSON Serializer Payloads")]
    public class DpsStatus
    {
        public string operationId { get; set; }
        public string status { get; set; }
        public RegistrationState registrationState { get; set; }
    }
}
