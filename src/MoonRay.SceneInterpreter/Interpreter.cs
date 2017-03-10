using MoonSharp.Interpreter;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MoonRay.Math;
using MoonRay.Scene;
using MoonRay.Primitives;

namespace MoonRay.SceneInterpreter
{
    public class Interpreter
    {
        private static readonly CoreModules opts =
              CoreModules.TableIterators
            | CoreModules.Table
            | CoreModules.Math;

        private Color ToColor(DynValue color)
        {
            var t = color.Table;
            double r = t.Get(1).Number;
            double g = t.Get(2).Number;
            double b = t.Get(3).Number;

            return new Color(r, g, b);
        }

        private Vector3 ToVector(DynValue vector)
        {
            var v = vector.Table;

            double x = v.Get(1).Number;
            double y = v.Get(2).Number;
            double z = v.Get(3).Number;

            return new Vector3(x, y, z);
        }
        
        private IGeometry GetPlaneGeometry(Table t)
        {
            var normal = ToVector(t.Get("Normal"));
            var offset = t.Get("Offset").Number;
            return new Plane(normal, offset);
        }

        private IGeometry GetSphereGeometry(Table t)
        {
            var center = ToVector(t.Get("Center"));
            var radius = t.Get("Radius").Number;
            return new Sphere(center, radius);
        }

        private Object ToObject(DynValue d_obj)
        {
            var t_obj = d_obj.Table;

            var type = t_obj.Get("Type").String;
            var material = ToMaterial(t_obj.Get("Material"));

            IGeometry geometry;

            switch (type)
            {
                case "Plane":
                    geometry = GetPlaneGeometry(t_obj);
                    break;
                case "Sphere":
                    geometry = GetSphereGeometry(t_obj);
                    break;
                default:
                    throw new System.InvalidOperationException("Unknown geometry");
            }
            
            return new Object(geometry, material);
        }

        private Material ToMaterial(DynValue d_material)
        {
            var t_material = d_material.Table;

            var material = new LambdaMaterial();

            material.Diffuse = ToColorFunc(t_material.Get("Diffuse"));
            material.Specular = ToColorFunc(t_material.Get("Specular"));
            material.Roughness = ToScalarFunc(t_material.Get("Roughness"));
            material.Reflection = ToScalarFunc(t_material.Get("Reflection"));

            return material;
        }

        private ColorFunc ToColorFunc(DynValue d)
        {
            if (d.Type == DataType.Table)
            {
                var color = ToColor(d);
                return v => color;
            }
            else if (d.Type == DataType.Function)
            {
                var fn = d.Function;
                return vec => ToColor(fn.Call(vec.x, vec.y, vec.z));
            }
            else
            {
                throw new System.InvalidOperationException("Invalid datatype");
            }
        }

        private ScalarFunc ToScalarFunc(DynValue d)
        {
            if (d.Type == DataType.Number)
            {
                return v => d.Number;
            }
            else if (d.Type == DataType.Function)
            {
                var fn = d.Function;
                return vec => fn.Call(vec.x, vec.y, vec.z).Number;
            }
            else
            {
                throw new System.InvalidOperationException("Invalid datatype");
            }
        }

        private Camera ToCamera(DynValue d)
        {
            var t = d.Table;

            var position = ToVector(t.Get("Position"));
            var lookAt = ToVector(t.Get("LookAt"));
            var screenX = t.Get("ScreenX").Number;
            var screenY = t.Get("ScreenY").Number;
            return new Camera(position, lookAt, screenX, screenY);
        }

        private Light ToLight(DynValue d)
        {
            var t = d.Table;

            var position = ToVector(t.Get("Position"));
            var color = ToColor(t.Get("Color"));

            return new Light(position, color);
        }

        public Scene.Scene ToScene(DynValue d_scene)
        {
            var t_scene = d_scene.Table;

            Table t_objects = t_scene.Get("Objects").Table;
            Table t_lights = t_scene.Get("Lights").Table;

            var objects = t_objects.Values.Select(d_obj => ToObject(d_obj)).ToList();
            var lights = t_lights.Values.Select(d_light => ToLight(d_light)).ToList();

            var camera = ToCamera(t_scene.Get("Camera"));
            Color background = ToColor(t_scene.Get("Background"));

            return new Scene.Scene(lights, objects, background, camera);
        }

        public Scene.Scene EvalScene(string description)
        {
            var script = new Script(opts); // !!

            // Set up everything
            var emptyscene = new Dictionary<string, object>();
            emptyscene.Add("Objects", new object[] { });
            emptyscene.Add("Camera", new object[] { });
            emptyscene.Add("Lights", new object[] { });

            script.Globals["Scene"] = emptyscene;

            // Run the script
            script.DoString(description);

            // Evaluate the results
            var d_scene = script.Globals.Get("Scene");
            return ToScene(d_scene);
        }
    }
}
