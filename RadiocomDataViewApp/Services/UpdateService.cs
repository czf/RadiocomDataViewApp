using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using RadiocomDataViewApp.Interfaces;

namespace RadiocomDataViewApp.Services
{
    public class UpdateService : IUpdateService, IAsyncDisposable, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly DateTime _welcomeTime;

        private readonly Timer _welcomeTimer;
        private bool disposedValue;
        private readonly Random _random;

        public UpdateService(HttpClient httpClient, DateTime welcomeTime)
        {
            _httpClient = httpClient;
            _welcomeTime = welcomeTime;
            _random = new Random();

             _welcomeTimer = new Timer(HasApplicationUpdate, null, TimeSpan.FromHours(1), TimeSpan.FromHours(5));
        }

        public event Action OnWelcomeHasChanged;

        private async void HasApplicationUpdate(object state)
        {

            bool welcomeHasChanged = await _httpClient.GetFromJsonAsync<ApplicationUpdateCheck>("/appsettings.json?r=" + _random.Next())
               .ContinueWith(x => _welcomeTime != x.Result.welcome);
            if (welcomeHasChanged)
            {
                OnWelcomeHasChanged?.Invoke();
            }
        }

        

        private class ApplicationUpdateCheck 
        {
            public DateTime welcome { get; set; }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _welcomeTimer.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public ValueTask DisposeAsync()
        {
            return ((IAsyncDisposable)_welcomeTimer).DisposeAsync();
        }
    }
}
