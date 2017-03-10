namespace MoonRay.Scene

open MoonRay.Math

type Color(r : float, g : float, b : float) =     
    member this.R = r
    member this.G = g
    member this.B = b    
    
    static member (+)  (a : Color, b : Color) = new Color(a.R + b.R, a.G + b.G, a.B + b.B)
    static member (-)  (a : Color, b : Color) = new Color(a.R - b.R, a.G - b.G, a.B - b.B)
    static member (.*) (a : Color, b : Color) = new Color(a.R * b.R, a.G * b.G, a.B * b.B)
    
    static member (*) (r : float, a : Color) = new Color(r * a.R, r * a.G, r * a.B)
    static member (*) (a : Color, r : float) = r * a  
    static member Zero = new Color(0.0, 0.0, 0.0)

type Material = 
    abstract Diffuse    : Vector3 -> Color
    abstract Specular   : Vector3 -> Color
    abstract Roughness  : Vector3 -> float
    abstract Reflection : Vector3 -> float
    
type IGeometry = 
    abstract IntersectRay : Ray -> float
    abstract Normal       : Vector3 -> Vector3

type Object(geometry : IGeometry, material : Material) =
    member this.Geometry = geometry
    member this.Material = material

type Light(position : Vector3, color : Color) =
    member this.Position = position
    member this.Color    = color

type Camera(pos : Vector3, lookAt : Vector3, sx : float, sy : float) = 
    let fwd  = lookAt - pos
       
    let down = new Vector3(0.0, 1.0, 0.0)
    let right = sx * (fwd .** down).Unit
    let up    = sy * (fwd .** right).Unit
       
    member this.Voxel(x, y) = pos + fwd + (2.0 * x - 1.0) * right + (2.0 * y - 1.0) * up
    member this.CameraPos = pos

type Scene(lights     : Light seq,
           objects    : Object seq,
           background : Color,
           camera     : Camera) =

    member this.Objects    = objects
    member this.Lights     = lights
    member this.Background = background
    member this.Camera     = camera