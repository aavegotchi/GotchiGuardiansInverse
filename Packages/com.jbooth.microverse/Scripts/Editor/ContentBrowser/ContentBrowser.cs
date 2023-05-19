using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using static JBooth.MicroVerseCore.Browser.Package;

namespace JBooth.MicroVerseCore.Browser
{
    public class ContentBrowser : EditorWindow
    {
        static GUIContent COptionalVisibility = new GUIContent("O", "Optional content visibility");
        static GUIContent CDescriptionVisibility = new GUIContent("D", "Description visibility");
        static GUIContent CHelpVisibility = new GUIContent("?", "Help information visibility");

        const string FilterOptionAllText = "All";

        [MenuItem("Window/MicroVerse/Content Browser")]
        public static void CreateWindow()
        {
            var w = EditorWindow.GetWindow<ContentBrowser>();
            w.Show();
            w.wantsMouseEnterLeaveWindow = true;
            w.wantsMouseMove = true;
            w.titleContent = new GUIContent("MicroVerse Browser");
        }

        public static List<T> LoadAllInstances<T>() where T : ScriptableObject
        {
            return AssetDatabase.FindAssets($"t: {typeof(T).Name}").ToList()
                        .Select(AssetDatabase.GUIDToAssetPath)
                        .Select(AssetDatabase.LoadAssetAtPath<T>)
                        .ToList();
        }

        public enum Grouping
        {
            Package,
            ContentType
        }

        /// <summary>
        /// Subset of the Falloff filter type
        /// </summary>
        public enum FalloffDefault
        {
            Box = FalloffFilter.FilterType.Box,
            Range = FalloffFilter.FilterType.Range,
        }

        public enum Tab 
        {
            Height = ContentType.Height,
            Texture = ContentType.Texture,
            Vegetation = ContentType.Vegetation,
            Objects = ContentType.Objects,
            Audio = ContentType.Audio,
            Biomes = ContentType.Biomes,
            Roads = ContentType.Roads
        }

        GUIContent[] tabNames = new GUIContent[7] { new GUIContent("Height"), new GUIContent("Texturing"), new GUIContent("Vegetation"), new GUIContent("Objects"), new GUIContent("Audio"), new GUIContent("Biomes"), new GUIContent("Roads") };
        
        private Tab tab = Tab.Height;

        List<Package> filteredCollectionPackages = null;
        List<Package> filteredAdPackages = null;
        Package selectedPackage;

        List<BrowserContent> filteredCollections = null;
        List<BrowserContent> filteredAds = null;
        List<PresetItem> filteredPresets = null;

        List<ContentCollection> allCollections = null;
        List<BrowserContent> allContent = null;


        private static int headerWidth = 180;
        private static int listWidth = headerWidth + 10;

        private Vector2 listScrollPosition = Vector2.zero;

        private Color selectionColor = Color.green;

        public FalloffDefault filterTypeDefault = FalloffDefault.Box;
        public Vector3 heightStampDefaultScale = new Vector3(300, 120, 300);
        public Vector3 textureStampDefaultScale = new Vector3(300, 120, 300);
        public Vector3 vegetationStampDefaultScale = new Vector3(300, 120, 300);

        /// <summary>
        /// Selected item per tab, hash of browser content, since the object can change
        /// </summary>
        private Dictionary<Tab, Package> selectedTabItems = new Dictionary<Tab, Package>();

        private bool optionalVisible = true;
        private bool descriptionVisible = true;
        private bool helpBoxVisible = true;

        private ContentSelectionGrid contentSelectionGrid;
        private ContentDragHandler contentDragHandler;

        /// <summary>
        /// Index of the Author popup
        /// </summary>
        private int selectedAuthorFilterIndex = 0;

        /// <summary>
        /// Search text of the selected author filter.
        /// Don't use this string directly, rather use dedicated methods like <see cref="IsInFilter(BrowserContent)"/>.
        /// </summary>
        private string selectedAuthorFilterText = FilterOptionAllText;

