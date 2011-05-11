﻿using System;
using Coypu.Robustness;
using NUnit.Framework;

namespace Coypu.Tests.When_making_browser_interactions_robust
{
	[TestFixture]
	public class When_querying_for_an_expected_result
	{
		private WaitAndRetryRobustWrapper waitAndRetryRobustWrapper;

		[SetUp]
		public void SetUp()
		{
			Configuration.Timeout = TimeSpan.FromMilliseconds(200);
			Configuration.RetryInterval = TimeSpan.FromMilliseconds(10);
			waitAndRetryRobustWrapper = new WaitAndRetryRobustWrapper();
		}

		[Test]
		public void When_the_expected_result_is_found_It_should_return_the_expected_result_immediately()
		{
			var expectedResult = new object();
			Func<object> returnsTrueImmediately = () => expectedResult;

			var actualResult = waitAndRetryRobustWrapper.Query(returnsTrueImmediately, expectedResult);

			Assert.That(actualResult, Is.EqualTo(expectedResult));
		}

		[Test]
		public void When_the_expected_result_is_never_found_It_should_return_the_actual_result_after_timeout()
		{
			var expectedTimeout = TimeSpan.FromMilliseconds(200);
			Configuration.Timeout = expectedTimeout;
			Configuration.RetryInterval = TimeSpan.FromMilliseconds(10);

			var expectedResult = new object();
			var unexpectedResult = new object();

			Func<object> returnsUnexpectedResult = () => unexpectedResult;

			var startTime = DateTime.Now;
			var actualResult = waitAndRetryRobustWrapper.Query(returnsUnexpectedResult, expectedResult);
			var endTime = DateTime.Now;

			Assert.That(actualResult, Is.EqualTo(unexpectedResult));

			var actualDuration = (endTime - startTime);
			var discrepancy = TimeSpan.FromMilliseconds(50);
			Assert.That(actualDuration, Is.InRange(expectedTimeout, expectedTimeout + discrepancy));
		}

		[Test]
		public void When_exceptions_are_always_thrown_It_should_rethrow_after_timeout()
		{
			Func<bool> alwaysThrows = () => { throw new ExplicitlyThrownTestException("This query always errors"); };

			Assert.Throws<ExplicitlyThrownTestException>(() => waitAndRetryRobustWrapper.Query(alwaysThrows, true));
		}

		[Test]
		public void When_exceptions_are_thrown_It_should_retry_And_when_expected_result_found_subsequently_It_should_return_expected_result_immediately()
		{
			var tries = 0;
			var expectedResult = new object();
			Func<object> throwsFirstTimeThenReturnsExpectedResult =
				() =>
					{
						tries++;
						if (tries < 3)
						{
							throw new ExplicitlyThrownTestException("This query always errors");
						}
						return expectedResult;
					};

			Assert.That(waitAndRetryRobustWrapper.Query(throwsFirstTimeThenReturnsExpectedResult, expectedResult), Is.EqualTo(expectedResult));
			Assert.That(tries, Is.EqualTo(3));
		}

        [Test]
        public void When_a_not_supported_exception_is_thrown_It_should_not_retry()
        {
            var tries = 0;
            Func<bool> throwsNotSupported =
                () =>
                {
                    tries++;
                    throw new NotSupportedException("This query always errors");
                };

            Assert.Throws<NotSupportedException>(() => waitAndRetryRobustWrapper.Query(throwsNotSupported, true));
            Assert.That(tries, Is.EqualTo(1));
        }

		[Test]
		public void When_exceptions_are_thrown_It_should_retry_And_when_unexpected_result_found_subsequently_It_should_return_unexpected_result_after_timeout()
		{
			var expectedTimeout = TimeSpan.FromMilliseconds(250);
			Configuration.Timeout = expectedTimeout;
			Configuration.RetryInterval = TimeSpan.FromMilliseconds(10);

			var expectedResult = new object();
			var unexpectedResult = new object();
			var tries = 0;
			Func<object> throwsFirstTimeThenReturnOppositeResult =
				() =>
					{
						tries++;
						if (tries < 3)
						{
							throw new ExplicitlyThrownTestException("This query always errors");
						}
						return unexpectedResult;
					};

			Assert.That(waitAndRetryRobustWrapper.Query(throwsFirstTimeThenReturnOppositeResult, expectedResult), Is.EqualTo(unexpectedResult));
			Assert.That(tries, Is.InRange(4,27));
		}
	}
}