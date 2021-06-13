using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace DiamondEngine 
{
    public class Counter
    {
        public enum CounterTypes
        {
            NONE,

            //Boons
            BOKATAN_RES, 
            WRECKER_RES, 
            CAD_BANE_SOH, 
            CAD_BANE_BOOTS, 

            //Enemies & Bosses
            ENEMY_BANTHA,
            ENEMY_STORMTROOPER,
            RANCOR,
            ENEMY_SKYTROOPER,
            ENEMY_LASER_TURRET,
            WAMPA,
            SKEL,
            ENEMY_DEATHTROOPER,
            ENEMY_HEAVYTROOPER,
            MOFFGIDEON,

            //Other
            LEVELS,
            RUN_COINS,
            MAX,
        }

        public int amount = 0;
        public CounterTypes type;
        public static int maxCombo = 0;
        public static bool isFinalScene = false;
        public static bool firstRun = true; // When we have save / load functionality, this should be in it
        public static bool firstTutorial = true;

        public Counter()
        {
            amount = 0;
            type = CounterTypes.NONE;
        }
        public Counter(int _amount, CounterTypes ty)
        {
            amount = _amount;
            type = ty;
        }

        public static Dictionary<CounterTypes, Counter> GameCounters = new Dictionary<CounterTypes, Counter>();
        public static void SumToCounterType(CounterTypes type)
        {
            if (GameCounters.ContainsKey(type))
            {
                GameCounters[type].amount += 1;
            }
            else 
            {
                Counter aux = new Counter(1, type);
                GameCounters.Add(type, aux);
            }
        }

        public static void ResetCounters()
        {
            GameCounters.Clear();
            maxCombo = 0;
            isFinalScene = false;
        }

        public static void DebugAllCounters()
        {
            //if(GameCounters.ContainsKey(CounterTypes.BOKATAN_RES))
            //    Debug.Log("Bokatan Res: " + GameCounters[CounterTypes.BOKATAN_RES].amount);
            
            //if (GameCounters.ContainsKey(CounterTypes.WRECKER_RES))
            //    Debug.Log("Wrecker Res: " + GameCounters[CounterTypes.WRECKER_RES].amount);

            //if (GameCounters.ContainsKey(CounterTypes.CAD_BANE_SOH))
            //    Debug.Log("Cad Bane Soh: " + GameCounters[CounterTypes.CAD_BANE_SOH].amount);

            //if (GameCounters.ContainsKey(CounterTypes.CAD_BANE_BOOTS))
            //    Debug.Log("Cad Bane Boots: " + GameCounters[CounterTypes.CAD_BANE_BOOTS].amount);
        }

        public enum GameResult
        {
            NONE,
            VICTORY,
            DEFEAT,
        }

        public static GameResult gameResult = GameResult.NONE;
    }
}
