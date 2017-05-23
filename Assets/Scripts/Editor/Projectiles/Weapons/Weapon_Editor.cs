using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Weapon))]
public class Weapon_Editor : Editor {

    SerializedProperty _weaponName;
    SerializedProperty _weaponProjectile;
    SerializedProperty _weaponReload;

    SerializedProperty _weaponFiringMode;
    SerializedProperty _weaponPossibleSpread;
    SerializedProperty _weaponFireRatePerSecond;
    SerializedProperty _weaponBurstsPerSecond;
    SerializedProperty _weaponShotsPerBurst;
    SerializedProperty _weaponBulletsPerShot;

    SerializedProperty _weaponAmmoSystem;
    SerializedProperty _weaponMaxStoredAmmo;
    SerializedProperty _weaponCurrentStoredAmmo;
    SerializedProperty _weaponAmmoPackAmount;
    SerializedProperty _weaponAmmoPerClip;

    SerializedProperty _weaponShotAudioClip;

    Dictionary<SerializedProperty, string> _toolTips = new Dictionary<SerializedProperty, string>();

    bool _generalWeaponsSettingFoldout = true;
    bool _weaponFiringSettingFoldout = true;
    bool _weaponAmmoSettingsFoldout = true;
    bool _weaponAudioSettingsFoldout = false;

    GUIStyle foldoutStyle = new GUIStyle();


    void OnEnable()
    {
        _weaponName = FindProperty(serializedObject, Weapon.PROPERTY_WEAPON_NAME);

        _weaponProjectile = FindProperty(serializedObject,Weapon.PROPERTY_WEAPON_PROJECTILE);
        _weaponReload = FindProperty(serializedObject,Weapon.PROPERTY_RELOAD_TIME);

        _weaponFiringMode = FindProperty(serializedObject,Weapon.PROPERTY_WEAPON_FIRING_MODE);
        _weaponPossibleSpread = FindProperty(serializedObject,Weapon.PROPERTY_POSSIBLE_SPREAD);
        _weaponFireRatePerSecond = FindProperty(serializedObject,Weapon.PROPERTY_FIRERATE_PER_SECOND);
        _weaponBurstsPerSecond = FindProperty(serializedObject, Weapon.PROPERTY_BURSTS_PER_SECOND);
        _weaponShotsPerBurst = FindProperty(serializedObject, Weapon.PROPERTY_SHOTS_PER_BURST);
        _weaponBulletsPerShot = FindProperty(serializedObject, Weapon.PROPERTY_BULLETS_PER_SHOT);

        _weaponAmmoSystem = FindProperty(serializedObject, Weapon.PROPERTY_AMMO_SYSTEM);
        _weaponMaxStoredAmmo = FindProperty(serializedObject, Weapon.PROPERTY_MAX_STORED_AMMO);
        _weaponCurrentStoredAmmo = FindProperty(serializedObject, Weapon.PROPERTY_CURRENT_STORED_AMMO);
        _weaponAmmoPackAmount = FindProperty(serializedObject, Weapon.PROPERTY_AMMO_PACK_AMOUNT);
        _weaponAmmoPerClip = FindProperty(serializedObject, Weapon.PROPERTY_AMMO_PER_CLIP);

        _weaponShotAudioClip = FindProperty(serializedObject, Weapon.PROPERTY_SHOT_AUDIO_CLIP);

    }

    private SerializedProperty FindProperty(SerializedObject obj,string name)
    {
        SerializedProperty prop = obj.FindProperty(name);
        TooltipAttribute tooltip = (TooltipAttribute)obj.targetObject.GetType().GetField(name).GetCustomAttributes(false)[0];
        if (tooltip != null)
        {
            _toolTips[prop] = tooltip.tooltip;
        }
        return prop;
    }

