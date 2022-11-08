using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UObject = UnityEngine.Object;
using MagicUI;
using MagicUI.Elements;
using MagicUI.Graphics;
using MagicUI.Core;
using Image = MagicUI.Elements.Image;
using System.Linq;

namespace CharmDisplay
{
    public class CharmDisplay : Mod
    {
        private static readonly TextureLoader _charmSpriteLoader = new(typeof(CharmDisplay).Assembly, "CharmDisplay.Resources");
        private int totalCharms = 40;

        List<List<Sprite>> charmSprites = new();

        internal static CharmDisplay Instance;

        public CharmDisplay() : base("CharmDisplay") { }

        private LayoutRoot layout;
        private StackLayout charmPanel;
        List<int> equippedCharms;
        int paddingLeft = 160;
        int paddingTop = 230;
        int spacing = 6;
        int spriteSize = 50;

        private bool debugBoundStatePersistent = false;

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing");

            Instance = this;

            SetupCanvas();
            //ModHooks.CharmUpdateHook += UpdateCanvas;
            On.UIManager.UIClosePauseMenu += OnUnpause;
            On.UIManager.GoToPauseMenu += OnPause;
            On.UIManager.ContinueGame += OnGameContinue;

            Log("Initialized");
        }

        private void SetupCanvas()
        {
            layout ??= new(true, "Persistent layout")
                {
                    RenderDebugLayoutBounds = debugBoundStatePersistent
                };

            charmPanel = new(layout, "Charm Panel")
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Spacing = spacing,
                Padding = new Padding(paddingLeft, paddingTop, 0, 0),
                Orientation = Orientation.Horizontal,
            };
        }

        private void OnUnpause(On.UIManager.orig_UIClosePauseMenu origUIClosePauseMenu, UIManager self)
        {
            origUIClosePauseMenu(self);
            UpdateCanvas();
        }

        private IEnumerator OnPause(On.UIManager.orig_GoToPauseMenu orig, UIManager uiManager)
        {
            yield return orig(uiManager);

            HideCanvas();
        }

        private void OnGameContinue(On.UIManager.orig_ContinueGame orig, UIManager self)
        {
            orig(self);

            // UpdateCanvas(); // Causes save file to not load since charmSprites don't exist yet
        }

        private void UpdateCanvas()
        {
            charmPanel.Children.Clear();
            equippedCharms = PlayerData.instance.equippedCharms;

            foreach (int charm in equippedCharms)
            {
                charmPanel.Children.Add(new Image(layout, CharmIconList.Instance.spriteList[charm])
                {
                    Height = spriteSize,
                    Width = spriteSize,
                });
            }
        }

        private void HideCanvas()
        {
            charmPanel.Children.Clear();
        }

        public override string GetVersion() => "1.1.19";
    }
}