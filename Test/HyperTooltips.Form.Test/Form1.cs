#region Copyright (C) Tekla Corporation 2007
//
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written permission of the copyright owner.
//
// Filename: Form1.cs
//
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Tekla.UI.HyperToolTips
{
    public partial class Form1 : Form
    {                    
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //TipLoader.PopulateWithAllControls(this, hypertooltipsProvider1);
            checkBox2.Checked = hypertooltipsProvider1.Active;
            checkBox3.Checked = hypertooltipsProvider1.Debug;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            hypertooltipsProvider1.Active = checkBox2.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            hypertooltipsProvider1.Debug = checkBox3.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (Form2 form = new Form2())
            {
                form.ShowDialog();
            }

        }
    }
}