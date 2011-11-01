using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mute
{
    class MuteList
    {
        private Dictionary<string, int> votes; // <name, votes>
        private Dictionary<string, DateTime> muted; //<name, (time muted)>

        private Dictionary<string, DateTime> voters; // <name, (time voted)>

        MutePlugin instance;

        public MuteList(MutePlugin muteplugin)
        {
            votes = new Dictionary<string, int>();
            muted = new Dictionary<string, DateTime>();
            voters = new Dictionary<string, DateTime>();
            instance = muteplugin;
        }

        //returns true if user is now muted
        //false if user was already muted
        public bool mute(string name, bool permanent)
        {
            name = name.ToLower();

            if (muted.ContainsKey(name))
            {
                return false;
            }
            if (permanent)
            {
                muted.Add(name, DateTime.UtcNow.AddHours(instance.permatime));
            }
            else
            {
                muted.Add(name, DateTime.UtcNow);
            }

            return true;
        }

        //returns true if vote succeeded
        //false if the user is already muted
        public bool vote(string name)
        {
            name = name.ToLower();

            if (checklist(name))
                return false;

            if (votes.ContainsKey(name))
            {
                int votesforname;
                votes.TryGetValue(name, out votesforname);

                if (votesforname == (instance.minvotes-1))
                {
                    mute(name, false);
                    votes.Remove(name);
                }
                else
                {
                    votes.Remove(name);
                    votes.Add(name, votesforname + 1);
                }
            }
            else
            {
                votes.Add(name, 1);
            }
            return true;
        }

        //returns true if the user is muted
        //false if not
        public bool checklist(string name)
        {
            name = name.ToLower();

            if (muted.ContainsKey(name))
            {
                DateTime timemuted;
                muted.TryGetValue(name, out timemuted);

                if (timemuted < DateTime.UtcNow.AddMinutes(-(instance.timemuted)))
                {
                    muted.Remove(name);
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        //returns null if player not in muted list
        //so call the checklist method first
        //before calling this method
        public DateTime getTimeMuted(string name)
        {
            name = name.ToLower();

            DateTime timemuted;
            muted.TryGetValue(name, out timemuted);

            return timemuted;
        }

        //returns null if player not in voters list
        //so call the uservoted method first
        //before calling this method
        public DateTime getTimeVoted(string name)
        {
            name = name.ToLower();

            DateTime timevoted;
            voters.TryGetValue(name, out timevoted);

            return timevoted;
        }

        //returns true if the user voted
        //false if not
        public bool uservoted(string name)
        {
            name = name.ToLower();

            if (voters.ContainsKey(name))
            {
                DateTime timevoted;
                voters.TryGetValue(name, out timevoted);

                if (timevoted < DateTime.UtcNow.AddMinutes(-(instance.timebetweenvotes)))
                {
                    voters.Remove(name);
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        public void uservote(string name)
        {
            name = name.ToLower();

            if (voters.ContainsKey(name))
            {
                voters.Remove(name);
            }

            voters.Add(name, DateTime.UtcNow);
        }

        // returns true if the user was unmuted just now
        // returns false if the user was never muted
        public bool unmute(string name)
        {
            name = name.ToLower();

            if (muted.ContainsKey(name))
            {
                muted.Remove(name);
                return true;
            }

            return false;
        }

    }
}
