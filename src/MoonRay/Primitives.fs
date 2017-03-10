namespace MoonRay.Primitives

open MoonRay.Math
open MoonRay.Scene

type Sphere(m : Vector3, r : float) =
    interface IGeometry with
        member this.Normal v = (v - m).Unit
        member this.IntersectRay(ray) =
            let v = ray.Base - m
            let u = ray.Direction
            let p = -(v .* u)
            let disc = (u .* v) ** 2.0 - (v .* v - r * r)
            
            if disc < 0.0   then -1.0
            elif disc = 0.0 then  p
            else
                let sqrD = disc ** 0.5
                let res = [(p + sqrD); (p - sqrD)] |> List.filter ((<) 0.0)
                if res.IsEmpty then -1.0 else List.min res

type Plane(n : Vector3, d : float) = 
    static member FromNormalForm(n : Vector3, p : Vector3, t) = 
        let n0 = n.Unit
        let pl1 = new Plane(n0, -n0 .* p)
        if pl1.Positive t then pl1 else new Plane(-n0, n0 .* p)
        
    member this.Positive (v : Vector3) = (v .* n - d) > 0.0 
         
    interface IGeometry with
        member this.Normal(v) = n
        member this.IntersectRay(ray) = 
            let denom = n .* ray.Direction 
            if denom > 0.0
                then -1.0
                else (n .* ray.Base + d) * (-1.0 / denom)

type Triangle(a : Vector3, b : Vector3, c : Vector3) =         
    let n = ((c - a) .** (b - a))
    let plane = new Plane(n, n .* a)
    let borders = [ Plane.FromNormalForm(n .** (a - b), a, c);
                    Plane.FromNormalForm(n .** (b - c), b, a);
                    Plane.FromNormalForm(n .** (c - a), c, b) ]
    
    interface IGeometry with
        member this.Normal e = n
        member this.IntersectRay(ray) =
            match (plane :> IGeometry).IntersectRay ray with
            | t when t < 0.0 -> -1.0
            | t ->
                let pnt = ray.At t
                if borders |> List.forall (fun p -> p.Positive pnt) then t else -1.0