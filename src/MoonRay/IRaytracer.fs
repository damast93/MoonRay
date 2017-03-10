namespace MoonRay

open MoonRay.Math
open MoonRay.Scene

type SetPixelCallback = delegate of int * int * Color -> unit

type IRaytracer = 
    abstract member RenderScene : scene:Scene * width : int * height : int * recursionDepth:int * setPixel : SetPixelCallback -> unit