//using UnityEditor;
//using Unity.Game.Gameplay;
//using UnityEngine;

//[CustomEditor(typeof(SpellController))]
//public class SpellControllerEditor : Editor
//{
//    #region SerializedProperties
//    SerializedProperty SpellType;

//    // Area Spell
//    SerializedProperty ExplosionRadius;
//    SerializedProperty FuseTime;

//    // General
//    SerializedProperty MaxLifeTime;
//    SerializedProperty Speed;
//    SerializedProperty Damage;
//    SerializedProperty OnCastSfx;

//    // Hit Detection
//    SerializedProperty DetectionRadius;
//    SerializedProperty Root;
//    SerializedProperty Tip;
//    SerializedProperty HittableLayers;
//    SerializedProperty OnHitLifetime;

//    // Impact VFX
//    SerializedProperty ImpactVfx;
//    SerializedProperty ImpactVfxLifetime;
//    SerializedProperty ImpactVfxSpawnOffset;

//    // Debug
//    SerializedProperty DisplayColliderRadius;
//    SerializedProperty ColliderColor;
//    SerializedProperty ExplosionRadiusColor;

//    bool generalGroup = true;
//    bool hitDetectionGroup = true;
//    bool impactVfxGroup = true;
//    bool debugGroup = false;
//    bool areaSpellGroup = true;
//    #endregion

//    private void OnEnable()
//    {
//        SpellType = serializedObject.FindProperty("SpellType");

//        ExplosionRadius = serializedObject.FindProperty("ExplosionRadius");
//        FuseTime = serializedObject.FindProperty("FuseTime");

//        MaxLifeTime = serializedObject.FindProperty("MaxLifeTime");
//        Speed = serializedObject.FindProperty("Speed");
//        Damage = serializedObject.FindProperty("Damage");
//        OnCastSfx = serializedObject.FindProperty("OnCastSfx");

//        DetectionRadius = serializedObject.FindProperty("DetectionRadius");
//        Root = serializedObject.FindProperty("Root");
//        Tip = serializedObject.FindProperty("Tip");
//        HittableLayers = serializedObject.FindProperty("HittableLayers");
//        OnHitLifetime = serializedObject.FindProperty("OnHitLifetime");

//        ImpactVfx = serializedObject.FindProperty("ImpactVfx");
//        ImpactVfxLifetime = serializedObject.FindProperty("ImpactVfxLifetime");
//        ImpactVfxSpawnOffset = serializedObject.FindProperty("ImpactVfxSpawnOffset");

//        DisplayColliderRadius = serializedObject.FindProperty("DisplayColliderRadius");
//        ColliderColor = serializedObject.FindProperty("ColliderColor");
//        ExplosionRadiusColor = serializedObject.FindProperty("ExplosionRadiusColor");
//    }

//    public override void OnInspectorGUI()
//    {
//        SpellController _spellController = (SpellController)target;

//        serializedObject.Update();

//        GUI.enabled = false;
//        MonoScript script = MonoScript.FromMonoBehaviour((MonoBehaviour)target);
//        EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
//        GUI.enabled = true;

//        EditorGUILayout.PropertyField(SpellType);
//        GUILayout.Space(10);

//        if(_spellController.SpellType == SpellController.SpellTypes.Area)
//        {
//            areaSpellGroup = EditorGUILayout.BeginFoldoutHeaderGroup(areaSpellGroup, "Area Spell Settings");
//            if (areaSpellGroup)
//            {
//                EditorGUILayout.PropertyField(ExplosionRadius);
//                EditorGUILayout.PropertyField(FuseTime);
//            }
//            EditorGUILayout.EndFoldoutHeaderGroup();
//            EditorGUILayout.Space(10);
//        }
        

//        generalGroup = EditorGUILayout.BeginFoldoutHeaderGroup(generalGroup, "General");
//        if (generalGroup)
//        {
//            EditorGUILayout.PropertyField(MaxLifeTime);
//            EditorGUILayout.PropertyField(Speed);
//            EditorGUILayout.PropertyField(Damage);
//            EditorGUILayout.PropertyField(OnCastSfx);
//        }
//        EditorGUILayout.EndFoldoutHeaderGroup();
//        EditorGUILayout.Space(10);

//        hitDetectionGroup = EditorGUILayout.BeginFoldoutHeaderGroup(hitDetectionGroup, "Hit Detection");
//        if (hitDetectionGroup)
//        {
//            EditorGUILayout.PropertyField(DetectionRadius);
//            EditorGUILayout.PropertyField(Root);
//            EditorGUILayout.PropertyField(Tip);
//            EditorGUILayout.PropertyField(HittableLayers);
//            EditorGUILayout.PropertyField(OnHitLifetime);
//        }
//        EditorGUILayout.EndFoldoutHeaderGroup();
//        EditorGUILayout.Space(10);

//        impactVfxGroup = EditorGUILayout.BeginFoldoutHeaderGroup(impactVfxGroup, "Impact VFX");
//        if (impactVfxGroup)
//        {
//            EditorGUILayout.PropertyField(ImpactVfx);
//            if (_spellController.ImpactVfx)
//            {
//                EditorGUILayout.PropertyField(ImpactVfxLifetime);
//                EditorGUILayout.PropertyField(ImpactVfxSpawnOffset);
//            }
//        }
//        EditorGUILayout.EndFoldoutHeaderGroup();
//        EditorGUILayout.Space(10);

//        debugGroup = EditorGUILayout.BeginFoldoutHeaderGroup(debugGroup, "Debug");
//        if (debugGroup)
//        {
//            EditorGUILayout.PropertyField(DisplayColliderRadius);
//            if (_spellController.DisplayColliderRadius)
//            {
//                EditorGUILayout.PropertyField(ColliderColor);

//                if(_spellController.SpellType == SpellController.SpellTypes.Area)
//                {
//                    EditorGUILayout.PropertyField(ExplosionRadiusColor);
//                }
//            }
//        }
//        EditorGUILayout.EndFoldoutHeaderGroup();
//        EditorGUILayout.Space(10);


//        serializedObject.ApplyModifiedProperties();
//    }
//}
