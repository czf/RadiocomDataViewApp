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
        private static Random _random = new Random();

        public List<ItemCount> GetMostPlayedSongs(AggregateTimeRange timeRange)
        {
            int multiplier = GetTimeRangeMultipler(timeRange);
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

        private static int GetTimeRangeMultipler(AggregateTimeRange timeRange)
        {
            int multiplier;
            switch (timeRange)
            {
                case AggregateTimeRange.None:
                    throw new InvalidEnumArgumentException("Must specify time range value other than None");
                case AggregateTimeRange.SevenDays:
                    multiplier = 1;
                    break;
                case AggregateTimeRange.ThreeMonths:
                    multiplier = 20;
                    break;
                case AggregateTimeRange.AllTime:
                    multiplier = 50;
                    break;
                default:
                    throw new NotSupportedException("Value: " + timeRange.ToString());
                    break;
            }

            return multiplier;
        }

        public List<ItemCount> GetMostPlayedArtists(AggregateTimeRange timeRange)
        {   
            int multiplier = GetTimeRangeMultipler(timeRange);
            return new List<ItemCount>()
            {
                new ItemCount(){Count=100*multiplier,Name="artist name", ItemId = 77},
                new ItemCount(){Count=100*multiplier,Name="artist", ItemId = 53},
                new ItemCount(){Count=75*multiplier, Name="artist name thats longer", ItemId=12},
                new ItemCount(){Count=51*multiplier, Name="artist 1111", ItemId = 9},
                new ItemCount(){Count=51*multiplier, Name="artist something", ItemId = 204},
                new ItemCount(){Count=51*multiplier, Name="something artist", ItemId = 513}
            };
        }

        public int GetTotalUniqueSongs(AggregateTimeRange timeRange)
        {
            int multiplier = GetTimeRangeMultipler(timeRange);

            return multiplier * _random.Next(300, 350);
        }

        public int GetTotalUniqueArtists(AggregateTimeRange timeRange)
        {
            int multiplier = GetTimeRangeMultipler(timeRange);

            return multiplier * _random.Next(300, 350);
        }
    }
}
