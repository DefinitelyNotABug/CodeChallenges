using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CodeCodeChallenge.Tests.Integration.Helpers
{
    public class TestServer : IDisposable, IAsyncDisposable
    {
        private WebApplicationFactory<Program> applicationFactory;

        public TestServer()
        {
            applicationFactory = new WebApplicationFactory<Program>();
        }

        public HttpClient NewClient()
        {
            return applicationFactory.CreateClient();
        }


        public ValueTask DisposeAsync()
        {
            return ((IAsyncDisposable)applicationFactory).DisposeAsync();
        }

        public void Dispose()
        {
            ((IDisposable)applicationFactory).Dispose();
        }
    }
}
