local spikeHelper = require("helpers.spikes")
local drawableSprite = require("structs.drawable_sprite")
local xnaColors = require("consts.xna_colors")
local utils = require("utils")
local entities = require("entities")

local variants = {
    "default",
    "outline",
}

function createEntityHandler(name, direction, variants)
    variants = variants or spikeHelper.spikeVariants

    local handler = {}

    local spriteFunction = spikeHelper.getSpikeSprites

    handler.name = name

    handler.fieldInformation = {
        color = {
            fieldType = "color",
            allowXNAColors = true,
        }
    }

    handler.placements = getSpikePlacements(direction, variants)
    
    handler.canResize = spikeHelper.getCanResize(direction)
    handler.fieldInformation = getFieldInformations(variants)

    function handler.sprite(room, entity)
        local color = entity.color or "LightSkyBlue"
        local sprites = spriteFunction(entity, direction, false)
        for _, sprite in ipairs(sprites) do
            sprite:setColor(color)
        end
        return sprites
    end

    function handler.selection(room, entity)
        local sprites = spriteFunction(entity, direction, false)

        return entities.getDrawableRectangle(sprites)
    end

    return handler
end

function getFieldInformations(variants)
    return {
        type = {
            options = variants
        },
        color = {
            fieldType = "color",
            allowXNAColors = true,
        }
    }
end

function getSpikePlacements(direction, variants)
    local placements = {}
    local horizontal = direction == "left" or direction == "right"
    local lengthKey = horizontal and "height" or "width"

    for i, variant in ipairs(variants) do
        placements[i] = {
            name = string.format("Monument Spikes [EmHelper] (%s) (%s)", direction, variant),
            data = {
                type = variant,
                color = "LightSkyBlue",
                active = true,
            }
        }

        placements[i].data[lengthKey] = 8
    end

    return placements
end

local monumentspikeUp = createEntityHandler("EmHelper/MonumentspikesUp", "up", variants)
local monumentspikeDown = createEntityHandler("EmHelper/MonumentspikesDown", "down", variants)
local monumentspikeLeft = createEntityHandler("EmHelper/MonumentspikesLeft", "left", variants)
local monumentspikeRight = createEntityHandler("EmHelper/MonumentspikesRight", "right", variants)

return {
    monumentspikeUp,
    monumentspikeDown,
    monumentspikeLeft,
    monumentspikeRight
}