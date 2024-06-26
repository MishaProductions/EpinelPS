﻿using nksrv.Utils;

namespace nksrv.LobbyServer.Msgs.Outpost
{
    [PacketPath("/mission/getrewarded/jukebox")]
    public class GetJukeboxRewards : LobbyMsgHandler
    {
        protected override async Task HandleAsync()
        {
            var req = ReadData<ReqGetJukeboxRewardedData>();

            // TODO: save these things
            var response = new ResGetJukeboxRewardedData();

            WriteData(response);
        }
    }
}
