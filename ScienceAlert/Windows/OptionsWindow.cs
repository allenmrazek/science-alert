///******************************************************************************
//                   Science Alert for Kerbal Space Program                    
// ******************************************************************************
//    Copyright (C) 2014 Allen Mrazek (amrazek@hotmail.com)

//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// *****************************************************************************/
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using System.Globalization;
//using ReeperCommon;

//namespace ScienceAlert.Windows
//{
//    using ProfileManager = ScienceAlertProfileManager;

//    /// <summary>
//    /// It pretty much is what it sounds like
//    /// </summary>
//    internal partial class OptionsWindow : MonoBehaviour, IDrawable
//    {
//        private readonly int windowId = UnityEngine.Random.Range(0, int.MaxValue);

//        // --------------------------------------------------------------------
//        //    Members
//        // --------------------------------------------------------------------

//        // Control position and scrollbars
//        private Rect windowRect;
//        private Vector2 scrollPos = new Vector2();                  // scrollbar for profile experiment settings
//        private Vector2 additionalScrollPos = new Vector2();        // scrollbar for additional options window
//        private Vector2 profileScrollPos = Vector2.zero;            // scrollbar for profile list window

//        private Dictionary<string /* expid */, int /* selected index */> experimentIds = new Dictionary<string, int>();
//        private List<GUIContent> filterList = new List<GUIContent>();
//        private string thresholdValue = "0";                        // temporary threshold string

//        internal enum OpenPane
//        {
//            None,
//            AdditionalOptions,
//            LoadProfiles
//        }

//        private OpenPane submenu = OpenPane.None;


//        private ScienceAlert scienceAlert;

//        // Materials and textures
//        Texture2D collapseButton = new Texture2D(24, 24);
//        Texture2D expandButton = new Texture2D(24, 24);
//        Texture2D openButton = new Texture2D(24, 24);
//        Texture2D saveButton = new Texture2D(24, 24);
//        Texture2D returnButton = new Texture2D(24, 24);
//        Texture2D deleteButton = new Texture2D(24, 24);
//        Texture2D renameButton = new Texture2D(24, 24);
//        Texture2D blackPixel = new Texture2D(1, 1);
//        GUISkin whiteLabel;

//        // locale
//        NumberFormatInfo formatter;

//        // audio
//        new AudioPlayer audio;

///******************************************************************************
// *                    Implementation Details
// ******************************************************************************/

//        void Start()
//        {
//            // culture setting
//            Log.Normal("Configuring NumberFormatInfo for current locale");
//            formatter = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
//            formatter.CurrencySymbol = string.Empty;
//            formatter.CurrencyDecimalDigits = 2;
//            formatter.NumberDecimalDigits = 2;
//            formatter.PercentDecimalDigits = 2;
            


//            scienceAlert = gameObject.GetComponent<ScienceAlert>();
//            audio = GetComponent<AudioPlayer>();


//            windowRect = new Rect(0, 0, 324, Screen.height / 5 * 3);

//            var rawIds = ResearchAndDevelopment.GetExperimentIDs();
//            var sortedIds = rawIds.OrderBy(expid => ResearchAndDevelopment.GetExperiment(expid).experimentTitle);

//            Log.Debug("OptionsWindow: sorted {0} experiment IDs", sortedIds.Count());

//            foreach (var id in sortedIds)
//            {
//                experimentIds.Add(id, (int)Convert.ChangeType(ProfileManager.ActiveProfile[id].Filter, ProfileManager.ActiveProfile[id].Filter.GetTypeCode()));
//                Log.Debug("Settings: experimentId {0} has filter index {1}", id, experimentIds[id]);
//            }

//            /*
//                Unresearched = 0,                           
//                NotMaxed = 1,                               
//                LessThanFiftyPercent = 2,                   
//                LessThanNinetyPercent = 3    
//             */
//            filterList.Add(new GUIContent("Unresearched"));
//            filterList.Add(new GUIContent("Not maxed"));
//            filterList.Add(new GUIContent("< 50% collected"));
//            filterList.Add(new GUIContent("< 90% collected"));

//            //sciMinValue = Settings.Instance.ScienceThreshold.ToString();
//            //sciMinValue = ScienceAlertProfileManager.ActiveProfile.scienceThreshold.ToString();

