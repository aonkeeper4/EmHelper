module EmHelperSwitchtrigger

using ..Ahorn, Maple

@mapdef Trigger "EmHelper/Switchtrigger" Switchtrigger(x::Integer, y::Integer, width::Integer=16, height::Integer=16, color::String="82d9ff", onetime::Bool=false,)

const placements = Ahorn.PlacementDict(
    "Switch Trigger (EmHelper)" => Ahorn.EntityPlacement(
        Switchtrigger,
        "rectangle",
    ),
)

end