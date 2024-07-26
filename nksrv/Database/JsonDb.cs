﻿using Newtonsoft.Json;
using nksrv.LobbyServer;
using nksrv.StaticInfo;
using nksrv.Utils;
using Swan.Logging;

namespace nksrv.Database
{
    public class AccessToken
    {
        public string Token = "";
        public long ExpirationTime;
        public ulong UserID;
    }
    public class FieldInfo
    {
        public List<NetFieldStageData> CompletedStages = [];
        public List<NetFieldObject> CompletedObjects = [];
    }

    public class FieldInfoNew
    {
        public List<int> CompletedStages = [];
        public List<NetFieldObject> CompletedObjects = [];
    }

    public class Character
    {
        public int Csn = 0;
        public int Tid = 0;
        public int CostumeId = 0;

        public int Level = 1;
        public int UltimateLevel = 1;
        public int Skill1Lvl = 1;
        public int Skill2Lvl = 1;
        public int Grade = 0;
    }
    public class MainQuestData
    {
        public int TableId = 0;
        public bool IsReceieved = false;
    }

    public class UserPointData
    {
        public int UserLevel = 1;
        public int ExperiencePoint = 0;
    }

    public class ItemData
    {
        public int ItemType;
        public long Csn;
        public int Count;
        public int Level;
        public int Exp;
        public int Position;
        public int Corp;
        public long Isn;
    }
    public class EventData
    {
        public List<string> CompletedScenarios = new();
    }

    public class User
    {
        // User info
        public string Username = "";
        public string Password = "";
        public string PlayerName = "";
        public ulong ID;
        public long RegisterTime;
        public int LastNormalStageCleared;
        public int LastHardStageCleared;
        public string Nickname = "SomePlayer";
        public int ProfileIconId = 39900;
        public bool ProfileIconIsPrism = false;
        public bool IsAdmin = false;

        public bool IsBanned = false;
        public DateTime BanStart;
        public DateTime BanEnd;
        public int BanId = 0;

        // Game data
        public List<string> CompletedScenarios = [];
        public Dictionary<string, FieldInfo> FieldInfo = []; // here for backwards compatibility

        public Dictionary<string, FieldInfoNew> FieldInfoNew = [];
        public Dictionary<string, string> MapJson = [];
        public Dictionary<CurrencyType, long> Currency = new() {
            { CurrencyType.ContentStamina, 2 }
        };

        public List<ItemData> Items = new();
        public List<Character> Characters = [];
        public NetWholeUserTeamData RepresentationTeamData = new();
        public Dictionary<int, ClearedTutorialData> ClearedTutorialData = [];
        public NetWallpaperData[] WallpaperList = [];
        public Dictionary<int, NetUserTeamData> UserTeams = new Dictionary<int, NetUserTeamData>();
        public Dictionary<int, bool> MainQuestData = new();
        public int InfraCoreExp = 0;
        public int InfraCoreLvl = 1;
        public UserPointData userPointData = new();
        public DateTime LastLogin = DateTime.UtcNow;
        public DateTime BattleTime = DateTime.UtcNow;

        public NetOutpostBattleLevel OutpostBattleLevel = new() { Level = 1 };
        public int GachaTutorialPlayCount = 0;
        public List<int> CompletedTacticAcademyLessons = [];
        public List<int> CompletedSideStoryStages = new();

        // Event data
        public Dictionary<int, EventData> EventInfo = new();

        public void SetQuest(int tid, bool recievedReward)
        {
            if (MainQuestData.ContainsKey(tid))
            {
                MainQuestData[tid] = recievedReward;
                return;
            }
            else
            {
                MainQuestData.Add(tid, recievedReward);
            }
        }

        public int GenerateUniqueItemId()
        {
            var num = Rng.RandomId();

            while (Items.Any(x => x.Isn == num))
            {
                num = Rng.RandomId();
            }

            return num;
        }

        public bool IsStageCompleted(int id, bool isNorm)
        {
            foreach (var item in FieldInfoNew)
            {
                if (item.Key.Contains("hard") && isNorm) continue;
                if (item.Key.Contains("normal") && !isNorm) continue;
                if (item.Value.CompletedStages.Contains(id))
                {
                    return true;
                }
            }

            return false;
        }

