module EmHelperMonumentswitchblock

using ..Ahorn, Maple

@mapdef Entity "EmHelper/Monumentswitchblock" Monumentswitchblock(x::Integer, y::Integer, width::Integer=Maple.defaultBlockWidth, height::Integer=Maple.defaultBlockHeight, pattern::Integer=0, active::Bool=false, color::String="82d9ff")

const placements = Ahorn.PlacementDict(
    "Monument Switch Block (EmHelper)" => Ahorn.EntityPlacement(
        Monumentswitchblock,
        "rectangle",
    ),
)

Ahorn.minimumSize(entity::Monumentswitchblock) = 16, 16
Ahorn.resizable(entity::Monumentswitchblock) = true, true

Ahorn.selection(entity::Monumentswitchblock) = Ahorn.getEntityRectangle(entity)

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

function renderMonumentswitchblock(ctx::Ahorn.Cairo.CairoContext, entity::Monumentswitchblock)
    x, y = Ahorn.position(entity)

    width = Int(get(entity.data, "width", 32))
    height = Int(get(entity.data, "height", 32))
    entitypattern = Int(get(entity.data, "pattern", 0))
    block = ("objects/monumentswitchblock/solid"*string(entitypattern))

    entitycolor = String(get(entity.data, "color", "82d9ff"))
    entitytint = getColor(entitycolor)

    tilesWidth = div(width, 8)
    tilesHeight = div(height, 8)
    Ahorn.drawRectangle(ctx, x + 2, y + 2, width - 4, height - 4, entitytint)

    for i in 2:tilesWidth - 1
        Ahorn.drawImage(ctx, block, x + (i - 1) * 8, y, 8, 0, 8, 8, tint = entitytint)
        Ahorn.drawImage(ctx, block, x + (i - 1) * 8, y + height - 8, 8, 16, 8, 8, tint = entitytint)
    end

    for i in 2:tilesHeight - 1
        Ahorn.drawImage(ctx, block, x, y + (i - 1) * 8, 0, 8, 8, 8, tint = entitytint)
        Ahorn.drawImage(ctx, block, x + width - 8, y + (i - 1) * 8, 16, 8, 8, 8, tint = entitytint)
    end

    Ahorn.drawImage(ctx, block, x, y, 0, 0, 8, 8, tint = entitytint)
    Ahorn.drawImage(ctx, block, x + width - 8, y, 16, 0, 8, 8, tint = entitytint)
    Ahorn.drawImage(ctx, block, x, y + height - 8, 0, 16, 8, 8, tint = entitytint)
    Ahorn.drawImage(ctx, block, x + width - 8, y + height - 8, 16, 16, 8, 8, tint = entitytint)

    
end
function Ahorn.renderAbs(ctx::Ahorn.Cairo.CairoContext, entity::Monumentswitchblock, room::Maple.Room)
    renderMonumentswitchblock(ctx, entity)
end
end