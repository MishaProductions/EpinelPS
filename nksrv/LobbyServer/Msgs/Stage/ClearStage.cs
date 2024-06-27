﻿using nksrv.Utils;
using Swan.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nksrv.LobbyServer.Msgs.Stage
{
    [PacketPath("/stage/clearstage")]
    public class ClearStage : LobbyMsgHandler
    {
        protected override async Task HandleAsync()
        {
            var req = await ReadData<ReqClearStage>();

            var response = new ResClearStage();
            var user = GetUser();

            // TOOD: save to user info
            Console.WriteLine($"Stage " + req.StageId + " completed, result is " + req.BattleResult);

            if (req.BattleResult == 1)
            {
                user.LastStageCleared = req.StageId;

                if (user.FieldInfo.Count == 0)
                {
                    user.FieldInfo.Add(0, new FieldInfo() { });
                }

                // TODO: figure out how stageid corresponds to chapter
                user.FieldInfo[GetChapterForStageId(req.StageId)].CompletedStages.Add(new NetFieldStageData() { StageId = req.StageId });
                JsonDb.Save();

                if (req.StageId == 6000003)
                {
                    // TODO: Is this the right place to copy over default characters?
                    // TODO: What is CSN and TID? Also need to add names for these
                    // Note: CSN appears to be a character ID, still not sure what TID is
                    user.Characters.Add(new Utils.Character() { Csn = 47263455, Tid = 201001 });
                    user.Characters.Add(new Utils.Character() { Csn = 47273456, Tid = 330501 });
                    user.Characters.Add(new Utils.Character() { Csn = 47263457, Tid = 130201 });
                    user.Characters.Add(new Utils.Character() { Csn = 47263458, Tid = 230101 });
                    user.Characters.Add(new Utils.Character() { Csn = 47263459, Tid = 301201 });

                    user.TeamData.TeamNumber = 1;
                    user.TeamData.TeamCombat = 1446; // TODO: Don't hardcode this
                    user.TeamData.Slots.Clear();
                    user.TeamData.Slots.Add(new NetWholeTeamSlot { Slot = 1, Csn = 47263455, Tid = 201001, Lvl = 1});
                    user.TeamData.Slots.Add(new NetWholeTeamSlot { Slot = 2, Csn = 47273456, Tid = 330501, Lvl = 1 });
                    user.TeamData.Slots.Add(new NetWholeTeamSlot { Slot = 3, Csn = 47263457, Tid = 130201, Lvl = 1 });
                    user.TeamData.Slots.Add(new NetWholeTeamSlot { Slot = 4, Csn = 47263458, Tid = 230101, Lvl = 1 });
                    user.TeamData.Slots.Add(new NetWholeTeamSlot { Slot = 5, Csn = 47263459, Tid = 301201, Lvl = 1 });
                }
            }

            WriteData(response);
        }

        public static int GetChapterForStageId(int stageId)
        {
            if (6000001 <= stageId && stageId <= 6000003)
            {
                return 0;
            }
            else if (6001001 <= stageId && stageId <= 6001004)
            {
                return 1;
            }
            else
            {
                Logger.Error("Unknown stage id: " + stageId);
                return 100;
            }
        }
    }
}
