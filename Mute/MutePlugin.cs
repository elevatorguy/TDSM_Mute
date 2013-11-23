
using System;
using System.IO;
using Terraria;
using TShockAPI;
using TerrariaApi.Server;
using TShockAPI.Hooks;
using TShockAPI.Net;
using System.Collections.Generic;

namespace Mute
{
    [ApiVersion(1, 14)]
    public partial class MutePlugin : TerrariaPlugin
    {
        PropertiesFile properties;
        private MuteList mutelist;

        public static List<MyPlayer> PlayerList;

        public int timemuted
        {
            get { return properties.getValue("time-muted", 5); }
        }

        public int timebetweenvotes
        {
            get { return properties.getValue("time-between-votes", 5); }
        }

        public int permatime
        {
            get { return properties.getValue("semi-permanent", 168); }
        }

        public int minvotes
        {
            get { return properties.getValue("min-votes", 5); }
        }

        public MutePlugin(Main game) : base(game)
        {
            //constructor
        }

        public override string Name
        {
            get
            {
                return "Mute2";
            }
        }

        public override string Author
        {
            get
            {
                return "elevatorguy";
            }
        }

        public override string Description
        {
            get
            {
                return "Fancier muting.";
            }
        }

        public override Version Version
        {
            get
            {
                return new Version(4, 2);
            }
        }

        public override void Initialize()
        {
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
            ServerApi.Hooks.NetGreetPlayer.Register(this, OnGreetPlayer);
            ServerApi.Hooks.ServerLeave.Register(this, OnLeave);
            ServerApi.Hooks.ServerChat.Register(this, OnChat);
        }

        public void OnInitialize(EventArgs e)
        {
            Commands.ChatCommands.Add(new Command("mute.player.xmute", MuteCommand, "xmute"));
            Commands.ChatCommands.Add(new Command("mute.admin.xmute", AdminMuteCommand, "xmute"));
            Commands.ChatCommands.Add(new Command("mute.admin.xunmute", UnMuteCommand, "xunmute"));

            string pluginFolder = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "Mute";
            CreateDirectory(pluginFolder);

            properties = new PropertiesFile(pluginFolder + Path.DirectorySeparatorChar + "mute.properties");
            initPropFile();

            mutelist = new MuteList(this);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
            }
            
            base.Dispose(disposing);
        }

        void OnLeave(LeaveEventArgs e)
        {
            lock (PlayerList)
            {
                for (int i = 0; i < PlayerList.Count; i++)
                {
                    if (PlayerList[i].Index == e.Who)
                    {
                        PlayerList.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        void OnGreetPlayer(GreetPlayerEventArgs e)
        {
            lock (PlayerList)
                PlayerList.Add(new MyPlayer(e.Who));
        }

        void OnChat(ServerChatEventArgs e)
        {
            TSPlayer player = TShock.Players[e.Who];
            string playername = player.Name;

            string text = e.Text;
            if (!text.StartsWith("/") || text.StartsWith("/me")) //if talking or using /me
            {
                if (mutelist.checklist(playername))
                {
                    DateTime timemuted = mutelist.getTimeMuted(playername);
                    TimeSpan timeleft = timemuted.AddMinutes(5) - DateTime.UtcNow;

                    if (timemuted > DateTime.UtcNow.AddHours(1))
                    {
                        player.SendMessage("You are muted for a long time.", Color.Red);
                    }
                    else
                    {
                        player.SendMessage("You have " + timeleft.Minutes + ":" + timeleft.Seconds + " before you can chat again.", Color.Red);
                    }

                    e.Handled = true;
                }
            }
            return;
        }

        private static void CreateDirectory(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }

        private void initPropFile()
        {
            properties.Load();
            var dummy = timemuted;
            var dummy2 = timebetweenvotes;
            var dummy3 = permatime;
            var dummy4 = minvotes;
            properties.Save(true);
        }
    }
}