//            openButton = ResourceUtil.GetEmbeddedTexture("ScienceAlert.Resources.btnOpen.png", false);
//            saveButton = ResourceUtil.GetEmbeddedTexture("ScienceAlert.Resources.btnSave.png", false);
//            returnButton = ResourceUtil.GetEmbeddedTexture("ScienceAlert.Resources.btnReturn.png", false);
//            deleteButton = ResourceUtil.GetEmbeddedTexture("ScienceAlert.Resources.btnDelete.png", false);
//            renameButton = ResourceUtil.GetEmbeddedTexture("ScienceAlert.Resources.btnRename.png", false);

//            var tex = ResourceUtil.GetEmbeddedTexture("ScienceAlert.Resources.btnExpand.png", false);

//            if (tex == null)
//            {
//                Log.Error("Failed to retrieve expand button texture from stream");
//            }
//            else
//            {
//                Log.Debug("Collapse button texture loaded successfully");
//                expandButton = tex;
                
//                collapseButton = UnityEngine.Texture.Instantiate(expandButton) as Texture2D;
//                ResourceUtil.FlipTexture(collapseButton, true, true);

//                collapseButton.Compress(false);
//                expandButton.Compress(false);
//            }

//            blackPixel.SetPixel(0, 0, Color.black); blackPixel.Apply();
//            blackPixel.filterMode = FilterMode.Bilinear;

//            whiteLabel = (GUISkin)GUISkin.Instantiate(Settings.Skin);
//            whiteLabel.label.onNormal.textColor = Color.white;
//            whiteLabel.toggle.onNormal.textColor = Color.white;
//            whiteLabel.label.onActive.textColor = Color.white;

//            //redToggle = (GUISkin)GUISkin.Instantiate(Settings.Skin);
//            //redToggle.toggle.onNormal.textColor =
//            //redToggle.toggle.onHover.textColor =
//            //redToggle.toggle.onActive.textColor = Color.red;
//            //redToggle.toggle.normal.

//            scienceAlert.Button.OnClick += OnToolbarClicked;

//            submenu = OpenPane.None;
//        }



//        void OnDestroy()
//        {
//            Log.Debug("OptionsWindow destroyed");


//        }



//        public void Update()
//        {
//            // Required by IDrawable
//        }



//        #region Events



//        /// <summary>
//        /// Called when ScienceAlert toolbar button was clicked
//        /// </summary>
//        /// <param name="ci"></param>
//        public void OnToolbarClicked(Toolbar.ClickInfo ci)
//        {
//            if (InputLockManager.GetControlLock("ScienceAlertThreshold") != ControlTypes.None)
//            {
//                Log.Debug("OptionsWindow.OnToolbarClicked: found threshold lock");
//                InputLockManager.RemoveControlLock("ScienceAlertThreshold");
//            }

            


//            if (ci.used) return;

//            if (scienceAlert.Button.Drawable == null)
//            {
//                if (ci.button == 1) // right-click
//                {
//                    ci.Consume();

//                    if (scienceAlert.Button.Drawable is OptionsWindow)
//                    {
//                        CloseOptionsWindow();
//                    }
//                    else
//                    {
//                        scienceAlert.Button.Drawable = this;
//                        audio.Play("click1");
//                    }
//                }
//            }
//            else if (scienceAlert.Button.Drawable is OptionsWindow)
//            {
//                // we're open, non-right mouse button was clicked so close the window
//                ci.Consume();
//                CloseOptionsWindow();
//            }
//        }



//        private void CloseOptionsWindow()
//        {
//            if (!(scienceAlert.Button.Drawable is OptionsWindow)) Log.Warning("OptionsWindow.CloseOptionsWindow called with non-options window drawable"); // would indicate a bug

//            thresholdValue = string.Empty; // reset user-entered string; will be updated with proper value next time the options window is opened

//            Log.Debug("Closing options window");
//            scienceAlert.Button.Drawable = null;
//            audio.Play("click1", 1f, 0.05f);
//        }



//        #endregion



//        #region GUI helper methods

//        /// <summary>
//        /// Helper method
//        /// </summary>
//        /// <param name="value"></param>
//        /// <param name="content"></param>
//        /// <param name="style"></param>
//        /// <param name="options"></param>
//        /// <returns></returns>
//        private bool AudibleToggle(bool value, string content, GUIStyle style = null, GUILayoutOption[] options = null)
//        {
//            return AudibleToggle(value, new GUIContent(content), style, options);
//        }



