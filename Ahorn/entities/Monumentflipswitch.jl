module EmHelperMonumentflipswitch

using ..Ahorn, Maple

@mapdef Entity "EmHelper/Monumentflipswitch" Monumentflipswitch(x::Integer, y::Integer, onlyEnable::Bool=false, onlyDisable::Bool=false, color::String="82d9ff", mute::Bool=false, pattern::Integer=0)

const placements = Ahorn.PlacementDict(
    "Monument FlipSwitch (EmHelper, only enable)" => Ahorn.EntityPlacement(
        Monumentflipswitch,
        "point",
        Dict{String, Any}(
            "onlyEnable" => true
        )
    ),
    "Monument FlipSwitch (EmHelper, only disable)" => Ahorn.EntityPlacement(
        Monumentflipswitch,
        "point",
        Dict{String, Any}(
            "onlyDisable" => true
        )
    ),
    "Monument FlipSwitch (EmHelper, both)" => Ahorn.EntityPlacement(
        Monumentflipswitch
    ),
)

function switchSprite(entity::Monumentflipswitch)
    onlyDisable = get(entity.data, "onlyDisable", false)
    onlyEnable = get(entity.data, "onlyEnable", false)
    patternNumber = get(entity.data, "pattern", 0)
    pattern = string(patternNumber)
    if onlyDisable
        return "objects/monumentflipswitch"*pattern*"/switch13.png"

    elseif onlyEnable
        return "objects/monumentflipswitch"*pattern*"/switch15.png"

    else
        return "objects/monumentflipswitch"*pattern*"/switch01.png"
    end
end

function Ahorn.selection(entity::Monumentflipswitch)
    x, y = Ahorn.position(entity)
    sprite = switchSprite(entity)

    return Ahorn.getSpriteRectangle(sprite, x, y)
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

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::Monumentflipswitch, room::Maple.Room)
    sprite = switchSprite(entity)

    entitycolor = String(get(entity.data, "color", "82d9ff"))
    entitytint = getColor(entitycolor)

    Ahorn.drawSprite(ctx, sprite, 0, 0, tint=entitytint)
end

end