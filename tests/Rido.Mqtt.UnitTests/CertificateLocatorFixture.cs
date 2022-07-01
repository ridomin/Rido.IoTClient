using Rido.MqttCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rido.Mqtt.UnitTests
{
    public class CertificateLocatorFixture
    {
        [Fact]
        public void ParseCertFromFile()
        {
            string certSettings = "onething.pfx|1234";
            var cert = ClientCertificateLocator.Load(certSettings);
            Assert.NotNull(cert);
            Assert.Equal("CN=onething", cert.SubjectName.Name);
            Assert.Equal("8E983707D3F802E6717BBCD193129946573F31D4", cert.Thumbprint);
        }

        [Fact]
        public void LoadCertFromStore()
        {
            var testCert = ClientCertificateLocator.Load("onething.pfx|1234");
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            store.Add(testCert);

            string certSettings = "8E983707D3F802E6717BBCD193129946573F31D4";
            var cert = ClientCertificateLocator.Load(certSettings);
            Assert.NotNull(cert);
            Assert.Equal("CN=onething", cert.SubjectName.Name);
            Assert.Equal("8E983707D3F802E6717BBCD193129946573F31D4", cert.Thumbprint);

            store.Remove(testCert);
            store.Close();
        }
    }
}