        public long GetCurrencyVal(CurrencyType type)
        {
            if (Currency.ContainsKey(type))
                return Currency[type];
            else
            {
                Currency.Add(type, 0);
                return 0;
            }
        }
        public void AddCurrency(CurrencyType type, long val)
        {
            if (Currency.ContainsKey(type)) Currency[type] += val;
            else Currency.Add(type, val);
        }
        public bool SubtractCurrency(CurrencyType type, long val)
        {
            if (Currency.ContainsKey(type)) Currency[type] -= val;
            else return false;

            if (Currency[type] < 0)
            {
                Currency[type] += val;
                return false;
            }
            return true;
        }
        public bool CanSubtractCurrency(CurrencyType type, long val)
        {
            if (Currency.ContainsKey(type))
            {
                if (Currency[type] >= val) return true;
                else return false;
            }
            else
            {
                if (val == 0) return true;
                else return false;
            }
        }
    }
    public class CoreInfo
    {
        public int DbVersion = 3;
        public List<User> Users = [];

        public List<AccessToken> LauncherAccessTokens = [];

        public Dictionary<string, GameClientInfo> GameClientTokens = [];
        public string ServerName = "<color=\"green\">Private Server</color>";
    }
    internal class JsonDb
    {
        public static CoreInfo Instance { get; internal set; }

        // Note: change this in sodium
        public static byte[] ServerPrivateKey = Convert.FromBase64String("FSUY8Ohd942n5LWAfxn6slK3YGwc8OqmyJoJup9nNos=");
        public static byte[] ServerPublicKey = Convert.FromBase64String("04hFDd1e/BOEF2h4b0MdkX2h6W5REeqyW+0r9+eSeh0=");

        static JsonDb()
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/db.json"))
            {
                "users: warning: configuration not found, writing default data".Warn();
                Instance = new CoreInfo();
                Save();
            }

            var j = JsonConvert.DeserializeObject<CoreInfo>(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/db.json"));
            if (j != null)
            {
                Instance = j;

                if (Instance.DbVersion == 0)
                {
                    Instance.DbVersion = 1;
                    // In older versions, field info key used chapter number, but now difficultly is appened.
                    Console.WriteLine("Starting database update...");

                    foreach (var user in Instance.Users)
                    {
                        foreach (var f in user.FieldInfoNew.ToList())
                        {
                            var isNumeric = int.TryParse(f.Key, out int n);
                            if (isNumeric)
                            {
                                var val = f.Value;
                                user.FieldInfoNew.Remove(f.Key);
                                user.FieldInfoNew.Add(n + "_Normal", val);
                            }
                        }
                    }
                    Console.WriteLine("Database update completed");
                }
                else if (Instance.DbVersion == 1)
                {
                    Console.WriteLine("Starting database update...");
                    // there was a bug where equipment position was not saved, so remove all items from each characters
                    Instance.DbVersion = 2;
                    foreach (var user in Instance.Users)
                    {
                        foreach (var f in user.Items.ToList())
                        {
                            f.Csn = 0;
                        }
                    }
                    Console.WriteLine("Database update completed");
                }
                else if (Instance.DbVersion == 2)
                {
                    Console.WriteLine("Starting database update...");
                    // I used to use a class for FieldInfo cleared stages, but now int list is used
                    Instance.DbVersion = 3;
                    foreach (var user in Instance.Users)
                    {
                        foreach (var f in user.FieldInfo)
                        {
                            var newField = new FieldInfoNew();
                            foreach (var stage in f.Value.CompletedStages)
                            {
                                newField.CompletedStages.Add(stage.StageId);
                            }
                            user.FieldInfoNew.Add(f.Key, newField);
                        }
                        user.FieldInfo.Clear();
                    }
                    Console.WriteLine("Database update completed");
                }
                Save();

                ValidateDb();
                "Loaded db".Info();
            }
            else
            {
                throw new Exception("Failed to read configuration json file");
            }
        }

        private static void ValidateDb()
        {
            // check if character level is valid
            foreach (var item in Instance.Users)
            {
                foreach (var c in item.Characters)
                {
                    if (c.Level > 1000)
                    {
                        $"Warning: Character level for character {c.Tid} cannot be above 1000, setting to 1000".Warn();
                        c.Level = 1000;
                    }
                }
            }
        }

        public static User? GetUser(ulong id)
        {
            return Instance.Users.Where(x => x.ID == id).FirstOrDefault();
        }
        public static void Save()
        {
            if (Instance != null)
            {
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "/db.json", JsonConvert.SerializeObject(Instance, Formatting.Indented));
            }
        }
    }
}
