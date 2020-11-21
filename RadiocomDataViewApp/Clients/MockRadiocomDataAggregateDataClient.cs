using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using RadiocomDataViewApp.Interfaces;
using RadiocomDataViewApp.Objects;

namespace RadiocomDataViewApp.Clients
{
    public class MockRadiocomDataAggregateDataClient : IRadiocomDataAggregateDataClient
    {
        public List<ItemCount> GetMostPlayedSongs(MostPlayedSongsTimeRange timeRange)
        {
            int multiplier = 0;
            switch (timeRange)
            {
                case MostPlayedSongsTimeRange.None:
                    throw new InvalidEnumArgumentException("Must specify time range value other than None");
                case MostPlayedSongsTimeRange.SevenDays:
                    multiplier = 1;
                    break;
                case MostPlayedSongsTimeRange.ThreeMonths:
                    multiplier = 20;
                    break;
                case MostPlayedSongsTimeRange.AllTime:
                    multiplier = 50;
                    break;
                default:
                    throw new NotSupportedException("Value: " + timeRange.ToString());
                    break;
            }
            return new List<ItemCount>()
            {
                new ItemCount(){Count=100*multiplier,Name="song name", ItemId = 77},
                new ItemCount(){Count=100*multiplier,Name="song", ItemId = 53},
                new ItemCount(){Count=75*multiplier, Name="song name thats longer", ItemId=12},
                new ItemCount(){Count=51*multiplier, Name="song 1111", ItemId = 9},
                new ItemCount(){Count=51*multiplier, Name="song something", ItemId = 204},
                new ItemCount(){Count=51*multiplier, Name="something song", ItemId = 513}
            };
        }
    }
}
