using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

[DefaultExecutionOrder(-9999)]
public class GameManager : MonoBehaviour
{
    private readonly UniTaskCompletionSource initalize = new UniTaskCompletionSource();
    public UniTask ResourceInitalize => initalize.Task;


    void Awake()
    {
        // 의존성 주입으로 Lazy Initalize 해결
        Locator<GameManager>.Provide(this);

        EventManager eventManager = new EventManager();
        Locator<EventManager>.Provide(eventManager);

        ResourceManager resourceManager = new ResourceManager();
        Locator<ResourceManager>.Provide(resourceManager);

        Initalize().Forget();
    }

    private async UniTask Initalize()
    {
        try
        {
            var resourceManger = Locator<ResourceManager>.Get();
            var dart = await resourceManger.Get<GameObject>("Dart");
            var throwSound = await resourceManger.Get<GameObject>("ThrowSound");
            var dartComponent = dart.GetComponent<Dart>();
            var sfxSoundComponent = throwSound.GetComponent<SFXSound>();

            DartFactory factory = new DartFactory(dartComponent, poolSize: 20);
            Locator<DartFactory>.Provide(factory);

            SFXFactory soundFactory = new SFXFactory(sfxSoundComponent, poolSize: 20);
            Locator<SFXFactory>.Provide(soundFactory);

            initalize.TrySetResult();
        }
        catch(Exception e)
        {
            initalize.TrySetException(e);
        }
    }

}
