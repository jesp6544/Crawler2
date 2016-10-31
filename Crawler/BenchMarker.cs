using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler {

    internal class BenchMarker {
        private int maxQueueItems = 100;
        private Queue<long> timeQueue = new Queue<long>();

        public BenchMarker(int maxItems) {
            this.maxQueueItems = maxItems;
        }

        public void Insert(long lastScan) {
            timeQueue.Enqueue(lastScan);

            if(timeQueue.Count > maxQueueItems)
                timeQueue.Dequeue();
        }

        public long AverageTime
        {
            get
            {
                try {
                    return timeQueue.Sum() / timeQueue.Count;
                } catch(Exception) {
                    return 0;
                }
            }
        }
    }
}