using System;
using System.Collections.Generic;
using NUnit.Framework;
using Streaming.Common.Extensions;

namespace Streaming.Tests.Extensions
{
    public class DateTimeExtensionsTests
    {
        public static IEnumerable<TestIsWithinCase> TestDatesSource
        {
            get
            {
                yield return new TestIsWithinCase
                {
                    Value = new DateTime(1970, 1, 1, 0, 0, 0),
                    From = new DateTime(1970, 1, 1, 0, 0, 0),
                    To = new DateTime(1970, 1, 1, 0, 0, 1),
                    Result = true,
                    Description = "IsWithin should return true if Value == From"
                };

                yield return new TestIsWithinCase
                {
                    Value = new DateTime(1970, 1, 1, 0, 0, 1),
                    From = new DateTime(1970, 1, 1, 0, 0, 0),
                    To = new DateTime(1970, 1, 1, 0, 0, 1),
                    Result = true,
                    Description = "IsWithin should return true if Value == To"
                };

                yield return new TestIsWithinCase
                {
                    Value = new DateTime(1970, 1, 1, 0, 0, 0),
                    From = new DateTime(1970, 1, 1, 0, 0, 1),
                    To = new DateTime(1970, 1, 1, 0, 0, 2),
                    Result = false
                };
                
                yield return new TestIsWithinCase
                {
                    Value = new DateTime(1970, 1, 1, 0, 0, 1),
                    From = new DateTime(1970, 1, 1, 0, 0, 0),
                    To = new DateTime(1970, 1, 1, 0, 0, 2),
                    Result = true
                };

                yield return new TestIsWithinCase
                {
                    Value = new DateTime(1971, 1, 1, 0, 0, 0),
                    From = new DateTime(1972, 1, 1, 0, 0, 1),
                    To = new DateTime(1970, 1, 1, 0, 0, 2),
                    Result = false,
                    Description = "IsWithin should return false if From > To"
                };
            }
        }
        
        [Test, TestCaseSource(nameof(TestDatesSource))]
        public void IsWithin_Tests(TestIsWithinCase input)
        {
            Assert.AreEqual(input.Result, input.Value.IsWithin(input.From, input.To), input.Description ?? 
                $"For {input.Value} and range [{input.From};{input.To}] expected result is {input.Result} but was {!input.Result}");
        }

        public class TestIsWithinCase
        {
            public DateTime Value { get; set; }
            public DateTime From { get; set; }
            public DateTime To { get; set; }
            public bool Result { get; set; }
            public string Description { get; set; }
        }
    }
}