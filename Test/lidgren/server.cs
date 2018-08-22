using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lidgren_server {

    class Program {

        static NetServer Server;
        static NetPeerConfiguration Config;
        static NetConnection Player1Connection;
        static NetConnection Player2Connection;

        static List<NetConnection> WaitingPlayerConnections;
        static List<NetConnection> WaitingSpectatorConnections;

        static void Main(string[] args) {

            Config = new NetPeerConfiguration("alpaka");
            Config.Port = 20000;
            Config.MaximumConnections = 200;

            Config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

            Server = new NetServer(Config);
            Server.Start();
            Console.WriteLine("Server Started...");

            WaitingPlayerConnections = new List<NetConnection>();
            WaitingSpectatorConnections = new List<NetConnection>();
            NetIncomingMessage msg;

            while (true) {
                if ((msg = Server.ReadMessage()) != null) {

                    switch (msg.MessageType) {
                        case NetIncomingMessageType.ConnectionApproval:

                        if (msg.ReadByte() == (byte)PacketTypes.JOIN) {
                            Console.WriteLine("Incoming JOIN");
                            byte b = msg.ReadByte();
                            if (b == 0) {
                                if (Player1Connection == null || Player2Connection == null) {
                                    msg.SenderConnection.Approve();
                                    WaitingPlayerConnections.Add(msg.SenderConnection);
                                    Console.WriteLine("Pending new connection to: " + msg.ReadString());

                                } else {
                                    msg.SenderConnection.Deny();
                                    Console.WriteLine("Denied connection to: " + msg.ReadString());
                                }
                            } else if (b == 1) {
                                msg.SenderConnection.Approve();
                                WaitingSpectatorConnections.Add(msg.SenderConnection);
                                Console.WriteLine("Pending new connection to: " + msg.ReadString());

                            } else {
                                msg.SenderConnection.Deny();
                                Console.WriteLine("Denied connection to: " + msg.ReadString());
                            }
                        }

                        break;

                        case NetIncomingMessageType.Data:
                        if (msg.ReadByte() == (byte)PacketTypes.CHOICE) {

                            NetOutgoingMessage outmsg = Server.CreateMessage();
                            outmsg.Write((byte)PacketTypes.CHOICE);
                            if (msg.SenderConnection == Player1Connection) {
                                Console.WriteLine("Player1 Choice Recieved - Forwarding to Player2...");
                                outmsg.Write(msg.ReadByte());
                                outmsg.Write(msg.ReadByte());
                                outmsg.Write(msg.ReadByte());
                                Server.SendMessage(outmsg, Player2Connection, NetDeliveryMethod.ReliableOrdered);
                            } else if (msg.SenderConnection == Player2Connection) {
                                Console.WriteLine("Player2 Choice Recieved - Forwarding to Player1...");
                                outmsg.Write(msg.ReadByte());
                                outmsg.Write(msg.ReadByte());
                                outmsg.Write(msg.ReadByte());
                                Server.SendMessage(outmsg, Player1Connection, NetDeliveryMethod.ReliableOrdered);

                            }
                        }
                        break;

                        case NetIncomingMessageType.StatusChanged:

                        Console.WriteLine(msg.SenderConnection.ToString() + " status changed. " + msg.SenderConnection.Status);
                        switch (msg.SenderConnection.Status) {
                            case NetConnectionStatus.Connected:
                            NetOutgoingMessage outmsg = Server.CreateMessage();

                            if (WaitingPlayerConnections.Contains(msg.SenderConnection) && (Player1Connection == null || Player2Connection == null)) {
                                outmsg.Write((byte)PacketTypes.JOIN);
                                if (Player1Connection == null) {
                                    Player1Connection = msg.SenderConnection;
                                    Console.WriteLine("Player1 Joined");
                                    outmsg.Write("Sucessfully Joined as Player1");
                                } else {
                                    Player2Connection = msg.SenderConnection;
                                    Console.WriteLine("Player2 Joined");
                                    outmsg.Write("Sucessfully Joined as Player2");
                                }
                            } else if (WaitingSpectatorConnections.Contains(msg.SenderConnection)) {
                                outmsg.Write((byte)PacketTypes.JOIN);
                                Console.WriteLine("New Spectator Joined");
                                outmsg.Write("Sucessfully Joined as Spectator");
                            } else {
                                Console.WriteLine("Too Many Connections - Could not Connect");
                                msg.SenderConnection.Deny();
                            }
                            Server.SendMessage(outmsg, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered);

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
                        Console.WriteLine("Not Important Message");
                        break;
                    }
                }

                // While loops run as fast as your computer lets. While(true) can lock your computer up. Even 1ms sleep, lets other programs have piece of your CPU time
                System.Threading.Thread.Sleep(1);
            }
        }
    }

    enum PacketTypes {
        JOIN,
        CHOICE,
        DEBUG
    }
}
