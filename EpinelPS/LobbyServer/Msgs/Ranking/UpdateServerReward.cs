﻿using EpinelPS.Utils;

namespace EpinelPS.LobbyServer.Msgs.Ranking
{
    [PacketPath("/ranking/updateserverreward")]
    public class UpdateServerReward : LobbyMsgHandler
    {
        protected override async Task HandleAsync()
        {
            var req = await ReadData<ReqUpdateRankingServerReward>();
            var response = new ResUpdateRankingServerReward();


            await WriteDataAsync(response);
        }
    }
}
