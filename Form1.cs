using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Calculator
{
	public partial class Form1 : Form
	{
		readonly int ext;
		public Form1()
		{
			InitializeComponent();
			ext = Height - 114;
			Height -= ext;
		}
		private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar != '\r') return;
			button1_Click(null, null);
		}
		private void button1_Click(object sender, System.EventArgs e)
		{
			string s = textBox1.Text;
			textBox2.Text = "Error";
			listBox1.Items.Clear();
			Translate tr = new Translate();
			tr.Trace += delegate(string before, string after) { listBox1.Items.Add(before); };
			try { s = tr.Eval(s); }
			catch { }
			textBox2.Text = s;
		}
		private void button2_Click(object sender, System.EventArgs e)
		{
			if (Height < ext)
			{
				Height += ext;
				((Button)sender).Text = "<<";
			}
			else
			{
				Height -= ext;
				((Button)sender).Text = ">>";
			}
		}
	}
}
