using Oxide.Core.Plugins;
using System.Collections.Generic;
using Newtonsoft.Json;

#region Changelogs and ToDo
/**********************************************************************
 * 
 * v1.0.1   :   Info fix & Plugin is now free 
 * v1.0.0   :   Initial release (Autopsie17) | (helped by Krungh Crow) 
 *
 **********************************************************************/
#endregion

namespace Oxide.Plugins
{
    [Info("NoRestore", "Autopsie17", "1.0.1")]
    [Description("(Addon)Prevents the Restore Upon Death plugin from restoring player inventory in certain zones.")]
    class NoRestore : RustPlugin
    {	
		#region References
        [PluginReference] Plugin ZoneManager;
		#endregion
		
		#region Variables
        const string bypass_perm = "NoRestore.bypass"; //added bypass so players/admins with this perm get their stuff restored
		const string nonotify_perm = "NoRestore.nonotify"; //added nonotify so players/admins with this permission no longer get a message.
		ulong ChatIcon;
		string prefix;
		bool notification = true;
		public List<object> ZoneID;
        #endregion
		
		#region Configuration
		void Init()
        {
            if (!LoadConfigVariables())
            {
            Puts("Config file issue detected. Please delete file, or check syntax and fix.");
            return;
            }

            permission.RegisterPermission(bypass_perm, this);
			permission.RegisterPermission(nonotify_perm, this);
			
            ZoneID = configData.ZoneCFG.ZoneID;
            prefix = configData.ChatCFG.Prefix;
			ChatIcon = configData.ChatCFG.ChatIcon;
			notification = configData.ChatCFG.notification;
        }
		
		private ConfigData configData;
		
		class ConfigData
        {
            [JsonProperty(PropertyName = "Chat Settings")]
            public SettingsChat ChatCFG = new SettingsChat();
            [JsonProperty(PropertyName = "Zone Settings")]
            public SettingsZone ZoneCFG = new SettingsZone();
			
        }
		
		class SettingsChat
        {
            [JsonProperty(PropertyName = "Chat Prefix")]
            public string Prefix = "<size=12><color=#AE3624>SERVERNAME</color> <size=16>|</size> <color=#738D45>NoRestore</color>\n";
			[JsonProperty(PropertyName = "Notification In Chat")]
            public bool notification = true;
			[JsonProperty(PropertyName = "ChatIcon (SteamID)")]
            public ulong ChatIcon = 0;
			
        }
		
		class SettingsZone
        {
			[JsonProperty(PropertyName = "List ZoneIDs")]
			public List<object> ZoneID = new List<object>{};
			
        }
		
		private bool LoadConfigVariables()
        {
            try
            {
            configData = Config.ReadObject<ConfigData>();
            }
            catch
            {
            return false;
            }
            SaveConf();
            return true;
        }

        protected override void LoadDefaultConfig()
        {
            Puts("Fresh install detected Creating a new config file.");
            configData = new ConfigData();
            SaveConf();
        }

        void SaveConf() => Config.WriteObject(configData, true);
        #endregion

        #region Restore
        //This hook is called from Restore Upon Death.
        private bool? OnRestoreUponDeath(BasePlayer player)
        {
			if(ZoneManager != null)
			{
				if((string[])ZoneManager.Call("GetPlayerZoneIDs", player) != null)
				{
                    if (permission.UserHasPermission(player.UserIDString, bypass_perm)) return null;
                    foreach (var zoneID in ZoneID)
					{
					    if((bool)ZoneManager.Call("isPlayerInZone", zoneID, player)) return false;
					}
					return null;
				}
			}
            return null;
        }
		
		void OnPlayerDeath(BasePlayer player, HitInfo info)
        {
			if (permission.UserHasPermission(player.UserIDString, bypass_perm) || permission.UserHasPermission(player.UserIDString, nonotify_perm)) return;
			foreach (var zoneID in ZoneID)
			{	
				if((bool)ZoneManager.Call("isPlayerInZone", zoneID, player))
				{
				if (notification) Player.Message(player, prefix + string.Format(msg("NoRestorCM", player.UserIDString)), ChatIcon);
				}
			}
        }
		#endregion
		
		#region Language
        protected override void LoadDefaultMessages()
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                ["NoRestorCM"] = "Inventory was not restored!"
            }, this);
            lang.RegisterMessages(new Dictionary<string, string>
            {
                ["NoRestorCM"] = "Inventar wurde nicht wiederhergestellt!"
            }, this, "de");
        }
        #endregion
		
		#region msg helper
		private string msg(string key, string id = null) => lang.GetMessage(key, this, id);
		#endregion
		
    }
}