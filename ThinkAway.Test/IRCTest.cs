using NUnit.Framework;
using ThinkAway.Net;

namespace ThinkAway.Test
{
    class IRCTest
    {
        [Test]
        public void StartIRC()
        {
            IRCBot bot = new IRCBot("Mona", "irc.freenode.net", 7000);
            bot.AddChannel("#ubuntu-cn");
            bool connect = bot.Connect();
            if (connect)
                bot.SendGlobalMessage("Hello, world!");
        }
    }
}