//        /// <summary>
//        /// Just a wrapper around GUILayout.Toggle which plays a sound when
//        /// its value changes.
//        /// </summary>
//        /// <param name="value"></param>
//        /// <param name="content"></param>
//        /// <param name="style"></param>
//        /// <param name="options"></param>
//        /// <returns></returns>
//        private bool AudibleToggle(bool value, GUIContent content,  GUIStyle style = null, GUILayoutOption[] options = null)
//        {
//            bool result = GUILayout.Toggle(value, content, style == null ? Settings.Skin.toggle : style, options);
//            if (result != value)
//            {
//                audio.Play("click1");

//#if DEBUG
//                Log.Debug("Toggle '{0}' is now {1}", content.text, result);
//#endif
//            }
//            return result;
//        }



//        /// <summary>
//        /// Simple wrapper
//        /// </summary>
//        /// <param name="currentValue"></param>
//        /// <param name="settings"></param>
//        /// <returns></returns>
//        private int AudibleSelectionGrid(int currentValue, ref ProfileData.ExperimentSettings settings)
//        {
//            int newValue = GUILayout.SelectionGrid(currentValue, filterList.ToArray(), 2, GUILayout.ExpandWidth(true));
//            if (newValue != currentValue)
//            {
//                audio.Play("click1");
//                settings.Filter = (ProfileData.ExperimentSettings.FilterMethod)newValue;
//            }

//            return newValue;
//        }



//        /// <summary>
//        /// Simple wrapper
//        /// </summary>
//        /// <param name="content"></param>
//        /// <param name="options"></param>
//        /// <returns></returns>
//        private bool AudibleButton(GUIContent content, params GUILayoutOption[] options)
//        {
//            bool pressed = GUILayout.Button(content, options);

//            if (pressed)
//                audio.Play("click1");

//            return pressed;
//        }

//        #endregion


//        #region Drawing functions

//        /// <summary>
//        /// Called by the toolbar button (whichever implementation) when it's
//        /// time to draw the window.
//        /// </summary>
//        /// <param name="position"></param>
//        /// <returns>Dimensions of rendered window</returns>
//        public Vector2 Draw(Vector2 position)
//        {
//            var oldSkin = GUI.skin;
//            GUI.skin = Settings.Skin;

//            windowRect.x = position.x;
//            windowRect.y = position.y;

//            if (!HasOpenPopup)
//                windowRect = GUILayout.Window(windowId, windowRect, RenderControls, "Science Alert");

//            GUI.skin = oldSkin;

//            return new Vector2(windowRect.width, windowRect.height);
//        }



//        private void RenderControls(int windowId)
//        {
//            GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.Height(Screen.height / 5 * 3));
//            {

//                GUILayout.Label(new GUIContent("Global Warp Settings"), GUILayout.ExpandWidth(true));
//                Settings.Instance.GlobalWarp = (Settings.WarpSetting)GUILayout.SelectionGrid((int)Settings.Instance.GlobalWarp, new GUIContent[] { new GUIContent("By Experiment"), new GUIContent("Globally on"), new GUIContent("Globally off") }, 3, GUILayout.ExpandWidth(false));

//                GUILayout.Label(new GUIContent("Global Alert Sound"), GUILayout.ExpandWidth(true));
//                Settings.Instance.SoundNotification = (Settings.SoundNotifySetting)GUILayout.SelectionGrid((int)Settings.Instance.SoundNotification, new GUIContent[] { new GUIContent("By Experiment"), new GUIContent("Always"), new GUIContent("Never") }, 3, GUILayout.ExpandWidth(false));

//                GUILayout.Space(4f);

//                GUILayout.BeginHorizontal();
//                    GUILayout.Label(new GUIContent("Additional Options"));
//                    GUILayout.FlexibleSpace();
//                    //additionalOptions = AudibleButton(new GUIContent(additionalOptions ? collapseButton : expandButton)) ? !additionalOptions : additionalOptions;

//                    if (AudibleButton(new GUIContent(submenu == OpenPane.AdditionalOptions ? collapseButton : expandButton)))
//                        submenu = submenu == OpenPane.AdditionalOptions ? OpenPane.None : OpenPane.AdditionalOptions;
                    
