local drawableSprite = require("structs.drawable_sprite")
local xnaColors = require("consts.xna_colors")
local utils = require("utils")
local Monumentflipswitch = {}

Monumentflipswitch.name = "EmHelper/Monumentflipswitch"
Monumentflipswitch.depth = 2000

Monumentflipswitch.fieldInformation = {
    color = {
        fieldType = "color",
        allowXNAColors = true,
    }
}

local switches = {
    "objects/monumentflipswitch0%s",
    "objects/monumentflipswitch1%s",
    "objects/monumentflipswitch2%s",
    "objects/monumentflipswitch3%s"
}

Monumentflipswitch.placements = {
    {
        name = "Monument Flip Switch (both) [EmHelper]",
        data = {
            onlyEnable = false,
            onlyDisable = false,
            color = "LightSkyBlue",
            mute = false,
            pattern = 0
        },
    },
    {
        name = "Monument Flip Switch (disable) [EmHelper]",
        data = {
            onlyEnable = false,
            onlyDisable = true,
            color = "LightSkyBlue",
            mute = false,
            pattern = 0
        },
    },
    {
        name = "Monument Flip Switch (enable) [EmHelper]",
        data = {
            onlyEnable = true,
            onlyDisable = false,
            color = "LightSkyBlue",
            mute = false,
            pattern = 0
        },
    }
}

function Monumentflipswitch.sprite(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local onlyEnable = entity.onlyEnable
    local onlyDisable = entity.onlyDisable
    local pattern = (entity.pattern % 4) or 0
    local flipswitchtexture = string.format(switches[pattern+1], "/switch01")

    if onlyEnable then
        flipswitchtexture = string.format(switches[pattern+1], "/switch13")
    elseif onlyDisable then
        flipswitchtexture = string.format(switches[pattern+1], "/switch15")
    end

    local flipswitchSprite = drawableSprite.fromTexture(flipswitchtexture, entity)

    flipswitchSprite:setJustification(1, 1)
    flipswitchSprite:addPosition(16, 16)

    return flipswitchSprite
end

function Monumentflipswitch.rectangle(room, entity)
    return utils.rectangle(entity.x-8, entity.y-6, 16, 21)
end

return Monumentflipswitch