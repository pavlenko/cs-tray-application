using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TrayApplication.Action
{
    public class ExitAction : AbstractAction
    {
        public ExitAction(IDictionary<string, object> arguments) : base(arguments)
        {}

        public override void Execute(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}