//                GUILayout.EndHorizontal();

//                switch (submenu)
//                {
//                    case OpenPane.None:
//                        DrawProfileSettings();
//                        break;

//                    case OpenPane.AdditionalOptions:
//                        DrawAdditionalOptions();
//                        break;

//                    case OpenPane.LoadProfiles:
//                        DrawProfileList();
//                        break;
//                }
//            }
//            GUILayout.EndVertical();
//        }

//        #endregion


//        /// <summary>
//        /// Regular, non-profile specific additional configuration options
//        /// </summary>
//        private void DrawAdditionalOptions()
//        {
//            GUI.skin = whiteLabel;

//            additionalScrollPos = GUILayout.BeginScrollView(additionalScrollPos, Settings.Skin.scrollView, GUILayout.ExpandHeight(true));
//            {
//                GUILayout.Space(4f);

//                GUILayout.BeginVertical(GUILayout.ExpandHeight(true));
//                {

//#region Alert settings
//                    {
//                        GUILayout.Box("Miscellaneous Alert Settings", GUILayout.ExpandWidth(true));

//                        //-----------------------------------------------------
//                        // global flask animation
//                        //-----------------------------------------------------
//                        GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
//                        {
//                            GUILayout.Label("Globally Enable Animation", GUILayout.ExpandWidth(true));
//                            Settings.Instance.FlaskAnimationEnabled = AudibleToggle(Settings.Instance.FlaskAnimationEnabled, string.Empty, null, new GUILayoutOption[] { GUILayout.ExpandWidth(false) });
//                            if (!Settings.Instance.FlaskAnimationEnabled && scienceAlert.Button.IsAnimating) scienceAlert.Button.SetLit();
//                        }
//                        GUILayout.EndHorizontal();


//                        //-----------------------------------------------------
//                        // Display next report value in button
//                        //-----------------------------------------------------
//                        {
//                            Settings.Instance.ShowReportValue = AudibleToggle(Settings.Instance.ShowReportValue, "Display Report Value");
//                        }

//                        //-----------------------------------------------------
//                        // Display current biome in experiment list
//                        //-----------------------------------------------------
//                        {
//                            Settings.Instance.DisplayCurrentBiome = AudibleToggle(Settings.Instance.DisplayCurrentBiome, "Display Biome in Experiment List");
//                        }
//                    } // end alert settings
//                    #endregion


//#region scan interface options
//                    // scan interface options
//                    {
//                        GUILayout.Box("Third-party Integration Options", GUILayout.ExpandWidth(true));

//                        GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
//                        {
//                            var oldInterface = Settings.Instance.ScanInterfaceType;
//                            var prevColor = GUI.color;

//                            if (!SCANsatInterface.IsAvailable()) GUI.color = Color.red;

//                            bool enableSCANinterface = AudibleToggle(Settings.Instance.ScanInterfaceType == Settings.ScanInterface.ScanSat, "Enable SCANsat integration", null, new GUILayoutOption[] { GUILayout.ExpandWidth(true) });

//                            GUI.color = prevColor;

//                            if (enableSCANinterface && oldInterface != Settings.ScanInterface.ScanSat) // Settings won't return SCANsatInterface as the set interface if it wasn't found
//                                if (!SCANsatInterface.IsAvailable())
//                                {
//                                    PopupDialog.SpawnPopupDialog("SCANsat Not Found", "SCANsat was not found. You must install SCANsat to use this feature.", "Okay", false, Settings.Skin);
//                                    enableSCANinterface = false;
//                                }

//                            Settings.Instance.ScanInterfaceType = enableSCANinterface ? Settings.ScanInterface.ScanSat : Settings.ScanInterface.None;

//                            scienceAlert.ScanInterfaceType = Settings.Instance.ScanInterfaceType;
//                        }
//                        GUILayout.EndHorizontal();
//                    } // end scan interface options


//                    // toolbar interface options
//                    { 
//                        GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
//                        {
//                            var oldInterface = Settings.Instance.ToolbarInterfaceType;
//                            var prevColor = GUI.color;

//                            if (!ToolbarManager.ToolbarAvailable) GUI.color = Color.red;

