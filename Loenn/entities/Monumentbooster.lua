local drawableSprite = require("structs.drawable_sprite")
local xnaColors = require("consts.xna_colors")
local utils = require("utils")
local Monumentbooster = {}

Monumentbooster.name = "EmHelper/Monumentbooster"
Monumentbooster.depth = -8500

Monumentbooster.fieldInformation = {
    color = {
        fieldType = "color",
        allowXNAColors = true,
    }
}

Monumentbooster.placements = {
    {
        name = "Monument Booster (Green) [EmHelper]",
        data = {
            pattern = 0,
            red = false,
            color = "LightSkyBlue",
            active = true
        }
    },
    {
        name = "Monument Booster (Red) [EmHelper]",
        data = {
            pattern = 0,
            red = true,
            color = "LightSkyBlue",
            active = true
        }
    }
}

local eTexture = "objects/monumentbooster/e"
local bubbleTexture = "objects/monumentbooster/ebooster00"

function Monumentbooster.sprite(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local red = entity.red

    if red then
        bubbleTexture = "objects/monumentbooster/eboosterRed00"
    else
        bubbleTexture = "objects/monumentbooster/ebooster00"
    end

    local bubbleSprite = drawableSprite.fromTexture(bubbleTexture, {x = x, y = y})
    local eSprite = drawableSprite.fromTexture(eTexture, entity)

    bubbleSprite:setJustification(1, 1)
    bubbleSprite:addPosition(16, 16)

    eSprite:setJustification(1, 1)
    eSprite:addPosition(16, 16)

    return {
        bubbleSprite, 
        eSprite,
    }
end

function Monumentbooster.rectangle(room, entity)
    return utils.rectangle(entity.x-8, entity.y-8, 16, 16)
end

return Monumentbooster