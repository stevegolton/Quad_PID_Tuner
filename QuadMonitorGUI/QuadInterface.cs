using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Net.Sockets;

namespace QuadMonitorGUI
{
	class QuadInterface : IDisposable
	{
		//private Stream mTxprtStream;
		private Thread mThread;
		private TcpClient mTcpClient;
		private bool mCancelThread;

		public float[] PIDRateGains;
		public float[] PIDAngleGains;

		public ushort[] Receiver;
		public float[] Motors;
		public float[] Rotation;
		private byte sig1;
		private byte sig2;

		public QuadInterface()
		{
			//mTxprtStream = strm;

			mThread = new Thread(BackgroundThread);
			mCancelThread = false;

			PIDRateGains = new float[3];
			PIDAngleGains = new float[3];

			Receiver = new ushort[4];
			Motors = new float[4];
			Rotation = new float[3];

			mThread.Start();
		}
	
		public void Dispose()
		{
			mCancelThread = true;
			mThread.Join();
		}

		public void BackgroundThread(object crap)
		{
			while(!mCancelThread)
			{
				try
				{
					// Create a new SerialPort object with default settings.
					mTcpClient = new TcpClient();

					Console.Write("Connecting...");
					mTcpClient.Connect("192.168.4.1", 14550);
					Console.WriteLine("OK");

					while (!mCancelThread)
					{
						ReadDaTing(mTcpClient);
						//PIDRateGains[0] += 1.0F;
					}
				}
				catch (SocketException)
				{
					Thread.Sleep(10);
				}
				catch (IOException)
				{
					Thread.Sleep(10);
				}
			}
		}

		public void WriteCfgData(TcpClient sp)
		{
			byte[] buf = new byte[2 + sizeof(float) * 6];
			MemoryStream ms = new MemoryStream(buf);
			BinaryWriter writer = new BinaryWriter(ms);

			// Write header
			writer.Write((byte)0xA5);
			writer.Write((byte)0xA5);

			writer.Write(PIDAngleGains[0]);
			writer.Write(PIDAngleGains[1]);
			writer.Write(PIDAngleGains[2]);

			writer.Write(PIDRateGains[0]);
			writer.Write(PIDRateGains[1]);
			writer.Write(PIDRateGains[2]);

			sp.GetStream().Write(buf, 0, buf.Length);
		}

		public void ReadDaTing(TcpClient sp)
		{
			int length;
			int ctr = 0;
			byte[] buf = new byte[256];
			MemoryStream ms = new MemoryStream(buf);
			BinaryReader reader = new BinaryReader(ms);

			sig2 = (byte)sp.GetStream().ReadByte();

			//Console.WriteLine(sig2);

			if ((sig1 == 0xA5) && (sig2 == 0xA5))
			{
				// Message time
				sig1 = 0;
				sig2 = 0;

				length = sp.GetStream().ReadByte();
				length += sp.GetStream().ReadByte() << 8;

				if (length > 256)
				{
					return;
				}

				while (ctr < length)
				{
					ctr += sp.GetStream().Read(buf, ctr, length);
				}

				/*
				Console.WriteLine("YEAHH!" + length);
				for (int i = 0; i < length; i++)
				{
					Console.Write("{0:X} ", buf[i]);
				}
				Console.WriteLine();
				 * */

				Receiver[0] = reader.ReadUInt16();
				Receiver[1] = reader.ReadUInt16();
				Receiver[2] = reader.ReadUInt16();
				Receiver[3] = reader.ReadUInt16();

				Motors[0] = reader.ReadSingle();
				Motors[1] = reader.ReadSingle();
				Motors[2] = reader.ReadSingle();
				Motors[3] = reader.ReadSingle();

				Rotation[0] = reader.ReadSingle();
				Rotation[1] = reader.ReadSingle();
				Rotation[2] = reader.ReadSingle();

				Console.WriteLine(reader.ReadSingle());
				Console.WriteLine(reader.ReadSingle());
				Console.WriteLine(reader.ReadSingle());

				Console.WriteLine(reader.ReadSingle());
				Console.WriteLine(reader.ReadSingle());
				Console.WriteLine(reader.ReadSingle());

				WriteCfgData(sp);

				//Console.WriteLine("Message! " + length);
			}
			else
			{
				sig1 = sig2;
			}
		}
	}
}
