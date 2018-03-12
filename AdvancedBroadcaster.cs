using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fougerite;
using System.IO;

namespace AdvancedBroadcaster
{
    public class AdvancedBroadcaster : Fougerite.Module
    {
        public IniParser Settings;
        public static IniParser cfg;
        public static IniParser cfg1;
        public int RegisterMessages = 1;
        public int RegisterNotice = 1;
        public bool EnableNotice;
        public bool EnableMessages;
        public int BroadCastTime = 30;
        public int NoticeTime = 40;

        public override string Name { get { return "AdvancedBroadcaster"; } }
        public override string Author { get { return "ice cold"; } }
        public override string Description { get { return "Broadcast random notices/messages in chat"; } }
        public override Version Version { get { return new Version("1.0"); } }

        public static System.Random rnd;

        public override void Initialize()
        {
            rnd = new System.Random();
            cfg = new IniParser(Path.Combine(ModuleFolder, "Messages.ini"));
            cfg1 = new IniParser(Path.Combine(ModuleFolder, "Notices.ini"));

            if (!File.Exists(Path.Combine(ModuleFolder, "Settings.ini")))
            {
                File.Create(Path.Combine(ModuleFolder, "Settings.ini")).Dispose();
                Settings = new IniParser(Path.Combine(ModuleFolder, "Settings.ini"));
                Settings.AddSetting("Messages", "EnabledMessages", "true");
                Settings.AddSetting("Messages", "BroadCastTime", BroadCastTime.ToString());
                Settings.AddSetting("Messages", "Registered", RegisterMessages.ToString());
                Settings.AddSetting("Notices", "EnableNotices", "true");
                Settings.AddSetting("Notices", "NoticeTime", NoticeTime.ToString());
                Settings.AddSetting("Notices", "Registered", RegisterNotice.ToString());
                Settings.Save();
            }
            else
            {
                Settings = new IniParser(Path.Combine(ModuleFolder, "Settings.ini"));
                EnableMessages = bool.Parse(Settings.GetSetting("Messages", "EnabledMessages"));
                BroadCastTime = int.Parse(Settings.GetSetting("Messages", "BroadCastTime"));
                RegisterMessages = int.Parse(Settings.GetSetting("Messages", "Registered"));
                EnableNotice = bool.Parse(Settings.GetSetting("Notices", "EnableNotices"));
                NoticeTime = int.Parse(Settings.GetSetting("Notices", "NoticeTime"));
                RegisterNotice = int.Parse(Settings.GetSetting("Notices", "Registered"));
            }
            if (EnableMessages)
            {
                Logger.Log("ChatBroadcast enabled");
                BroadCast(BroadCastTime * 1000, null).Start();
            }
            if (EnableNotice)
            {
                Logger.Log("Notices enabled");
                Notice(NoticeTime * 1000, null).Start();
            }
        }
        public TimedEvent BroadCast(int timeoutDelay, Dictionary<string, object> args)
        {
            TimedEvent timedEvent = new TimedEvent(timeoutDelay);
            timedEvent.Args = args;
            timedEvent.OnFire += CallBackChat;
            return timedEvent;
        }
        public void CallBackChat(TimedEvent e)
        {
            e.Kill();
            int b = rnd.Next(1, RegisterMessages);
            string j = cfg.GetSetting("Messages", b.ToString());
            Server.GetServer().Broadcast(j);
            BroadCast(BroadCastTime * 1000, null).Start();
        }
        public TimedEvent Notice(int timeoutDelay, Dictionary<string, object> args)
        {
            TimedEvent timedEvent = new TimedEvent(timeoutDelay);
            timedEvent.Args = args;
            timedEvent.OnFire += CallBackNotice;
            return timedEvent;
        }
        public void CallBackNotice(TimedEvent e)
        {
            e.Kill();
            int b = rnd.Next(1, RegisterNotice);
            string j = cfg1.GetSetting("Notices", b.ToString());
            Server.GetServer().BroadcastNotice(j);
            Notice(NoticeTime * 1000, null).Start();
        }
    }
}
