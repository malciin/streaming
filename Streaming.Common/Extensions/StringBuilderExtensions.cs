using System;
using System.Text;

namespace Streaming.Common.Extensions
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder Prepend(this StringBuilder builder, string value)
        {
            builder.Insert(0, value);
            return builder;
        }
        public static StringBuilder PrependLine(this StringBuilder builder, string value)
            => builder.Prepend($"{value}{Environment.NewLine}");
    }
}
