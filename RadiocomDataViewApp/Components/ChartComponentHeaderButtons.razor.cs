using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Blazorise.Charts;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.CSharp;
using Microsoft.AspNetCore.Components.Web;
using Blazorise;
using System.Diagnostics.CodeAnalysis;
using System.Collections;
using RadiocomDataViewApp.Pages;
using RadiocomDataViewApp.Objects;

namespace RadiocomDataViewApp.Components
{

    public partial class ChartComponentHeaderButtons : ComponentBase
    {        
        [Parameter]
        [CascadingParameter(Name = nameof(ChartComponentHeaderButtons.HeaderButtonConfigs))]
        public List<HeaderButtonState> HeaderButtonConfigs { get; set; }

        private async Task InvokeButtonStateEvent(MouseEventArgs mouseEventArgs, EventCallback callback, HeaderButtonState buttonState)
        {
            HeaderButtonState selected = null;
            foreach (var state in HeaderButtonConfigs)
            {
                state.ButtonColor = state == buttonState ? Color.Secondary : Color.Primary;
                state.Active = state == buttonState;
                if(  state == buttonState)
                {
                    selected = state;
                    state.Loading = true;
                }
            }
            selected.Loading = true;
            await callback.InvokeAsync(mouseEventArgs);
            selected.Loading = false;
        }
    }

}