//                            bool enableBlizzyToolbar = AudibleToggle(Settings.Instance.ToolbarInterfaceType == Settings.ToolbarInterface.BlizzyToolbar, "Use Blizzy toolbar");
//                            GUI.color = prevColor;

//                            if (enableBlizzyToolbar && oldInterface != Settings.ToolbarInterface.BlizzyToolbar)
//                                if (!ToolbarManager.ToolbarAvailable)
//                                {
//                                    PopupDialog.SpawnPopupDialog("Blizzy Toolbar Not Found", "Blizzy's toolbar was not found. You must install Blizzy's toolbar to use this feature.", "Okay", false, Settings.Skin);
//                                    enableBlizzyToolbar = false;
//                                }

//                            Settings.Instance.ToolbarInterfaceType = enableBlizzyToolbar ? Settings.ToolbarInterface.BlizzyToolbar : Settings.ToolbarInterface.ApplicationLauncher;

//                            if (scienceAlert.ToolbarType != Settings.Instance.ToolbarInterfaceType)
//                                scienceAlert.ToolbarType = Settings.Instance.ToolbarInterfaceType;
//                        }
//                        GUILayout.EndHorizontal();
//                    } // end toolbar interface options
//#endregion


//#region crewed vessel settings
//                    {
//                        GUILayout.Box("Crewed Vessel Settings", GUILayout.ExpandWidth(true));

//                        Settings.Instance.ReopenOnEva = AudibleToggle(Settings.Instance.ReopenOnEva, "Re-open list on EVA");
//                        { // eva report on top
//                            var prev = Settings.Instance.EvaReportOnTop;

//                            Settings.Instance.EvaReportOnTop = AudibleToggle(Settings.Instance.EvaReportOnTop, "List EVA report first");

//                            if (Settings.Instance.EvaReportOnTop != prev)
//                                GetComponent<ExperimentManager>().ScheduleRebuildObserverList();
//                        }

//                        // Surface sample on vessel
//                        {
//                            var prev = Settings.Instance.CheckSurfaceSampleNotEva;

//                            Settings.Instance.CheckSurfaceSampleNotEva = AudibleToggle(prev, "Track surface sample in vessel");

//                            if (prev != Settings.Instance.CheckSurfaceSampleNotEva)
//                                GetComponent<ExperimentManager>().ScheduleRebuildObserverList();
//                        }

//                    } // end crewed vessel settings
//                    #endregion

//                }
//                GUILayout.EndVertical();

//                GUI.skin = Settings.Skin;
//            }
//            GUILayout.EndScrollView();
//        }



//        /// <summary>
//        /// Draws modifyable settings for the current active profile, assuming one is
//        /// active and valid
//        /// </summary>
//        private void DrawProfileSettings()
//        {
//            if (ProfileManager.HasActiveProfile)
//            {
//                //-----------------------------------------------------
//                // Active profile header with buttons
//                //-----------------------------------------------------
//#region active profile header

//                GUILayout.BeginHorizontal();
//                {
//                    GUILayout.Box(string.Format("Profile: {0}", ProfileManager.ActiveProfile.DisplayName), GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));

//                    // rename profile button
//                    if (AudibleButton(new GUIContent(renameButton), GUILayout.MaxWidth(24)))
//                        SpawnRenamePopup(ProfileManager.ActiveProfile);

//                    // Save profile (only enabled if profile was actually modified)
//                    GUI.enabled = ProfileManager.ActiveProfile.modified;
//                    if (AudibleButton(new GUIContent(saveButton), GUILayout.MaxWidth(24)))
//                    {
//                        SaveCurrentProfile();
//                    }
//                    GUI.enabled = true;

//                    // Open profile (always available, warn user if profile modified)
//                    if (AudibleButton(new GUIContent(openButton), GUILayout.MaxWidth(24)))
//                        submenu = OpenPane.LoadProfiles;

//                }
//                GUILayout.EndHorizontal();

//                #endregion

//                //-----------------------------------------------------
//                // scrollview with experiment options
//                //-----------------------------------------------------
//#region experiment scrollview 

//                scrollPos = GUILayout.BeginScrollView(scrollPos, Settings.Skin.scrollView);
//                {
//                    GUI.skin = Settings.Skin;
//                    GUILayout.Space(4f);


//                    //-----------------------------------------------------
//                    // min threshold slider ui
//                    //-----------------------------------------------------
//                    #region min threshold slider
//                    GUILayout.Box("Alert Threshold");

