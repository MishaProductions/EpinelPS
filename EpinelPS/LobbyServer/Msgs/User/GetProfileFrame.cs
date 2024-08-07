﻿using EpinelPS.Utils;

namespace EpinelPS.LobbyServer.Msgs.User
{
    [PacketPath("/User/GetProfileFrame")]
    public class GetProfileFrame : LobbyMsgHandler
    {
        protected override async Task HandleAsync()
        {
            var req = await ReadData<ReqGetProfileFrame>();
            var response = new ResGetProfileFrame();

            // TODO

            await WriteDataAsync(response);
        }
    }
}
