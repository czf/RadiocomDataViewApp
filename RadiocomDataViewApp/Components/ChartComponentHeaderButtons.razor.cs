﻿using System;
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
using System.Threading;

namespace RadiocomDataViewApp.Components
{

    public partial class ChartComponentHeaderButtons : ComponentBase
    {
        //private Dictionary<int, Button> ButtonsList { get; set; } = new Dictionary<int, Button>();
        //private Dictionary<int, Tab> TabsList { get; set; } = new Dictionary<int, Tab>();

        
        //[CascadingParameter(Name = nameof(ChartComponentHeaderButtons.Buttons))]
        //public Dictionary<string, EventCallback> Buttons { get; set; }

        [CascadingParameter(Name = nameof(ChartComponentHeaderButtons.HeaderButtonConfigs))]
        public List<HeaderButtonState> HeaderButtonConfigs { get; set; }

        private async Task InvokeButtonStateEvent(MouseEventArgs mouseEventArgs, EventCallback callback, HeaderButtonState buttonState)
        {
            HeaderButtonState selected = null;
            Console.WriteLine("invoke state event");
            foreach (var state in HeaderButtonConfigs)
            {
                Console.WriteLine($"text:{state.Text} color:{state.ButtonColor}");
                state.ButtonColor = state == buttonState ? Color.Secondary : Color.Primary;
                state.Active = state == buttonState;
                if(  state == buttonState)
                {
                    selected = state;
                    state.Loading = true;
                    Thread.Yield();
                }
            }
            selected.Loading = true;
            await callback.InvokeAsync(mouseEventArgs);
            selected.Loading = false;
        }

        
        




    }

}