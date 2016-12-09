using System;
using System.Collections.Generic;
using System.Linq;

namespace Crawler
{
    public class BenchMarker
    {
        private readonly int _maxQueueItems = 100;
        private readonly Queue<long> _timeQueue = new Queue<long>();

        public BenchMarker(int maxItems)
        {
            _maxQueueItems = maxItems;
        }

        public void Insert(long lastScan)
        {
            _timeQueue.Enqueue(lastScan);

            if (_timeQueue.Count > _maxQueueItems)
                _timeQueue.Dequeue();
        }

        public long AverageTime
        {
            get
            {
                try
                {
                    return _timeQueue.Sum() / _timeQueue.Count;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }
    }
}