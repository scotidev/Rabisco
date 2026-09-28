using UnityEngine;

namespace Rabisco.Core.SaveSystem
{
    /// <summary>
    /// Save manager. Handles saving and loading game data to PlayerPrefs as serialized JSON.
    /// Supports 3 save slots plus a persistent current-slot tracker.
    /// </summary>
    public class SaveManager : ISaveService
    {
        #region CONSTANTS

        private const int c_SlotCount = 3;
        private const string c_SlotKeyPrefix = "save_slot_";
        private const string c_CurrentSlotKey = "save_current_slot";

        #endregion

        #region FIELDS

        private SlotData slotData = new();
        private int currentSlot;

        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// Creates a new SaveManager and restores the last-used slot index from PlayerPrefs.
        /// </summary>
        public SaveManager()
        {
            currentSlot = PlayerPrefs.GetInt(c_CurrentSlotKey, 0);

            if (!IsValidSlot(currentSlot))
            {
                currentSlot = 0;
            }
        }

        #endregion

        #region PROPERTIES

        public LevelData Level => slotData.Level;
        public SettingsData Settings => slotData.Settings;
        public InventoryData Inventory => slotData.Inventory;
        public PlayerData Player => slotData.Player;
        public BossData Boss => slotData.Boss;

        #endregion

        #region ISaveService

        /// <summary>
        /// Serializes all save data to JSON and stores it in the specified PlayerPrefs slot.
        /// </summary>
        /// <param name="slotIndex">Slot index (0-2).</param>
        public void SaveToSlot(int slotIndex)
        {
            if (!IsValidSlot(slotIndex))
            {
                Debug.LogError($"[SaveManager] Invalid slot index: {slotIndex}.");
                return;
            }

            string json = JsonUtility.ToJson(slotData);
            PlayerPrefs.SetString(GetSlotKey(slotIndex), json);
            PlayerPrefs.Save();

            Debug.Log($"[SaveManager] Saved to slot {slotIndex}.");
        }

        /// <summary>
        /// Loads save data from the specified PlayerPrefs slot into memory.
        /// If the slot is empty, the current data remains unchanged.
        /// </summary>
        /// <param name="slotIndex">Slot index (0-2).</param>
        public void LoadFromSlot(int slotIndex)
        {
            if (!IsValidSlot(slotIndex))
            {
                Debug.LogError($"[SaveManager] Invalid slot index: {slotIndex}.");
                return;
            }

            string json = PlayerPrefs.GetString(GetSlotKey(slotIndex));
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogWarning($"[SaveManager] Slot {slotIndex} is empty. No data loaded.");
                return;
            }

            SlotData loaded = JsonUtility.FromJson<SlotData>(json);
            if (loaded != null)
            {
                slotData = loaded;
                Debug.Log($"[SaveManager] Loaded from slot {slotIndex}.");
            }
        }

        /// <summary>
        /// Checks whether the specified slot contains save data.
        /// </summary>
        /// <param name="slotIndex">Slot index (0-2).</param>
        /// <returns>True if the slot has saved data; otherwise false.</returns>
        public bool HasSave(int slotIndex)
        {
            if (!IsValidSlot(slotIndex)) return false;
            string json = PlayerPrefs.GetString(GetSlotKey(slotIndex));
            return !string.IsNullOrEmpty(json);
        }

        /// <summary>
        /// Deletes the save data in the specified slot.
        /// </summary>
        /// <param name="slotIndex">Slot index (0-2).</param>
        public void DeleteSlot(int slotIndex)
        {
            if (!IsValidSlot(slotIndex)) return;
            PlayerPrefs.DeleteKey(GetSlotKey(slotIndex));
            PlayerPrefs.Save();
            Debug.Log($"[SaveManager] Deleted slot {slotIndex}.");
        }

        /// <summary>
        /// Returns the index of the currently active slot.
        /// </summary>
        /// <returns>Current slot index (0-2).</returns>
        public int GetCurrentSlot() => currentSlot;

        /// <summary>
        /// Sets the active slot and persists the choice.
        /// </summary>
        /// <param name="slotIndex">Slot index (0-2).</param>
        public void SetCurrentSlot(int slotIndex)
        {
            if (!IsValidSlot(slotIndex)) return;
            currentSlot = slotIndex;
            PlayerPrefs.SetInt(c_CurrentSlotKey, slotIndex);
            PlayerPrefs.Save();
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Resets all in-memory save data to defaults. Does not touch stored slots.
        /// </summary>
        public void ResetToDefaults()
        {
            slotData = new SlotData();
        }

        /// <summary>
        /// Returns the total number of available save slots.
        /// </summary>
        /// <returns>Always 3.</returns>
        public int GetSlotCount() => c_SlotCount;

        /// <summary>
        /// Auto-saves to the current slot. Called between levels and at checkpoints.
        /// </summary>
        public void AutoSave()
        {
            SaveToSlot(currentSlot);
        }

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Validates that the given slot index is within the allowed range.
        /// </summary>
        private bool IsValidSlot(int slotIndex) => slotIndex >= 0 && slotIndex < c_SlotCount;

        /// <summary>
        /// Builds the PlayerPrefs key for the given slot index.
        /// </summary>
        private static string GetSlotKey(int slotIndex) => c_SlotKeyPrefix + slotIndex;

        #endregion
    }
}
