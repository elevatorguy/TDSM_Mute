
using System;
using NDesk.Options;
using Terraria;
using TShockAPI;
using TerrariaApi.Server;
using System.Collections.Generic;

namespace Mute
{
    public partial class MutePlugin : TerrariaPlugin
    {
        void MuteCommand(CommandArgs argzz)
        {
            var permamute = false;

            var options = new OptionSet()
			{
				{ "p|permanent", v => permamute = true },
			};

            TSPlayer player = argzz.Player;
            if (player == null)
                return;

            List<string> args = options.Parse(argzz.Parameters);

            if (mutelist.checklist(player.Name))
            {
                player.SendErrorMessage("You can't vote to mute others while you are muted.");
                return;
            }

            if (mutelist.uservoted(player.Name))
            {
                DateTime timevoted = mutelist.getTimeVoted(player.Name);
                TimeSpan timeleft = timevoted.AddMinutes(timebetweenvotes) - DateTime.UtcNow;

                player.SendInfoMessage("You have " + timeleft.Minutes + ":" + timeleft.Seconds + " before you can vote again.");
                return;
            }

            if (args.Count != 1)
            {
                player.SendErrorMessage("Error");
                return;
            }

            string playername = args[0];

            List<MyPlayer> targets = MyPlayer.GetPlayersByName(playername.ToLower());

            if (targets.Count == 0)
            {
                player.SendErrorMessage("Player not Found.", Color.Red);
                return;
            }

            MyPlayer target = targets[0];

            int time = timemuted;

            if (target == null)
            {
                player.SendErrorMessage("Could not find specified player.");
                return;
            }

            mutelist.uservote(player.Name);

            if  (!mutelist.vote(playername))
            {
                player.SendErrorMessage("The player \"" + target.TSPlayer.Name + "\" is already muted.");
            }
            else if (mutelist.checklist(playername))
            {
                player.SendInfoMessage(target.TSPlayer.Name + " has been muted for " + time + " minutes.");
                target.TSPlayer.SendInfoMessage("You have been muted for " + time + " minutes.");
                MyPlayer.notifyAll(target.TSPlayer.Name + " has been muted for " + time + " minutes.");
            }
            else
            {
                player.SendInfoMessage("Submitted your vote to mute " + target.TSPlayer.Name);
                target.TSPlayer.SendInfoMessage("You recieved a vote to be muted from chat.");
            }
        }

        void AdminMuteCommand(CommandArgs argzz)
        {
            var permamute = false;

            var options = new OptionSet()
			{
				{ "p|permanent", v => permamute = true },
			};

            TSPlayer player = argzz.Player;
            if (player == null)
                return;

            List<string> args = options.Parse(argzz.Parameters);

            if (mutelist.checklist(player.Name))
            {
                player.SendErrorMessage("You can't vote to mute others while you are muted.");
                return;
            }

            if (args.Count != 1)
            {
                player.SendErrorMessage("Error");
                return;
            }

            string playername = args[0];

            List<MyPlayer> targets = MyPlayer.GetPlayersByName(playername.ToLower());

            if (targets.Count == 0)
            {
                player.SendErrorMessage("Player not Found.", Color.Red);
                return;
            }

            MyPlayer target = targets[0];

            int time = timemuted;

            if (target == null)
            {
                player.SendErrorMessage("Could not find specified player.");
                return;
            }

            
            string period = time + " minutes.";
            if (permamute)
                period = "a long time.";

            if (!mutelist.mute(playername, permamute))
            {
                player.SendErrorMessage("The player \"" + target.TSPlayer.Name + "\" is already muted.");
            }
            else
            {
                player.SendInfoMessage(target.TSPlayer.Name + " has been muted for " + period);
                target.TSPlayer.SendInfoMessage("You have been muted from chat for " + period);
                MyPlayer.notifyAll(target.TSPlayer.Name + " has been muted for " + period);
            }
        }

        void UnMuteCommand(CommandArgs argzz)
        {
            var options = new OptionSet()
			{
			};

            List<string> args = options.Parse(argzz.Parameters);
            TSPlayer player = argzz.Player;
            if (args.Count != 1)
            {
                player.SendErrorMessage("Error", Color.Red);
                return;
            }

            string playername = args[0];

            List<MyPlayer> targets = MyPlayer.GetPlayersByName(playername.ToLower());

            if (targets.Count == 0)
            {
                player.SendErrorMessage("Player not Found.", Color.Red);
                return;
            }

            MyPlayer target = targets[0];

            if (target == null)
            {
                player.SendErrorMessage("Could not find specified player.");
                return;
            }

            if (mutelist.unmute(playername))
            {
                player.SendInfoMessage(playername + " has been unmuted.");
                target.TSPlayer.SendInfoMessage("You have been unmuted from chat.");
                MyPlayer.notifyAll(playername + " has been unmuted.");
            }
            else
            {
                player.SendMessage(playername + " was already unmuted.", Color.Red);
            }
        }
    }
}


