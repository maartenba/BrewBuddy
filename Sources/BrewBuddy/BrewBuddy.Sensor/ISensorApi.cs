using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrewBuddy.Sensor
{
    public interface ISensorApi
    {
        bool IsLinkedToBrew { get; }

        void PrepareCommitBatch();
        bool Commit(DateTime when, double value);
    }
}
