module EmHelperMonumentspikes

using ..Ahorn, Maple

@mapdef Entity "EmHelper/MonumentspikesUp" MonumentspikesUp(x::Integer, y::Integer, width::Integer=Maple.defaultSpikeWidth, type::String="default",color::String="82d9ff", active::Bool=true)
@mapdef Entity "EmHelper/MonumentspikesDown" MonumentspikesDown(x::Integer, y::Integer, width::Integer=Maple.defaultSpikeWidth, type::String="default", color::String="82d9ff",active::Bool=true)
@mapdef Entity "EmHelper/MonumentspikesLeft" MonumentspikesLeft(x::Integer, y::Integer, height::Integer=Maple.defaultSpikeHeight, type::String="default",color::String="82d9ff",active::Bool=true)
@mapdef Entity "EmHelper/MonumentspikesRight" MonumentspikesRight(x::Integer, y::Integer, height::Integer=Maple.defaultSpikeHeight, type::String="default", color::String="82d9ff",active::Bool=true)

const spikeTypes = String[
    "default",
    "outline"
]

const entities = Dict{String, Type}(
    "up" => MonumentspikesUp,
    "down" => MonumentspikesDown,
    "left" => MonumentspikesLeft,
    "right" => MonumentspikesRight
)

const spikesUnion = Union{MonumentspikesUp, MonumentspikesDown, MonumentspikesLeft, MonumentspikesRight}

const placements = Ahorn.PlacementDict()
for (dir, entity) in entities
    key = "Monument Spikes ($(uppercasefirst(dir))) (EmHelper)"
    placements[key] = Ahorn.EntityPlacement(
        entity,
        "rectangle"
    )
end

Ahorn.editingOptions(entity::spikesUnion) = Dict{String, Any}(
    "type" => spikeTypes,
)

const directions = Dict{String, String}(
    "EmHelper/MonumentspikesUp" => "up",
    "EmHelper/MonumentspikesDown" => "down",
    "EmHelper/MonumentspikesLeft" => "left",
    "EmHelper/MonumentspikesRight" => "right"
)

const offsets = Dict{String, Tuple{Integer, Integer}}(
    "up" => (4, -4),
    "down" => (4, 4),
    "left" => (-4, 4),
    "right" => (4, 4)
)

const rotations = Dict{String, Number}(
    "up" => 0,
    "right" => pi / 2,
    "down" => pi,
    "left" => pi * 3 / 2
)

const resizeDirections = Dict{String, Tuple{Bool, Bool}}(
    "up" => (true, false),
    "down" => (true, false),
    "left" => (false, true),
    "right" => (false, true),
)

function Ahorn.renderSelectedAbs(ctx::Ahorn.Cairo.CairoContext, entity::spikesUnion)
    direction = get(directions, entity.name, "up")
    theta = rotations[direction] - pi / 2

    width = Int(get(entity.data, "width", 0))
    height = Int(get(entity.data, "height", 0))

    x, y = Ahorn.position(entity)
    cx, cy = x + floor(Int, width / 2) - 8 * (direction == "left"), y + floor(Int, height / 2) - 8 * (direction == "up")

    Ahorn.drawArrow(ctx, cx, cy, cx + cos(theta) * 24, cy + sin(theta) * 24, Ahorn.colors.selection_selected_fc, headLength=6)
end

function Ahorn.selection(entity::spikesUnion)
    if haskey(directions, entity.name)
        x, y = Ahorn.position(entity)

        width = Int(get(entity.data, "width", 8))
        height = Int(get(entity.data, "height", 8))

        direction = get(directions, entity.name, "up")
        variant = get(entity.data, "hotType", "default")

        width = Int(get(entity.data, "width", 8))
        height = Int(get(entity.data, "height", 8))

        ox, oy = offsets[direction]

        return Ahorn.Rectangle(x + ox - 4, y + oy - 4, width, height)
    end
end

Ahorn.minimumSize(entity::spikesUnion) = (8, 8)

function Ahorn.resizable(entity::spikesUnion)
    if haskey(directions, entity.name)
        variant = get(entity.data, "type", "default")
        direction = get(directions, entity.name, "up")

        return resizeDirections[direction]
    end
end

function getColor(color)
    if haskey(Ahorn.XNAColors.colors, color)
        return Ahorn.XNAColors.colors[color]

    else
        try
            return ((Ahorn.argb32ToRGBATuple(parse(Int, replace(color, "#" => ""), base=16))[1:3] ./ 255)..., 1.0)

        catch

        end
    end

    return (1.0, 1.0, 1.0, 1.0)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::spikesUnion)
    if haskey(directions, entity.name)
        entitycolor = String(get(entity.data, "color", "82d9ff"))
        entitytint = getColor(entitycolor)
        variant = get(entity.data, "type", "default")
        direction = get(directions, entity.name, "up")
    
        width = get(entity.data, "width", 8)
        height = get(entity.data, "height", 8)

        for ox in 0:8:width - 8, oy in 0:8:height - 8
            drawX = ox + offsets[direction][1]
            drawY = oy + offsets[direction][2]

            Ahorn.drawSprite(ctx, "danger/spikes/$(variant)_$(direction)00", drawX, drawY, tint=entitytint)
        end
    end
end

end
