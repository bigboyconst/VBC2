local pathSpinner = {}

local easeMode = {
    In = "In",
    Out = "Out",
    Both = "Both"
}

pathSpinner.name = "VBC2/PathSpinner"
pathSpinner.depth = -8500
pathSpinner.closePath = false
pathSpinner.stopAtNodes = false
pathSpinner.easeMode = easeMode.Both
pathSpinner.speed = 2.0
pathSpinner.smoothPath = false


return pathSpinner