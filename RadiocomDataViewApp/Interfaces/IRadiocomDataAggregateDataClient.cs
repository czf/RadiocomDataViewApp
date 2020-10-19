﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RadiocomDataViewApp.Objects;

namespace RadiocomDataViewApp.Interfaces
{
    public interface IRadiocomDataAggregateDataClient
    {
        List<ItemCount> GetMostPlayedSongs();
    }
}