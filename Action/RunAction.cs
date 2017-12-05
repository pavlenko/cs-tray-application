using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TrayApplication.Action
{
    public class RunAction : AbstractRunAction
    {
        protected string bin;

        protected string cwd;

        protected string arguments;

        private readonly Process _process;

        public RunAction(IDictionary<string, object> arguments) : base(arguments)
        {
            _process = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = cwd,
                    FileName         = bin,
                    Arguments        = this.arguments
                }
            };
        }

        protected override void _doExecute()
        {
            _process.Start();
            if (wait_until_terminated)
            {
                _process.WaitForExit();
            }
        }
    }
}