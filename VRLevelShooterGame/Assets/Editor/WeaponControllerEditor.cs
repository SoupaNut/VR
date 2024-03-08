using UnityEngine;
using UnityEditor;
using Unity.Game.Shared;

[CustomEditor(typeof(WeaponController))]
public class WeaponControllerEditor : Editor
{

    //public override void OnInspectorGUI()
    //{
        

    //    WeaponController weaponController = target as WeaponController;

    //    weaponController.HasInfiniteAmmo = EditorGUILayout.Toggle("Has Infinite Ammo", weaponController.HasInfiniteAmmo);

    //    if(!weaponController.HasInfiniteAmmo)
    //    {
    //        weaponController.MaxAmmo = EditorGUILayout.FloatField("Max Ammo", weaponController.MaxAmmo);
    //    }

    //    base.OnInspectorGUI();

    //    //if (!weaponController.HasInfiniteAmmo)
    //    //{
    //    //    weaponController.MaxAmmo = EditorGUILayout.FloatField(weaponController.MaxAmmo);
    //    //}
    //}
}
