namespace Rido.BrokerProxy
{

    public interface Imemmon
    {
        public enum DiagnosticsMode
        {
            minimal = 0,
            complete = 1,
            full = 2
        }

        public class Cmd_getRuntimeStats_Response 
        {
            public Dictionary<string, string> diagnosticResults { get; set; } = new Dictionary<string, string>();
        }
    }
}
