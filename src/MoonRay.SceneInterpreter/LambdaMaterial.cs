using MoonRay.Scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoonRay.Math;

namespace MoonRay.SceneInterpreter
{
    internal delegate Color ColorFunc(Vector3 v);
    internal delegate double ScalarFunc(Vector3 v);

    internal class LambdaMaterial : Material
    {
        public ColorFunc Diffuse { get; set; }
        public ColorFunc Specular { get; set; }
        public ScalarFunc Reflection { get; set; }
        public ScalarFunc Roughness { get; set; }

        Color Material.Diffuse(Vector3 v)
        {
            return Diffuse(v);
        }

        double Material.Reflection(Vector3 v)
        {
            return Reflection(v);
        }

        double Material.Roughness(Vector3 v)
        {
            return Roughness(v);
        }

        Color Material.Specular(Vector3 v)
        {
            return Specular(v);
        }
    }
}
