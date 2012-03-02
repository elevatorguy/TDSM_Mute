
using Terraria_Server;
using Terraria_Server.Plugins;
using Terraria_Server.Commands;
using System;
using Terraria_Server.Misc;
using System.IO;
using Terraria_Server.Logging;
namespace Mute
{
    public partial class MutePlugin : BasePlugin
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

        public int minvotes
        {
            get { return properties.getValue("min-votes", 5); }
        }

        public MutePlugin()
        {
            Name = "Mute";
            Description = "mute command";
            Author = "elevatorguy";
            Version = "0.37.0";
            TDSMBuild = 37;
        }

        protected override void Initialized(object state)
        {
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

        protected override void Disposed(object state)
        {

        }

        protected override void Enabled()
        {
            mutelist = new MuteList(this);
            ProgramLog.Plugin.Log(base.Name + " enabled");
        }

        protected override void Disabled()
        {
            ProgramLog.Plugin.Log(base.Name + " disabled.");
        }

        [Hook(HookOrder.TERMINAL)]
        void onPlayerChat(ref HookContext ctx, ref HookArgs.PlayerChat args)
        {
            Player player = ctx.Sender as Player;
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

                ctx.SetResult(HookResult.IGNORE);
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

