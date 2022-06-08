using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace Rido.MqttCore
{
    public class ClientCertificateLocator
    {
        public static X509Certificate2 Load(string certSettings)
        {
            X509Certificate2 cert = null;
            if (certSettings.Contains(".pfx|")) // mycert.pfx|mypwd
            {
                var segments = certSettings.Split('|');
                string path = segments[0];
                var pwd = segments[1];
                cert = new X509Certificate2(path, pwd);
            }
            else if (certSettings.Length==40) //thumbprint
            {
                using (X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
                {
                    store.Open(OpenFlags.ReadOnly);
                    var certs = store.Certificates.Find(X509FindType.FindByThumbprint, certSettings, false);
                    if (certs != null && certs.Count>0)
                    {
                        cert =  certs[0];
                    }
                    store.Close();
                }
            } 
            else
            {
                throw new KeyNotFoundException("certSettings format not recognized");
            }

            if(cert==null)
            {
                throw new KeyNotFoundException("cert not found");
            }

            if (!cert.HasPrivateKey)
            {
                Trace.TraceWarning("Cert found with no private key");
            }
            Trace.TraceInformation("Loaded Cert: " + cert.SubjectName.Name + " " + cert.Thumbprint);
            return cert;
        }
    }
}
