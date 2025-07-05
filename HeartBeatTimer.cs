using Common.Logging;
using ESBasic.Threading.Engines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseServer.Core
{
    public class HeartBeatTimer : BaseCycleEngine
    {
        public Action EventHeartBeat;
        private ILog loger = LogManager.GetLogger("PlatformNetLoger");
        protected override bool DoDetect()
        {
            try
            {
                if (EventHeartBeat != null)
                    EventHeartBeat();
            }
            catch (Exception ex)
            {
                loger.Error(ex);
            }
            return true;
        }
    }
}
