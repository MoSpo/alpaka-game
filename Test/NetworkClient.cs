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

        double timer = 0;

        NetClient Client;

        public NetworkClient() {
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

            WriteLine("Client Started...\n");

        }


        public void Update(double dt) {
            if (timer <= 500) timer += dt;

            CheckServerMessages();

            if (connected && Keyboard.GetState().IsKeyDown(Keys.C) & timer > 500) {
                GetInput();
                timer = 0;
            }
        }

        private void GetInput() {
            NetOutgoingMessage outmsg = Client.CreateMessage();

            if (ready) {
                outmsg.Write((byte)PacketTypes.DEBUG); //TODO
                //outmsg.Write(debug);
                Client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);

                outmsg = Client.CreateMessage();
                outmsg.Write((byte)PacketTypes.READY);
                Client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);
                string s = "Finished turn...";
                Console.WriteLine(s);
                consoleOutput += s + "\n";

            } else {
                byte a = 0; //TODO
                byte b = 0;
                byte c = 0;

                outmsg.Write((byte)PacketTypes.CHOICE);
                outmsg.Write(a);
                outmsg.Write(b);
                outmsg.Write(c);
                Client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);
                string s = "Choice Sent... (" + a + " " + b + " " + c + ")";
                Console.WriteLine(s);
                consoleOutput += s + "\n";
            }
        }

        private void CheckServerMessages() {
            NetIncomingMessage msg;

            // If new messages arrived
            if ((msg = Client.ReadMessage()) != null) {
                string s = "";

                switch (msg.MessageType) {
                    case NetIncomingMessageType.Data:
                    PacketTypes type = (PacketTypes)msg.ReadByte();
                    switch (type) {
                        case PacketTypes.JOIN:
                        connected = true;
                        s = msg.MessageType + ":[" + type + "] " + msg.ReadString();
                        break;
                        case PacketTypes.CHOICE:
                        s = msg.MessageType + ":[" + type + "] " + msg.ReadByte() + " " + msg.ReadByte() + " " + msg.ReadByte();
                        break;
                        case PacketTypes.READY:
                        ready = !ready;
                        s = msg.MessageType + ":[" + type + "] ";
                        break;
                    }

                    break;
                    case NetIncomingMessageType.StatusChanged:
                    s = msg.MessageType + ":[" + (NetConnectionStatus)msg.ReadByte() + "] " + msg.ReadString();
                    break;
                    default:
                    s = msg.MessageType + ": " + msg.ReadString();
                    break;
                }
                Console.WriteLine(s);
                consoleOutput += s + "\n";
            }
        }

        protected void Draw(SpriteBatch spriteBatch, SpriteFont font) {
            spriteBatch.DrawString(font, consoleOutput, new Vector2(0, 0), Color.White);
        }

        private void WriteLine(string arg) {
            consoleOutput += arg;
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
