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

            ModHooks.CharmUpdateHook += UpdateCanvas;

            On.GameManager.FinishedEnteringScene += OnFinishedEnteringScene;
            On.QuitToMenu.Start += OnQuitToMenu;

            On.UIManager.UIClosePauseMenu += OnUnpause;
            On.UIManager.GoToPauseMenu += OnPause;

            On.InvAnimateUpAndDown.AnimateUp += OnInventoryOpen;
            //On.InvAnimateUpAndDown.AnimateDown += OnInventoryClose;
            On.HeroController.AcceptInput += HeroController_AcceptInput;

            On.GameMap.SetupMapMarkers += OnShowMap;
            //On.GameMap.DisableMarkers += OnHideMap;

            On.HeroController.LeaveScene += HeroController_LeaveScene;

            //On.TransitionPoint. += Teest;

            Log("Initialized");
        }

        private void HeroController_LeaveScene(On.HeroController.orig_LeaveScene orig, HeroController self, GlobalEnums.GatePosition? gate)
        {
            orig(self, gate);
            HideCanvas();
        }

        private void HeroController_AcceptInput(On.HeroController.orig_AcceptInput orig, HeroController self)
        {
            orig(self);
            ShowCanvas();
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

        private void OnFinishedEnteringScene(On.GameManager.orig_FinishedEnteringScene orig, GameManager self)
        {
            orig(self);
            ShowCanvas();
        }

        private IEnumerator OnQuitToMenu(On.QuitToMenu.orig_Start orig, QuitToMenu self)
        {
            HideCanvas();
            return orig(self);
        }

        private void OnUnpause(On.UIManager.orig_UIClosePauseMenu origUIClosePauseMenu, UIManager self)
        {
            origUIClosePauseMenu(self);
            charmPanel.Visibility = Visibility.Visible;
        }

        private IEnumerator OnPause(On.UIManager.orig_GoToPauseMenu orig, UIManager uiManager)
        {
            yield return orig(uiManager);

            HideCanvas();
        }

        private void OnInventoryOpen(On.InvAnimateUpAndDown.orig_AnimateUp orig, InvAnimateUpAndDown self)
        {
            orig(self);
            HideCanvas();
        }

        private void OnInventoryClose(On.InvAnimateUpAndDown.orig_AnimateDown orig, InvAnimateUpAndDown self)
        {
            orig(self);
            ShowCanvas();
        }

        private void OnShowMap(On.GameMap.orig_SetupMapMarkers orig, GameMap self)
        {
            orig(self);
            HideCanvas();
        }

        private void OnHideMap(On.GameMap.orig_DisableMarkers orig, GameMap self)
        {
            orig(self);
            ShowCanvas();
        }

        private void UpdateCanvas(PlayerData data, HeroController hero)
        {
            if (CharmIconList.Instance == null)
                return;

            charmPanel.Children.Clear();
            equippedCharms = PlayerData.instance.equippedCharms;

            foreach (int charm in equippedCharms)
            {
                charmPanel.Children.Add(new Image(layout, CharmIconList.Instance.GetSprite(charm))
                {
                    Height = spriteSize,
                    Width = spriteSize,
                });
            }
        }

        private void HideCanvas()
        {
            charmPanel.Visibility = Visibility.Collapsed;
        }

        private void ShowCanvas()
        {
            charmPanel.Visibility = Visibility.Visible;
        }

        public override string GetVersion() => "1.1.25";
    }
}