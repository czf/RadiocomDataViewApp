
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace RadiocomDataViewApp.Pages
{
    public partial class Index : ComponentBase
    {

        [CascadingParameter]
        public EventCallback OnMenuHamburgerClick { get; set; }
        [CascadingParameter]
        public Action ClickMenu { get; set; }

        protected override Task OnParametersSetAsync()
        {
            return base.OnParametersSetAsync();
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }
    }
}

