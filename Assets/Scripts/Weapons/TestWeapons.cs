using UnityEngine;

namespace DudeResqueSquad
{

    public class TestWeapons : MonoBehaviour
    {
        #region Inspector properties

        [SerializeField] private HandleWeapon _entityHandler = null;
        [SerializeField] private Weapon _weapon = null;

        #endregion

        #region Public methods

        public void SetWeapon()
        {
            _entityHandler.ChangeWeapon(_weapon, "weapon001");
        }

        #endregion
    }
}