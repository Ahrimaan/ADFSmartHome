using System;
using System.Collections.Generic;
using System.Text;

namespace TimeSeriesCollector.Contracts
{
    public interface IModelMapper<Tin, TOut> where Tin : class where TOut : class
    {
        public TOut MapTo(Tin model);
    }
}