    public override void OnInspectorGUI()
    {
        foldoutStyle = EditorStyles.foldout;
        foldoutStyle.font = EditorStyles.boldFont;

        serializedObject.Update();

        _weaponName.stringValue = EditorGUILayout.TextField(new GUIContent("Weapon Name", _toolTips[_weaponName]), _weaponName.stringValue);

        _generalWeaponsSettingFoldout = EditorGUILayout.Foldout(_generalWeaponsSettingFoldout, new GUIContent("General Weapons Settings"), foldoutStyle);
        if (_generalWeaponsSettingFoldout)
        {
            EditorGUILayout.PropertyField(_weaponProjectile, new GUIContent("Weapon Projectile", _toolTips[_weaponProjectile]));
            _weaponReload.floatValue = ClampedFloatField(new GUIContent("Weapon Reload Time", _toolTips[_weaponReload]),_weaponReload.floatValue, 0);
        }

        GUILayout.Space(5);

        _weaponFiringSettingFoldout = EditorGUILayout.Foldout(_weaponFiringSettingFoldout, new GUIContent("Weapons Firing Settings"), foldoutStyle);
        if (_weaponFiringSettingFoldout)
        {
            EditorGUILayout.PropertyField(_weaponFiringMode, new GUIContent("Weapon Firing Mode", _toolTips[_weaponFiringMode]));
            _weaponPossibleSpread.vector2Value = EditorGUILayout.Vector2Field(new GUIContent("Weapon Possible Spread", _toolTips[_weaponPossibleSpread]), _weaponPossibleSpread.vector2Value);

            GUILayout.Space(2);

            _weaponBulletsPerShot.intValue = ClampedIntField(new GUIContent("Bullets Per Shots", _toolTips[_weaponBulletsPerShot]), _weaponBulletsPerShot.intValue, 1);
            _weaponFireRatePerSecond.floatValue = ClampedFloatField(new GUIContent("Shots Per Second", _toolTips[_weaponFireRatePerSecond]), _weaponFireRatePerSecond.floatValue,0);
            Weapon.WeaponFiringMode firingMode = (Weapon.WeaponFiringMode)_weaponFiringMode.enumValueIndex;
            if (firingMode == Weapon.WeaponFiringMode.Burst || firingMode == Weapon.WeaponFiringMode.AutomaticBursts)
            {
                _weaponShotsPerBurst.intValue = ClampedIntField(new GUIContent("Shots Per Burst", _toolTips[_weaponShotsPerBurst]), _weaponShotsPerBurst.intValue, 1);
            }
            if (firingMode == Weapon.WeaponFiringMode.AutomaticBursts)
            {
                _weaponBurstsPerSecond.floatValue = ClampedFloatField(new GUIContent("Bursts Per Second", _toolTips[_weaponBurstsPerSecond]), _weaponBurstsPerSecond.floatValue, 0);
            }
            EditorGUILayout.LabelField(new GUIContent("Total Bullets Per Seconds : " + CalculateTotalBulletsPerSecond(firingMode), "How many bullets per second in total"), EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.LabelField(new GUIContent("Total Bullets Per Burst : " + CalculateTotalBulletsPerBurst(firingMode), "How many bullets per burst in total"), EditorStyles.centeredGreyMiniLabel);
        }

        GUILayout.Space(5);

        _weaponAmmoSettingsFoldout = EditorGUILayout.Foldout(_weaponAmmoSettingsFoldout, new GUIContent("Weapon Ammo Settings"), foldoutStyle);
        if (_weaponAmmoSettingsFoldout)
        {
            EditorGUILayout.PropertyField(_weaponAmmoSystem, new GUIContent("Weapon Ammo System", _toolTips[_weaponAmmoSystem]));

            GUILayout.Space(2);

            _weaponMaxStoredAmmo.intValue = ClampedIntField(new GUIContent("Max Stored Ammo"), _weaponMaxStoredAmmo.intValue, 0);
            _weaponCurrentStoredAmmo.intValue = ClampedIntField(new GUIContent("Current Stored Ammo"), _weaponCurrentStoredAmmo.intValue, 0, _weaponMaxStoredAmmo.intValue);

            GUILayout.Space(2);

            _weaponAmmoPackAmount.intValue = ClampedIntField(new GUIContent("Ammo per Ammo Pack Pickup"), _weaponAmmoPackAmount.intValue, 0, _weaponMaxStoredAmmo.intValue);

            GUILayout.Space(2);

            Weapon.AmmoSystem ammoSystem = (Weapon.AmmoSystem)_weaponAmmoSystem.enumValueIndex;
            if (ammoSystem == Weapon.AmmoSystem.Clip)
            {
                _weaponAmmoPerClip.intValue = ClampedIntField(new GUIContent("Ammo Per Clip"), _weaponAmmoPerClip.intValue, 0, _weaponMaxStoredAmmo.intValue);
            }
        }

        GUILayout.Space(5);

        _weaponAudioSettingsFoldout = EditorGUILayout.Foldout(_weaponAudioSettingsFoldout, new GUIContent("Weapon Audio Settings"), foldoutStyle);
        if (_weaponAudioSettingsFoldout)
        {
            EditorGUILayout.PropertyField(_weaponShotAudioClip, new GUIContent("Weapon Shot Audio Clip", _toolTips[_weaponShotAudioClip]));
        }

        serializedObject.ApplyModifiedProperties();
    }

    private float ClampedFloatField(GUIContent guiContent, float propertyFloat, float min = float.MinValue, float max = float.MaxValue)
    {
        return Mathf.Clamp(EditorGUILayout.FloatField(guiContent, propertyFloat), min, max);
    }

    private int ClampedIntField(GUIContent guiContent, int propertyInt, int min = int.MinValue, int max = int.MaxValue)
    {
        return Mathf.Clamp(EditorGUILayout.IntField(guiContent, propertyInt), min, max);
    }

    private float CalculateTotalBulletsPerSecond(Weapon.WeaponFiringMode firingMode)
    {
        if(firingMode == Weapon.WeaponFiringMode.Semi || firingMode == Weapon.WeaponFiringMode.Burst)
        {
            return float.PositiveInfinity;
        }
        float shotDuration = _weaponFireRatePerSecond.floatValue;
        if (firingMode == Weapon.WeaponFiringMode.Automatic)
        {
            return shotDuration;
        }
        float burstDuration = shotDuration / _weaponBulletsPerShot.intValue;
        float burstDurationWithDelay = burstDuration / _weaponBurstsPerSecond.floatValue;
        return burstDurationWithDelay;
    }

    private float CalculateTotalBulletsPerBurst(Weapon.WeaponFiringMode firingMode)
    {
        if (firingMode == Weapon.WeaponFiringMode.Semi)
        {
            return _weaponBulletsPerShot.intValue;
        }
        if (firingMode == Weapon.WeaponFiringMode.Automatic)
        {
            return float.PositiveInfinity;
        }
        float burstDuration = _weaponShotsPerBurst.intValue * _weaponBulletsPerShot.intValue;
        return burstDuration;
    }

}
