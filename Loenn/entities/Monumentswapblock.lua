local drawableSprite = require("structs.drawable_sprite")
local drawableLine = require("structs.drawable_line")
local drawableNinePatch = require("structs.drawable_nine_patch")
local drawableRectangle = require("structs.drawable_rectangle")
local utils = require("utils")
local xnaColors = require("consts.xna_colors")
local Monumentswapblock = {}

local themeTextures = {
    frames = {
        "objects/monumentswapblock/block0",
        "objects/monumentswapblock/block1",
        "objects/monumentswapblock/block2",
        "objects/monumentswapblock/block3",
    },
    trail = "objects/monumentswapblock/target",
    path = false

}
local frameNinePatchOptions = {
    mode = "fill",
    borderMode = "repeat"
}

local trailNinePatchOptions = {
    mode = "fill",
    borderMode = "repeat",
    useRealSize = true
}

local pathNinePatchOptions = {
    mode = "fill",
    fillMode = "repeat",
    border = 0
}

Monumentswapblock.fieldInformation = {
    color = {
        fieldType = "color",
        allowXNAColors = true,
    }
}

local pathDepth = 8999
local trailDepth = 8999
local blockDepth = -9999

Monumentswapblock.name = "EmHelper/Monumentswapblock"
Monumentswapblock.nodeLimits = {1, 1}
Monumentswapblock.placements = {
    name = "Monument Swapblock [EmHelper]",
        data = {
            width = 16,
            height = 16,
            pattern = 0,
            color = "LightSkyBlue",
            mute = false,
        }
}
Monumentswapblock.minimumSize = {16, 16}

local function addBlockSprites(sprites, entity, frameTexture)
    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.width or 8, entity.height or 8
    local color = entity.color or "LightSkyBlue"
    local frameNinePatch = drawableNinePatch.fromTexture(frameTexture, frameNinePatchOptions, x, y, width, height)
    local frameSprites = frameNinePatch:getDrawableSprite()

    for _, sprite in ipairs(frameSprites) do
        sprite.depth = blockDepth
        sprite:setColor(color)
        table.insert(sprites, sprite)
    end
end

local function addTrailSprites(sprites, entity, trailTexture, path)
    local nodes = entity.nodes or {}
    local x, y = entity.x or 0, entity.y or 0
    local nodeX, nodeY = nodes[1].x or x, nodes[1].y or y
    local width, height = entity.width or 8, entity.height or 8
    local drawWidth, drawHeight = math.abs(x - nodeX) + width, math.abs(y - nodeY) + height

    x, y = math.min(x, nodeX), math.min(y, nodeY)

    if path then
        local pathDirection = x == nodeX and "V" or "H"
        local pathTexture = string.format("objects/swapblock/path%s", pathDirection)
        local pathNinePatch = drawableNinePatch.fromTexture(pathTexture, pathNinePatchOptions, x, y, drawWidth, drawHeight)
        local pathSprites = pathNinePatch:getDrawableSprite()

        for _, sprite in ipairs(pathSprites) do
            sprite.depth = pathDepth

            table.insert(sprites, sprite)
        end
    end

    local frameNinePatch = drawableNinePatch.fromTexture(trailTexture, trailNinePatchOptions, x, y, drawWidth, drawHeight)
    local frameSprites = frameNinePatch:getDrawableSprite()

    for _, sprite in ipairs(frameSprites) do
        sprite.depth = trailDepth

        table.insert(sprites, sprite)
    end
end

function Monumentswapblock.sprite(room, entity)
    local pattern = (entity.pattern % 4) or 0
    local sprites = {}
    local frame = themeTextures.frames[pattern + 1] or themeTextures.frames[1]
    addTrailSprites(sprites, entity, themeTextures.trail, themeTextures.path)
    addBlockSprites(sprites, entity, frame)

    return sprites
end

function Monumentswapblock.nodeSprite(room, entity)
    local pattern = (entity.pattern % 4) or 0
    local sprites = {}
    local frame = themeTextures.frames[pattern + 1] or themeTextures.frames[1]

    addBlockSprites(sprites, entity, frame)

    return sprites
end

function Monumentswapblock.selection(room, entity)
    local nodes = entity.nodes or {}
    local x, y = entity.x or 0, entity.y or 0
    local nodeX, nodeY = nodes[1].x or x, nodes[1].y or y
    local width, height = entity.width or 8, entity.height or 8

    return utils.rectangle(x, y, width, height), {utils.rectangle(nodeX, nodeY, width, height)}
end

return Monumentswapblock