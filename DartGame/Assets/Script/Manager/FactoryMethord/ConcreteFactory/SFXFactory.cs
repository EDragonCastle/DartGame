using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SFXFactory : FactoryMethod<SFXSound>
{
    public SFXFactory(SFXSound prefab, int poolSize = 30, Transform _parent = null) : base(prefab, poolSize, _parent)
    {
    }
}
