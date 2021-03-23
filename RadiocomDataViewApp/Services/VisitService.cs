using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using RadiocomDataViewApp.Interfaces;

namespace RadiocomDataViewApp.Services
{
    public class VisitService : IVisitService
    {
        private const string VISITED_KEY = "visited";
        private readonly ILocalStorageService _localStorageService;

        private DateTime _welcomeDate;

        public event Func<Task> OnVisitStateChange;

        public VisitService(ILocalStorageService localStorageService, DateTime welcomeDate)
        {
            _localStorageService = localStorageService;
            _welcomeDate = welcomeDate;
        }

        public async Task<bool> HasVisitedAsync() 
        {
            return await _localStorageService.GetItemAsync<DateTime>(VISITED_KEY) > _welcomeDate; 
        }
        public async Task SetVisitedAsync()
        {
            await _localStorageService.SetItemAsync(VISITED_KEY, DateTime.Now);
            await OnVisitStateChange?.Invoke();
        }

        public async Task ClearVisitedStateAsync()
        {
            await _localStorageService.RemoveItemAsync(VISITED_KEY);
            await OnVisitStateChange?.Invoke();

        }

    }

}
