using UnityEditor;
using Unity.Game.Gameplay;
using UnityEngine;

[CustomEditor(typeof(SpellData))]
public class SpellDataEditor : Editor
{
    #region SerializedProperties
    SerializedProperty SpellPrefab;

    SerializedProperty UseVoice;
    SerializedProperty VoiceName;

    SerializedProperty UseMovement;
    SerializedProperty MovementName;
    SerializedProperty LineMaterial;

    SerializedProperty Artwork;
    SerializedProperty CardTemplate;
    SerializedProperty SchoolSymbol;
    SerializedProperty TypeIcon;

    SerializedProperty CardName;
    SerializedProperty Description;
    SerializedProperty School;
    SerializedProperty PipCost;
    SerializedProperty Accuracy;

    bool generalGroup = true;
    bool voiceRecognizerGroup = true;
    bool movementRecognizerGroup = true;
    bool cardArtGroup = true;
    bool cardInfoGroup = true;
    #endregion

    private void OnEnable()
    {
        SpellPrefab = serializedObject.FindProperty("SpellPrefab");

        UseVoice = serializedObject.FindProperty("UseVoice");
        VoiceName = serializedObject.FindProperty("VoiceName");

        UseMovement = serializedObject.FindProperty("UseMovement");
        MovementName = serializedObject.FindProperty("MovementName");
        LineMaterial = serializedObject.FindProperty("LineMaterial");

        Artwork = serializedObject.FindProperty("Artwork");
        CardTemplate = serializedObject.FindProperty("CardTemplate");
        SchoolSymbol = serializedObject.FindProperty("SchoolSymbol");
        TypeIcon = serializedObject.FindProperty("TypeIcon");

        CardName = serializedObject.FindProperty("CardName");
        Description = serializedObject.FindProperty("Description");
        School = serializedObject.FindProperty("School");
        PipCost = serializedObject.FindProperty("PipCost");
        Accuracy = serializedObject.FindProperty("Accuracy");
    }

    public override void OnInspectorGUI()
    {
        SpellData _spellData = (SpellData) target;
        
        serializedObject.Update();

        generalGroup = EditorGUILayout.BeginFoldoutHeaderGroup(generalGroup, "General");
        if (generalGroup)
        {
            EditorGUILayout.PropertyField(SpellPrefab);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        GUILayout.Space(10);

        voiceRecognizerGroup = EditorGUILayout.BeginFoldoutHeaderGroup(voiceRecognizerGroup, "Voice");
        if (voiceRecognizerGroup)
        {
            EditorGUILayout.PropertyField(UseVoice);
            if (_spellData.UseVoice)
            {
                EditorGUILayout.PropertyField(VoiceName);
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        GUILayout.Space(10);

        movementRecognizerGroup = EditorGUILayout.BeginFoldoutHeaderGroup(movementRecognizerGroup, "Movement");
        if (movementRecognizerGroup)
        {
            EditorGUILayout.PropertyField(UseMovement);
            if (_spellData.UseMovement)
            {
                EditorGUILayout.PropertyField(MovementName);
                EditorGUILayout.PropertyField(LineMaterial);
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        GUILayout.Space(10);

        cardArtGroup = EditorGUILayout.BeginFoldoutHeaderGroup(cardArtGroup, "Card Art");
        if(cardArtGroup)
        {
            EditorGUILayout.PropertyField(Artwork);
            EditorGUILayout.PropertyField(CardTemplate);
            EditorGUILayout.PropertyField(SchoolSymbol);
            EditorGUILayout.PropertyField(TypeIcon);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        GUILayout.Space(10);

        cardInfoGroup = EditorGUILayout.BeginFoldoutHeaderGroup(cardInfoGroup, "Card Info");
        if (cardInfoGroup)
        {
            EditorGUILayout.PropertyField(CardName);
            EditorGUILayout.PropertyField(Description);
            EditorGUILayout.PropertyField(School);
            EditorGUILayout.PropertyField(PipCost);
            EditorGUILayout.PropertyField(Accuracy);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        GUILayout.Space(10);


        serializedObject.ApplyModifiedProperties();
    }
}
