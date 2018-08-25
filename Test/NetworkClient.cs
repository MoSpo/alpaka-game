using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using Lidgren.Network;

namespace Alpaka {

    public class NetworkClient {

        string consoleOutput;

        bool connected;
        bool ready = true;
        bool debug = true;

        double timer = 0;

        NetClient Client;

        Game1 g;

        public NetworkClient(Game1 g) {
            string path = Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), @"ip.txt");
            StreamReader sr = new StreamReader(path);
            string ip = sr.ReadLine();

            NetPeerConfiguration Config = new NetPeerConfiguration("alpaka");
            Client = new NetClient(Config);

            // Create hail message
            NetOutgoingMessage hailmsg = Client.CreateMessage();

            Client.Start();

            hailmsg.Write((byte)PacketTypes.JOIN);
            hailmsg.Write((byte)0); //Whether spectating or participating
            hailmsg.Write("Username"); //Name of user

            Client.Connect(ip, 20000, hailmsg);

            WriteLine("Client Started...");

            this.g = g;
        }

        public void Update(double dt) {
            NetIncomingMessage msg;

            // If new messages arrived
            if ((msg = Client.ReadMessage()) != null) {

                switch (msg.MessageType) {
                    case NetIncomingMessageType.Data:
                    PacketTypes type = (PacketTypes)msg.ReadByte();
                    switch (type) {
                        case PacketTypes.JOIN:
                        connected = true;
                        WriteLine(msg.MessageType + ":[" + type + "] " + msg.ReadString());
                        break;
                        case PacketTypes.CHOICE:
                        byte a = msg.ReadByte();
                        byte b = msg.ReadByte();
                        byte c = msg.ReadByte();
                        g.setOpponentChoice(a, b, c);
                        WriteLine(msg.MessageType + ":[" + type + "] " + a + " " + b + " " + c);
                        break;
                        case PacketTypes.READY:
                        if (!g.firstready) g.firstready = true;
                        else g.play = !g.play;
                        WriteLine(msg.MessageType + ":[" + type + "] ");
                        break;
                    }

                    break;
                    case NetIncomingMessageType.StatusChanged:
                    WriteLine(msg.MessageType + ":[" + (NetConnectionStatus)msg.ReadByte() + "] " + msg.ReadString());
                    break;
                    default:
                    WriteLine(msg.MessageType + ": " + msg.ReadString());
                    break;
                }
            }
        }

        public void SendInput(byte action, byte movement, byte creature) {
            NetOutgoingMessage outmsg = Client.CreateMessage();
            outmsg.Write((byte)PacketTypes.CHOICE);
            outmsg.Write(action);
            outmsg.Write(movement);
            outmsg.Write(creature);
            Client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);
            WriteLine("Choice Sent... (" + action + " " + movement + " " + creature + ")");
        }

        public void SendReady() {
            NetOutgoingMessage outmsg = Client.CreateMessage();
            outmsg.Write((byte)PacketTypes.READY);
            Client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);
            WriteLine("Finished turn...");
        }

        public void SendDebug(string info) {
            if (debug) {
                NetOutgoingMessage outmsg = Client.CreateMessage();
                outmsg.Write((byte)PacketTypes.DEBUG);
                outmsg.Write(info);
                Client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);
                WriteLine("Sent debug info...");
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font) {
            spriteBatch.DrawString(font, consoleOutput, new Vector2(0, 0), Color.White);
        }

        private void WriteLine(string arg) {
            consoleOutput += arg + "\n";
            Console.WriteLine(arg);
        }
    }

    enum PacketTypes {
        JOIN,
        READY,
        CHOICE,
        DEBUG,
    }
}
