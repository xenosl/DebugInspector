namespace ShuHai.DebugInspector.Editor
{
    internal static class MessageBuilder
    {
        public static string BuildValueAccessError(ValueAccessResult? valueGetResult, ValueAccessResult? valueSetResult)
        {
            var builder = new IndentedStringBuilder();

            if (valueGetResult != null && !valueGetResult.Value)
            {
                builder.AppendLine("Get:");
                using (new IndentedStringBuilder.Scope(builder))
                    valueGetResult.Value.BuildFailString(builder);
            }

            if (builder.Length > 0)
                builder.AppendLine();

            if (valueSetResult != null && !valueSetResult.Value)
            {
                builder.AppendLine("Set:");
                using (new IndentedStringBuilder.Scope(builder))
                    valueSetResult.Value.BuildFailString(builder);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Build a readable string that contains error logs.
        /// </summary>
        /// <param name="builder"> The <see cref="IndentedStringBuilder"/> used to build the string. </param>
        /// <returns>
        /// <see langword="true"/> if the string successfully built; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool BuildErrorLogString(this ValueAccessResult self, IndentedStringBuilder builder)
        {
            Ensure.Argument.NotNull(builder, "builder");

            if (!self.HasErrorLog)
                return false;

            builder.Append("Error Logs:\n");
            using (new IndentedStringBuilder.Scope(builder))
            {
                foreach (var log in self.ErrorLogs)
                    builder.AppendLine(log.ToString());
                builder.Builder.RemoveTailLineFeed();
            }

            return true;
        }

        public static bool BuildExceptionString(this ValueAccessResult self, IndentedStringBuilder builder)
        {
            Ensure.Argument.NotNull(builder, "builder");

            if (self.Exception == null)
                return false;

            builder.AppendLine("Exception:");
            using (new IndentedStringBuilder.Scope(builder))
                builder.Append(self.Exception.Message);

            return true;
        }

        /// <summary>
        /// Build a readable string that contains error logs and exception message.
        /// </summary>
        /// <param name="builder"> The <see cref="IndentedStringBuilder"/> used to build the string. </param>
        /// <returns>
        /// <see langword="true"/> if the string successfully built; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool BuildFailString(this ValueAccessResult self, IndentedStringBuilder builder)
        {
            Ensure.Argument.NotNull(builder, "builder");

            if (self.Succeed)
                return false;

            var errLogBuilt = BuildErrorLogString(self, builder);

            if (self.Exception != null)
            {
                if (errLogBuilt)
                    builder.AppendLine();
                BuildExceptionString(self, builder);
            }

            return true;
        }
    }
}