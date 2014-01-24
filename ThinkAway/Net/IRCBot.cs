/*
 * IRCBot Class
 * Latest release as of: 02/18/09
 * 
 * by Andrew Brown
 * http://www.drusepth.net/
 * 
 * Manages an IRC session and provides functions to send and receive data.
 * 
 * Usage:
 * IRCBot bot = new IRCBot("Mona", "irc.tddirc.net", 6667);
 * bot.addChannel("#hackerthreads");
 * bot.connect();
 * bot.sendGlobalMessage("Hello, world!");
 * 
 */

using System;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;

namespace ThinkAway.Net
{
    public class IRCBot
    {

        // Bot Information
        private static string _nickname;
        private static string _server;
        private static int _port;
        private static readonly List<string> Channels = new List<string>();
        private int _verbosity;

        // Socket Stuff
        private StreamWriter _swrite;
        private StreamReader _sread;
        private NetworkStream _sstream;
        private TcpClient _irc;

        // Flags
        public bool BotOn = true;
        private bool _debugMode;

        // Other information
        private string _line; // incoming line
        private string[] _splitLine; // array of line, expoded by \s

        public IRCBot(string nickIn, string serverIn, int portIn)
        {
            _nickname = nickIn;
            _server = serverIn;
            _port = portIn;
        }

        public void AddChannel(string channel)
        {
            Channels.Add(channel);
        }

        public bool Connect()
        {
            try
            {
                if (_verbosity == 1 || _verbosity == 2)
                    Console.WriteLine("[IRCBot.cs][Connecting to {0}:{1}]", _server, _port);

                _irc = new TcpClient(_server, _port);
                _sstream = _irc.GetStream();
                _sread = new StreamReader(_sstream);
                _swrite = new StreamWriter(_sstream);

                Identify(_nickname);

                Thread tIRC = new Thread(IrcThread);
                tIRC.Start();

                return true;

            }
            catch (Exception e)
            {
                if (_debugMode)
                    Console.WriteLine("[IRCBot.cs][Error connecting to {0}: {1}]", _server, e);

                return false;
            }

        }

        protected bool Identify(string nick)
        {
            try
            {
                // Identify
                if (_verbosity == 4 || _verbosity == 5)
                    Console.WriteLine("[IRCBot.cs][>][USER {0} {0} {0} :{1}", nick, nick);

                _swrite.WriteLine("USER {0} {0} {0} :{1}", nick, nick);
                _swrite.Flush();

                if (_verbosity == 4 || _verbosity == 5)
                    Console.WriteLine("[IRCBot.cs][>][NICK {0}]", nick);

                _swrite.WriteLine("NICK {0}", nick);
                _swrite.Flush();

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void IrcThread()
        {
            while (BotOn)
            {
                if ((_line = _sread.ReadLine()) != null)
                {
                    if (_verbosity == 3 || _verbosity == 4)
                        Console.WriteLine("[IRCBot.cs][<][{0}]", _line);

                    _splitLine = _line.Split(' ');

                    if (_splitLine.Length > 0)
                    {
                        switch (_splitLine[1])
                        {
                            case "366":
                                break;

                            case "376":
                            case "422":
                                foreach (string t in Channels)
                                {
                                    if (_verbosity == 4 || _verbosity == 5)
                                        Console.WriteLine("[IRCBot.cs][>][JOIN {0}]", t);

                                    _swrite.WriteLine("JOIN {0}", t);
                                    _swrite.Flush();
                                }
                                break;
                        }

                        if (_splitLine[0] == "PING")
                        {
                            _swrite.WriteLine("PONG {0}", _splitLine[1]);
                            _swrite.Flush();
                        }
                    }
                }
            }

            // Clean up
                        _swrite.Close();
                        _sread.Close();
                        _irc.Close();

        }

        public void SetDebugMode(bool onoff)
        {
            _debugMode = onoff;
        }

        public void SetVerbosity(int level)
        {
            /*
             * Verbosity Levels:
             * 0 = No Output
             * 1 = Print networking (connect/disconnect)
             * 2 = Print networking + pings
             * 3 = Print all incoming data from server
             * 4 = Print all incoming data + outgoing data
             * 5 = Only print outgoing data
             */
            _verbosity = level;
        }

        public string GetChannel()
        {
            return _line.Split(' ')[2];
        }

        public string GetUsernameSpeaking()
        {
            // :drusepth!drusepth@google.gov PRIVMSG #hackerthreads :Welcome to #hackerthreads I love you
            return _line.Split('!')[0].Split(':')[1];
        }

        public string GetHostSpeaking()
        {
            return _line.Split('!')[1].Split(' ')[0];
        }

        public string GetSpokenLine()
        {
            if (_line.Split(':').Length >= 2)
                return _line.Split(':')[2];

            return "";
        }

        public void SendGlobalMessage(string msg)
        {
            foreach (string t in Channels)
            {
                _swrite.WriteLine("PRIVMSG {0} :{1}",
                                  t, 
                                  msg
                    );
                _swrite.Flush();
            }
        }
    }
}
