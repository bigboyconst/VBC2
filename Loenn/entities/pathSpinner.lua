local easeMode = {
    In = "In",
    Out = "Out",
    Both = "Both"
}

local pathSpinner = {
    name = "VBC2/PathSpinner",
    depth = 0,
    texture = "danger/VBC2/Panda/blade00",
    placements = {
        {
            name = "normal",
            data = {
                closePath = false,
                stopAtNodes = false,
                easeMode = easeMode.Both,
                speed = 2.0,
                smoothPath = false
            }
        }
    },
    nodeLimits = {0, -1},
    nodeLineRenderType = "line"
}

pathSpinner.fieldInformation = {
    easeMode = {
        options = { easeMode.In, easeMode.Out, easeMode.Both },
        editable = false
    }
}

return pathSpinner