using BrutalCompanyMinus;
using BrutalCompanyMinus.Minus;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using BrutalCompanyMinus.Minus.Handlers;
using BrutalCompanyMinus.Minus.MonoBehaviours;

namespace BCMHQModule.Patches
{
    /// <summary>
    /// Configuration lockdown patch
    /// </summary>
    [HarmonyPatch(typeof(Configuration))]
    public class LockConfigurationPatcher
    {
        [HarmonyPatch(nameof(Configuration.CreateConfig))]
        [HarmonyPostfix]
        public static void PostfixCreateConfig()
        {
            for (int i = 0; i < EventManager.events.Count; i++)
            {
                MEvent liveEvent = EventManager.events[i];
                if (liveEvent == null) continue;

                try
                {
                    Type eventType = liveEvent.GetType();
                    MEvent defaultTemplate = (MEvent)Activator.CreateInstance(liveEvent.GetType());
                    defaultTemplate.Initalize();

                    FieldInfo instanceField = eventType.GetField("Instance", BindingFlags.Public | BindingFlags.Static);
                    instanceField?.SetValue(null, liveEvent);

                    bool originalExecutedState = liveEvent.Executed;

                    liveEvent.Weight = defaultTemplate.Weight;
                    liveEvent.Enabled = defaultTemplate.Enabled;
                    liveEvent.ColorHex = defaultTemplate.ColorHex;
                    liveEvent.Type = defaultTemplate.Type;
                    liveEvent.Descriptions = [.. defaultTemplate.Descriptions];
                    liveEvent.EventsToRemove = [.. defaultTemplate.EventsToRemove];
                    liveEvent.EventsToSpawnWith = [.. defaultTemplate.EventsToSpawnWith];
                    liveEvent.ScaleList = new Dictionary<MEvent.ScaleType, MEvent.Scale>(defaultTemplate.ScaleList);
                    liveEvent.monsterEvents = [.. defaultTemplate.monsterEvents];
                    liveEvent.scrapTransmutationEvent = defaultTemplate.scrapTransmutationEvent;

                    liveEvent.Executed = originalExecutedState;
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError($"Failed to force defaults on event {liveEvent.Name()}: {ex.Message}");
                }
            }

            Configuration.useCustomWeights.Value = false;
            Configuration.eventsToSpawn = Helper.getScale("2, 0.03, 2.0, 5.0");
            Configuration.weightsForExtraEvents = Helper.ParseValuesFromString("40, 39, 15, 5, 1");
            Configuration.showEventsInChat.Value = false;
            Configuration.eventTypeScales =
            [
                Helper.getScale("5, 0.25, 5, 30"),
                Helper.getScale("40, -0.15, 25, 40"),
                Helper.getScale("10, -0.05, 5, 10"),
                Helper.getScale("23, -0.1, 13, 23"),
                Helper.getScale("3, 0.14, 3, 17"),
                Helper.getScale("15, -0.05, 10, 15")
            ];
            Configuration.difficultyTransitions = Helper.GetDifficultyTransitionsFromString("Easy,00FF00,0|Medium,008000,15|Hard,FF0000,30|Very Hard,800000,50|Insane,140000,75");
            Configuration.ignoreMaxCap.Value = false;
            Configuration.difficultyMaxCap.Value = 100.0f;
            Configuration.scaleByDaysPassed.Value = true;
            Configuration.daysPassedDifficultyMultiplier.Value = 1.0f;
            Configuration.daysPassedDifficultyCap.Value = 60.0f;
            Configuration.scaleByScrapInShip.Value = true;
            Configuration.scrapInShipDifficultyMultiplier.Value = 0.0025f;
            Configuration.scrapInShipDifficultyCap.Value = 30.0f;
            Configuration.scaleByQuota.Value = false;
            Configuration.quotaDifficultyMultiplier.Value = 0.005f;
            Configuration.quotaDifficultyCap.Value = 100f;
            Configuration.scaleByMoonGrade.Value = true;
            Configuration.gradeAdditives = Helper.GetMoonRiskFromString("D,-8|C,-8|B,-4|A,5|S,10|S+,15|S++,20|S+++,30|Other,10");
            Configuration.scaleByWeather.Value = false;
            Configuration.weatherAdditives = new Dictionary<LevelWeatherType, float>
            {
                { LevelWeatherType.None, 0.0f },
                { LevelWeatherType.Rainy, 2.0f },
                { LevelWeatherType.DustClouds, 2.0f },
                { LevelWeatherType.Flooded, 4.0f },
                { LevelWeatherType.Foggy, 4.0f },
                { LevelWeatherType.Stormy, 7.0f },
                { LevelWeatherType.Eclipsed, 7.0f },
            };
            Configuration.spawnChanceMultiplierScaling = Helper.getScale("1.0, 0.017, 1.0, 2.0");
            Configuration.insideSpawnChanceAdditive = Helper.getScale("0.0, 0.0, 0.0, 0.0");
            Configuration.outsideSpawnChanceAdditive = Helper.getScale("0.0, 0.0, 0.0, 0.0");
            Configuration.spawnCapMultiplier = Helper.getScale("1.0, 0.017, 1.0, 2.0");
            Configuration.insideEnemyMaxPowerCountScaling = Helper.getScale("0,0,0,0");
            Configuration.outsideEnemyPowerCountScaling = Helper.getScale("0,0,0,0");
            Configuration.enemyBonusHpScaling = Helper.getScale("0,0,0,0");
            Configuration.scrapValueMultiplier = Helper.getScale("1.0, 0.003, 1.0, 1.3");
            Configuration.scrapAmountMultiplier = Helper.getScale("1.0, 0.003, 1.0, 1.3");
            Configuration.goodEventIncrementMultiplier.Value = 1.0f;
            Configuration.badEventIncrementMultiplier.Value = 1.0f;

            Configuration.nutSlayerLives.Value = 5;
            Configuration.nutSlayerHp.Value = 6;
            Configuration.nutSlayerMovementSpeed.Value = 9.5f;
            Configuration.nutSlayerImmortal.Value = true;
            Assets.grabbableTurret.maxValue = 75;
            Assets.grabbableTurret.minValue = 50;
            Assets.grabbableLandmine.maxValue = 150;
            Assets.grabbableLandmine.minValue = 100;
            Configuration.slayerShotgunMinValue.Value = 200;
            Configuration.slayerShotgunMaxValue.Value = 300;

            Configuration.useWeatherMultipliers.Value = true;
            Configuration.randomizeWeatherMultipliers.Value = false;
            Configuration.enableTerminalText.Value = true;

            Configuration.weatherRandomRandomMinInclusive.Value = 0.9f;
            Configuration.weatherRandomRandomMaxInclusive.Value = 1.2f;

            Configuration.enableQuotaChanges.Value = false;

            Configuration.enableAllEnemies.Value = false;
            Configuration.enableAllAllEnemies.Value = false;

            Weather createNewWeatherSettings(LevelWeatherType levelWeatherType, float value, float amount)
            {
                return new Weather(levelWeatherType, value, amount);
            }

            Configuration.noneMultiplier = createNewWeatherSettings(LevelWeatherType.None, 1f, 1f);
            Configuration.dustCloudMultiplier = createNewWeatherSettings(LevelWeatherType.DustClouds, 1.05f, 1f);
            Configuration.rainyMultiplier = createNewWeatherSettings(LevelWeatherType.Rainy, 1.05f, 1f);
            Configuration.stormyMultiplier = createNewWeatherSettings(LevelWeatherType.Stormy, 1.35f, 1.2f);
            Configuration.foggyMultiplier = createNewWeatherSettings(LevelWeatherType.Foggy, 1.15f, 1.1f);
            Configuration.floodedMultiplier = createNewWeatherSettings(LevelWeatherType.Flooded, 1.25f, 1.15f);
            Configuration.eclipsedMultiplier = createNewWeatherSettings(LevelWeatherType.Eclipsed, 1.35f, 1.2f);

            foreach (KeyValuePair<int, LevelProperties> k in Configuration.levelProperties)
            {
                k.Value.minScrapAmount = Helper.getScale("1,0,1,1");
                k.Value.maxScrapAmount = Helper.getScale("1,0,1,1");
                k.Value.minScrapValue = Helper.getScale("1,0,1,1");
                k.Value.maxScrapValue = Helper.getScale("1,0,1,1");
            }

            FacilityGhost.actionCurrentTime = 15f;
            FacilityGhost.ghostCrazyActionInterval = 0.1f;
            FacilityGhost.ghostCrazyPeriod = 5f;
            FacilityGhost.crazyGhostChance = 0.1f;
            FacilityGhost.DoNothingWeight = 25;
            FacilityGhost.OpenCloseBigDoorsWeight = 20;
            FacilityGhost.MessWithLightsWeight = 16;
            FacilityGhost.MessWithBreakerWeight = 4;
            FacilityGhost.disableTurretsWeight = 5;
            FacilityGhost.disableLandminesWeight = 5;
            FacilityGhost.disableSpikeTrapsWeight = 5;
            FacilityGhost.turretRageWeight = 5;
            FacilityGhost.OpenCloseDoorsWeight = 9;
            FacilityGhost.lockUnlockDoorsWeight = 3;
            FacilityGhost.chanceToOpenCloseDoor = 0.3f;
            FacilityGhost.rageTurretsChance = 0.3f;

            RealityShift.normalScrapWeight = 85;
            RealityShift.grabbableLandmineWeight = 15;
            RealityShift.transmuteChance = 0.5f;
            RealityShift.enemyTeleportChance = 0.1f;

            DDay.bombardmentInterval = 100;
            DDay.bombardmentTime = 15;
            DDay.fireInterval = 1;
            DDay.fireAmount = 8;
            DDay.displayWarning = true;
            DDay.volume = 0.3f;
            ArtilleryShell.speed = 100f;

            // skipping Mimics config as that is not in vanilla bcm

            EventManager.disabledEvents.Clear();
            EventManager.allVeryBad.Clear();
            EventManager.allBad.Clear();
            EventManager.allNeutral.Clear();
            EventManager.allGood.Clear();
            EventManager.allVeryGood.Clear();
            EventManager.allRemove.Clear();

            List<MEvent> activeEvents = new List<MEvent>();
            foreach (MEvent e in EventManager.events)
            {
                if (!e.Enabled)
                {
                    EventManager.disabledEvents.Add(e);
                }
                else
                {
                    activeEvents.Add(e);
                    switch (e.Type)
                    {
                        case MEvent.EventType.VeryBad: EventManager.allVeryBad.Add(e); break;
                        case MEvent.EventType.Bad: EventManager.allBad.Add(e); break;
                        case MEvent.EventType.Neutral: EventManager.allNeutral.Add(e); break;
                        case MEvent.EventType.Good: EventManager.allGood.Add(e); break;
                        case MEvent.EventType.VeryGood: EventManager.allVeryGood.Add(e); break;
                        case MEvent.EventType.Remove: EventManager.allRemove.Add(e); break;
                    }
                }
            }
            EventManager.events = activeEvents;

            EventManager.UpdateEventTypeCounts();
            EventManager.UpdateAllEventWeights();
        }
    }
}
