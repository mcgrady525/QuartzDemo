using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using System.IO;
using Tracy.Frameworks.Common.Extends;
using Tracy.Frameworks.Common.Consts;
using QuartzDemo.Common.Helper;

namespace QuartzDemo.Task
{
    public class HelloQuartzJob: IStatefulJob
    {
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="context"></param>
        public void Execute(JobExecutionContext context)
        {
            try
            {
                var msg = string.Format("【{0}】hello Quartz!", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                QuartzDemoHelper.WriteLogs(msg);
            }
            catch (Exception ex)
            {
                QuartzDemoHelper.WriteLogs(ex.ToString());
            }
        }
    }
}
