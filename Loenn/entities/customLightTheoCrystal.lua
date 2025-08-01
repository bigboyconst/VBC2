local customLightTheoCrystal = {}

customLightTheoCrystal.name = "VBC2/CustomLightTheoCrystal"

customLightTheoCrystal.depth = 0

customLightTheoCrystal.placements = {
    {
        name = "normal",
        data = {
            lightColor = "FFFFFF",
            bloomRadius = 12.0,
            startFade = 16,
            endFade = 32,
        }
    }
}

customLightTheoCrystal.fieldInformation = {
    bloomRadius={
        fieldType="number",
        minimumValue=0.0
    },
    startFade={
        fieldType="integer",
        minimumValue=0
    },
    endFade={
        fieldType="integer",
        minimumValue=0
    },
    lightColor={
        fieldType="color",
    }
}

customLightTheoCrystal.fieldOrder = {
    "x", "y", "lightColor", "bloomRadius", "startFade", "endFade"
}

function customLightTheoCrystal.texture(room, entity)
    return "characters/theoCrystal/idle00"
end

return customLightTheoCrystal