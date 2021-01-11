using Microsoft.AspNetCore.Components;

namespace RadiocomDataViewApp.Pages
{
    public partial class Artist : ComponentBase
    {
        [Parameter]
        public int ArtistId{ get; set; }

        public string ArtistName { get; set; }

        public string hello() => "hello";

    }
}