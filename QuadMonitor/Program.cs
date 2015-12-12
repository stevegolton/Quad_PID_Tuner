using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuadMonitor
{
	class Program
	{
		static bool mQuitThreadFlag = false;
		static SerialPort mSerialPort;
		static byte[] buf = new byte[256];

		static byte sig1 = 0;
		static byte sig2 = 1;

		static Single[] afGainsAngle = new Single[3];
		static Single[] afGainsRate = new Single[3];

		static void Main(string[] args)
		{
			StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
			string command;
			Thread readThread = new Thread(Read);
			bool running = true;

			readThread.Start();

			while (running)
			{
				try
				{
					Thread.Sleep(50);
					command = Console.ReadLine();
					command = "";

					if (0 == stringComparer.Compare(command, "quit"))
					{
						running = false;
					}
					else
					{
						mSerialPort.WriteLine(command);
					}
				}
				catch(System.InvalidOperationException)
				{
					Console.WriteLine("Unable to connect to COM port!");
				}
				catch(System.IO.IOException)
				{
					Console.WriteLine("Unable to connect to COM port!");
				}
			}

			mQuitThreadFlag = true;
			mSerialPort.Close();
			readThread.Join();
		}

		public static void Read()
		{
			bool running;

			while (mQuitThreadFlag == false)
			{
				try
				{
					// Create a new SerialPort object with default settings.
					mSerialPort = new SerialPort();

					// Allow the user to set the appropriate properties.
					mSerialPort.PortName = "COM4";
					mSerialPort.BaudRate = 9600;
					//serialPort.Parity = SetPortParity(_serialPort.Parity);
					//serialPort.DataBits = SetPortDataBits(_serialPort.DataBits);
					//serialPort.StopBits = SetPortStopBits(_serialPort.StopBits);
					//serialPort.Handshake = SetPortHandshake(_serialPort.Handshake);

					// Set the read/write timeouts
					mSerialPort.ReadTimeout = 500;
					mSerialPort.WriteTimeout = 500;

					mSerialPort.Open();

					while (true)
					{
						try
						{
							ReadDaTing(mSerialPort);

							//byte message = (byte)mSerialPort.ReadByte();
							//Console.WriteLine(message);
						}
						catch (TimeoutException) { }
					}
				}
				catch (System.IO.IOException)
				{

				}
			}
		}

		public static void WriteCfgData(SerialPort sp)
		{
			byte[] buf = new byte[sizeof(float) * 6];
			MemoryStream ms = new MemoryStream(buf);
			BinaryWriter writer = new BinaryWriter(ms);

			writer.Write(afGainsAngle[0]);
			writer.Write(afGainsAngle[1]);
			writer.Write(afGainsAngle[2]);

			writer.Write(afGainsRate[0]);
			writer.Write(afGainsRate[1]);
			writer.Write(afGainsRate[2]);

			sp.Write(buf, 0, buf.Length);
		}

		public static void ReadDaTing(SerialPort sp)
		{
			int length;
			int ctr = 0;
			byte[] buf = new byte[256];
			MemoryStream ms = new MemoryStream(buf);
			BinaryReader reader = new BinaryReader(ms);

			sig2 = (byte)sp.ReadByte();

			//Console.WriteLine(sig2);

			if ((sig1 == 0xA5) && (sig2 == 0xA5))
			{
				// Message time
				sig1 = 0;
				sig2 = 0;

				length = sp.ReadByte();
				length += sp.ReadByte() << 8;

				while (ctr < length)
				{
					ctr += sp.Read(buf, ctr, length);
				}

				Console.WriteLine("YEAHH!" + length);
				for(int i = 0; i < length; i++)
				{
					Console.Write("{0:X} ", buf[i]);
				}
				Console.WriteLine();

				Console.WriteLine(reader.ReadUInt16());
				Console.WriteLine(reader.ReadUInt16());
				Console.WriteLine(reader.ReadUInt16());
				Console.WriteLine(reader.ReadUInt16());

				Console.WriteLine(reader.ReadSingle());
				Console.WriteLine(reader.ReadSingle());
				Console.WriteLine(reader.ReadSingle());
				Console.WriteLine(reader.ReadSingle());

				Console.WriteLine(reader.ReadSingle());
				Console.WriteLine(reader.ReadSingle());
				Console.WriteLine(reader.ReadSingle());

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
