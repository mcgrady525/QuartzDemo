using Quartz;
using Quartz.Impl;
using QuartzDemo.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using QuartzDemo.Common.Helper;

namespace QuartzDemo.Task
{
    public partial class QuartzDemoService : ServiceBase
    {
        public QuartzDemoService()
        {
            InitializeComponent();
        }
        IScheduler scheduler;

        protected override void OnStart(string[] args)
        {
            try
            {
                JobsConfigurationSection jobsSection = ConfigurationManager.GetSection("jobs") as JobsConfigurationSection;
                if (jobsSection.Items.Count > 0)
                {
                    ISchedulerFactory sf = new StdSchedulerFactory();
                    scheduler = sf.GetScheduler();

                    foreach (Job jobConfiguration in jobsSection.Items)
                    {
                        if (jobConfiguration.Enabled)
                        {
                            JobDetail job = new JobDetail(jobConfiguration.Name, Type.GetType(jobConfiguration.Type, true));
                            Trigger trigger = GetTrigger(jobConfiguration);
                            scheduler.ScheduleJob(job, trigger);

                            QuartzDemoHelper.WriteLogs(string.Format("{0}服务启动成功", jobConfiguration.Name));
                        }
                    }
                    scheduler.Start();
                }
                QuartzDemoHelper.WriteLogs("QuartzDemoService服务启动成功");
            }
            catch (Exception ex)
            {
                QuartzDemoHelper.WriteLogs(string.Format("QuartzDemoService服务启动失败, error:{0}", ex.ToString()));
                throw new Exception("QuartzDemoService服务启动失败", ex);
            }
        }

        protected override void OnStop()
        {
            if (scheduler != null)
            {
                scheduler.Shutdown(false);
            }
        }

        private Trigger GetTrigger(Job jobConfiguration)
        {
            if (string.Equals(jobConfiguration.Trigger, "simple", StringComparison.CurrentCultureIgnoreCase))
            {
                int seconds;
                if (int.TryParse(jobConfiguration.ScheduleExp, out seconds))
                {
                    //简单的trigger
                    return new SimpleTrigger("Trigger_" + jobConfiguration.Name,
                        DateTime.UtcNow.AddSeconds(10), //延迟多少秒开始
                        null,
                        SimpleTrigger.RepeatIndefinitely,
                        TimeSpan.FromSeconds(seconds));
                }
                else
                {
                    throw new ConfigurationErrorsException("scheduleExp属性的配置值无效,应为表示间隔秒数的整数值：" + jobConfiguration.ScheduleExp);
                }
            }
            else if (string.Equals(jobConfiguration.Trigger, "cron", StringComparison.CurrentCultureIgnoreCase))
            {
                //CronTrigger
                CronTrigger trigger = new CronTrigger("Trigger_" + jobConfiguration.Name);
                trigger.CronExpressionString = jobConfiguration.ScheduleExp;
                return trigger;
            }
            throw new ConfigurationErrorsException("trigger属性的配置值无效：" + jobConfiguration.Trigger);
        }
    }
}
