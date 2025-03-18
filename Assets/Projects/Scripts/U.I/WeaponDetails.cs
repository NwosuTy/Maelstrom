using TMPro;
using UnityEngine;

namespace Creotly_Studios
{
    public class WeaponDetails : MonoBehaviour
    {
        [field: Header("Status")]
        [field: SerializeField] public WeaponType weaponType {get; private set;}

        [field: Header("Parameters")]
        [field: SerializeField] public TextMeshProUGUI damageValueText {get; private set;}

        public void WeaponDetails_Update(PlayerManager playerManager)
        {
            
        }
    }
}
