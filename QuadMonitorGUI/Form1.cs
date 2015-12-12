using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuadMonitorGUI
{
	public partial class Form1 : Form
	{
		QuadInterface mQuadInterface;
		Timer mTimer;

		public Form1()
		{
			InitializeComponent();

			mQuadInterface = new QuadInterface();

			mTimer = new Timer();
			mTimer.Interval = 50;
			mTimer.Tick += mTimer_Tick;
			mTimer.Start();
		}

		void mTimer_Tick(object sender, EventArgs e)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("Throttle: ");
			sb.AppendLine(mQuadInterface.Receiver[0].ToString());
			sb.Append("Elevation: ");
			sb.AppendLine(mQuadInterface.Receiver[1].ToString());
			sb.Append("Roll: ");
			sb.AppendLine(mQuadInterface.Receiver[2].ToString());
			sb.Append("Yaw: ");
			sb.AppendLine(mQuadInterface.Receiver[3].ToString());

			sb.Append("M1: ");
			sb.AppendLine(mQuadInterface.Motors[0].ToString());
			sb.Append("M2: ");
			sb.AppendLine(mQuadInterface.Motors[1].ToString());
			sb.Append("M3: ");
			sb.AppendLine(mQuadInterface.Motors[2].ToString());
			sb.Append("M4: ");
			sb.AppendLine(mQuadInterface.Motors[3].ToString());

			sb.Append("Roll: ");
			sb.AppendLine(mQuadInterface.Rotation[0].ToString());
			sb.Append("Elev: ");
			sb.AppendLine(mQuadInterface.Rotation[1].ToString());
			sb.Append("Yaw: ");
			sb.AppendLine(mQuadInterface.Rotation[2].ToString());

			sb.Append("PID Angle Kp: ");
			sb.AppendLine(mQuadInterface.PIDAngleGains[0].ToString());
			sb.Append("PID Angle Ki: ");
			sb.AppendLine(mQuadInterface.PIDAngleGains[1].ToString());
			sb.Append("PID Angle Kd: ");
			sb.AppendLine(mQuadInterface.PIDAngleGains[2].ToString());

			sb.Append("PID Rate Kp: ");
			sb.AppendLine(mQuadInterface.PIDRateGains[0].ToString());
			sb.Append("PID Rate Ki: ");
			sb.AppendLine(mQuadInterface.PIDRateGains[1].ToString());
			sb.Append("PID Rate Kd: ");
			sb.AppendLine(mQuadInterface.PIDRateGains[2].ToString());

			tbResults.Invoke( (MethodInvoker) delegate
			{
				tbResults.Text = sb.ToString();
			});
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			mQuadInterface.Dispose();
		}

		private float Convert(ScrollEventArgs e)
		{
			return ((float)(e.NewValue) / 91F)*2;
		}

		private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
		{
			mQuadInterface.PIDAngleGains[0] = Convert(e);
		}

		private void vScrollBar2_Scroll(object sender, ScrollEventArgs e)
		{
			mQuadInterface.PIDAngleGains[1] = Convert(e);
		}

		private void vScrollBar3_Scroll(object sender, ScrollEventArgs e)
		{
			mQuadInterface.PIDAngleGains[2] = Convert(e)/10;
		}

		private void vScrollBar6_Scroll(object sender, ScrollEventArgs e)
		{
			mQuadInterface.PIDRateGains[0] = Convert(e);
		}

		private void vScrollBar5_Scroll(object sender, ScrollEventArgs e)
		{
			mQuadInterface.PIDRateGains[1] = Convert(e);
		}

		private void vScrollBar4_Scroll(object sender, ScrollEventArgs e)
		{
			mQuadInterface.PIDRateGains[2] = Convert(e)/10;
		}
	}
}
