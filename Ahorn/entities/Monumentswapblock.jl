module EmHelperMonumentswapblock

using ..Ahorn, Maple

@mapdef Entity "EmHelper/Monumentswapblock" Monumentswapblock(x::Integer, y::Integer, width::Integer=Maple.defaultBlockWidth, height::Integer=Maple.defaultBlockHeight, pattern::Integer=0, color::String="82d9ff", mute::Bool=false)

function swapFinalizer(entity)
    x, y = Ahorn.position(entity)

    width = Int(get(entity.data, "width", 8))
    height = Int(get(entity.data, "height", 8))

    entity.data["nodes"] = [(x + width, y)]
end

const placements = Ahorn.PlacementDict(
    "Monument Swap Block (EmHelper)" => Ahorn.EntityPlacement(
        Monumentswapblock,
        "rectangle",
        Dict{String, Any}(),
        swapFinalizer
    )
)

Ahorn.nodeLimits(entity::Monumentswapblock) = 1, 1

Ahorn.minimumSize(entity::Monumentswapblock) = 16, 16
Ahorn.resizable(entity::Monumentswapblock) = true, true

function Ahorn.selection(entity::Monumentswapblock)
    x, y = Ahorn.position(entity)
    stopX, stopY = Int.(entity.data["nodes"][1])

    width = Int(get(entity.data, "width", 8))
    height = Int(get(entity.data, "height", 8))

    return [Ahorn.Rectangle(x, y, width, height), Ahorn.Rectangle(stopX, stopY, width, height)]
end

function renderTrail(ctx, x::Number, y::Number, width::Number, height::Number, trail::String, color::String)
    entitytint = getColor(color)
    tilesWidth = div(width, 8)
    tilesHeight = div(height, 8)

    for i in 2:tilesWidth - 1
        Ahorn.drawImage(ctx, trail, x + (i - 1) * 8, y + 2, 6, 0, 8, 6, tint=entitytint)
        Ahorn.drawImage(ctx, trail, x + (i - 1) * 8, y + height - 8, 6, 14, 8, 6, tint=entitytint)
    end

    for i in 2:tilesHeight - 1
        Ahorn.drawImage(ctx, trail, x + 2, y + (i - 1) * 8, 0, 6, 6, 8, tint=entitytint)
        Ahorn.drawImage(ctx, trail, x + width - 8, y + (i - 1) * 8, 14, 6, 6, 8, tint=entitytint)
    end

    for i in 2:tilesWidth - 1, j in 2:tilesHeight - 1
        Ahorn.drawImage(ctx, trail, x + (i - 1) * 8, y + (j - 1) * 8, 6, 6, 8, 8, tint=entitytint)
    end

    Ahorn.drawImage(ctx, trail, x + width - 8, y + 2, 14, 0, 6, 6, tint=entitytint)
    Ahorn.drawImage(ctx, trail, x + width - 8, y + height - 8, 14, 14, 6, 6, tint=entitytint)
    Ahorn.drawImage(ctx, trail, x + 2, y + 2, 0, 0, 6, 6, tint=entitytint)
    Ahorn.drawImage(ctx, trail, x + 2, y + height - 8, 0, 14, 6, 6, tint=entitytint)
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

function renderMonumentswapblock(ctx::Ahorn.Cairo.CairoContext, x::Number, y::Number, width::Number, height::Number, frame::String, color::String)
    entitytint = getColor(color)
    tilesWidth = div(width, 8)
    tilesHeight = div(height, 8)

    for i in 2:tilesWidth - 1
        Ahorn.drawImage(ctx, frame, x + (i - 1) * 8, y, 8, 0, 8, 8, tint=entitytint)
        Ahorn.drawImage(ctx, frame, x + (i - 1) * 8, y + height - 8, 8, 16, 8, 8, tint=entitytint)
    end

    for i in 2:tilesHeight - 1
        Ahorn.drawImage(ctx, frame, x, y + (i - 1) * 8, 0, 8, 8, 8, tint=entitytint)
        Ahorn.drawImage(ctx, frame, x + width - 8, y + (i - 1) * 8, 16, 8, 8, 8, tint=entitytint)
    end

    for i in 2:tilesWidth - 1, j in 2:tilesHeight - 1
        Ahorn.drawImage(ctx, frame, x + (i - 1) * 8, y + (j - 1) * 8, 8, 8, 8, 8, tint=entitytint)
    end

    Ahorn.drawImage(ctx, frame, x, y, 0, 0, 8, 8, tint=entitytint)
    Ahorn.drawImage(ctx, frame, x + width - 8, y, 16, 0, 8, 8, tint=entitytint)
    Ahorn.drawImage(ctx, frame, x, y + height - 8, 0, 16, 8, 8, tint=entitytint)
    Ahorn.drawImage(ctx, frame, x + width - 8, y + height - 8, 16, 16, 8, 8, tint=entitytint)
end

function Ahorn.renderSelectedAbs(ctx::Ahorn.Cairo.CairoContext, entity::Monumentswapblock, room::Maple.Room)
    entitycolor = String(get(entity.data, "color", "82d9ff"))

    startX, startY = Int(entity.data["x"]), Int(entity.data["y"])
    stopX, stopY = Int.(entity.data["nodes"][1])
    entitypattern = Int(get(entity.data, "pattern", 0))
    width = Int(get(entity.data, "width", 32))
    height = Int(get(entity.data, "height", 32))
    renderMonumentswapblock(ctx, startX, startY, width, height, "objects/monumentswapblock/block"*string(entitypattern), entitycolor)
    Ahorn.drawArrow(ctx, startX + width / 2, startY + height / 2, stopX + width / 2, stopY + height / 2, Ahorn.colors.selection_selected_fc, headLength=6)
end

function Ahorn.renderAbs(ctx::Ahorn.Cairo.CairoContext, entity::Monumentswapblock, room::Maple.Room)
    entitycolor = String(get(entity.data, "color", "82d9ff"))
 
    startX, startY = Int(entity.data["x"]), Int(entity.data["y"])
    stopX, stopY = Int.(entity.data["nodes"][1])

    width = Int(get(entity.data, "width", 32))
    height = Int(get(entity.data, "height", 32))
    entitypattern = Int(get(entity.data, "pattern", 0))
    renderTrail(ctx, min(startX, stopX), min(startY, stopY), abs(startX - stopX) + width, abs(startY - stopY) + height, "objects/swapblock/target", entitycolor)
    renderMonumentswapblock(ctx, startX, startY, width, height, "objects/monumentswapblock/block"*string(entitypattern), entitycolor)
end

end