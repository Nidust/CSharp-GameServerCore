using System;
using System.Collections.Generic;
using System.Threading;

namespace Core.Server.DummyClient.Bot
{
    public class BotManager
    {
        private object mLock;
        private List<Bot> mBotList;

        private Boolean mDisconnectAll;
        private AutoResetEvent mDisconnectAllEvent;

        public BotManager()
        {
            mLock = new object();
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
            lock (mLock)
            {
                mBotList.Add(bot);
            }
        }

        public void Remove(Bot bot)
        {
            lock (mLock)
            {
                mBotList.Remove(bot);
            }
        }

        public void DisconnectAll()
        {
            mDisconnectAll = true;

            lock (mLock)
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
