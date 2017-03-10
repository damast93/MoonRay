namespace MoonRay.Math

type Vector3(x : float, y : float, z : float) = 
    static member rnd = new System.Random()

    member this.x = x
    member this.y = y
    member this.z = z
    
    member this.LengthSq = x*x + y*y + z*z
    member this.Length = this.LengthSq ** 0.5
    member this.Unit = this * (1.0 / this.Length)
    
    static member (+) (a : Vector3, b : Vector3) = new Vector3(a.x + b.x, a.y + b.y, a.z + b.z)
    static member (-) (a : Vector3, b : Vector3) = new Vector3(a.x - b.x, a.y - b.y, a.z - b.z)
    static member (~-) (a : Vector3) = -1.0 * a
    
    static member (*) (r : float, a : Vector3) = new Vector3(r * a.x, r * a.y, r * a.z)
    static member (*) (a : Vector3, r : float) = r * a  
    
    static member (.*) (a : Vector3, b : Vector3) = a.x * b.x + a.y * b.y + a.z * b.z
    static member (.**) (a : Vector3, b : Vector3) = new Vector3((a.y * b.z) - (a.z * b.y), (a.z * b.x) - (a.x * b.z), (a.x * b.y) - (a.y * b.x))
    
    override this.ToString() = (x, y, z).ToString()

    static member RandomNormal() =
        let x,y = Vector3.rnd.NextDouble(), Vector3.rnd.NextDouble()
        let z = Seq.head (seq { let z' = Vector3.rnd.NextDouble() in if Vector3(x,y,z').LengthSq > 0.0 then yield z' })
        Vector3(x,y,z).Unit

type Ray(p : Vector3, u : Vector3) = 
    let u0 = u.Unit
    
    member this.Base = p
    member this.Direction = u0
 
    member this.At t = this.Base + t * this.Direction
    static member FromPoints (A : Vector3, B : Vector3) = new Ray(A, B - A)