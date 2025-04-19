using System;
using System.Threading.Tasks;
using Core.Controller;
using Network;

namespace Core.NetworkPacket.ClientPacket.LoginServicePacket
{
    public class LoginServLoginFail : PacketBase
    {
        private readonly LoginServiceController _controller;
        private readonly string _code;
        
        public LoginServLoginFail(IServiceProvider serviceProvider, Packet p, LoginServiceController controller) : base(serviceProvider)
        {
            _controller = controller;
            _code = p.ReadString();
        }

        public override async Task Execute()
        {
            _controller.LoginFail(_code);
            await Task.CompletedTask;
        }
    }
}