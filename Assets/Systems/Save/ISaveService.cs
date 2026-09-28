namespace Rabisco.Core.SaveSystem
{
    /// <summary>
    /// Save service. Manages saving and loading game data across multiple slots.
    /// </summary>
    public interface ISaveService : IGameService
    {
        /// <summary>
        /// Saves current game data to the specified slot.
        /// </summary>
        /// <param name="slotIndex">Slot index (0-2).</param>
        void SaveToSlot(int slotIndex);
        /// <summary>
        /// Loads game data from the specified slot into memory.
        /// </summary>
        /// <param name="slotIndex">Slot index (0-2).</param>
        void LoadFromSlot(int slotIndex);
        /// <summary>
        /// Checks whether the specified slot contains saved data.
        /// </summary>
        /// <param name="slotIndex">Slot index (0-2).</param>
        bool HasSave(int slotIndex);
        /// <summary>
        /// Deletes the save data in the specified slot.
        /// </summary>
        /// <param name="slotIndex">Slot index (0-2).</param>
        void DeleteSlot(int slotIndex);
        /// <summary>
        /// Returns the currently active slot index.
        /// </summary>
        int GetCurrentSlot();
        /// <summary>
        /// Sets the active slot for future save/load operations.
        /// </summary>
        /// <param name="slotIndex">Slot index (0-2).</param>
        void SetCurrentSlot(int slotIndex);
    }
}
