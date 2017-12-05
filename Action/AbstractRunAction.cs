using System;
using System.Collections.Generic;

namespace TrayApplication.Action
{
    public abstract class AbstractRunAction : AbstractAction
    {
        /// <summary>
        /// Flag for ignore execution error
        /// </summary>
        protected bool ignore_errors = false;

        /// <summary>
        /// Flag for wait execution completed
        /// </summary>
        protected bool wait_until_terminated = false;

        protected AbstractRunAction(IDictionary<string, object> arguments) : base(arguments)
        {}

        public override void Execute(object sender, EventArgs e)
        {
            if (ignore_errors)
            {
                try
                {
                    _doExecute();
                }
                catch (System.Exception exception)
                {
                    Console.WriteLine(exception);//TODO log this insted of console
                    throw;
                }
            }
            else
            {
                _doExecute();
            }
        }

        protected abstract void _doExecute();
    }
}