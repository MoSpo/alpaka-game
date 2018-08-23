using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lidgren_server {

    class server {

        static NetServer Server;
        static NetPeerConfiguration Config;
        static NetConnection Player1Connection;
        static NetConnection Player2Connection;
        static bool Player1Ready;
        static bool Player2Ready;

        static List<NetConnection> WaitingPlayerConnections;
        static List<NetConnection> WaitingSpectatorConnections;

        static StreamWriter sw;

        static void Replacewithmain (string[] args) { // Main(string[] args) {

            string path = Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), @"log.txt");
            sw = new StreamWriter(path);

            Config = new NetPeerConfiguration("alpaka");
            Config.Port = 20000;
            Config.MaximumConnections = 200;

            Config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

            Server = new NetServer(Config);
            Server.Start();
            WriteLine("##########\r\nServer Started...");

            WaitingPlayerConnections = new List<NetConnection>();
            WaitingSpectatorConnections = new List<NetConnection>();

            Player1Ready = false;
            Player2Ready = false;

            NetIncomingMessage msg;

            while (true) {
                if ((msg = Server.ReadMessage()) != null) {

                    switch (msg.MessageType) {
                        case NetIncomingMessageType.ConnectionApproval:

                        if (msg.ReadByte() == (byte)PacketTypes.JOIN) {
                            WriteLine("Incoming JOIN");
                            byte b = msg.ReadByte();
                            if (b == 0) {
                                if (Player1Connection == null || Player2Connection == null) {
                                    msg.SenderConnection.Approve();
                                    WaitingPlayerConnections.Add(msg.SenderConnection);
                                    WriteLine("Pending new connection to: " + msg.ReadString());

                                } else {
                                    msg.SenderConnection.Deny();
                                    WriteLine("Denied connection to: " + msg.ReadString());
                                }
                            } else if (b == 1) {
                                msg.SenderConnection.Approve();
                                WaitingSpectatorConnections.Add(msg.SenderConnection);
                                WriteLine("Pending new connection to: " + msg.ReadString());

                            } else {
                                msg.SenderConnection.Deny();
                                WriteLine("Denied connection to: " + msg.ReadString());
                            }
                        }

                        break;

                        case NetIncomingMessageType.Data:

                        NetOutgoingMessage outmsg = Server.CreateMessage();
                        switch (msg.ReadByte()) {
                            case ((byte)PacketTypes.CHOICE): //Transmit of choice

                            outmsg.Write((byte)PacketTypes.CHOICE);
                            if (msg.SenderConnection == Player1Connection && !Player1Ready) {
                                WriteLine("Player1 Choice Recieved - Forwarding to Player2...");
                                outmsg.Write(msg.ReadByte());
                                outmsg.Write(msg.ReadByte());
                                outmsg.Write(msg.ReadByte());
                                Server.SendMessage(outmsg, Player2Connection, NetDeliveryMethod.ReliableOrdered);
                                Player1Ready = true;
                            } else if (msg.SenderConnection == Player2Connection && !Player2Ready) {
                                WriteLine("Player2 Choice Recieved - Forwarding to Player1...");
                                outmsg.Write(msg.ReadByte());
                                outmsg.Write(msg.ReadByte());
                                outmsg.Write(msg.ReadByte());
                                Server.SendMessage(outmsg, Player1Connection, NetDeliveryMethod.ReliableOrdered);
                                Player2Ready = true;
                            }

                            if (Player1Ready && Player2Ready) {
                                outmsg = Server.CreateMessage();
                                outmsg.Write((byte)PacketTypes.READY);
                                Server.SendMessage(outmsg, Player1Connection, NetDeliveryMethod.ReliableOrdered);
                                outmsg = Server.CreateMessage();
                                outmsg.Write((byte)PacketTypes.READY);
                                Server.SendMessage(outmsg, Player2Connection, NetDeliveryMethod.ReliableOrdered);
                                WriteLine("Starting Turn:");
                            }
                            break;
                            case ((byte)PacketTypes.READY): //Telling server the player has finished the battle scene
                            if (msg.SenderConnection == Player1Connection && Player1Ready) {
                                WriteLine("Player1 Finished Turn...");
                                Player1Ready = false;
                            } else if (msg.SenderConnection == Player2Connection && Player2Ready) {
                                WriteLine("Player2 Finished Turn...");
                                Player2Ready = false;
                            }

                            if (!Player1Ready && !Player2Ready) {
                                outmsg = Server.CreateMessage();
                                outmsg.Write((byte)PacketTypes.READY);
                                Server.SendMessage(outmsg, Player1Connection, NetDeliveryMethod.ReliableOrdered);
                                outmsg = Server.CreateMessage();
                                outmsg.Write((byte)PacketTypes.READY);
                                Server.SendMessage(outmsg, Player2Connection, NetDeliveryMethod.ReliableOrdered);
                                WriteLine("Player Choices:");
                            }

                            break;
                            case ((byte)PacketTypes.DEBUG):
                            if (msg.SenderConnection == Player1Connection) {
                                WriteLine("#Player1 Debug#");
                                WriteLine(msg.ReadString());

                            } else if (msg.SenderConnection == Player2Connection) {
                                WriteLine("#Player2 Debug#");
                                WriteLine(msg.ReadString());
                            }
                            break;
                        }
                        break;

                        case NetIncomingMessageType.StatusChanged:

                        WriteLine(msg.SenderConnection.ToString() + " status changed. " + msg.SenderConnection.Status);
                        switch (msg.SenderConnection.Status) {
                            case NetConnectionStatus.Connected:
                            outmsg = Server.CreateMessage();

                            if (WaitingPlayerConnections.Contains(msg.SenderConnection) && (Player1Connection == null || Player2Connection == null)) {
                                outmsg.Write((byte)PacketTypes.JOIN);
                                if (Player1Connection == null) {
                                    Player1Connection = msg.SenderConnection;
                                    WriteLine("Player1 Joined");
                                    outmsg.Write("Sucessfully Joined as Player1");
                                } else {
                                    Player2Connection = msg.SenderConnection;
                                    WriteLine("Player2 Joined");
                                    outmsg.Write("Sucessfully Joined as Player2");
                                }
                            } else if (WaitingSpectatorConnections.Contains(msg.SenderConnection)) {
                                outmsg.Write((byte)PacketTypes.JOIN);
                                WriteLine("New Spectator Joined");
                                outmsg.Write("Sucessfully Joined as Spectator");
                            } else {
                                WriteLine("Too Many Connections - Could not Connect");
                                msg.SenderConnection.Deny();
                            }
                            Server.SendMessage(outmsg, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                            if (Player1Connection != null && Player2Connection != null) {
                                outmsg = Server.CreateMessage();
                                outmsg.Write((byte)PacketTypes.READY);
                                Server.SendMessage(outmsg, Player1Connection, NetDeliveryMethod.ReliableOrdered);
                                outmsg = Server.CreateMessage();
                                outmsg.Write((byte)PacketTypes.READY);
                                Server.SendMessage(outmsg, Player2Connection, NetDeliveryMethod.ReliableOrdered);
                            }

                            break;
                            case NetConnectionStatus.InitiatedConnect:
                            break;
                            default:
                            break;
                        }

                        //   if (inc.SenderConnection.Status == NetConnectionStatus.Disconnected || inc.SenderConnection.Status == NetConnectionStatus.Disconnecting)
                        // Find disconnected character and remove it

                        break;
                        default:
                        WriteLine("Not Important Message");
                        break;
                    }
                }

                // While loops run as fast as your computer lets. While(true) can lock your computer up. Even 1ms sleep, lets other programs have piece of your CPU time
                System.Threading.Thread.Sleep(1);
            }
        }

        static void WriteLine(string arg) {
            sw.WriteLine(arg);
            sw.Flush();
            Console.WriteLine(arg);
        }
    }

    enum PacketTypes {
        JOIN,
        READY,
        CHOICE,
        DEBUG
    }
}
