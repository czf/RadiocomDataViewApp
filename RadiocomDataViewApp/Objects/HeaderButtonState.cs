using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;

namespace RadiocomDataViewApp.Objects
{
    public class HeaderButtonState
    {
        public string Text { get; set; }
        public Color ButtonColor { get; set; } = Color.Primary;
        public bool Outline { get; set; }
        public bool Active { get; set; }
        public bool Loading { get; set; } 
        public EventCallback ButtonClickCallback { get; set; }
    }
}
