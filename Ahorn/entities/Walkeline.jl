module EmHelperWalkeline

using ..Ahorn, Maple

@mapdef Entity "EmHelper/Walkeline" Walkeline(x::Integer, y::Integer, haircolor::String="212121", left::Bool=true, weak::Bool=false, dangerous::Bool=false, ally::Bool=true, bouncy::Bool=false, smart::Bool=false, mute::Bool=false, nobackpack::Bool=false, idle::Bool=false, deathflag::String="WalkelineIsDead", triggerhappy::Bool=false)

const placements = Ahorn.PlacementDict(
   "Walkeline (EmHelper)" => Ahorn.EntityPlacement(
      Walkeline
    )
)

function Ahorn.selection(entity::Walkeline)
    x, y = Ahorn.position(entity)
    Ahorn.Rectangle(x-8, y-17, 13, 17)
end

function walkelineSprite(entity::Walkeline)
    nobackpack = get(entity.data, "nobackpack", false)

    if nobackpack
        return "characters/windupwalkeline/ahornbody", "characters/windupwalkeline/ahornhair"

    else
        return "characters/walkeline/ahornbody", "characters/walkeline/ahornhair"
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

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::Walkeline, room::Maple.Room)
    left = get(entity.data, "left", true)
    sprite, hairsprite = walkelineSprite(entity)
    entitycolor = String(get(entity.data, "haircolor", "212121"))
    entitytint = getColor(entitycolor)
    if left  
       Ahorn.Cairo.save(ctx)
       Ahorn.scale(ctx, -1, 1)
       Ahorn.drawSprite(ctx, sprite, +3, 0, jx=0.5, jy=1.0)
       Ahorn.drawSprite(ctx, hairsprite, +3, 0, jx=0.5, jy=1.0, tint = entitytint)
       Ahorn.restore(ctx)
    else
       Ahorn.drawSprite(ctx, sprite, 0, 0, jx=0.5, jy=1.0)
       Ahorn.drawSprite(ctx, hairsprite, 0, 0, jx=0.5, jy=1.0, tint = entitytint)
    end

end

end