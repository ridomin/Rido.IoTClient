namespace Rido.IoTClient.AzDps
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "JSON Serializer Payloads")]
    public class RegistrationState
    {
        public string registrationId { get; set; }
        public string assignedHub { get; set; }
        public string deviceId { get; set; }
        public string subStatus { get; set; }
    }
}
