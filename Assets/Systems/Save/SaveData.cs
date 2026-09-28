using System;
using System.Collections.Generic;

namespace Rabisco.Core.SaveSystem
{
    /// <summary>
    /// Root save data for a single slot. Contains all per-system data sections.
    /// </summary>
    [Serializable]
    public class SlotData
    {
        public LevelData Level = new();
        public SettingsData Settings = new();
        public InventoryData Inventory = new();
        public PlayerData Player = new();
        public BossData Boss = new();
    }

    /// <summary>
    /// Current level progress data.
    /// </summary>
    [Serializable]
    public class LevelData
    {
        public int currentLevel = 1;
        public string currentCheckpointId = "";
    }

    /// <summary>
    /// Audio and language settings.
    /// </summary>
    [Serializable]
    public class SettingsData
    {
        public string language = "pt";
        public float masterVolume = 1f;
        public float sfxVolume = 1f;
        public float musicVolume = 1f;
    }

    /// <summary>
    /// Collected items and soul fragments.
    /// </summary>
    [Serializable]
    public class InventoryData
    {
        public List<string> collectedItems = new();
        public int soulFragments = 0;
    }

    /// <summary>
    /// Unlocked player movesets.
    /// </summary>
    [Serializable]
    public class PlayerData
    {
        public string[] unlockedMovesets = Array.Empty<string>();
    }

    /// <summary>
    /// Defeated bosses.
    /// </summary>
    [Serializable]
    public class BossData
    {
        public List<string> defeatedBosses = new();
    }
}
