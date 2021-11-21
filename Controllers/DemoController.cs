
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Quartz.Spi;
using Quartz.NET.Demo.Jobs;
using Quartz.NET.Demo.Models;

namespace Quartz.NET.Demo.Controllers
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

        public async Task<IActionResult> Index(int id) {
            Console.Out.WriteLine("/Demo/Index");

            var scheduler = await GetScheduler();
            await DeleteExistingJob(scheduler);
            var jobKey = await ScheduleSimpleTrigger(scheduler);
            await RescheduleSimpleTrigger(scheduler, jobKey);
            await RescheduleCronTrigger(scheduler, jobKey);
            
            return View();
        }

        public async Task<IScheduler> GetScheduler() {
            var scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.Start();

            return scheduler;
        }

        public async Task DeleteExistingJob(IScheduler scheduler) {
            var jobKey = new JobKey("myJob", "group1");

            if (await scheduler.CheckExists(jobKey)) {
                System.Console.WriteLine($"Deleting job {jobKey.Name}");
                await scheduler.DeleteJob(jobKey);
            }
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

        public async Task RescheduleCronTrigger(IScheduler scheduler, JobKey jobKey) {

            var jobDetails = await scheduler.GetJobDetail(jobKey);
            var triggers = await scheduler.GetTriggersOfJob(jobDetails.Key);

            // If triggers not found
            if (triggers.Count == 0) {
                return;
            }

            var trigger = triggers.First();

            var expression = new CronExpression("0 26 13 17 10 ?");
            var nextTriggerDateTime = expression.GetTimeAfter(DateTime.Now);
            var newTrigger = TriggerBuilder.Create()
                .WithIdentity("myNewTrigger", "group1")
                .WithCronSchedule(expression.CronExpressionString)
                .Build();

            System.Console.WriteLine($"Recheduling {jobDetails.Key} to run at {nextTriggerDateTime?.ToLocalTime()}");

            await scheduler.RescheduleJob(trigger.Key, newTrigger);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

