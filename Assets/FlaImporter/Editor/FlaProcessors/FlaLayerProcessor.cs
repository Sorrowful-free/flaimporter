using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.FlaImporter.Editor.Data.RawData;
using Assets.FlaImporter.Editor.EditorCoroutine;
using UnityEngine;

namespace Assets.FlaImporter.Editor.FlaProcessors
{
    public static class FlaLayerProcessor
    {
       

        public static IEnumerator ProcessFlaLayer(GameObject rootGameObject, FlaLayerRaw layerData, int frameRate, AnimationClip clip)
        {
            switch (layerData.LayerType)
            {
                case "mask":
                    break;

                case "IK Pose":
                    break;

                case "motion object":
                    break;

                case "guide": // guid
                    break;

                case "folder": // folder ?
                    break;

                default: // classic animation or other
                    yield return ProcessDefaultFlaLayer(rootGameObject,layerData, frameRate, clip).StartAsEditorCoroutine();
                    break;
            }
        }

        private static IEnumerator ProcessDefaultFlaLayer(GameObject rootGameObject,FlaLayerRaw layerData, int frameRate, AnimationClip clip)
        {
            if (!layerData.Visible)
            {
                yield break; 
            }
        }

        
    }
}
