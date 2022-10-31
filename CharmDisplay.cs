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
        int paddingLeft = 175;
        int paddingTop = 245;
        int spacing = 45;

        private bool debugBoundStatePersistent = true;

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing");

            Instance = this;
            for (int charmNum = 0; charmNum < totalCharms; charmNum++)
            {
                Log($"\"{charmNum + 1}.png\"");
                List<Sprite> emptyList = new List<Sprite>();
                charmSprites.Add(emptyList);
                charmSprites[charmNum].Add(_charmSpriteLoader.GetTexture($"Resources.{charmNum + 1}.png").ToSprite());

                if (charmNum == 22 || charmNum == 23 || charmNum == 25)
                {
                    charmSprites[charmNum].Add(_charmSpriteLoader.GetTexture($"{charmNum + 1}_G.png").ToSprite());
                }
                else if (charmNum == 36)
                {
                    charmSprites[charmNum].Add(_charmSpriteLoader.GetTexture($"{charmNum + 1}_L.png").ToSprite());
                    charmSprites[charmNum].Add(_charmSpriteLoader.GetTexture($"{charmNum + 1}_R.png").ToSprite());
                    charmSprites[charmNum].Add(_charmSpriteLoader.GetTexture($"{charmNum + 1}_Void.png").ToSprite());
                }
            }

            SetupCanvas();
            ModHooks.CharmUpdateHook += UpdateCanvas;

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
                Spacing = 5,
                Padding = new Padding(paddingLeft, paddingTop, 0, 0),
                Orientation = Orientation.Horizontal,
            };
        }

        private void UpdateCanvas(PlayerData data, HeroController controller)
        {
            charmPanel.Destroy();
            charmPanel = new(layout, "Charm Panel")
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Spacing = spacing,
                Padding = new Padding(paddingLeft, paddingTop, 0, 0),
                Orientation = Orientation.Horizontal,
            };
            equippedCharms = data.equippedCharms;

            foreach (int charm in equippedCharms)
            {
                charmPanel.Children.Add(new Image(layout, charmSprites[charm - 1].First()));
               /* charmPanel.Children.Add(new TextObject(layout)
                {
                    Text = charm.ToString(),
                    FontSize = 23,
                    Padding = new Padding(0, 0)
                });*/
            }
        }

        public override string GetVersion() => "1.1.0";
    }
}