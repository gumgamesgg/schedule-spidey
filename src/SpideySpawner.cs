using System.Collections;
using MelonLoader;
using MelonLoader.Utils;
using ScheduleOne.AvatarFramework;
using ScheduleOne.NPCs;
using UnityEngine;

namespace ScheduleSpidey;

public class SpideySpawner: MonoBehaviour
{
    private AssetBundle m_Bundle;
    private GameObject m_Spidey;
    private Material m_SpideyMat;
    private void Awake()
    {

        string bundlePath = Path.Combine(MelonEnvironment.ModsDirectory, "scheduleSpidey/assets/spidey");
        m_Bundle = AssetBundle.LoadFromFile(bundlePath);

        if (m_Bundle == null)
        {
            MelonLogger.Error("Could not load bundle");
            return;
        }

        m_Spidey = m_Bundle.LoadAsset<GameObject>("Spiderman_rigged");
        m_SpideyMat = m_Bundle.LoadAsset<Material>("Spiderman_mat");

        ImportUtils.FixMaterial(m_SpideyMat);

        m_Bundle.Unload(false);

    }


    private void Start()
    {
        StartCoroutine(CheckAndSpawnRoutine());

    }

    private IEnumerator CheckAndSpawnRoutine()
    {
        while (true)
        {
            NPC[] npcs = FindObjectsOfType<NPC>();
            if (npcs.Length > 0)
            {
                foreach (NPC npc in npcs)
                {
                    MeshSwapper.SwapMeshes(m_Spidey, npc.Avatar.BodyContainer.transform, npc.Avatar.BodyMeshes, m_SpideyMat);

                    npc.Avatar.FaceMesh.enabled = false;
                    npc.Avatar.Eyes.leftEye.Container.gameObject.SetActive(false);
                    npc.Avatar.Eyes.rightEye.Container.gameObject.SetActive(false);
                    AvatarSettings emptySettings = ScriptableObject.CreateInstance<AvatarSettings>();
                    npc.Avatar.ApplyAccessorySettings(emptySettings);
                    npc.Avatar.ApplyHairSettings(emptySettings);
                    npc.Avatar.ApplyEyebrowSettings(emptySettings);
                    Destroy(emptySettings);
                }
                yield break;
            }
            yield return new WaitForSeconds(1.0f);
        }
    }
}
