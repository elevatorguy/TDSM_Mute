
using Terraria_Server.Commands;
using System;
using Terraria_Server;
using Terraria_Server.Plugins;
using NDesk.Options;

namespace Mute
{
    public partial class MutePlugin : BasePlugin
    {
        void MuteCommand(ISender sender, ArgumentList argz)
        {
            var permamute = false;

            var options = new OptionSet()
			{
				{ "p|permanent", v => permamute = true },
			};

            var args = options.Parse(argz);

            if (mutelist.checklist(sender.Name))
            {
                sender.sendMessage("You can't vote to mute others while you are muted.");
                return;
            }

            if (mutelist.uservoted(sender.Name) && !sender.Op)
            {
                DateTime timevoted = mutelist.getTimeVoted(sender.Name);
                TimeSpan timeleft = timevoted.AddMinutes(timebetweenvotes) - DateTime.UtcNow;

                sender.sendMessage("You have " + timeleft.Minutes + ":" + timeleft.Seconds + " before you can vote again.");
                return;
            }

            if (args.Count != 1)
                throw new CommandError("");

            string playername = args[0];

            Player target = Server.GetPlayerByName(playername.ToLower());
            int time = timemuted;

            if (target == null)
                throw new CommandError("Could not find specified player.");

            if (!sender.Op && target.Op)
            {
                sender.sendMessage("Only OPs can mute OPs.");
                return;
            }

            if (!sender.Op)
            {
                mutelist.uservote(sender.Name);

                if (!mutelist.vote(playername))
                {
                    sender.sendMessage("The player \"" + target.Name + "\" is already muted.");
                }
                else if (mutelist.checklist(playername))
                {
                    sender.sendMessage(target.Name + " has been muted for " + time + " minutes.");
                    target.sendMessage("You have been muted for " + time + " minutes.");
                    Server.notifyAll(target.Name + " has been muted for " + time + " minutes.");
                }
                else
                {
                    sender.sendMessage("Submitted your vote to mute " + target.Name);
                    target.sendMessage("You recieved a vote to be muted from chat.");
                }
            }
            else
            {
                string period = time + " minutes.";
                if (permamute)
                    period = "a long time.";

                if (!mutelist.mute(playername, permamute))
                {
                    sender.sendMessage("The player \"" + target.Name + "\" is already muted.");
                }
                else
                {
                    sender.sendMessage(target.Name + " has been muted for " + period);
                    target.sendMessage("You have been muted from chat for " + period);
                    Server.notifyAll(target.Name + " has been muted for " + period);
                }
            }
        }

        void UnMuteCommand(ISender sender, ArgumentList args)
        {
            if (args.Count != 1)
                throw new CommandError("");

            string playername = args[0];

            Player target = Server.GetPlayerByName(playername.ToLower());

            if (target == null)
                throw new CommandError("Could not find specified player.");

            if (mutelist.unmute(playername))
            {
                sender.sendMessage(playername + " has been unmuted.");
                target.sendMessage("You have been unmuted from chat.");
                Server.notifyAll(playername + " has been unmuted.");
            }
            else
            {
                sender.sendMessage(playername + " was already unmuted.");
            }
        }
    }
}


