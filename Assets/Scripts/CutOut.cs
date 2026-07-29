using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
public class CutOut : Image
{
    private Material cutoutMaterial;

    public override Material materialForRendering
    {
        get
        {
            if (cutoutMaterial == null)
            {
                cutoutMaterial = new Material(base.materialForRendering);
                cutoutMaterial.SetInt("_Stencil", 1);
                cutoutMaterial.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
            }
            return cutoutMaterial;
        }
    }
}
