using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace RadiocomDataViewApp.Interfaces
{
    public interface IVisitService
    {
        Task<bool> HasVisitedAsync();
        Task SetVisitedAsync();

        Task ClearVisitedStateAsync();

        event Func<Task> OnVisitStateChange;
    }
}