        private Grouping grouping = Grouping.Package;

        public Tab GetSelectedTab()
        {
            return tab;
        }

        /// <summary>
        /// Get the selected browser content if it can be uniquely identified.
        /// Might not be the case if the same author creates multiple of the same content id and content type.
        /// In case it can't be uniquely identified an error popup will show up and null will be returned.
        /// </summary>
        /// <returns>The selected content asset or null if no content is available or multiple assets were found</returns>
        public BrowserContent GetSelectedBrowserContentAsset()
        {

            // at least package is required for detecting the current collection
            if (grouping == Grouping.ContentType)
            {
                EditorUtility.DisplayDialog("Error", $"Not supported for grouping by {grouping}.\nAborting operation.", "Ok");
                return null;
            }

            BrowserContent contentAsset = null;

            int count = 0;

            foreach (ContentCollection contentCollection in filteredCollections)
            {
                if (!IsInFilter(contentCollection))
                    continue;

                if (contentCollection.id == selectedPackage.id && contentCollection.contentType == selectedPackage.contentType)
                {
                    contentAsset = contentCollection;
                    count++;
                }
            }

            if( count > 1)
            {
                EditorUtility.DisplayDialog("Error", $"Multiple content collections found: {count}\nConsider using a filter, eg Author filter.\nAborting operation.", "Ok");
                return null;
            }

            return contentAsset;
        }

        /// <summary>
        /// This returns the first ad for the currently selected package that's found or null if none was found.
        /// </summary>
        /// <returns></returns>
        public ContentAd GetFirstSelectedAd()
        {
            if (selectedPackage == null || selectedPackage.packageType != PackageType.Ad)
                return null;

            foreach( BrowserContent content in allContent)
            {
                if (!(content is ContentAd))
                    continue;

                if (content.id == selectedPackage.id && content.contentType == selectedPackage.contentType)
                    return content as ContentAd;
            }

            return null;
        }

        public List<BrowserContent> GetAllContent()
        {
            return allContent;
        }

        private void OnEnable()
        {
            // initialize settings; can't be initialized as global variable because this class is a scriptable object
            optionalVisible = MicroVerseSettingsProvider.OptionalVisible;
            descriptionVisible = MicroVerseSettingsProvider.DescriptionVisible;
            helpBoxVisible = MicroVerseSettingsProvider.HelpVisible;

            // content selection grid
            if(contentSelectionGrid == null)
            {
                contentSelectionGrid = new ContentSelectionGrid(this);
            }

            contentSelectionGrid.OnEnable();

            // content drag handler
            if (contentDragHandler == null)
            {
                contentDragHandler = new ContentDragHandler(this);
            }

            contentDragHandler.OnEnable();

        }

        private void OnDisable()
        {
            // content selection grid
            contentSelectionGrid.OnDisable();
            contentSelectionGrid = null;

            // content drag handler
            contentDragHandler.OnDisable();
            contentDragHandler = null;
        }

        bool HasContentForAd(ContentAd ad, List<ContentCollection> content)
        {
            for (int i = 0; i < content.Count; ++i)
            {
                if (content[i].id == ad.id && !string.IsNullOrEmpty(ad.id))
                    return true;
                if (content[i].packName == ad.packName && !string.IsNullOrEmpty(ad.packName))
                    return true;
                
            }
            return false;
        }

        private void OnFocus()
        {
            allContent = LoadAllInstances<BrowserContent>();
        }

