module EmHelperMonumentbooster

using ..Ahorn, Maple

@mapdef Entity "EmHelper/Monumentbooster" MonumentBooster(x::Integer, y::Integer, pattern::Integer=0, red::Bool=false, color::String="82d9ff", active::Bool=true)

const placements = Ahorn.PlacementDict(
    "Monument Booster (EmHelper, green)" => Ahorn.EntityPlacement(
        MonumentBooster,
        "point",
        Dict{String, Any}(
            "red" => false
        )
    ),
    "Monument Booster (EmHelper, red)" => Ahorn.EntityPlacement(
        MonumentBooster,
        "point",
        Dict{String, Any}(
            "red" => true
        )
    ),
)

function boosterSprite(entity::MonumentBooster)
    red = get(entity.data, "red", false)
    
    if red
        return "objects/monumentbooster/eboosterRed00"

    else
        return "objects/monumentbooster/ebooster00"
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

function Ahorn.selection(entity::MonumentBooster)
    x, y = Ahorn.position(entity)
    sprite = boosterSprite(entity)

    return Ahorn.getSpriteRectangle(sprite, x, y)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::MonumentBooster, room::Maple.Room)
    sprite = boosterSprite(entity)
    esprite = "objects/monumentbooster/e"
    entitycolor = String(get(entity.data, "color", "82d9ff"))
    entitytint = getColor(entitycolor)
    Ahorn.drawSprite(ctx, sprite, 0, 0)
    Ahorn.drawSprite(ctx, esprite, 0, 0, tint = entitytint)
end

end