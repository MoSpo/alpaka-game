using System;
using System.IO;
using System.Net.Sockets;

namespace Alpaka.Scenes.Battle {
	public class BattleNetwork {

		private TcpClient client;
		private string ip;
		private int port;
		private byte[] sentOfflineServerMessage;
		private byte[] recievedOfflineServerMessage;


		public BattleNetwork(string ip, int port) {
			this.ip = ip;
			this.port = port;
		}

		public void StartOnlineClient() {
			try {
				client = new TcpClient();
				client.Connect(ip, port);
			} catch (Exception e) {
				Console.WriteLine("TCP Error..... " + e.StackTrace);
			}
		}

		public void StartOfflineClient() {
			client = null;
		}

		public byte[] ReadAsOfflineServer() {
			return sentOfflineServerMessage;
		}

		public void SendAsOfflineServer(byte[] byteStream) {
			recievedOfflineServerMessage = byteStream;
		}

		public void CloseClient() {
			client.Close();
		}


		public void SendServer(byte[] byteStream) {
			if (client == null) {
				sentOfflineServerMessage = byteStream;
			} else {
				if (IsConnected) {
					Stream stream = client.GetStream();
					stream.Write(byteStream, 0, byteStream.Length);
				}
			}
		}

		public byte[] ReadServer() {
			if (client == null) {
				return recievedOfflineServerMessage;
			} else {
				if (IsConnected) {
					Stream stream = client.GetStream();
					byte[] temp = new byte[100];
					int packageSize = stream.Read(temp, 0, 100);
					byte[] byteStream = new byte[packageSize];
					for (int i = 0; i < packageSize; i++) {
						byteStream[i] = temp[i];
					}
					return byteStream;
				}
				return null;
			}
		}

		public bool IsConnected {
			get {
				try {
					if (client.Client != null && client.Client.Connected) {
						/* pear to the documentation on Poll:
						 * When passing SelectMode.SelectRead as a parameter to the Poll method it will return 
						 * -either- true if Socket.Listen(Int32) has been called and a connection is pending;
						 * -or- true if data is available for reading; 
						 * -or- true if the connection has been closed, reset, or terminated; 
						 * otherwise, returns false
						 */

						// Detect if client disconnected
						if (client.Client.Poll(0, SelectMode.SelectRead)) {
							byte[] buff = new byte[1];
							if (client.Client.Receive(buff, SocketFlags.Peek) == 0) {
								// Client disconnected
								return false;
							} else {
								return true;
							}
						}
						return true;
					} else {
						return false;
					}
				} catch {
					return false;
				}
			}
		}

	}
}
