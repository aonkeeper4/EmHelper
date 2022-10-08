module EmHelperMonumentpressureplate

using ..Ahorn, Maple

@mapdef Entity "EmHelper/Monumentpressureplate" Monumentpressureplate(x::Integer, y::Integer, pattern::Integer=0, onetime::Bool=false, color::String="82d9ff", mute::Bool=false, isButton::Bool=false, disable::Bool=false)

const placements = Ahorn.PlacementDict(
   "Monument Pressureplate (EmHelper)" => Ahorn.EntityPlacement(
      Monumentpressureplate
    )
)
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

function Ahorn.selection(entity::Monumentpressureplate)
    x, y = Ahorn.position(entity)

    return Ahorn.Rectangle(x-6, y-4, 12, 4)
end


function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::Monumentpressureplate, room::Maple.Room)
    sprite = "objects/monumentpressureplate/solid"
    entitypattern = Int(get(entity.data, "pattern", 0))

    entitycolor = String(get(entity.data, "color", "82d9ff"))
    entitytint = getColor(entitycolor)

    Ahorn.drawSprite(ctx, sprite*string(entitypattern), 0, -8, tint=entitytint)
end
end