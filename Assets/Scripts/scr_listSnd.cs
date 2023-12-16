using UnityEngine;
using System.Collections.Generic;

//[CreateAssetMenu(menuName = "ScrObjects/Sound Table")]
public class scr_listSnd : MonoBehaviour
{
    public AudioClip[] tableSound;

    void Start()
    {
        Object[] soundObjects = scr_assetBundle.m.LoadAsset("audio.sound");

        tableSound = new AudioClip[soundObjects.Length];

        for (int i = 0; i < soundObjects.Length; i++)
        {
            tableSound[i] = (AudioClip)soundObjects[i];
        }
    }
/*    [ContextMenu("LoadSounds")]
    void LoadSounds()
    {
        tableSound.Clear();
        string[] soundPaths = Directory.GetFiles("Assets/Audio/Sounds", "*.wav");

        foreach (string path in soundPaths)
        {
            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            if (clip != null)
            {
                tableSound.Add(clip);
            }
        }
    }*/
}
