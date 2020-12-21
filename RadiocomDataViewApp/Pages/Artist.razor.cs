using Microsoft.AspNetCore.Components;

namespace RadiocomDataViewApp.Pages
{
    public partial class Artist
    {
        [Parameter]
        public int ArtistId{ get; set; }

        public string hello() => "hello";

    }
}