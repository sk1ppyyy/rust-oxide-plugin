﻿using System;
using System.ComponentModel;
using Newtonsoft.Json;
using Oxide.Core.Configuration;
using Oxide.Core.Plugins;
using UnityEngine;
#regionInfo
/**********************************************************************
 * The plugin is still owned by MJSU, we just edited it!
 *
 * v1.0.0   :  PowerPlant event support 
 * Icon by Freepik
 *
 **********************************************************************/
#endregion
namespace Oxide.Plugins
{
    [Info("Magic PowerPlant Event Panel", "MJSU edit by MeinRust", "1.0.0")]
    [Description("Displays if the PowerPlant event is active and changes the color of the PowerPlant icon when a PowerPlant is active.")]
    public class MagicPowerPlantEventPanel : RustPlugin
    {
        #region Class Fields
        [PluginReference] private readonly Plugin MagicPanel, PowerPlantEvent;

        private PluginConfig _pluginConfig; //Plugin Config
        private int _activePowerPlantEvent;

        private enum UpdateEnum { All = 1, Panel = 2, Image = 3, Text = 4 }
        #endregion

        #region Setup & Loading
        protected override void LoadDefaultConfig()
        {
            PrintWarning("Loading Default Config");
        }

        protected override void LoadConfig()
        {
            string path = $"{Manager.ConfigPath}/MagicPanel/{Name}.json";
            DynamicConfigFile newConfig = new DynamicConfigFile(path);
            if (!newConfig.Exists())
            {
                LoadDefaultConfig();
                newConfig.Save();
            }
            try
            {
                newConfig.Load();
            }
            catch (Exception ex)
            {
                RaiseError("Failed to load config file (is the config file corrupt?) (" + ex.Message + ")");
                return;
            }
            
            newConfig.Settings.DefaultValueHandling = DefaultValueHandling.Populate;
            _pluginConfig = AdditionalConfig(newConfig.ReadObject<PluginConfig>());
            newConfig.WriteObject(_pluginConfig);
        }

        private PluginConfig AdditionalConfig(PluginConfig config)
        {
            config.Panel = new Panel
            {
                Image = new PanelImage
                {
                    Enabled = config.Panel?.Image?.Enabled ?? true,
                    Color = config.Panel?.Image?.Color ?? "#FFFFFFFF",
                    Order = config.Panel?.Image?.Order ?? 0,
                    Width = config.Panel?.Image?.Width ?? 1f,
                    Url = config.Panel?.Image?.Url ?? "https://www.dropbox.com/s/ffecmsy5nf1gzxj/PowerPlantEvent.png?dl=1",
                    Padding = config.Panel?.Image?.Padding ?? new TypePadding(0.1f, 0.1f, 0.05f, 0.05f)
                }
            };
            config.PanelSettings = new PanelRegistration
            {
                BackgroundColor = config.PanelSettings?.BackgroundColor ?? "#FFF2DF08",
                Dock = config.PanelSettings?.Dock ?? "center",
                Order = config.PanelSettings?.Order ?? 1,
                Width = config.PanelSettings?.Width ?? 0.02f
            };
            return config;
        }

        private void OnServerInitialized()
        {
            MagicPanelRegisterPanels();
        }

        private void MagicPanelRegisterPanels()
        {
            if (MagicPanel == null)
            {
                PrintError("Missing plugin dependency MagicPanel: https://umod.org/plugins/magic-panel");
                return;
            }

            if (PowerPlantEvent == null)
            {
                PrintError("Missing plugin dependency PowerPlantEvent");
                return;
            }
        
            MagicPanel?.Call("RegisterGlobalPanel", this, Name, JsonConvert.SerializeObject(_pluginConfig.PanelSettings), nameof(GetPanel));
        }
        #endregion

        #region Earthquake Hooks

        private void OnPowerPlantEventStart()
        {
            _activePowerPlantEvent++;
            if (_activePowerPlantEvent == 1)
            {
                MagicPanel?.Call("UpdatePanel", Name, (int)UpdateEnum.Image);
            }
        }

        private void OnPowerPlantEventEnd()
        {
            _activePowerPlantEvent = Mathf.Min(0, _activePowerPlantEvent - 1);
            if (_activePowerPlantEvent == 0)
            {
                MagicPanel?.Call("UpdatePanel", Name, (int)UpdateEnum.Image);
            }
        }
        #endregion

        #region MagicPanel Hook
        private Hash<string, object> GetPanel()
        {
            Panel panel = _pluginConfig.Panel;
            PanelImage image = panel.Image;
            if (image != null)
            {
                image.Color = _activePowerPlantEvent != 0 ? _pluginConfig.ActiveColor : _pluginConfig.InactiveColor;
            }

            return panel.ToHash();
        }
        #endregion

        #region Classes

        private class PluginConfig
        {
            [DefaultValue("#00FF00FF")]
            [JsonProperty(PropertyName = "Active Color")]
            public string ActiveColor { get; set; }

            [DefaultValue("#FFFFFF1A")]
            [JsonProperty(PropertyName = "Inactive Color")]
            public string InactiveColor { get; set; }

            [JsonProperty(PropertyName = "Panel Settings")]
            public PanelRegistration PanelSettings { get; set; }

            [JsonProperty(PropertyName = "Panel Layout")]
            public Panel Panel { get; set; }
        }

        private class PanelRegistration
        {
            public string Dock { get; set; }
            public float Width { get; set; }
            public int Order { get; set; }
            public string BackgroundColor { get; set; }
        }

        private class Panel
        {
            public PanelImage Image { get; set; }
            
            public Hash<string, object> ToHash()
            {
                return new Hash<string, object>
                {
                    [nameof(Image)] = Image.ToHash(),
                };
            }
        }

        private abstract class PanelType
        {
            public bool Enabled { get; set; }
            public string Color { get; set; }
            public int Order { get; set; }
            public float Width { get; set; }
            public TypePadding Padding { get; set; }
            
            public virtual Hash<string, object> ToHash()
            {
                return new Hash<string, object>
                {
                    [nameof(Enabled)] = Enabled,
                    [nameof(Color)] = Color,
                    [nameof(Order)] = Order,
                    [nameof(Width)] = Width,
                    [nameof(Padding)] = Padding.ToHash(),
                };
            }
        }

        private class PanelImage : PanelType
        {
            public string Url { get; set; }
            
            public override Hash<string, object> ToHash()
            {
                Hash<string, object> hash = base.ToHash();
                hash[nameof(Url)] = Url;
                return hash;
            }
        }

        private class TypePadding
        {
            public float Left { get; set; }
            public float Right { get; set; }
            public float Top { get; set; }
            public float Bottom { get; set; }

            public TypePadding(float left, float right, float top, float bottom)
            {
                Left = left;
                Right = right;
                Top = top;
                Bottom = bottom;
            }
            
            public Hash<string, object> ToHash()
            {
                return new Hash<string, object>
                {
                    [nameof(Left)] = Left,
                    [nameof(Right)] = Right,
                    [nameof(Top)] = Top,
                    [nameof(Bottom)] = Bottom
                };
            }
        }
        #endregion
    }
}
