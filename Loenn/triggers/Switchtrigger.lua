local Switchtrigger = {}
local xnaColors = require("consts.xna_colors")

Switchtrigger.name = "Emhelper/Switchtrigger"
Switchtrigger.fieldInformation = {
    color = {
        fieldType = "color",
        allowXNAColors = true,
    }
}
Switchtrigger.placements = {
    name = "Switchtrigger [EmHelper]",
    data = {
        color = "LightSkyBlue",
        onetime = false
    }
}

return Switchtrigger