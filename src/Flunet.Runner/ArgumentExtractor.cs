using System;
using System.Linq;

namespace Flunet.Runner
{
    public static class ArgumentExtractor
    {
        public static string GetArgument(this string[] args, params string[] aliases)
        {
            string result =
                args.FirstOrDefault
                (argument => aliases.Any(alias => argument.StartsWith
                                                      (alias,
                                                       StringComparison.InvariantCultureIgnoreCase)));

            if (result != null)
            {
                result = result.Substring(result.IndexOf(":") + 1);
                return result.Trim('"');
            }

            return null;
        }
    }
}