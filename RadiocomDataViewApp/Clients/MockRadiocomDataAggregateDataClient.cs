using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RadiocomDataViewApp.Interfaces;
using RadiocomDataViewApp.Objects;

namespace RadiocomDataViewApp.Clients
{
    public class MockRadiocomDataAggregateDataClient : IRadiocomDataAggregateDataClient
    {
        public List<ItemCount> GetMostPlayedSongs()
        {
            return new List<ItemCount>()
            {
                new ItemCount(){Count=100,Name="song name"},
                new ItemCount(){Count=100,Name="song"},
                new ItemCount(){Count=75, Name="song name thats longer"},
                new ItemCount(){Count=51, Name="song 1111"},
                new ItemCount(){Count=51, Name="song something"},
                new ItemCount(){Count=51, Name="something song"}
            };
        }
    }
}
