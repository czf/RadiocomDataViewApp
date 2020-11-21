using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace RadiocomDataViewApp.Objects
{
    public class HeaderButtonConfig
    {
        public string ButtonLabelText { get; set; }
        public EventCallback ButtonClickCallback { get; set; }
    }
}
