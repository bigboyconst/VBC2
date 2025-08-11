local utils = require("utils")

local springDepth = -8501
local springTexture = "objects/spring/00"

local seekerSpringUp = {}

seekerSpringUp.name = "VBC2/SeekerSpring"
seekerSpringUp.depth = springDepth
seekerSpringUp.justification = {0.5, 1.0}
seekerSpringUp.texture = springTexture
seekerSpringUp.selection = function(room, entity)
    return utils.rectangle(entity.x - 6, entity.y - 4, 12, 4)
end

seekerSpringUp.placements = {
    {
        name = "up",
        data = {
            playerCanUse = true
        }
    }
}

local seekerSpringRight = {}

seekerSpringRight.name = "VBC2/SeekerSpringRight"
seekerSpringRight.depth = springDepth
seekerSpringRight.justification = {0.5, 1.0}
seekerSpringRight.rotation = -math.pi / 2
seekerSpringRight.texture = springTexture
seekerSpringRight.selection = function(room, entity)
    return utils.rectangle(entity.x - 4, entity.y - 6, 4, 12)
end

seekerSpringRight.placements = {
    {
        name = "right", 
        data = {
            playerCanUse = true
        }
    }
}

local seekerSpringLeft = {}

seekerSpringLeft.name = "VBC2/SeekerSpringLeft"
seekerSpringLeft.depth = springDepth
seekerSpringLeft.justification = {0.5, 1.0}
seekerSpringLeft.texture = springTexture
seekerSpringLeft.rotation = math.pi / 2
seekerSpringLeft.selection = function(room, entity)
    return utils.rectangle(entity.x, entity.y - 6, 4, 12)
end

seekerSpringLeft.placements = {
    {
        name = "left",
        data = {
            playerCanUse = true
        }
    }
}

local seekerSpringDown = {}

seekerSpringDown.name = "VBC2/SeekerSpringDown"
seekerSpringDown.depth = springDepth
seekerSpringDown.justification = {0.5, 1.0}
seekerSpringDown.texture = springTexture
seekerSpringDown.rotation = math.pi
seekerSpringDown.selection = function(room, entity)
    return utils.rectangle(entity.x - 6, entity.y, 12, 4)
end
seekerSpringDown.placements = {
    {
        name = "down",
        data = {
            playerCanUse = true
        }
    }
}

return {
    seekerSpringUp,
    seekerSpringRight,
    seekerSpringLeft,
    seekerSpringDown
}