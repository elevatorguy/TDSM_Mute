using System.Collections.Generic;
using TShockAPI;

namespace Mute
{
    public class MyPlayer
    {
        public int Index { get; set; }
        public TSPlayer TSPlayer { get { return TShock.Players[Index]; } }

        public MyPlayer(int index)
        {
            Index = index;
        }

        public static void notifyAll(string message)
        {
            foreach (MyPlayer plrs in MutePlugin.PlayerList)
            {
                plrs.TSPlayer.SendInfoMessage(message);
            }
        }

        public static List<MyPlayer> GetPlayersByName(string plrName)
        {
            List<MyPlayer> players = new List<MyPlayer>();
            foreach (MyPlayer plrs in MutePlugin.PlayerList)
            {
                if (plrs.TSPlayer.Name.ToLower() == plrName.ToLower())
                {
                    players.Clear();
                    players.Add(plrs);
                    break;
                }
                else if (plrs.TSPlayer.Name.ToLower().Contains(plrName.ToLower()))
                {
                    players.Add(plrs);
                }
            }
            return players;
        }

        public static List<MyPlayer> GetPlayersByIPMask(string IP)
        {
            List<MyPlayer> players = new List<MyPlayer>();
            foreach (MyPlayer plrs in MutePlugin.PlayerList)
            {
                string[] plrIP = plrs.TSPlayer.IP.Split('.');
                string[] argIP = IP.Split('.');
                if (argIP[0] == plrIP[0] || argIP[0] == "*")
                {
                    if (argIP[1] == plrIP[1] || argIP[1] == "*")
                    {
                        if (argIP[2] == plrIP[2] || argIP[2] == "*")
                        {
                            if (argIP[3] == plrIP[3] || argIP[3] == "*")
                            {
                                players.Add(plrs);
                            }
                        }
                    }
                }
            }
            return players;
        }
    }

}