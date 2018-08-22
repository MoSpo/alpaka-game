using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using Lidgren.Network;

namespace lidgren_client {
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameClient : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        string consoleOutput;

        bool connected;

        private Random random; //

        static NetClient Client;

        enum PacketTypes {
            JOIN,
            CHOICE,
            DEBUG
        }

        public GameClient() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }


        protected override void Initialize() {

            random = new Random();

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

            consoleOutput += "Client Started...\n";

            base.Initialize();
        }


        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("File");
        }


        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }


        protected override void Update(GameTime gameTime) {
            CheckServerMessages();
            if (connected && Keyboard.GetState().IsKeyDown(Keys.C)) {
                GetInput();
            }
            base.Update(gameTime);
        }

        private void GetInput() {
            byte a = (byte)random.Next(0, 4);
            byte b = (byte)random.Next(0, 6);
            byte c = (byte)random.Next(0, 8);

            NetOutgoingMessage outmsg = Client.CreateMessage();

            outmsg.Write((byte)PacketTypes.CHOICE);
            outmsg.Write(a);
            outmsg.Write(b);
            outmsg.Write(c);
            Client.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);
            string s = "Choice Sent... (" + a + " " + b + " " + c + ")";
            Console.WriteLine(s);
            consoleOutput += s + "\n";
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

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.DrawString(font, consoleOutput, new Vector2(0, 0), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
