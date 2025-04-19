using System;
using System.Threading.Tasks;
using Core.Controller;
using L2Logger;
using Network;

namespace Core.NetworkPacket.ClientPacket.LoginServicePacket
{
    public class LoginServKickAccount : PacketBase
    {
        private readonly string _account;
        private readonly LoginServiceController _controller;
        public LoginServKickAccount(IServiceProvider serviceProvider, Packet p, LoginServiceController controller) : base(serviceProvider)
        {
            _controller = controller;
            _account = p.ReadString();
        }

        public override async Task Execute()
        {
            LoggerManager.Info("Add kick player functionality.");
            await Task.CompletedTask;
        }
    }
}