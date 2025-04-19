using System;
using System.Threading.Tasks;
using Core.Controller;
using DataBase.Interfaces;
using L2Logger;
using Network;
using Security;

namespace Core.NetworkPacket.ClientPacket.LoginServicePacket
{
    public class LoginServAcceptPlayer : PacketBase
    {
        private readonly LoginServiceController _controller;
        private readonly int _accountId;
        private readonly IAccountRepository _accountRepository;
        private readonly SessionKey _sessionKey;

        public LoginServAcceptPlayer(IServiceProvider serviceProvider, Packet p, LoginServiceController controller) : base(serviceProvider)
        {
            _controller = controller;
            _accountId = p.ReadInt();
            _sessionKey = new SessionKey(p.ReadInt(), p.ReadInt(), p.ReadInt(), p.ReadInt());
        }

        public override async Task Execute()
        {
            var account = await _accountRepository.GetByIdAsync(_accountId);
            _controller.AwaitAddAccount(account, _sessionKey);
        }
    }
}