        void DrawToolbar()
        {
            if (tab == Tab.Height || tab == Tab.Texture || tab == Tab.Vegetation)
            {
                EditorGUILayout.BeginVertical("box");
                {
                    EditorGUILayout.LabelField("Preset", EditorStyles.miniBoldLabel);

                    float prev = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 80f;
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            
                            if (tab == Tab.Height)
                            {
                                EditorGUILayout.PrefixLabel("Falloff Type");
                                filterTypeDefault = (FalloffDefault)EditorGUILayout.EnumPopup(filterTypeDefault, GUILayout.Width(120));
                                EditorGUILayout.LabelField("Size", GUILayout.Width(80));
                                heightStampDefaultScale = EditorGUILayout.Vector3Field("", heightStampDefaultScale, GUILayout.Width(200));
                            }
                            else if (tab == Tab.Texture)
                            {
                                EditorGUILayout.LabelField("Size", GUILayout.Width(80));
                                textureStampDefaultScale = EditorGUILayout.Vector3Field("", textureStampDefaultScale, GUILayout.Width(200));
                            }
                            else if (tab == Tab.Vegetation)
                            {
                                EditorGUILayout.LabelField("Size", GUILayout.Width(80));
                                vegetationStampDefaultScale = EditorGUILayout.Vector3Field("", vegetationStampDefaultScale,GUILayout.Width(200));
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUIUtility.labelWidth = prev;

                }
                EditorGUILayout.EndVertical();
            }
        }

        /// <summary>
        /// Get popup options by getting a distinct list of all authors.
        /// Only uses content collections, no Ads considers
        /// </summary>
        /// <returns></returns>
        private string[] GetFilterOptions()
        {
            List<string> options = allContent
                .Where( x => x is ContentCollection) // installed
                .Select(x => x.author) // author name
                .Distinct() // unique
                .ToList();

            options.Sort();
            
            // add "All" on top of the list
            options.Insert(0, FilterOptionAllText);

            return options.ToArray();
        }

        /// <summary>
        /// Determine whether the collection is in the filter or not.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        private bool IsInFilter( BrowserContent collection)
        {
            // All
            if (selectedAuthorFilterIndex == 0)
                return true;

            if (collection == null || collection.author == null)
                return true;

            return selectedAuthorFilterText.Equals( collection.author);
        }

        /// <summary>
        /// Update the content to be displayed
        /// </summary>
        private void UpdateFilteredContent( Tab selectedTab)
        {
            if (allContent == null)
                allContent = LoadAllInstances<BrowserContent>();

            List<ContentAd> allAds = allContent
                .Where(x => (int)x.contentType == (int)selectedTab)
                .Where(x => x is ContentAd)
                .Cast<ContentAd>()
                .Where(x => IsInFilter(x))
                .ToList();

            allCollections = allContent
                .Where(x => (int)x.contentType == (int)selectedTab)
                .Where(x => x is ContentCollection)
                .Cast<ContentCollection>()
                .Where(x => IsInFilter(x))
                .ToList();


            // get all Ad ids which are invalid, ie require an object, but the object isn't set
            List<string> invalidAdIds = allAds
                .Where(x => x.requireInstalledObject && x.installedObject == null && x.id != null)
                .Select(x => x.id)
                .ToList();

            // remove collections which have invalid Ad ids
            allCollections = allCollections.Where(x => !invalidAdIds.Contains(x.id)).ToList();

            // create filtered content
            filteredCollections = new List<BrowserContent>();
            filteredCollections.AddRange(allCollections);

            // add ads
            filteredAds = new List<BrowserContent>();
            filteredAds.AddRange( allAds.Where(x => x.requireInstalledObject && x.installedObject == null || !HasContentForAd(x, allCollections)).ToList());

            filteredCollections = filteredCollections.OrderByDescending(x => x.GetType().Name).ThenBy(x => x.packName).ToList();

            // create draggable presets
            // TODO: adjust filter; allow all in one tab
            // TODO: group list by pack

            filteredPresets = new List<PresetItem>();
            foreach (ContentCollection contentCollection in filteredCollections)
            {
                // moving this filter to where the grid is being painted
                // we got "ArgumentException: Getting control 0's position in a group with only 0 controls when doing repaint" on openeing of the browser
                // because of the order of OnGUI and other methods
                /*
                var content = GetBCFromHash(selectedContent);
                bool isInFilter = content != null && contentCollection.packName == content.packName;
                if (!isInFilter)
                    continue;
                */

                // author filter
                if (!IsInFilter(contentCollection))
                    continue;

                ContentData[] contentData = contentCollection.contents;
                GUIContent[] guiContent = contentCollection.GetContents();

                for (int i = 0; i < contentData.Length; i++)
                {
                    filteredPresets.Add(new PresetItem(contentCollection, contentData[i], guiContent[i], i));
                }

            }

            // packages
            filteredCollectionPackages = new List<Package>();
            foreach (ContentCollection c in filteredCollections)
            {
                Package package = new Package(c.id, c.packName, c.contentType, PackageType.Collection);

                if (!filteredCollectionPackages.Contains(package))
                {
                    filteredCollectionPackages.Add(package);
                }

            }

            filteredAdPackages = new List<Package>();
            foreach (ContentAd c in filteredAds)
            {
                Package package = new Package(c.id, c.packName, c.contentType, PackageType.Ad);

                if (!filteredAdPackages.Contains(package))
                {
                    filteredAdPackages.Add(package);
                }
            }
        }

        private void OnGUI()
        {
            Package oldPackage = null;
            var oldTab = tab;

            EditorGUILayout.BeginHorizontal();
            {
                // tab bar
                tab = (Tab)GUILayout.Toolbar((int)tab, tabNames);

                // optional button
                if (GUILayout.Button(COptionalVisibility, EditorStyles.miniButton, GUILayout.Width(20)))
                {
                    optionalVisible = !optionalVisible;
                    MicroVerseSettingsProvider.OptionalVisible = optionalVisible;
                }

                // description button
                if (GUILayout.Button(CDescriptionVisibility, EditorStyles.miniButton, GUILayout.Width(20)))
                {
                    descriptionVisible = !descriptionVisible;
                    MicroVerseSettingsProvider.DescriptionVisible = descriptionVisible;
                }

                // help button
                if (GUILayout.Button(CHelpVisibility, EditorStyles.miniButton, GUILayout.Width(20)))
                {
                    helpBoxVisible = !helpBoxVisible;
                    MicroVerseSettingsProvider.HelpVisible = helpBoxVisible;
                }
            }
            EditorGUILayout.EndHorizontal();

            // set filtered content list; this can be optimized by not calling it all the time in OnGUI()
            // must happen after the tab got selected
            // could be optimized to happen only on tab switch and on package change. but then we wouldn't auto-detect changes, so leaving it as it is for now
            UpdateFilteredContent( tab);

            if (tab != oldTab)
            {
                oldPackage = selectedTabItems.GetValueOrDefault(oldTab);

                // try to keep the same package group selected during tab switch
                foreach (Package package in filteredCollectionPackages)
                {
                    if (package.IsInGroup(oldPackage))
                    {
                        selectedPackage = package;
                        selectedTabItems[tab] = selectedPackage;
                        break;
                    }
                }
            }

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginVertical("box", GUILayout.Width(listWidth));
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Group By", EditorStyles.miniBoldLabel, GUILayout.Width(60));
                grouping = (Grouping) EditorGUILayout.EnumPopup( grouping, GUILayout.Width(listWidth-80));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Author", EditorStyles.miniBoldLabel, GUILayout.Width(60));
                string[] options = GetFilterOptions();
                selectedAuthorFilterIndex = EditorGUILayout.Popup(selectedAuthorFilterIndex, options, GUILayout.Width(listWidth-80));
                EditorGUILayout.EndHorizontal();
                selectedAuthorFilterText = selectedAuthorFilterIndex < options.Length ? options[selectedAuthorFilterIndex] : "<Undefined>";

                // nothing selected => pick first one if available
                if (selectedPackage == null && filteredCollectionPackages.Count > 0)
                {
                    selectedPackage = filteredCollectionPackages[0];
                    selectedTabItems[tab] = selectedPackage;

                }

                if (grouping == Grouping.Package)
                {
                    DrawPackageList();
                }

                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.BeginVertical();
                    {
                        // falloff toolbar
                        DrawToolbar();

                        // content drag/drop
                        contentDragHandler.OnGUI();

                        // package content
                        if (grouping == Grouping.ContentType)
                        {
                            selectedPackage = null;
                            contentSelectionGrid.Draw(filteredPresets, grouping);
                        }
                        else if (grouping == Grouping.Package)
                        {
                            if (selectedPackage != null)
                            {
                                if (selectedPackage.packageType == PackageType.Collection)
                                {
                                    List<PresetItem> draggablePresets = filteredPresets.Where(x => x.collection.packName == selectedPackage.packName).ToList();
                                    contentSelectionGrid.Draw(draggablePresets, grouping);
                                }
                                else if (selectedPackage.packageType == PackageType.Ad)
                                {
                                    ContentAd selectedAd = GetFirstSelectedAd();

                                    if (selectedAd != null && !HasContentForAd(selectedAd, allCollections))
                                    {
                                        if (GUILayout.Button("Download", GUILayout.Width(420)))
                                        {
                                            var path = selectedAd.downloadPath;
                                            if (path.Contains("assetstore.unity.com"))
                                            {
                                                path += "?aid=25047";
                                            }
                                            Application.OpenURL(path);
                                        }
                                        Rect r = EditorGUILayout.GetControlRect(GUILayout.MaxWidth(420), GUILayout.MaxHeight(280));
                                        if (GUI.Button(r, ""))
                                        {
                                            var path = selectedAd.downloadPath;
                                            if (path.Contains("assetstore.unity.com"))
                                            {
                                                path += "?aid=25047";
                                            }
                                            Application.OpenURL(path);
                                        }
                                        if (selectedAd.image == null)
                                        {
                                            GUI.DrawTexture(r, Texture2D.whiteTexture);
                                        }
                                        else
                                        {
                                            GUI.DrawTexture(r, selectedAd.image);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draw all packages. First Installed, then Optional
        /// </summary>
        private void DrawPackageList()
        {
            listScrollPosition = GUILayout.BeginScrollView(listScrollPosition, GUILayout.Width(listWidth+20));
            {
                bool hasCollections = filteredCollectionPackages.Count > 0;
                bool hasAds = filteredAdPackages.Count > 0;

                if(hasCollections)
                {
                    EditorGUILayout.LabelField("Installed", EditorStyles.miniBoldLabel, GUILayout.Width(listWidth));
                    DrawPackageListItems(filteredCollectionPackages);
                }

                if (optionalVisible && hasAds)
                {
                    if (hasCollections)
                    {
                        EditorGUILayout.Space();
                    }

                    EditorGUILayout.LabelField("Optional", EditorStyles.miniBoldLabel, GUILayout.Width(listWidth));
                    DrawPackageListItems(filteredAdPackages);
                }
            }
            GUILayout.EndScrollView();
        }

        /// <summary>
        /// Draw the individual collections, eg Installed and Optional
        /// </summary>
        /// <param name="packages"></param>
        private void DrawPackageListItems( List<Package> packages)
        {
            foreach (Package package in packages)
            {
                Color prevColor = GUI.backgroundColor;
                {
                    selectedPackage = selectedTabItems.GetValueOrDefault(tab);

                    if (package.IsInGroup( selectedPackage))
                    {
                        GUI.backgroundColor = selectionColor;
                    }

                    if (GUILayout.Button(package.packName, GUILayout.Width(headerWidth+10)))
                    {
                        selectedPackage = package;
                        selectedTabItems[tab] = selectedPackage;
                    }
                }
                GUI.backgroundColor = prevColor;
            }
        }

        public bool IsDescriptionVisible()
        {
            return descriptionVisible;
        }

        public bool IsHelpBoxVisible()
        {
            return helpBoxVisible;
        }

        public int GetListWidth()
        {
            return listWidth;
        }
    }
}
