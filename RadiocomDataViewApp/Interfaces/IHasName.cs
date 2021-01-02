using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RadiocomDataViewApp.Interfaces
{
    public interface IHasName : IHasId
    {
        public string Name { get; set; }
    }
}
