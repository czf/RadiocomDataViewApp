using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;
using RadiocomDataViewApp.Clients;
using RadiocomDataViewApp.Interfaces;
using RadiocomDataViewApp.Objects;
using RadiocomDataViewApp.Objects.Dto;
namespace RadiocomDataViewApp.Pages
{
    public partial class ArtistWorks : ComponentBase
    {
        [Inject]
        public IRadiocomArtistWorkRepository RadiocomArtistWorkRepository { get; set; }

        [Parameter]
        public string AlphaChar { get; set; }

        Func<ArtistWorkDisplay, string> HrefGenerator = item => $"artistwork/{item.Id}";

        IEnumerable<ArtistWorkDisplay> artistWorks;

        protected override Task OnInitializedAsync()
        {
             return base.OnInitializedAsync()
                .ContinueWith(x=> 
                    InvokeAsync(async () =>
                        artistWorks = (await RadiocomArtistWorkRepository.GetArtistWorksAsync())
                                        .Select(x => new ArtistWorkDisplay(x))
                                        .OrderBy(x => x.Name)))
                .Unwrap();
            //artistWorks = null;
            
            //await 
            
        }

    }
}
