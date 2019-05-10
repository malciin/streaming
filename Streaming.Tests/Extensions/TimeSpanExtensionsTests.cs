using System;
using System.Collections.Generic;
using NUnit.Framework;
using Streaming.Common.Extensions;

namespace Streaming.Tests.Extensions
{
    public class TimeSpanExtensionsTests
    {
        #region CaseSources

        public static IEnumerable<TestIsWithinCase> TestIsWithinTimeSpanSource
        {
            get
            {
                yield return new TestIsWithinCase
                {
                    Value = TimeSpan.FromSeconds(1),
                    From = TimeSpan.FromSeconds(1),
                    To = TimeSpan.FromSeconds(2),
                    Result = true,
                    Description = "IsWithin should return true if Value == From"
                };

                yield return new TestIsWithinCase
                {
                    Value = TimeSpan.FromSeconds(2),
                    From = TimeSpan.FromSeconds(1),
                    To = TimeSpan.FromSeconds(2),
                    Result = true,
                    Description = "IsWithin should return true if Value == To"
                };

                yield return new TestIsWithinCase
                {
                    Value = TimeSpan.FromSeconds(1.5),
                    From = TimeSpan.FromSeconds(1),
                    To = TimeSpan.FromSeconds(2),
                    Result = true
                };
                
                yield return new TestIsWithinCase
                {
                    Value = TimeSpan.FromSeconds(3),
                    From = TimeSpan.FromSeconds(1),
                    To = TimeSpan.FromSeconds(2),
                    Result = false
                };

                yield return new TestIsWithinCase
                {
                    Value = TimeSpan.FromSeconds(1.5),
                    From = TimeSpan.FromSeconds(2),
                    To = TimeSpan.FromSeconds(1),
                    Result = false,
                    Description = "IsWithin should return false if From > To"
                };
            }
        }

        public static IEnumerable<TestEqualWithErrorCase> TestEqualWithErrorTimeSpanSource
        {
            get
            {
                yield return new TestEqualWithErrorCase
                {
                    Value = TimeSpan.FromSeconds(1),
                    Expected = TimeSpan.FromSeconds(2),
                    MaxError = TimeSpan.FromSeconds(1),
                    Result = true
                };
                
                yield return new TestEqualWithErrorCase
                {
                    Value = TimeSpan.FromSeconds(5),
                    Expected = TimeSpan.FromSeconds(7),
                    MaxError = TimeSpan.FromSeconds(1),
                    Result = false
                };
                
                yield return new TestEqualWithErrorCase
                {
                    Value = TimeSpan.FromSeconds(5),
                    Expected = TimeSpan.FromSeconds(4),
                    MaxError = TimeSpan.FromSeconds(1),
                    Result = true
                };
                
                yield return new TestEqualWithErrorCase
                {
                    Value = TimeSpan.FromSeconds(5),
                    Expected = TimeSpan.FromSeconds(5.5),
                    MaxError = TimeSpan.FromSeconds(0.5),
                    Result = true
                };
                
                yield return new TestEqualWithErrorCase
                {
                    Value = TimeSpan.FromSeconds(5),
                    Expected = TimeSpan.FromSeconds(6),
                    MaxError = TimeSpan.FromSeconds(0.5),
                    Result = false
                };
                
                yield return new TestEqualWithErrorCase
                {
                    Value = TimeSpan.FromSeconds(5),
                    Expected = TimeSpan.FromSeconds(5.01),
                    MaxError = TimeSpan.FromSeconds(0),
                    Result = false
                };
            }
        }
        
        #endregion

        [Test, TestCaseSource(nameof(TestIsWithinTimeSpanSource))]
        public void IsWithin_Tests(TestIsWithinCase input)
        {
            Assert.AreEqual(input.Result, input.Value.IsWithin(input.From, input.To), input.Description ??
                $"For {input.Value} and range [{input.From};{input.To}] expected result is {input.Result} but was {!input.Result}");
        }
        
        [Test, TestCaseSource(nameof(TestEqualWithErrorTimeSpanSource))]
        public void IsWithin_Tests(TestEqualWithErrorCase input)
        {
            Assert.AreEqual(input.Result, input.Value.EqualWithError(input.Expected, input.MaxError),
                $"For {input.Value} and expected timespan '{input.Expected}' with max error '{input.MaxError}' " +
                $"expected result is {input.Result} but was {!input.Result}");
        }

        public class TestIsWithinCase
        {
            public TimeSpan Value { get; set; }
            public TimeSpan From { get; set; }
            public TimeSpan To { get; set; }
            public bool Result { get; set; }
            public string Description { get; set; }
        }

        public class TestEqualWithErrorCase
        {
            public TimeSpan Value { get; set; }
            public TimeSpan Expected { get; set; }
            public TimeSpan MaxError { get; set; }
            public bool Result { get; set; }
        }
    }
}