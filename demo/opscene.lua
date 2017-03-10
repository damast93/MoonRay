-- Some beautiful scene
-----------------------------------

-- Helper functions, just for syntax
local function Color(r,g,b)
    return {r,g,b}
end

local function Vector(x,y,z)
    return {x,y,z}
end


-- Define materials

local shiny = {
    Diffuse   = Color(1,0.3,0.3),
    Specular  = Color(0.5,0.5,0.5),
    Reflection = 0.3,
    Roughness  = 50
}

local mirror = {
    Diffuse    = Color(0.2,0.2,0.2),
    Specular   = Color(0.8,0.8,0.8),
    Reflection = 1,
    Roughness  = 200
}

local checkerboard = {
    Specular   = Color(0,0,0),
    Roughness  = 1,
    Reflection = function(x,y,z)
        return (math.floor(x)+math.floor(z)) % 2 == 0 and 0.3 or 0.0
    end,
    Diffuse = function(x,y,z)
        return (math.floor(x)+math.floor(z)) % 2 == 0 and Color(0,0,0) or Color(0.5,0.5,0.5)
    end
}

-- Define objects

table.insert(Scene.Objects, {
    Type = "Plane",
    Normal = Vector(0,1,0),
    Offset = 0,
    Material = checkerboard })
    
table.insert(Scene.Objects, {
    Type = "Sphere",
    Center = Vector(0, 3, 0),
    Radius = 1,
    Material = mirror })

local n = 20

for k = 1,n do
    local phi = 2*math.pi*k/n
    local x, y, z = 3 * math.cos(phi), 3+2*math.sin(2*(phi+0.2)), 3 * math.sin(phi)
    
    table.insert(Scene.Objects, {
        Type = "Sphere",
        Center = Vector(x,y,z),
        Radius = 0.4,
        Material = shiny })
end
    
-- Define lights
    
table.insert(Scene.Lights, {
    Position = Vector(-5,4.5,2),
    Color = Color(1,1,1)
})

table.insert(Scene.Lights, {
    Position = Vector(5,6,6),
    Color = Color(1,0.6,0.6)
})

-- Define background & camera

Scene.Background = Color(0,0,0)

Scene.Camera.Position = Vector(0,7.5,-16)
Scene.Camera.LookAt = Vector(0,2,2)
Scene.Camera.ScreenX = 4
Scene.Camera.ScreenY = 4