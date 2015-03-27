#region Copyright (C) Tekla Corporation 2007
//
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written permission of the copyright owner.
//
// Filename: Program.cs
//
#endregion

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Tekla.UI.HyperToolTips
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Form1 form1 = new Form1();

            Application.Run(form1);
        }
    }
}