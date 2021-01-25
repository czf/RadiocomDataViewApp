using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;
using RadiocomDataViewApp.Interfaces;

namespace RadiocomDataViewApp.Components.Sitewide
{
    public partial class ApplicationUpdateModal : ComponentBase
    {
        private Modal ApplicationUpdate;
        
        [Inject]
        public IUpdateService UpdateService { get; set; }
        

        protected override async Task OnParametersSetAsync()
        {

            await base.OnParametersSetAsync();
            
            UpdateService.OnWelcomeHasChanged += WelcomeHasChanged;
        }

        private void WelcomeHasChanged()
        {
            ApplicationUpdate.Show();
        }
    }
}
