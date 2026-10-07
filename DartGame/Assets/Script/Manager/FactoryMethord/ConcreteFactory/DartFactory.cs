using UnityEngine;

public class DartFactory : FactoryMethod<Dart>
{
    public DartFactory(Dart prefab, int poolSize = 30, Transform _parent = null) : base(prefab, poolSize, _parent)
    {
    }
}
