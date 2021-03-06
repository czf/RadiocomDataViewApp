﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Routing;
using RadiocomDataViewApp.Interfaces;

namespace RadiocomDataViewApp.Components
{
    public class ListComponent<TItem>: BaseComponent where TItem : IHasName
    {
        [Parameter]
        public IEnumerable<TItem> Items { get; set; }
        
        [Parameter]
        public Func<TItem,string> ListItemHrefGenerator{ get; set; }

        private Dictionary<char, Container> ListSectionWrappers = new Dictionary<char, Container>();

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            List<RenderFragment> containerChildContents = new List<RenderFragment>();
            int line = 0;
            foreach(var group in Items.GroupBy(x => x.Name.ToUpperInvariant().FirstOrDefault()))
            {
                builder.OpenComponent<Container>(line++);
                builder.AddAttribute(line++, "class", "mb-4 bg-dark");
                RenderInput<TItem> containerInput = new RenderInput<TItem>() { line = 0, OriginalBuilder = builder, InputItems=group };
                
                builder.AddAttribute(line++, "ChildContent", ChildRender(containerInput));
                
                builder.AddComponentReferenceCapture(line++, (refCapture) =>
                {
                    ListSectionWrappers[group.Key] = (Container)refCapture;
                });
                
                builder.CloseComponent();
            }
        }
        private class RenderInput<TItemInput> where TItemInput : TItem
        {
            public int indx { get; set; }
            public int line { get; set; }
            public RenderTreeBuilder OriginalBuilder{ get; set; }
            public bool MoreToRender { get; set; }
            public IEnumerable<TItemInput> InputItems { get; set; }

        }

        private RenderFragment<RenderInput<TItem>> ChildRender;
        public ListComponent()
        {
            ChildRender = (RenderFragment<RenderInput<TItem>>)((input) => (RenderFragment)( (builder2) =>
            {
                builder2.OpenComponent<Heading>(input.line);
                builder2.AddAttribute(input.line++, "Size", HeadingSize.Is3);
                builder2.AddAttribute(input.line++, "ChildContent", (RenderFragment)((builder3) => builder3.AddContent(0, input.InputItems?.First().Name?.ToUpperInvariant()?.FirstOrDefault())));
                builder2.Class("border-bottom", input.line++);
                builder2.CloseComponent();
                foreach (var item in input.InputItems)
                {
                    builder2.OpenElement(input.line++, "a");
                    builder2.Href($"{ListItemHrefGenerator(item)}", input.line++);
                    builder2.Class("d-block", input.line++);

                    string name = item.Name;
                    if (String.IsNullOrEmpty(item.Name))
                    {
                        name = "<no name listed>";
                    }

                    builder2.AddContent(input.line++, name);
                    builder2.CloseElement();
                }

            }));
        }

    }

    


}
