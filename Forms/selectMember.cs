﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using IBMiCmd.IBMiTools;

namespace IBMiCmd.Forms
{
    public partial class selectMember : Form
    {
        public selectMember()
        {
            InitializeComponent();
            UpdateSystemName();
        }

        public void UpdateSystemName()
        {
            toolStripLabel1.Text = IBMi.GetConfig("system");
            listView1.Clear();
        }

        private List<ListViewItem> curItems = new List<ListViewItem>();

        public void UpdateListing(string Lib, string Obj)
        {
            Thread gothread = new Thread((ThreadStart)delegate {
                string[] members;
                ListViewItem curItem;

                curItems.Clear();
                listView1.Items.Clear();

                listView1.Items.Add(new ListViewItem("Loading...", 2));

                members = IBMiUtilities.GetMemberList(Lib, Obj);

                listView1.Items.Clear();
                if (members != null)
                {
                    foreach (string member in members)
                    {
                        curItem = new ListViewItem(Path.GetFileNameWithoutExtension(member), 0);
                        curItem.Tag = Lib + '.' + Obj;

                        curItems.Add(curItem);
                    }

                    listView1.Items.AddRange(curItems.ToArray());
                }
                else
                {
                    listView1.Items.Add(new ListViewItem("No members found!", 1));
                    if (Main.CommandWindow != null) Main.CommandWindow.loadNewCommands();
                }
            });
            gothread.Start();
        }
 
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (!IBMiUtilities.IsValidQSYSObjectName(toolStripTextBox1.Text))
            {
                MessageBox.Show("Library name is not valid.");
                return;
            }
            if (!IBMiUtilities.IsValidQSYSObjectName(toolStripTextBox2.Text))
            {
                MessageBox.Show("Object name is not valid.");
                return;
            }

            UpdateListing(toolStripTextBox1.Text, toolStripTextBox2.Text);
        }
    }
}