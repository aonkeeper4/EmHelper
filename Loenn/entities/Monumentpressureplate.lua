local drawableSprite = require("structs.drawable_sprite")
local xnaColors = require("consts.xna_colors")
local utils = require("utils")
local Monumentpressureplate = {}

Monumentpressureplate.name = "EmHelper/Monumentpressureplate"
Monumentpressureplate.depth = 2000

local patterns = {
    "objects/monumentpressureplate/solid0",
    "objects/monumentpressureplate/solid1",
    "objects/monumentpressureplate/solid2",
    "objects/monumentpressureplate/solid3"
}

Monumentpressureplate.fieldInformation = {
    color = {
        fieldType = "color",
        allowXNAColors = true,
    }
}

Monumentpressureplate.placements = {
    
    name = "Monument Pressure Plate [EmHelper]",
    data = {
        pattern = 0,
        onetime = false,
        color = "LightSkyBlue",
        mute = false,
        isButton = false,
        disable = false
        }
    
}

function Monumentpressureplate.sprite(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local pattern = (entity.pattern % 4) or 0

    local PressurePlateSprite = drawableSprite.fromTexture(patterns[pattern+1], entity)
    local eSprite = drawableSprite.fromTexture(eTexture, entity)

    PressurePlateSprite:setJustification(1, 1)
    PressurePlateSprite:addPosition(8, 0)

    return PressurePlateSprite
end

function Monumentpressureplate.rectangle(room, entity)
    return utils.rectangle(entity.x-6, entity.y-4, 12, 4)
end

return Monumentpressureplate