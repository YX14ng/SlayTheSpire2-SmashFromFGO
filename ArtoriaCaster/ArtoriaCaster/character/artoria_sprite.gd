extends AnimatedSprite2D
# Returns to idle after any one-shot animation (except death).

func _ready() -> void:
	animation_finished.connect(_on_animation_finished)
	play("idle")

func _on_animation_finished() -> void:
	if animation != &"die":
		play("idle")
