using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler {
    class BenchMarker {


        int maxQueueItems = 100;
        Queue<long> timeQueue = new Queue<long>();

        public BenchMarker(int maxItems) {
            this.maxQueueItems = maxItems;
        }

        public void Insert(long lastScan) {
            timeQueue.Enqueue(lastScan);

            if(timeQueue.Count > maxQueueItems)
                timeQueue.Dequeue();
        }

        public long AverageTime {
            get {
                return timeQueue.Sum() / timeQueue.Count;
            }
        }

    }
}
