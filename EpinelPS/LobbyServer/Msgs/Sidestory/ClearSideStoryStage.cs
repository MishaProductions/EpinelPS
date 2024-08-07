﻿using EpinelPS.Database;
using EpinelPS.LobbyServer.Msgs.Stage;
using EpinelPS.StaticInfo;
using EpinelPS.Utils;

namespace EpinelPS.LobbyServer.Msgs.Sidestory
{
    [PacketPath("/sidestory/stage/clear")]
    public class ClearSideStoryStage : LobbyMsgHandler
    {
        protected override async Task HandleAsync()
        {
            var req = await ReadData<ReqClearSideStoryStage>();
            var user = GetUser();

            var response = new ResClearSideStoryStage();

            if (!user.CompletedSideStoryStages.Contains(req.SideStoryStageId))
            {
                user.CompletedSideStoryStages.Add(req.SideStoryStageId);

                if (GameData.Instance.SidestoryRewardTable.ContainsKey(req.SideStoryStageId))
                {
                    var rewardData = GameData.Instance.GetRewardTableEntry(GameData.Instance.SidestoryRewardTable[req.SideStoryStageId]);

                    if (rewardData != null)
                        response.Reward = ClearStage.RegisterRewardsForUser(user, rewardData);
                    else
                        throw new Exception("failed to find reward");
                }

                JsonDb.Save();
            }


            await WriteDataAsync(response);
        }
    }
}
