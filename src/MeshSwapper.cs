using MelonLoader;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ScheduleSpidey;

/// <summary>
///     Swaps the SkinnedMeshRenderer of a GameObject with a custom mesh,
///     retargeting bones by name from the original rig.
/// </summary>
public static class MeshSwapper
{
    /// <summary>
    ///     Replaces all SkinnedMeshRenderers on the target with a clone of the replacement prefab,
    ///     retargeting bones from each original SMR.
    /// </summary>
    /// <param name="replacementPrefab">The prefab containing the replacement SkinnedMeshRenderer.</param>
    /// <param name="parent">Transform to parent the instantiated prefab under.</param>
    /// <param name="originalSmrs">The original SkinnedMeshRenderers to take bones from and disable.</param>
    /// <param name="material">Optional material to apply to the replacement SMR.</param>
    /// <param name="normalizeNames">Strip the "mixamorig:" prefix before matching bone names.</param>
    /// <returns>The instantiated replacement GameObject, or null on failure.</returns>
    public static GameObject SwapMeshes(GameObject replacementPrefab, Transform parent, IList<SkinnedMeshRenderer> originalSmrs,
        Material material = null, bool normalizeNames = false)
    {
        if (replacementPrefab == null)
        {
            MelonLogger.Error("[MeshSwapper] replacementPrefab is null.");
            return null;
        }

        if (originalSmrs == null || originalSmrs.Count == 0)
        {
            MelonLogger.Error("[MeshSwapper] No original SkinnedMeshRenderers provided.");
            return null;
        }

        GameObject instance = Object.Instantiate(replacementPrefab, parent);
        SkinnedMeshRenderer replacementSmr = instance.GetComponentInChildren<SkinnedMeshRenderer>();

        if (replacementSmr == null)
        {
            MelonLogger.Error("[MeshSwapper] No SkinnedMeshRenderer found in replacement prefab.");
            Object.Destroy(instance);
            return null;
        }

        // Use the first SMR as bone source for retargeting — all SMRs share the same rig
        RetargetBones(replacementSmr, originalSmrs[0], normalizeNames);

        foreach (SkinnedMeshRenderer smr in originalSmrs)
        {
            if (smr != null)
                smr.enabled = false;
        }

        if (material != null)
            replacementSmr.material = material;

        return instance;
    }

    /// <summary>
    ///     Convenience overload for swapping against a single SkinnedMeshRenderer.
    /// </summary>
    public static GameObject SwapMeshes(GameObject replacementPrefab, Transform parent, SkinnedMeshRenderer originalSmr,
        Material material = null, bool normalizeNames = false)
    {
        return SwapMeshes(replacementPrefab, parent, new[] { originalSmr }, material, normalizeNames);
    }

    /// <summary>
    ///     Retargets the bones of <paramref name="target" /> by matching bone names
    ///     against the bones of <paramref name="source" />.
    /// </summary>
    private static void RetargetBones(SkinnedMeshRenderer target, SkinnedMeshRenderer source, bool normalizeNames)
    {
        Dictionary<string, Transform> boneMap = BuildBoneMap(source.bones, normalizeNames);

        Transform[] originalBones = target.bones;
        Transform[] newBones = new Transform[originalBones.Length];

        for (int i = 0; i < originalBones.Length; i++)
        {
            if (originalBones[i] == null)
                continue;

            string key = normalizeNames ? Normalize(originalBones[i].name) : originalBones[i].name;

            if (boneMap.TryGetValue(key, out Transform match))
            {
                newBones[i] = match;
            }
            else
            {
                MelonLogger.Warning($"[MeshSwapper] Bone not found: '{originalBones[i].name}'");
                newBones[i] = originalBones[i];
            }
        }

        target.bones = newBones;
        target.rootBone = source.rootBone;
    }

    private static Dictionary<string, Transform> BuildBoneMap(Transform[] bones, bool normalizeNames)
    {
        Dictionary<string, Transform> map = new Dictionary<string, Transform>();
        foreach (Transform bone in bones)
        {
            if (bone == null) continue;
            string key = normalizeNames ? Normalize(bone.name) : bone.name;
            if (!map.ContainsKey(key))
                map[key] = bone;
        }
        return map;
    }

    // Strips "mixamorig:" prefix for cross-rig compatibility
    private static string Normalize(string name)
    {
        return name.Contains(":") ? name.Substring(name.IndexOf(':') + 1) : name;
    }
}
