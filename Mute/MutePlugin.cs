
using Terraria_Server;
using Terraria_Server.Plugin;
using Terraria_Server.Events;
using Terraria_Server.Commands;
using System;
namespace Mute
{
    public partial class Mute : Plugin
    {
        private MuteList mutelist;

		bool isEnabled = false;
				
		public override void Load()
		{
			Name = "Mute";
			Description = "mute command";
			Author = "elevatorguy";
			Version = "0.35.0";
			TDSMBuild = 35;
			
			isEnabled = true;

            AddCommand("mute")
                .WithDescription("Mute a player from chat.")
                .WithAccessLevel(AccessLevel.PLAYER)
                .WithHelpText("The effect lasts five minutes.")
                .WithHelpText("Regular Players can mute if 5 people vote.")
                .Calls(this.MuteCommand);
		}
		
		public override void Enable()
		{
            mutelist = new MuteList();
			isEnabled = true;
			Program.tConsole.WriteLine(base.Name + " enabled");
			this.registerHook(Hooks.PLAYER_CHAT);
		}
		
		public override void Disable()
		{
			Program.tConsole.WriteLine(base.Name + " disabled.");
			isEnabled = false;
		}
		
		public override void onPlayerChat(MessageEvent ev)
		{
            Player player = ev.Sender as Player;
            string playername = player.Name;

            if (mutelist.checklist(playername))
            {
                DateTime timemuted = mutelist.getTimeMuted(playername);
                TimeSpan timeleft = timemuted.AddMinutes(5) - DateTime.UtcNow;

                player.sendMessage("You have " + timeleft.Minutes + ":" + timeleft.Seconds + " before you can chat again.");

                ev.Cancelled = true;
            }

            return;
		}
	}
}

