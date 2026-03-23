using MelonLoader;
using ScheduleSpidey;
using UnityEngine;

[assembly: MelonInfo(typeof(Core), "ScheduleSpidey", "0.4.4 Alternate", "GUMGamesGG", null)]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace ScheduleSpidey
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("Initialized ScheduleSpidey!");
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName == "Main")
            {
                MelonLogger.Msg("Scene Main loaded");
                GameObject spideySpawner = GameObject.Find("SpideySpawner");
                if (spideySpawner == null)
                {
                    spideySpawner = new GameObject("SpideySpawner");
                    MelonLogger.Msg("SpideySpawner created");
                }
                else
                {

                    MelonLogger.Msg("SpideySpawner exists already.");
                }
                spideySpawner.AddComponent<SpideySpawner>();


            }
        }
    }
}
