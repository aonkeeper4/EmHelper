local drawableSprite = require("structs.drawable_sprite")
local xnaColors = require("consts.xna_colors")
local utils = require("utils")
local Walkeline = {}

Walkeline.name = "EmHelper/Walkeline"
Walkeline.depth = 0

Walkeline.fieldInformation = {
    haircolor = {
        fieldType = "color",
        allowXNAColors = true,
    }
}

Walkeline.placements = {
    
    name = "Walkeline [EmHelper]",
    data = {
        haircolor = "212121",
        left = true,
        weak = false,
        dangerous = false,
        ally = true,
        bouncy = false,
        smart = false,
        mute = false,
        nobackpack = false,
        idle = false,
        deathflag = "WalkelineIsDead",
        triggerhappy = false
        }
    
}

local bodyTexture = "characters/walkeline/ahornbody"
local hairTexture = "characters/walkeline/ahornhair"
local scale = 1.0

function Walkeline.sprite(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local nobackpack = entity.nobackpack
    local left = entity.left
    local spritex = 11 --hardcoded as fuck

    if nobackpack then
        bodyTexture = "characters/windupwalkeline/ahornbody"
    else
        bodyTexture = "characters/walkeline/ahornbody"
    end

    if left then
        scale = -1.0
        spritex = -18 -- i like your magic numbers funny man
    else
        scale = 1.0
        spritex = 17
    end

    local bodySprite = drawableSprite.fromTexture(bodyTexture, {x = x, y = y})
    local hairSprite = drawableSprite.fromTexture(hairTexture, entity)


    bodySprite:setJustification(1, 1)
    bodySprite:addPosition(spritex, 0)
    bodySprite:setScale(scale, 1.0)

    hairSprite:setJustification(1, 1)
    hairSprite:addPosition(spritex, 0)

    hairSprite:setScale(scale, 1.0)
    hairSprite:setColor(entity.haircolor)
    return {
        bodySprite, 
        hairSprite,
    }
end

function Walkeline.rectangle(room, entity)
    local left = entity.left
    return utils.rectangle(entity.x-7, entity.y-17, 13, 17)
end

return Walkeline