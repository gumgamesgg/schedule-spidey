using MelonLoader;
using UnityEngine;

namespace ScheduleSpidey;

public static class ImportUtils
{
    public static void FixMaterial(Material mat)
    {
        if (mat != null)
        {
            // We need to search for the compiled shader
            Shader fixedShader = Shader.Find(mat.shader.name);
            if (fixedShader != null)
            {
                mat.shader = fixedShader;
                Texture matMainTexture = mat.mainTexture;
                if (matMainTexture != null)
                {
                    MelonLogger.Msg(mat.name + " main texture found");
                }
                else
                {
                    MelonLogger.Warning(mat.name + " main texture NOT found");
                }
            }
            else
            {
                MelonLogger.Error(mat.shader.name + " not found");
            }
        }
    }
}
