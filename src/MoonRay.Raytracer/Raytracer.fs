namespace MoonRay.Raytracer

open MoonRay
open MoonRay.Math
open MoonRay.Scene

type Raytracer() = 
    interface IRaytracer with
        member this.RenderScene(scene, width, height, recursiveDepth, setPixel) = 

            let voxel(x, y) = scene.Camera.Voxel(float x / float width, float y / float height)
        
            let firstIntersection ray =
                let intersections = seq { for obj in scene.Objects do 
                                                let t = obj.Geometry.IntersectRay ray
                                                if t >= 0.0 then yield (t, obj) }
                                          
                if intersections |> Seq.isEmpty
                    then None
                    else Some(intersections |> Seq.minBy fst)
        
            let raytrace(x, y) =
                let ray = Ray.FromPoints(scene.Camera.CameraPos, voxel(x, y))
            
                let rec raytraceRec ray depth =
                    match firstIntersection ray with
                    | None         -> scene.Background
                    | Some(t, obj) -> 
                        let pos = ray.At t
                        let material = obj.Material
                        let norm = obj.Geometry.Normal pos
                    
                        let d = ray.Direction 
                        let reflectedDir = (d - (2.0 * norm .* d) * norm).Unit
                    
                        let lightSum = 
                            scene.Lights |> Seq.sumBy (fun light ->
                             
                                    let lightRay = Ray.FromPoints(light.Position, pos)
                             
                                    let (|Illuminated|Shaded|) = function
                                        | None -> Illuminated
                                        | Some(_, x) when x = obj -> Illuminated
                                        | _ -> Shaded
                             
                                    match firstIntersection lightRay with
                                    | Shaded -> Color.Zero
                                    | Illuminated -> 
                                        let phi      = -lightRay.Direction .* norm
                                        let theta    = -lightRay.Direction .* reflectedDir
                                        let direct   = if phi   >= 0.0 then light.Color * phi else Color.Zero
                                        let specular = if theta >= 0.0 then light.Color * (theta ** (material.Roughness pos)) else Color.Zero 
                                 
                                        (direct .* material.Diffuse pos) + (specular .* material.Specular pos)
                            )     

                        if material.Reflection pos <= 0.0 || depth <= 0
                            then lightSum + new Color(0.5, 0.5, 0.5)
                            else 
                                let reflectedRay = new Ray(pos + 0.0001 * reflectedDir, reflectedDir)
                                lightSum + material.Reflection pos * raytraceRec reflectedRay (depth - 1) 
                                                    
                raytraceRec ray recursiveDepth

            let screenPixels = [| for x = 0 to width - 1 do for y = 0 to height - 1 do yield (x, y) |]

            screenPixels |> Array.iter (fun (x,y) ->
                let color = raytrace(x,y)
                setPixel.Invoke(x,y,color)
            )