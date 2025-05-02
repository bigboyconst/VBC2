local reversedKevin = {}

reversedKevin.name = "VBC2/ReversedKevin"
reversedKevin.depth = -10500
reversedKevin.placements = {
    {
        name = "reversed_kevin",
        data = {
            width = 24,
            height = 24,
            axis = "Horizontal",
            chillout = false
        }
    }
}

reversedKevin.sprite = function(room, entity)
    local texture = "objects/crushblock/mid00"
    return {
        ninePatch = {
            texture = texture,
            width = entity.width,
            height = entity.height
        }
    }
end

reversedKevin.fieldInformation = {
    axis = {
        options = { "Horizontal", "Vertical" },
        editable = false
    }
}

return reversedKevin