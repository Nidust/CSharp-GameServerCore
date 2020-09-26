using System;
using System.Collections.Generic;
using System.Threading;

namespace Core.Server.DummyClient.Bot
{
    public class BotManager
    {
        private List<Bot> mBotList;

        private Boolean mDisconnectAll;
        private AutoResetEvent mDisconnectAllEvent;

        public BotManager()
        {
            mBotList = new List<Bot>();
            mDisconnectAllEvent = new AutoResetEvent(false);
        }

        public BotType CreateBot<BotType>() where BotType : Bot, new()
        {
            BotType newBot = new BotType();
            newBot.Manager = this;

            return newBot;
        }

        public void Add(Bot bot)
        {
            lock (mBotList)
            {
                mBotList.Add(bot);
            }
        }

        public void Remove(Bot bot)
        {
            lock (mBotList)
            {
                mBotList.Remove(bot);

                if (mDisconnectAll && mBotList.Count == 0)
                    mDisconnectAllEvent.Set();
            }
        }

        public void DisconnectAll()
        {
            mDisconnectAll = true;

            lock (mBotList)
            {
                foreach (Bot bot in mBotList)
                {
                    bot.Disconnect();
                }
            }

            mDisconnectAllEvent.WaitOne();
        }
    }
}
