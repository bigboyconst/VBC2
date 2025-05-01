module VBC2SampleTrigger

using ..Ahorn, Maple

@mapdef Trigger "VBC2/SampleTrigger" SampleTrigger(
    x::Integer, y::Integer, width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight,
    sampleProperty::Integer=0
)

const placements = Ahorn.PlacementDict(
    "Sample Trigger (VBC2)" => Ahorn.EntityPlacement(
        SampleTrigger,
        "rectangle",
    )
)

end