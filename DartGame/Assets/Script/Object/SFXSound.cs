using UnityEngine;

public class SFXSound : MonoBehaviour, IEntity
{
    public AudioSource source;

    public void OnDespawn()
    {
        source.Stop();
    }

    public void OnSpawn()
    {
        source.Play();
        Invoke(nameof(AutoRelease), source.clip.length);
    }

    public void SetTransform(Vector3 position, Quaternion rotation, float multiplier = 1, Transform parent = null)
    {

    }

    private void AutoRelease()
    {
        var sfxFactory = Locator<SFXFactory>.Get();
        sfxFactory.Despawn(this);
    }

}