//                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.MinHeight(14f));
//                    {
//                        if (ProfileManager.ActiveProfile.ScienceThreshold > 0f)
//                        {
//                            GUILayout.Label(string.Format("Alert Threshold: {0}", ProfileManager.ActiveProfile.ScienceThreshold.ToString("F2", formatter)));
//                        }
//                        else
//                        {
//                            var prev = GUI.color;
//                            GUI.color = XKCDColors.Salmon;
//                            GUILayout.Label("(disabled)");
//                            GUI.color = prev;
//                        }

//                        GUILayout.FlexibleSpace();
                        

//                        if (string.IsNullOrEmpty(thresholdValue)) thresholdValue = ProfileManager.ActiveProfile.scienceThreshold.ToString("F2", formatter);

//                        GUI.SetNextControlName("ThresholdText");
//                        string result = GUILayout.TextField(thresholdValue, GUILayout.MinWidth(60f));

//                        if (GUI.GetNameOfFocusedControl() == "ThresholdText") // only use text field value if it's focused; if we don't
//                        // do this, then it'll continuously overwrite the slider value
//                        {
//                            try
//                            {
//                                float parsed = float.Parse(result, formatter);
//                                ProfileManager.ActiveProfile.ScienceThreshold = parsed;

//                                thresholdValue = result;
//                            }
//                            catch (Exception) // just in case
//                            {
//                            }

//                            if (!InputLockManager.IsLocked(ControlTypes.ACTIONS_ALL))
//                                InputLockManager.SetControlLock(ControlTypes.ACTIONS_ALL, "ScienceAlertThreshold");
//                        }
//                        else if (InputLockManager.GetControlLock("ScienceAlertThreshold") != ControlTypes.None)
//                            InputLockManager.RemoveControlLock("ScienceAlertThreshold");


//                    }
//                    GUILayout.EndHorizontal();


//                    GUILayout.Space(3f); // otherwise the TextField will overlap the slider just slightly

//                    // threshold slider
//                    float newThreshold = GUILayout.HorizontalSlider(ProfileManager.ActiveProfile.ScienceThreshold, 0f, 100f, GUILayout.ExpandWidth(true), GUILayout.Height(14f));
//                    if (newThreshold != ProfileManager.ActiveProfile.scienceThreshold)
//                    {
//                        ProfileManager.ActiveProfile.ScienceThreshold = newThreshold;
//                        thresholdValue = newThreshold.ToString("F2", formatter);
//                    }


//                    // slider min/max value display. Put under slider because I couldn't get it centered on the sides
//                    // properly and it just looked strange
//                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.MaxHeight(10f));
//                    var prevColor = GUI.color;
//                    GUI.color = XKCDColors.Dark;
//                    GUILayout.Label("0");
//                    GUILayout.FlexibleSpace();
//                    GUILayout.Label("Science Amount");
//                    GUILayout.FlexibleSpace();
//                    GUILayout.Label("100");
//                    GUILayout.EndHorizontal();

//                    GUI.color = prevColor;

//                    #endregion

//                    GUILayout.Space(10f);

//                    // individual experiment settings
//                    var keys = new List<string>(experimentIds.Keys);

//                    foreach (var key in keys)
//                    {
//                        GUILayout.Space(4f);

//                        var settings = ProfileManager.ActiveProfile[key];

//                        // "asteroidSample" isn't listed in ScienceDefs (has a simple title of "Sample")
//                        //   note: band-aided this in ScienceAlert.Start; leaving this note here in case
//                        //         just switching an experiment's title causes issues later
//                        var title = ResearchAndDevelopment.GetExperiment(key).experimentTitle;
//#if DEBUG
//                        GUILayout.Box(title + string.Format(" ({0})", ResearchAndDevelopment.GetExperiment(key).id), GUILayout.ExpandWidth(true));
//#else
//                            GUILayout.Box(title, GUILayout.ExpandWidth(true));
//#endif

//                        settings.Enabled = AudibleToggle(settings.Enabled, "Enabled");
//                        settings.AnimationOnDiscovery = AudibleToggle(settings.AnimationOnDiscovery, "Animation on discovery");
//                        settings.SoundOnDiscovery = AudibleToggle(settings.SoundOnDiscovery, "Sound on discovery");
//                        settings.StopWarpOnDiscovery = AudibleToggle(settings.StopWarpOnDiscovery, "Stop warp on discovery");

