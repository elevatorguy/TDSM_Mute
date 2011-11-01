
using Terraria_Server;
using Terraria_Server.Plugin;
using Terraria_Server.Events;
using Terraria_Server.Commands;
using System;
using Terraria_Server.Misc;
using System.IO;
namespace Mute
{
    public partial class MutePlugin : Plugin
    {
        PropertiesFile properties;
        private MuteList mutelist;

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

        public override void Load()
        {
            Name = "Mute";
            Description = "mute command";
            Author = "elevatorguy";
            Version = "0.35.0";
            TDSMBuild = 35;

            string pluginFolder = Statics.PluginPath + Path.DirectorySeparatorChar + "Mute";
            CreateDirectory(pluginFolder);

            properties = new PropertiesFile(pluginFolder + Path.DirectorySeparatorChar + "mute.properties");
            initPropFile();

            AddCommand("mute")
                .WithDescription("Mute a player from chat.")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithHelpText("The effect lasts five minutes.")
                .WithHelpText("Regular Players can mute if "+timemuted+" people vote.")
                .Calls(this.MuteCommand);

            AddCommand("unmute")
                .WithDescription("Lets a player chat again")
                .WithHelpText("For unmuting permanent mutes")
                .Calls(this.UnMuteCommand);
        }

        public override void Enable()
        {
            mutelist = new MuteList(this);
            Program.tConsole.WriteLine(base.Name + " enabled");
            this.registerHook(Hooks.PLAYER_CHAT);
        }

        public override void Disable()
        {
            Program.tConsole.WriteLine(base.Name + " disabled.");
        }

        public override void onPlayerChat(MessageEvent ev)
        {
            Player player = ev.Sender as Player;
            string playername = player.Name;

            if (mutelist.checklist(playername))
            {
                DateTime timemuted = mutelist.getTimeMuted(playername);
                TimeSpan timeleft = timemuted.AddMinutes(5) - DateTime.UtcNow;

                if (timemuted > DateTime.UtcNow.AddHours(1))
                {
                    player.sendMessage("You are muted for a long time.");
                }
                else
                {
                    player.sendMessage("You have " + timeleft.Minutes + ":" + timeleft.Seconds + " before you can chat again.");
                }

                ev.Cancelled = true;
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
            properties.Save(true);
        }
    }
}

