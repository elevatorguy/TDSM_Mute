
using Terraria_Server.Commands;
using System;
using Terraria_Server;
using Terraria_Server.Plugin;

namespace Mute
{
	public partial class Mute : Plugin
	{
        void MuteCommand(Server server, ISender sender, ArgumentList args)
        {
            if (mutelist.checklist(sender.Name))
            {
                sender.sendMessage("You can't vote to mute others while you are muted.");
                return;
            }

            if (mutelist.uservoted(sender.Name) && !sender.Op)
            {
                DateTime timevoted = mutelist.getTimeVoted(sender.Name);
                TimeSpan timeleft = timevoted.AddMinutes(5) - DateTime.UtcNow;

                sender.sendMessage("You have " + timeleft.Minutes + ":" + timeleft.Seconds + " before you can vote again.");
                return;
            }

            if (args.Count != 1)
                throw new CommandError("");

            string playername = args[0];

            Player target = Server.GetPlayerByName(playername.ToLower());
            int time = 5;

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
                    sender.sendMessage("The player \"" + playername + "\" is already muted.");
                }
                else if (mutelist.checklist(playername))
                {
                    sender.sendMessage(playername + " has been muted for " + time + " minutes.");
                    target.sendMessage("You have been muted for " + time + " minutes.");
                    Server.notifyAll(playername + " has been muted for " + time + " minutes.");
                }
                else
                {
                    sender.sendMessage("Submitted your vote to mute " + playername);
                    target.sendMessage("You recieved a vote to be muted from chat.");
                }
            }
            else
            {
                if (!mutelist.mute(playername))
                {
                    sender.sendMessage("The player \"" + playername + "\" is already muted.");
                }
                else
                {
                    sender.sendMessage(playername + " has been muted for " + time + " minutes.");
                    target.sendMessage("You have been muted from chat for " + time + " minutes.");
                    Server.notifyAll(playername + " has been muted for " + time + " minutes.");
                }
            }
        }
	}
}