//                        // only add the Assume Onboard option if the experiment isn't
//                        // one of the default types
//                        if (!settings.IsDefault)
//                            settings.AssumeOnboard = AudibleToggle(settings.AssumeOnboard, "Assume onboard");

//                        GUILayout.Label(new GUIContent("Filter Method"), GUILayout.ExpandWidth(true), GUILayout.MinHeight(24f));

//                        int oldSel = experimentIds[key];
//                        experimentIds[key] = AudibleSelectionGrid(oldSel, ref settings);

//                        if (oldSel != experimentIds[key])
//                            Log.Debug("Changed filter mode for {0} to {1}", key, settings.Filter);


//                    }
//                }
//                GUILayout.EndScrollView();

//                #endregion
//            }
//            else
//            { // no active profile
//                GUI.color = Color.red;
//                GUILayout.Label("No profile active");
//            }
//        }



//        private void DrawProfileList()
//        {
//            profileScrollPos = GUILayout.BeginScrollView(profileScrollPos, Settings.Skin.scrollView);
//            {
//                if (ProfileManager.Count > 0)
//                {
//                    //DrawProfileList_HorizontalDivider();
//                    GUILayout.Label("Select a profile to load");
//                    GUILayout.Box(blackPixel, GUILayout.ExpandWidth(true), GUILayout.MinHeight(1f), GUILayout.MaxHeight(3f));

//                    var profileList = ProfileManager.Profiles;

//                    // always draw default profile first
//                    DrawProfileList_ListItem(ProfileManager.DefaultProfile);

//                    foreach (ProfileData.Profile profile in profileList.Values)
//                        if (profile != ProfileManager.DefaultProfile)
//                            DrawProfileList_ListItem(profile);

//                }
//                else // no profiles saved
//                {
//                    GUILayout.FlexibleSpace();
//                    GUILayout.Box("No profiles saved", GUILayout.MinHeight(64f));
//                    GUILayout.FlexibleSpace();
//                }
//            }
//            GUILayout.Space(10f);
//            GUILayout.BeginHorizontal();
//            {
//                GUILayout.FlexibleSpace();
//                if (AudibleButton(new GUIContent("Cancel", "Cancel load operation"))) submenu = OpenPane.None;
//            }
//            GUILayout.EndHorizontal();
//            GUILayout.EndScrollView();
//        }


//        private void DrawProfileList_ListItem(ProfileData.Profile profile)
//        {
//            GUILayout.BeginHorizontal();
//            {
//                GUILayout.Box(profile.name, GUILayout.ExpandWidth(true));

//                // rename button
//                GUI.enabled = profile != ProfileManager.DefaultProfile;
//                if (AudibleButton(new GUIContent(renameButton), GUILayout.MaxWidth(24), GUILayout.MinWidth(24)))
//                    SpawnRenamePopup(profile);
                
//                // open button
//                GUI.enabled = true;
//                if (AudibleButton(new GUIContent(openButton), GUILayout.MaxWidth(24), GUILayout.MinWidth(24)))
//                    SpawnOpenPopup(profile);

//                // delete button
//                GUI.enabled = profile != ProfileManager.DefaultProfile;
//                if (AudibleButton(new GUIContent(deleteButton), GUILayout.MaxWidth(24), GUILayout.MinWidth(24)))
//                    SpawnDeletePopup(profile);

//                GUI.enabled = true;
//            }
//            GUILayout.EndHorizontal();
//        }




//        #region Message handling functions

//        /// <summary>
//        /// This message will be sent by ScienceAlert when the user
//        /// changes scan interface types
//        /// </summary>
//        public void Notify_ScanInterfaceChanged()
//        {
//            Log.Debug("OptionsWindow.Notify_ScanInterfaceChanged");
//        }



//        /// <summary>
//        /// This message sent when toolbar has changed and re-registering
//        /// for events is necessary
//        /// </summary>
//        public void Notify_ToolbarInterfaceChanged()
//        {
//            Log.Debug("OptionsWindow.Notify_ToolbarInterfaceChanged");
//            scienceAlert.Button.OnClick += OnToolbarClicked;
//        }

//#endregion
//    }
//}
