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

namespace RadiocomDataViewApp.Components
{

    public partial class ChartComponentHeaderButtons : ComponentBase
    {
        private Dictionary<int, Button> ButtonsList { get; set; } = new Dictionary<int, Button>();
        private Dictionary<int, Tab> TabsList { get; set; } = new Dictionary<int, Tab>();

        private string element = string.Empty;
        [CascadingParameter(Name = nameof(ChartComponentHeaderButtons.Buttons))]
        public Dictionary<string, EventCallback> Buttons { get; set; }
        //protected List<blazorize>


        [CascadingParameter(Name = nameof(ChartComponentHeaderButtons.Users))]
        public List<vUser> Users { get; set; }

        [CascadingParameter(Name = nameof(ChartComponentHeaderButtons.HeaderButtonConfigs))]
        public List<HeaderButtonState> HeaderButtonConfigs { get; set; }

        //public ChartComponentHeaderButtons()
        //{

        //}
        private async Task InvokeButtonEvent(MouseEventArgs mouseEventArgs, int indx, EventCallback callback)
        {
            Console.WriteLine("invoke event");

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            foreach (var kvp in ButtonStates)
            {
                kvp.Value.ButtonColor = Color.Link;
            }

            foreach (var kvp in ButtonsList)
            {
                Console.WriteLine("active:" + (kvp.Value == ButtonsList[indx]));
                element = "test";
                //parameters[nameof(Button.Active)] = kvp.Value == ButtonsList[indx];
                //parameters[nameof(Button.Color)] = kvp.Value == ButtonsList[indx] ? Color.Danger : Color.Success;
                //await kvp.Value.SetParametersAsync(ParameterView.FromDictionary(parameters));


                //kvp.Value.Active = kvp.Value == ButtonsList[indx];
                //kvp.Value.Color = kvp.Value == ButtonsList[indx] ? Color.Danger : Color.Success;

            }
            await callback.InvokeAsync(mouseEventArgs);

        }

        private async Task InvokeButtonStateEvent(MouseEventArgs mouseEventArgs, EventCallback callback, HeaderButtonState buttonState)
        {
            Console.WriteLine("invoke state event");
            foreach (var state in HeaderButtonConfigs)
            {
                Console.WriteLine($"text:{state.Text} color:{state.ButtonColor}");
                state.ButtonColor = state == buttonState ? Color.Secondary : Color.Primary;
                state.Active = state == buttonState;
            }
            await callback.InvokeAsync(mouseEventArgs);
            //StateHasChanged();
        }

        public  class HeaderButtonState 
        {
            
            public string Text { get; set; }
            public Color ButtonColor { get; set; } = Color.Danger;
            public bool Outline { get; set; } = false;
            public bool Active { get; set; } = true;
            public EventCallback ButtonClickCallback { get; set; }
        }
        private Dictionary<int, HeaderButtonState> ButtonStates = new Dictionary<int, HeaderButtonState>();

        private bool valu = false;

        public HeaderButtonStateCollection ButtonStatesCollection = new HeaderButtonStateCollection();

        public class HeaderButtonStateCollection : IDictionary<int, HeaderButtonState>
        {
            private Dictionary<int, HeaderButtonState> _source = new Dictionary<int, HeaderButtonState>();

            public HeaderButtonState this[int key] 
            { 
                get 
                {
                    if (!_source.ContainsKey(key))
                    {
                        _source[key] = new HeaderButtonState();
                    }
                    return _source[key];
                }
                set 
                {
                    _source[key] = value;
                }
            }

            public ICollection<int> Keys => throw new NotImplementedException();

            public ICollection<HeaderButtonState> Values => throw new NotImplementedException();

            public int Count => throw new NotImplementedException();

            public bool IsReadOnly => throw new NotImplementedException();

            public void Add(int key, HeaderButtonState value)
            {
                throw new NotImplementedException();
            }

            public void Add(KeyValuePair<int, HeaderButtonState> item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(KeyValuePair<int, HeaderButtonState> item)
            {
                throw new NotImplementedException();
            }

            public bool ContainsKey(int key)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(KeyValuePair<int, HeaderButtonState>[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<KeyValuePair<int, HeaderButtonState>> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public bool Remove(int key)
            {
                throw new NotImplementedException();
            }

            public bool Remove(KeyValuePair<int, HeaderButtonState> item)
            {
                throw new NotImplementedException();
            }

            public bool TryGetValue(int key, [MaybeNullWhen(false)] out HeaderButtonState value)
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

    }
    public class MyButton: Button
    {
        [Parameter]
        public EventCallback<Color> ColorChanged { get; set; }

        [Parameter]
        public EventCallback<bool> ActiveChanged { get; set; }
    }
}