﻿using System;
using System.Linq;
using System.Threading.Tasks;
using AElf.Common;
using AElf.Kernel.Consensus.Application;
using AElf.Sdk.CSharp;
using FluentScheduler;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Local;

namespace AElf.Kernel.Consensus.Scheduler.FluentScheduler
{
    public class FluentSchedulerScheduler : IConsensusScheduler, ISingletonDependency
    {
        public ILocalEventBus LocalEventBus { get; set; }

        public ILogger Logger { get; set; }

        public FluentSchedulerScheduler()
        {
            LocalEventBus = NullLocalEventBus.Instance;

            Logger = NullLogger<FluentSchedulerScheduler>.Instance;
        }

        public void NewEvent(int countingMilliseconds, ConsensusRequestMiningEventData consensusRequestMiningEventData)
        {
            JobManager.UseUtcTime();

            var registry = new Registry();
            registry.Schedule(() => LocalEventBus.PublishAsync(consensusRequestMiningEventData))
                .ToRunOnceAt(TimestampHelper.GetUtcNow().AddMilliseconds(countingMilliseconds).ToDateTime());
            JobManager.Initialize(registry);
        }

        public void CancelCurrentEvent()
        {
            if (JobManager.RunningSchedules.Any())
            {
                JobManager.Stop();
            }
        }
    }
}