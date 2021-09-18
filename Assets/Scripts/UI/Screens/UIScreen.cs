using UnityEngine;

namespace DudeRescueSquad.UI
{
    public enum ScreenType
    {
        None = -1,

        // screens
        Settings = 0,
        Hangar,
        Base,
        BaseDestroyed,

        // ingame
        UIInGame = 20,

        // attack
        Attack = 30,
        AttackOutcome,
        RecentAttacks,

        // breeding
        Manufacturing = 40,
        AssemblyPlant,
        NewHelicopter,

        // progression
        PlayerProfile = 50,
        LevelUp,

        // store
        Store = 60,

        // forge
        Forge = 70,

        // chat
        Chat = 80,

        // alliance
        Alliance = 90,

        // sidequests
        Quests = 100,
        Missions,
        DailyRewards,

        // building
        BuildingInspector = 150,
        BuildingSelector,

        // armory
        Armory = 180,

        // story
        Story = 190,

        // common
        BaseObjectSelected = 200,
        LevelLoading,
    }

    public class UIScreen : MonoBehaviour
    {
        [Header("UIScreen")]
        public ScreenType screenType = ScreenType.None;
        public bool disableCamera = true;
        internal bool wasAdded = false;

        #region Unity methods

        private void OnEnable()
        {
        }

        #endregion

        #region public methods

        public virtual void OnCloseScreen()
        {
            UIScreenManager.Instance.HideScreen(screenType);
        }

        #endregion

        #region UI callbacks
        #endregion
    }
}