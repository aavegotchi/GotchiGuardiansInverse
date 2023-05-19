using UnityEngine;

namespace JBooth.MicroVerseCore.Browser
{
    /// <summary>
    /// Wrapper for a content collection item. To be used on selection grid.
    /// </summary>
    public class PresetItem
    {
        public ContentCollection collection;
        public ContentData content;
        public GUIContent guiContent;
        public int collectionIndex;

        public PresetItem(ContentCollection collection, ContentData content, GUIContent guiContent, int collectionIndex)
        {
            this.collection = collection;
            this.content = content;
            this.guiContent = guiContent;
            this.collectionIndex = collectionIndex;
        }
    }
}