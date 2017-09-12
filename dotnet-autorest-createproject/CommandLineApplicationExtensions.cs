using System;
using Microsoft.Extensions.CommandLineUtils;

namespace ESW.autorest.createProject
{
    public static class CommandLineApplicationExtensions
    {
        /// <summary>
        /// Generic invoke <see cref="Action"/> callback that simply invokes and returns 0.
        /// </summary>
        /// <param name="command">The <see cref="CommandLineApplication"/> that we are setting up.</param>
        /// <param name="invoke">The setup <see cref="Action"/> delegate we are invoking.</param>
        public static void OnExecute(this CommandLineApplication command, Action invoke)
            => command.OnExecute(
                () =>
                {
                    invoke();
                    return 0;
                });
    }
}
