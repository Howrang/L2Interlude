﻿using Core.Attributes;
using Core.Module.Handlers;
using Core.Module.Player;
using Core.NetworkPacket.ServerPacket;


//CLR: 4.0.30319.42000
//USER: GL
//DATE: 10.08.2024 23:31:36

namespace Core.Module.Handlers.Chat
{
    [ChatType(Type = ChatType.GENERAL)]
    class ChatGeneral : AbstractChatMessage
    {
        protected internal override void Chat(PlayerInstance player, ChatType chatType, string text, string paramsValue)
        {

            if (!commonChecksChat(player, chatType, text, paramsValue))
            {
                return;
            }

            player.SendPacketAsync(new Say2(player, chatType, text));
            foreach (PlayerInstance targetInstance in Initializer.WorldInit().GetVisiblePlayers(player))
            {

                targetInstance.SendPacketAsync(new Say2(player, chatType, text));
            }

        }
    }
}
