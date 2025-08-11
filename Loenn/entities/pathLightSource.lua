local easeMode = {
    In = "In",
    Out = "Out",
    Both = "Both"
}

local pathLightSource = {
    name = "VBC2/PathLightSource",
    depth = 0,
    placements = {
        {
            name = "normal",
            data = {
                bloomRadius = 12.0,
                lightColor = "FFFFFF",
                startFade = 16,
                endFade = 32,
                closePath = false,
                stopAtNodes = false,
                easeMode = easeMode.Both,
                movementSpeed = 2.0,
                smoothPath = false
            }
        }
    },
    nodeLimits = {0, -1},
    nodeLineRenderType = "line"
}

pathLightSource.fieldInformation = {
    easeMode = {
        options = { easeMode.In, easeMode.Out, easeMode.Both },
        editable = false
    },
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

pathLightSource.fieldOrder={
    "x","y","lightColor","bloomRadius","startFade","endFade","easeMode","movementSpeed","smoothPath","closePath","stopAtNodes"
}

return pathLightSource