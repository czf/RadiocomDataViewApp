using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using RadiocomDataViewApp.Interfaces;

namespace RadiocomDataViewApp.Components.Sitewide
{
    public partial class WelcomeModal : ComponentBase
    {
        [Inject]
        public IVisitService VisitService { get; set; }

        private Modal Welcome;

        private bool _hasVisited;


        protected override void OnAfterRender(bool firstRender)
        {
            //base.OnAfterRender(firstRender);
            //if (firstRender)
            //{
            //    Welcome.Show();
            //}
        }

        private Task ClosedModal()
        {
            Task visted = VisitService.SetVisitedAsync();            
            return visted;
        }

        //private async Task ClosedModal()
        //{
        //   
        //}

        private void CloseClick() {


            Welcome.Hide();
            StateHasChanged();

        }

        protected override async Task OnParametersSetAsync()
        {

            await base.OnParametersSetAsync();
            _hasVisited = await VisitService.HasVisitedAsync();
            VisitService.OnVisitStateChange += UpdateVisitedState;
        }

        async Task UpdateVisitedState()
        {
            _hasVisited = await VisitService.HasVisitedAsync();
            StateHasChanged();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (!_hasVisited)
            {
                Console.WriteLine("after");
                Welcome.Show();
            }
            
        }




        //public override async Task SetParametersAsync(ParameterView parameters)
        //{

        //    await base.SetParametersAsync(parameters);
        //    _hasVisited = await VisitService.HasVisitedAsync();

        //}
    }
}
