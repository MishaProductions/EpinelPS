﻿using EpinelPS.Utils;

namespace EpinelPS.LobbyServer.Msgs.Badge
{
    [PacketPath("/badge/delete")]
    public class DeleteBadge : LobbyMsgHandler
    {
        protected override async Task HandleAsync()
        {
            var req = await ReadData<ReqDeleteBadge>();

            var response = new ResDeleteBadge();

            await WriteDataAsync(response);
        }
    }
}
