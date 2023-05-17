
using UnityEngine;

namespace JBooth.MicroVerseCore
{
    [System.Serializable]
    public class Options
    {
        [System.Serializable]
        public class Settings
        {
            [Tooltip("Some terrain shaders need the terrain layers to stay in sync between terrains - this can increase the number of splat maps needed by increasing the texture count when some textures are only used on some terrains")]
            public bool keepLayersInSync = false;
        }

        [System.Serializable]
        public class Colors
        {
            public bool drawStampPreviews = true;
            public Color heightStampColor = Color.gray;
            public Color textureStampColor = Color.clear;
            public Color treeStampColor = Color.green;
            public Color detailStampColor = Color.yellow;
            public Color occluderStampColor = Color.magenta;
            public Color copyStampColor = Color.cyan;
            public Color pasteStampColor = Color.cyan * 0.8f;
            public Color maskStampColor = Color.red;
            public Color objectStampColor = Color.blue;
            public Color ambientAreaColor = new Color(0, 0, 1, 0.5f);
            public Color noisePreviewColor = new Color(1, 0, 0, 0.8f);
            public Color filterPreviewColor = new Color(0, 0, 1, 0.8f);
        }

        public Settings settings = new Settings();
        public Colors colors = new Colors();

    }


}
