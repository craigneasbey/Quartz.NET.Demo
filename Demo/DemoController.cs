
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Quartz.Spi;

namespace Quartz.NET.Demo.Demo
{
    public class DemoController : Controller
    {
        private readonly  ISchedulerFactory _schedulerFactory;
        private readonly  IJobFactory _jobFactory;

        public DemoController(
            ISchedulerFactory schedulerFactory, 
            IJobFactory jobFactory
        ) {
            _schedulerFactory = schedulerFactory;
            _jobFactory = jobFactory;
        }

        public async Task<string> Index(int id) {
            Console.Out.WriteLine("/Demo/Index");

            var scheduler = await GetScheduler();
            var jobKey = await ScheduleSimpleTrigger(scheduler);
            await RescheduleSimpleTrigger(scheduler, jobKey);
            
            return "OK";
        }

        public async Task<IScheduler> GetScheduler() {
            var scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.Start();

            return scheduler;
        }

        public async Task<JobKey> ScheduleSimpleTrigger(IScheduler scheduler) {
                        // define the job and tie it to our HelloJob class
            var jobDetails = JobBuilder.Create<HelloJob>()
                .WithIdentity("myJob", "group1")
                .WithDescription("my awesome job")
                .Build();

            // Trigger the job to run in 10 seconds
            var triggerDateTime = DateTime.Now.AddSeconds(10);

            var trigger = TriggerBuilder.Create()
                .WithIdentity("myTrigger", "group1")
                .StartAt(triggerDateTime)
                .Build();

            System.Console.WriteLine($"Scheduling {jobDetails.Key} to run at {triggerDateTime.ToLocalTime()}");
                
            await scheduler.ScheduleJob(jobDetails, trigger);

            return jobDetails.Key;
        }

        public async Task RescheduleSimpleTrigger(IScheduler scheduler, JobKey jobKey) {

            var jobDetails = await scheduler.GetJobDetail(jobKey);
            var triggers = await scheduler.GetTriggersOfJob(jobDetails.Key);

            // If triggers not found
            if (triggers.Count == 0) {
                return;
            }

            var trigger = triggers.First();

            var newTriggerDateTime = DateTime.Now.AddSeconds(20);
            var newTrigger = TriggerBuilder.Create()
                .WithIdentity("myNewTrigger", "group1")
                .StartAt(newTriggerDateTime)
                .Build();

            System.Console.WriteLine($"Recheduling {jobDetails.Key} to run at {newTriggerDateTime.ToLocalTime()}");

            await scheduler.RescheduleJob(trigger.Key, newTrigger);
        }
    }
}

