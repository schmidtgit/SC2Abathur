using Abathur.Constants;
using Google.Protobuf.Collections;
using NydusNetwork;
using NydusNetwork.API.Protocol;
using NydusNetwork.Logging;
using NydusNetwork.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Abathur.Factory {
    public class EssenceFactory {
        private ILogger log;
        private const int TIMEOUT = 100000;
        public static Dictionary<uint, uint[]> UnitProducers = new Dictionary<uint, uint[]>()
    {
      {
        45U,
        new uint[3]{ 18U, 132U, 130U }
      },
      {
        48U,
        new uint[1]{ 21U }
      },
      {
        51U,
        new uint[1]{ 21U }
      },
      {
        49U,
        new uint[1]{ 21U }
      },
      {
        498U,
        new uint[1]{ 27U }
      },
      {
        53U,
        new uint[1]{ 27U }
      },
      {
        33U,
        new uint[1]{ 27U }
      },
      {
        692U,
        new uint[1]{ 27U }
      },
      {
        50U,
        new uint[1]{ 21U }
      },
      {
        484U,
        new uint[1]{ 29U }
      },
      {
        52U,
        new uint[1]{ 29U }
      },
      {
        35U,
        new uint[1]{ 28U }
      },
      {
        54U,
        new uint[1]{ 28U }
      },
      {
        689U,
        new uint[1]{ 28U }
      },
      {
        56U,
        new uint[1]{ 28U }
      },
      {
        55U,
        new uint[1]{ 28U }
      },
      {
        57U,
        new uint[1]{ 28U }
      },
      {
        18U,
        new uint[1]{ 45U }
      },
      {
        19U,
        new uint[1]{ 45U }
      },
      {
        20U,
        new uint[1]{ 45U }
      },
      {
        21U,
        new uint[1]{ 45U }
      },
      {
        27U,
        new uint[1]{ 45U }
      },
      {
        28U,
        new uint[1]{ 45U }
      },
      {
        22U,
        new uint[1]{ 45U }
      },
      {
        24U,
        new uint[1]{ 45U }
      },
      {
        25U,
        new uint[1]{ 45U }
      },
      {
        23U,
        new uint[1]{ 45U }
      },
      {
        26U,
        new uint[1]{ 45U }
      },
      {
        29U,
        new uint[1]{ 45U }
      },
      {
        30U,
        new uint[1]{ 45U }
      },
      {
        132U,
        new uint[1]{ 18U }
      },
      {
        130U,
        new uint[1]{ 18U }
      },
      {
        5U,
        new uint[3]{ 21U, 27U, 28U }
      },
      {
        37U,
        new uint[1]{ 21U }
      },
      {
        39U,
        new uint[1]{ 27U }
      },
      {
        41U,
        new uint[1]{ 28U }
      },
      {
        6U,
        new uint[3]{ 21U, 27U, 28U }
      },
      {
        38U,
        new uint[1]{ 21U }
      },
      {
        40U,
        new uint[1]{ 29U }
      },
      {
        42U,
        new uint[1]{ 28U }
      },
      {
        84U,
        new uint[1]{ 59U }
      },
      {
        10U,
        new uint[1]{ 59U }
      },
      {
        73U,
        new uint[2]{ 62U, 133U }
      },
      {
        74U,
        new uint[2]{ 62U, 133U }
      },
      {
        77U,
        new uint[2]{ 62U, 133U }
      },
      {
        311U,
        new uint[2]{ 62U, 133U }
      },
      {
        75U,
        new uint[2]{ 62U, 133U }
      },
      {
        76U,
        new uint[2]{ 62U, 133U }
      },
      {
        78U,
        new uint[1]{ 67U }
      },
      {
        495U,
        new uint[1]{ 67U }
      },
      {
        80U,
        new uint[1]{ 67U }
      },
      {
        496U,
        new uint[1]{ 67U }
      },
      {
        79U,
        new uint[1]{ 67U }
      },
      {
        82U,
        new uint[1]{ 71U }
      },
      {
        83U,
        new uint[1]{ 71U }
      },
      {
        81U,
        new uint[1]{ 71U }
      },
      {
        4U,
        new uint[1]{ 71U }
      },
      {
        694U,
        new uint[1]{ 71U }
      },
      {
        59U,
        new uint[1]{ 84U }
      },
      {
        60U,
        new uint[1]{ 84U }
      },
      {
        61U,
        new uint[1]{ 84U }
      },
      {
        62U,
        new uint[1]{ 84U }
      },
      {
        63U,
        new uint[1]{ 84U }
      },
      {
        72U,
        new uint[1]{ 84U }
      },
      {
        66U,
        new uint[1]{ 84U }
      },
      {
        71U,
        new uint[1]{ 84U }
      },
      {
        1910U,
        new uint[1]{ 84U }
      },
      {
        67U,
        new uint[1]{ 84U }
      },
      {
        68U,
        new uint[1]{ 84U }
      },
      {
        69U,
        new uint[1]{ 84U }
      },
      {
        65U,
        new uint[1]{ 84U }
      },
      {
        64U,
        new uint[1]{ 84U }
      },
      {
        70U,
        new uint[1]{ 84U }
      },
      {
        133U,
        new uint[1]{ 62U }
      },
      {
        141U,
        new uint[2]{ 75U, 76U }
      },
      {
        126U,
        new uint[3]{ 86U, 100U, 101U }
      },
      {
        104U,
        new uint[1]{ 151U }
      },
      {
        105U,
        new uint[1]{ 151U }
      },
      {
        107U,
        new uint[1]{ 151U }
      },
      {
        110U,
        new uint[1]{ 151U }
      },
      {
        111U,
        new uint[1]{ 151U }
      },
      {
        494U,
        new uint[1]{ 151U }
      },
      {
        109U,
        new uint[1]{ 151U }
      },
      {
        106U,
        new uint[1]{ 151U }
      },
      {
        108U,
        new uint[1]{ 151U }
      },
      {
        112U,
        new uint[1]{ 151U }
      },
      {
        499U,
        new uint[1]{ 151U }
      },
      {
        129U,
        new uint[1]{ 106U }
      },
      {
        688U,
        new uint[1]{ 110U }
      },
      {
        9U,
        new uint[1]{ 105U }
      },
      {
        114U,
        new uint[1]{ 112U }
      },
      {
        502U,
        new uint[1]{ 107U }
      },
      {
        86U,
        new uint[1]{ 104U }
      },
      {
        98U,
        new uint[1]{ 104U }
      },
      {
        99U,
        new uint[1]{ 104U }
      },
      {
        88U,
        new uint[1]{ 104U }
      },
      {
        89U,
        new uint[1]{ 104U }
      },
      {
        90U,
        new uint[1]{ 104U }
      },
      {
        97U,
        new uint[1]{ 104U }
      },
      {
        96U,
        new uint[1]{ 104U }
      },
      {
        91U,
        new uint[1]{ 104U }
      },
      {
        94U,
        new uint[1]{ 104U }
      },
      {
        92U,
        new uint[1]{ 104U }
      },
      {
        95U,
        new uint[1]{ 104U }
      },
      {
        93U,
        new uint[1]{ 104U }
      },
      {
        100U,
        new uint[1]{ 86U }
      },
      {
        101U,
        new uint[1]{ 100U }
      },
      {
        102U,
        new uint[1]{ 92U }
      }
    };
        public static Dictionary<uint, uint[]> UnitRequiredBuildings = new Dictionary<uint, uint[]>()
    {
      {
        21U,
        new uint[1]{ 19U }
      },
      {
        50U,
        new uint[1]{ 26U }
      },
      {
        27U,
        new uint[1]{ 21U }
      },
      {
        26U,
        new uint[1]{ 21U }
      },
      {
        24U,
        new uint[1]{ 21U }
      },
      {
        132U,
        new uint[1]{ 21U }
      },
      {
        52U,
        new uint[1]{ 29U }
      },
      {
        29U,
        new uint[1]{ 27U }
      },
      {
        28U,
        new uint[1]{ 27U }
      },
      {
        57U,
        new uint[1]{ 30U }
      },
      {
        30U,
        new uint[1]{ 28U }
      },
      {
        130U,
        new uint[1]{ 22U }
      },
      {
        25U,
        new uint[1]{ 22U }
      },
      {
        23U,
        new uint[1]{ 22U }
      },
      {
        62U,
        new uint[1]{ 59U }
      },
      {
        63U,
        new uint[1]{ 59U }
      },
      {
        74U,
        new uint[1]{ 72U }
      },
      {
        77U,
        new uint[1]{ 72U }
      },
      {
        311U,
        new uint[1]{ 72U }
      },
      {
        75U,
        new uint[1]{ 68U }
      },
      {
        76U,
        new uint[1]{ 69U }
      },
      {
        4U,
        new uint[1]{ 70U }
      },
      {
        694U,
        new uint[1]{ 70U }
      },
      {
        79U,
        new uint[1]{ 64U }
      },
      {
        10U,
        new uint[1]{ 64U }
      },
      {
        496U,
        new uint[1]{ 64U }
      },
      {
        66U,
        new uint[1]{ 63U }
      },
      {
        71U,
        new uint[1]{ 72U }
      },
      {
        67U,
        new uint[1]{ 72U }
      },
      {
        65U,
        new uint[1]{ 72U }
      },
      {
        72U,
        new uint[1]{ 62U }
      },
      {
        70U,
        new uint[1]{ 71U }
      },
      {
        64U,
        new uint[1]{ 67U }
      },
      {
        68U,
        new uint[1]{ 65U }
      },
      {
        69U,
        new uint[1]{ 65U }
      },
      {
        105U,
        new uint[1]{ 89U }
      },
      {
        126U,
        new uint[1]{ 89U }
      },
      {
        100U,
        new uint[1]{ 89U }
      },
      {
        97U,
        new uint[1]{ 89U }
      },
      {
        96U,
        new uint[1]{ 89U }
      },
      {
        98U,
        new uint[1]{ 89U }
      },
      {
        99U,
        new uint[1]{ 89U }
      },
      {
        110U,
        new uint[1]{ 97U }
      },
      {
        688U,
        new uint[1]{ 97U }
      },
      {
        9U,
        new uint[1]{ 96U }
      },
      {
        111U,
        new uint[1]{ 94U }
      },
      {
        494U,
        new uint[1]{ 94U }
      },
      {
        101U,
        new uint[1]{ 94U }
      },
      {
        107U,
        new uint[2]{ 91U, 502U }
      },
      {
        504U,
        new uint[1]{ 91U }
      },
      {
        502U,
        new uint[1]{ 504U }
      },
      {
        108U,
        new uint[2]{ 92U, 102U }
      },
      {
        112U,
        new uint[2]{ 92U, 102U }
      },
      {
        114U,
        new uint[1]{ 102U }
      },
      {
        109U,
        new uint[1]{ 93U }
      },
      {
        89U,
        new uint[3]{ 86U, 100U, 101U }
      },
      {
        90U,
        new uint[3]{ 86U, 100U, 101U }
      },
      {
        129U,
        new uint[2]{ 100U, 101U }
      },
      {
        91U,
        new uint[2]{ 100U, 101U }
      },
      {
        94U,
        new uint[2]{ 100U, 101U }
      },
      {
        92U,
        new uint[2]{ 100U, 101U }
      },
      {
        95U,
        new uint[2]{ 100U, 101U }
      },
      {
        499U,
        new uint[1]{ 101U }
      },
      {
        93U,
        new uint[1]{ 101U }
      },
      {
        102U,
        new uint[1]{ 101U }
      },
      {
        142U,
        new uint[1]{ 95U }
      }
    };
        public static Dictionary<uint, uint> ResearchProducers = new Dictionary<uint, uint>()
    {
      {
        7U,
        22U
      },
      {
        8U,
        22U
      },
      {
        9U,
        22U
      },
      {
        11U,
        22U
      },
      {
        12U,
        22U
      },
      {
        13U,
        22U
      },
      {
        5U,
        22U
      },
      {
        6U,
        22U
      },
      {
        10U,
        22U
      },
      {
        30U,
        29U
      },
      {
        31U,
        29U
      },
      {
        32U,
        29U
      },
      {
        36U,
        29U
      },
      {
        37U,
        29U
      },
      {
        38U,
        29U
      },
      {
        27U,
        29U
      },
      {
        28U,
        29U
      },
      {
        29U,
        29U
      },
      {
        33U,
        29U
      },
      {
        34U,
        29U
      },
      {
        35U,
        29U
      },
      {
        17U,
        37U
      },
      {
        16U,
        37U
      },
      {
        19U,
        39U
      },
      {
        15U,
        37U
      },
      {
        122U,
        39U
      },
      {
        98U,
        39U
      },
      {
        136U,
        41U
      },
      {
        140U,
        41U
      },
      {
        20U,
        41U
      },
      {
        22U,
        41U
      },
      {
        21U,
        41U
      },
      {
        24U,
        41U
      },
      {
        76U,
        30U
      },
      {
        25U,
        26U
      },
      {
        39U,
        63U
      },
      {
        40U,
        63U
      },
      {
        41U,
        63U
      },
      {
        42U,
        63U
      },
      {
        43U,
        63U
      },
      {
        44U,
        63U
      },
      {
        45U,
        63U
      },
      {
        46U,
        63U
      },
      {
        47U,
        63U
      },
      {
        78U,
        72U
      },
      {
        79U,
        72U
      },
      {
        80U,
        72U
      },
      {
        81U,
        72U
      },
      {
        82U,
        72U
      },
      {
        83U,
        72U
      },
      {
        85U,
        72U
      },
      {
        84U,
        72U
      },
      {
        48U,
        70U
      },
      {
        49U,
        70U
      },
      {
        50U,
        70U
      },
      {
        86U,
        65U
      },
      {
        87U,
        65U
      },
      {
        130U,
        65U
      },
      {
        99U,
        64U
      },
      {
        1U,
        64U
      },
      {
        52U,
        68U
      },
      {
        141U,
        69U
      },
      {
        53U,
        90U
      },
      {
        54U,
        90U
      },
      {
        55U,
        90U
      },
      {
        59U,
        90U
      },
      {
        60U,
        90U
      },
      {
        61U,
        90U
      },
      {
        56U,
        90U
      },
      {
        57U,
        90U
      },
      {
        58U,
        90U
      },
      {
        68U,
        92U
      },
      {
        69U,
        92U
      },
      {
        70U,
        92U
      },
      {
        71U,
        92U
      },
      {
        72U,
        92U
      },
      {
        73U,
        92U
      },
      {
        4U,
        93U
      },
      {
        75U,
        96U
      },
      {
        2U,
        97U
      },
      {
        3U,
        97U
      },
      {
        62U,
        86U
      },
      {
        64U,
        86U
      },
      {
        63U,
        86U
      },
      {
        135U,
        91U
      },
      {
        66U,
        89U
      },
      {
        65U,
        89U
      },
      {
        74U,
        94U
      },
      {
        101U,
        94U
      }
    };
        public static Dictionary<uint, uint> ResearchRequiredBuildings = new Dictionary<uint, uint>()
    {
      {
        8U,
        29U
      },
      {
        9U,
        29U
      },
      {
        12U,
        29U
      },
      {
        13U,
        29U
      },
      {
        98U,
        29U
      },
      {
        40U,
        65U
      },
      {
        41U,
        65U
      },
      {
        43U,
        65U
      },
      {
        44U,
        65U
      },
      {
        46U,
        65U
      },
      {
        47U,
        65U
      },
      {
        79U,
        64U
      },
      {
        80U,
        64U
      },
      {
        82U,
        64U
      },
      {
        83U,
        64U
      },
      {
        54U,
        100U
      },
      {
        60U,
        100U
      },
      {
        69U,
        100U
      },
      {
        57U,
        100U
      },
      {
        72U,
        100U
      },
      {
        2U,
        100U
      },
      {
        75U,
        100U
      },
      {
        63U,
        100U
      },
      {
        3U,
        100U
      },
      {
        55U,
        101U
      },
      {
        61U,
        101U
      },
      {
        70U,
        101U
      },
      {
        58U,
        101U
      },
      {
        73U,
        101U
      },
      {
        65U,
        101U
      }
    };
        public static Dictionary<uint, uint> ResearchRequiredResearch = new Dictionary<uint, uint>()
    {
      {
        8U,
        7U
      },
      {
        9U,
        8U
      },
      {
        12U,
        11U
      },
      {
        13U,
        12U
      },
      {
        40U,
        39U
      },
      {
        41U,
        40U
      },
      {
        46U,
        45U
      },
      {
        47U,
        46U
      },
      {
        79U,
        78U
      },
      {
        80U,
        79U
      },
      {
        82U,
        81U
      },
      {
        83U,
        82U
      },
      {
        54U,
        53U
      },
      {
        55U,
        54U
      },
      {
        60U,
        59U
      },
      {
        61U,
        60U
      },
      {
        69U,
        68U
      },
      {
        70U,
        69U
      },
      {
        57U,
        56U
      },
      {
        58U,
        57U
      },
      {
        72U,
        71U
      },
      {
        73U,
        72U
      }
    };

        public EssenceFactory(ILogger log = null) => this.log = log;

        public Essence FetchFromClient(GameSettings settings) {
            this.log?.LogWarning((object)"\tFetching data from Client (THIS MIGHT TAKE A WHILE)");
            GameSettings settingsCopy = (GameSettings)settings.Clone();
            GameClient gameClient = new GameClient(settingsCopy);
            gameClient.Initialize(true);
            this.log?.LogSuccess((object)"\tSuccessfully connected to client.");
            Response r;
            if (!gameClient.TryWaitCreateGameRequest(out r, new int?(100000)))
                this.TerminateWithMessage("Failed to create game");
            if (r.CreateGame.Error == ResponseCreateGame.Types.Error.Unset) {
                this.log?.LogSuccess((object)string.Format("\tSuccessfully created game with {0}", (object)settingsCopy.GameMap));
            } else {
                this.log?.LogWarning((object)string.Format("\tFailed with {0} on {1}.", (object)r.JoinGame.Error, (object)settingsCopy.GameMap));
                if (!gameClient.TryWaitAvailableMapsRequest(out r, new int?(100000)))
                    this.TerminateWithMessage("Failed to fetch available maps");
                else
                    settingsCopy.GameMap = r.AvailableMaps.BattlenetMapNames.First<string>();
                if (!gameClient.TryWaitCreateGameRequest(out r, new int?(100000)))
                    this.TerminateWithMessage("Failed to create game");
                if (r.JoinGame.Error == ResponseJoinGame.Types.Error.Unset)
                    this.log?.LogSuccess((object)string.Format("\tSuccessfully created game with {0}", (object)settingsCopy.GameMap));
            }
            if (!gameClient.TryWaitJoinGameRequest(out r, new int?(100000)))
                this.TerminateWithMessage("Failed to join game");
            this.log?.LogSuccess((object)"\tSuccessfully joined game.");
            if (!gameClient.TryWaitPingRequest(out r, new int?(100000)))
                this.TerminateWithMessage("Failed to ping client");
            this.log?.LogSuccess((object)string.Format("\tSuccessfully pinged client - requesting databuild {0}", (object)r.Ping.DataBuild));
            string dataVersion = r.Ping.DataVersion;
            int dataBuild = (int)r.Ping.DataBuild;
            if (!gameClient.TryWaitDataRequest(out r, new int?(100000)))
                this.TerminateWithMessage("Failed to receive data");
            this.log?.LogSuccess((object)"\tSuccessfully received data.");
            this.log?.LogWarning((object)"\tManipulating cost of morphed units in UnitType data!");
            ResponseData responseData = this.ManipulateMorphedUnitCost(r.Data);
            if (!gameClient.TryWaitLeaveGameRequest(out r, new int?(100000)))
                this.TerminateWithMessage("Failed to leave StarCraft II game");
            this.log?.LogSuccess((object)"\tSuccessfully left game.");
            gameClient.Disconnect();
            this.log?.LogSuccess((object)"\tDisconnected from client.");
            return new Essence() {
                DataVersion = dataVersion,
                DataBuild = dataBuild,
                Abilities = {
          (IEnumerable<AbilityData>) responseData.Abilities
        },
                Buffs = {
          (IEnumerable<BuffData>) responseData.Buffs
        },
                UnitTypes = {
          (IEnumerable<UnitTypeData>) responseData.Units
        },
                Upgrades = {
          (IEnumerable<UpgradeData>) responseData.Upgrades
        },
                UnitProducers = {
          (IEnumerable<MultiValuePair>) this.CreateValuePair((IDictionary<uint, uint[]>) EssenceFactory.UnitProducers)
        },
                UnitRequiredBuildings = {
          (IEnumerable<MultiValuePair>) this.CreateValuePair((IDictionary<uint, uint[]>) EssenceFactory.UnitRequiredBuildings)
        },
                ResearchProducer = {
          (IEnumerable<ValuePair>) this.CreateValuePair((IDictionary<uint, uint>) EssenceFactory.ResearchProducers)
        },
                ResearchRequiredBuildings = {
          (IEnumerable<ValuePair>) this.CreateValuePair((IDictionary<uint, uint>) EssenceFactory.ResearchRequiredBuildings)
        },
                ResearchRequiredResearch = {
          (IEnumerable<ValuePair>) this.CreateValuePair((IDictionary<uint, uint>) EssenceFactory.ResearchRequiredResearch)
        }
            };
        }

        private ResponseData ManipulateMorphedUnitCost(ResponseData data) {
            RepeatedField<UnitTypeData> source = data.Units.Clone();
            foreach (UnitTypeData unit in data.Units) {
                if (GameConstants.IsMorphed(unit.UnitId)) {
                    if (unit.UnitId == 105U) {
                        unit.MineralCost *= 2U;
                        unit.VespeneCost *= 2U;
                        unit.FoodRequired *= 2f;
                    } else {
                        uint[] p;
                        if (EssenceFactory.UnitProducers.TryGetValue(unit.UnitId, out p)) {
                            UnitTypeData unitTypeData = source.FirstOrDefault<UnitTypeData>((Func<UnitTypeData, bool>)(u => (int)u.UnitId == (int)((IEnumerable<uint>)p).FirstOrDefault<uint>()));
                            if (unitTypeData != null) {
                                unit.MineralCost -= unitTypeData.MineralCost;
                                unit.VespeneCost -= unitTypeData.VespeneCost;
                                unit.FoodRequired -= unitTypeData.FoodRequired;
                            }
                        }
                    }
                }
            }
            return data;
        }

        public ICollection<MultiValuePair> CreateValuePair(
          IDictionary<uint, uint[]> dictionary) {
            List<MultiValuePair> multiValuePairList = new List<MultiValuePair>();
            foreach (KeyValuePair<uint, uint[]> keyValuePair in (IEnumerable<KeyValuePair<uint, uint[]>>)dictionary)
                multiValuePairList.Add(new MultiValuePair() {
                    Key = keyValuePair.Key,
                    Values = {
            (IEnumerable<uint>) keyValuePair.Value
          }
                });
            return (ICollection<MultiValuePair>)multiValuePairList;
        }

        public ICollection<ValuePair> CreateValuePair(
          IDictionary<uint, uint> dictionary) {
            List<ValuePair> valuePairList = new List<ValuePair>();
            foreach (KeyValuePair<uint, uint> keyValuePair in (IEnumerable<KeyValuePair<uint, uint>>)dictionary)
                valuePairList.Add(new ValuePair() {
                    Key = keyValuePair.Key,
                    Value = keyValuePair.Value
                });
            return (ICollection<ValuePair>)valuePairList;
        }

        private void TerminateWithMessage(string message) {
            this.log?.LogError((object)string.Format("{0} - PRESS ANY KEY TO TERMINATE", (object)message));
            Console.ReadKey();
            Environment.Exit(-1);
        }
    }
}
