using System;
using System.Threading.Tasks;

namespace Quartz.NET.Demo.Jobs
{
    public class HelloJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var jobKey = context.JobDetail.Key;

            await Console.Out.WriteLineAsync($"Greetings from HelloJob {jobKey} at {DateTime.Now.ToLocalTime()}!");
        }
    }
}