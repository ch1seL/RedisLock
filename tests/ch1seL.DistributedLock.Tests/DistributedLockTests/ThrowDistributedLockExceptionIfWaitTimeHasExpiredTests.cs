﻿using System;
using System.Linq;
using System.Threading.Tasks;
using ch1seL.DistributedLock.Tests.Base;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Xunit;

namespace ch1seL.DistributedLock.Tests.DistributedLockTests
{
    public class ThrowDistributedLockExceptionIfWaitTimeHasExpiredTests : LockTestsBase
    {
        [Theory]
        [MemberData(nameof(TestsData.LockServiceTypes),
            MemberType = typeof(TestsData))]
        public async Task Test(Type lockServiceType)
        {
            Init(TestsData.RegistrationByServiceType[lockServiceType]);

            Func<Task> act = () => Task.WhenAll(
                Enumerable.Repeat((object) null, 10)
                    .Select(_ => AddIntervalTaskWithLock(TimeSpan.FromMilliseconds(10), TimeSpan.FromSeconds(10))));

            var exception = await act.Should().ThrowExactlyAsync<DistributedLockException>();
            exception.Which.Status.Should().Be(DistributedLockBadStatus.Conflicted);
            Intervals.Should().HaveCount(1);
        }
    }
}